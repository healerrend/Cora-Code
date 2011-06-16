using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace CORA
{
    public static class LevelSaveLoad
    {
        public static void SaveLevel(LevelEditState l, string filename)
        {
            try
            {
                if (File.Exists(filename))
                    File.Delete(filename);
                FileStream stream = new FileStream(filename, FileMode.Create);
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write((byte)0);
                writer.Write((float)l.levelSize.X);
                writer.Write((float)l.levelSize.Y);
                foreach (Texture2D t in l.importedTextures)
                {
                    String path = (string)l.form.lstTextures.Items[l.importedTextures.IndexOf(t)];
                    writer.Write((byte)6);
                    writer.Write(path);
                }
                if (l.underlay != null)
                {
                    writer.Write((byte)20);
                    writer.Write((int)l.importedTextures.IndexOf(l.underlay));
                }
                foreach (Doodad d in l.background)
                {
                    writer.Write((byte)1);
                    d.WriteToFile(writer, l);
                }
                foreach (Drawable d in l.form.lstBlocks.Items)
                {
                    d.WriteToFile(writer, l);
                }
                writer.Write((byte)15);
                writer.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void LoadLevel(LevelEditState l, string filename, GraphicsDevice graphics)
        {
            try
            {
                l.background.Clear();
                l.doodads.Clear();
                l.objects.Clear();
                l.interactables.Clear();
                l.importedTextures.Clear();
                l.form.ClearAllCollections();
                FileStream stream = new FileStream(filename, FileMode.Open);
                BinaryReader reader = new BinaryReader(stream);
                Byte b = reader.ReadByte();
                if (b != 0)
                    throw new Exception("INVALID FORMAT.");
                l.levelSize.X = reader.ReadSingle(); //READ LEVEL SIZE
                l.levelSize.Y = reader.ReadSingle();
                l.form.txtLevelWidth.Text = l.levelSize.X.ToString();
                l.form.txtLevelHeight.Text = l.levelSize.Y.ToString();
                b = reader.ReadByte();
                FileStream textureStream;
                while (b == 6) //READ TEXTURES
                {
                    String path = reader.ReadString();
                    textureStream = new FileStream(path, FileMode.Open);
                    Texture2D tex = Texture2D.FromStream(graphics, textureStream);
                    l.importedTextures.Add(tex);
                    l.form.AddToTextures(path);
                    b = reader.ReadByte();
                    textureStream.Close();

                }
                if (b == 20)
                {
                    l.underlay = l.importedTextures[reader.ReadInt32()];
                    b = reader.ReadByte();
                }
                while (b == 1) //READ BACKGROUNDS
                {
                    b = reader.ReadByte();
                    Doodad d = ReadDoodad(reader, l);
                    l.background.Add(d);
                    l.form.AddToBackground(d);
                    b = reader.ReadByte();
                }
                while (b != 15) //READ BLOCKS
                {
                    Drawable d;
                    switch (b)
                    {
                        case 3:
                            d = ReadDoodad(reader, l);
                            l.doodads.Add((Doodad)d);
                            break;
                        case 4:
                            d = ReadAnimatedDoodad(reader, l);
                            l.doodads.Add((Doodad)d);
                            break;
                        case 5:
                            d = ReadControlPanel(reader, l);
                            l.interactables.Add((HitBoxInteractable)d);
                            break;
                        case 14:
                            d = ReadElevatorSurface(reader, l);
                            l.interactables.Add((HitBoxInteractable)d);
                            break; 
                        case 7:
                            d = ReadHangingLedge(reader, l);
                            l.interactables.Add((HitBoxInteractable)d);
                            break;
                        case 8:
                            d = ReadMovingHangingLedge(reader, l);
                            l.interactables.Add((HitBoxInteractable)d);
                            break;
                        case 9: 
                            d = ReadMovingPlatform(reader, l);
                            l.walls.Add((LevelBlock)d);
                            break;
                        case 10:
                            d = ReadPressurePlate(reader, l);
                            l.interactables.Add((HitBoxInteractable)d);
                            break;
                        case 11:
                            d = ReadRust(reader, l);
                            l.walls.Add((LevelBlock)d);
                            break;
                        case 12:
                            d = ReadSlope(reader, l);
                            l.walls.Add((LevelBlock)d);
                            break;
                        case 13:
                            d = ReadWall(reader, l);
                            l.walls.Add((LevelBlock)d);
                            break;
                        default:
                            d = null;
                            break;
                    }
                    if(d != null)
                        l.form.AddToBlocks(d);
                    b = reader.ReadByte();
                }
                reader.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void ExportLevel(LevelEditState l, string filename)
        {
            try
            {
                if (File.Exists(filename))
                    File.Delete(filename);
                FileStream stream = new FileStream(filename, FileMode.Create);
                StreamWriter writer = new StreamWriter(stream);
                StringBuilder texturesDec = new StringBuilder();
                StringBuilder texturesDef = new StringBuilder();
                StringBuilder mainString = new StringBuilder();
                mainString.AppendLine("this.levelSize.X = " + l.levelSize.X + ';');
                mainString.AppendLine("this.levelSize.Y = " + l.levelSize.Y + ';');
                if (l.underlay != null)
                {
                    int i = l.importedTextures.IndexOf(l.underlay);
                    string path = l.form.lstTextures.Items[i].ToString();
                    string[] tokens = path.Split('\\');
                    path = tokens.Last();
                    path = path.Substring(0, path.IndexOf('.'));
                    if (!texturesDec.ToString().Contains(path))
                    {
                        texturesDec.AppendLine("protected Texture2D " + path + ';');
                        texturesDef.AppendLine(path + " = content.Load<Texture2D>(\"realassets\\\\" + path + "\");");
                    }
                }
                foreach (Doodad d in l.form.lstBackgrounds.Items)
                    d.Export(l, texturesDec, texturesDef, mainString, true);
                foreach (Drawable d in l.form.lstBlocks.Items)
                    d.Export(l, texturesDec, texturesDef, mainString);
                writer.Write(texturesDec);
                writer.Write(texturesDef);
                writer.Write(mainString);
                writer.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static Doodad ReadDoodad(BinaryReader reader, LevelEditState l)
        {
            Doodad d = new Doodad(l.importedTextures[0], new Vector2(0,0));
            d.PosX = reader.ReadSingle();
            d.PosY = reader.ReadSingle();
            byte b = reader.ReadByte();
            if(b == 22)
                d.Sprite = l.importedTextures[reader.ReadInt16()];
            b = reader.ReadByte();
            if (b == 22)
                d.Name = reader.ReadString();
            return d;
        }
        public static AnimatedDoodad ReadAnimatedDoodad(BinaryReader reader, LevelEditState l)
        {
            AnimatedDoodad d = new AnimatedDoodad(null, 0, 0, 0, 0, true, 0, new Vector2(0, 0));
            d.PosX = reader.ReadSingle();
            d.PosY = reader.ReadSingle();
            byte b = reader.ReadByte();
            if (b == 22)
                d.Sprite = l.importedTextures[reader.ReadInt16()];
            d.Width = reader.ReadInt16();
            d.Height = reader.ReadInt16();
            d.Frames = reader.ReadInt16();
            d.Rows = reader.ReadInt16();
            d.Milliseconds = reader.ReadInt16();
            d.Repeat = reader.ReadBoolean();
            b = reader.ReadByte();
            if (b == 22)
                d.Name = reader.ReadString();
            return d;
        }
        public static ControlPanel ReadControlPanel(BinaryReader reader, LevelEditState l)
        {
            ControlPanel c = new ControlPanel(new BoundingBox(Vector3.Zero, Vector3.Zero), l, null, null, null);
            c.MinX = reader.ReadSingle();
            c.MinY = reader.ReadSingle();
            c.MaxX = reader.ReadSingle();
            c.MaxY = reader.ReadSingle();
            byte b = reader.ReadByte();
            if (b == 22)
                c.Sprite = l.importedTextures[reader.ReadInt16()];
            b = reader.ReadByte();
            if (b == 22)
                c.Name = reader.ReadString();
            return c;
        }
        public static ElevatorSurface ReadElevatorSurface(BinaryReader reader, LevelEditState l)
        {
            ElevatorSurface e = new ElevatorSurface(new BoundingBox(Vector3.Zero, Vector3.Zero), l, null, true, new Vector2(0,0), new Vector2(0,0));
            e.MinX = reader.ReadSingle();
            e.MinY = reader.ReadSingle();
            e.MaxX = reader.ReadSingle();
            e.MaxY = reader.ReadSingle();
            e.StartX = reader.ReadSingle();
            e.StartY = reader.ReadSingle();
            e.EndX = reader.ReadSingle();
            e.EndY = reader.ReadSingle();
            e.isRight = reader.ReadBoolean();
            byte b = reader.ReadByte();
            if (b == 22)
                e.Sprite = l.importedTextures[reader.ReadInt16()];
            b = reader.ReadByte();
            if (b == 22)
                e.Name = reader.ReadString();
            return e;
        }
        public static HangingLedge ReadHangingLedge(BinaryReader reader, LevelEditState l)
        {
            HangingLedge h = new HangingLedge(new BoundingBox(), l, null, new Microsoft.Xna.Framework.Point(), true);
            h.MinX = reader.ReadSingle();
            h.MinY = reader.ReadSingle();
            h.MaxX = reader.ReadSingle();
            h.MaxY = reader.ReadSingle();
            h.PointX = reader.ReadInt16();
            h.PointY = reader.ReadInt16();
            h.IsRight = reader.ReadBoolean();
            byte b = reader.ReadByte();
            if (b == 22)
                h.Sprite = l.importedTextures[reader.ReadInt16()];
            b = reader.ReadByte();
            if (b == 22)
                h.Name = reader.ReadString();
            return h;
        }
        public static MovingHangingLedge ReadMovingHangingLedge(BinaryReader reader, LevelEditState l)
        {
            MovingHangingLedge m = new MovingHangingLedge(new BoundingBox(), l, null, new Microsoft.Xna.Framework.Point(), true);
            m.MinX = reader.ReadSingle();
            m.MinY = reader.ReadSingle();
            m.MaxX = reader.ReadSingle();
            m.MaxY = reader.ReadSingle();
            m.PointX = reader.ReadInt16();
            m.PointY = reader.ReadInt16();
            m.IsRight = reader.ReadBoolean();
            byte b = reader.ReadByte();
            if (b == 22)
                m.Sprite = l.importedTextures[reader.ReadInt16()];
            b = reader.ReadByte();
            if (b == 22)
                m.Name = reader.ReadString();
            return m;
        }
        public static MovingPlatform ReadMovingPlatform(BinaryReader reader, LevelEditState l)
        {
            MovingPlatform m = new MovingPlatform(new BoundingBox(), l, new Microsoft.Xna.Framework.Point(), new Microsoft.Xna.Framework.Point(), MovingPlatformRotationType.Bouncing);
            m.MinX = reader.ReadSingle();
            m.MinY = reader.ReadSingle();
            m.MaxX = reader.ReadSingle();
            m.MaxY = reader.ReadSingle();
            m.BeginX = reader.ReadInt16();
            m.BeginY = reader.ReadInt16();
            m.EndX = reader.ReadInt16();
            m.EndY = reader.ReadInt16();
            m.SecondsPerCycle = reader.ReadSingle();
            byte b = reader.ReadByte();
            if (b == 22)
                m.Sprite = l.importedTextures[reader.ReadInt16()];
            b = reader.ReadByte();
            if (b == 22)
                m.Name = reader.ReadString();
            return m;
        }
        public static PressurePlate ReadPressurePlate(BinaryReader reader, LevelEditState l)
        {
            PressurePlate p = new PressurePlate(new BoundingBox(), l, null, null);
            p.MinX = reader.ReadSingle();
            p.MinY = reader.ReadSingle();
            p.MaxX = reader.ReadSingle();
            p.MaxY = reader.ReadSingle();
            byte b = reader.ReadByte();
            if (b == 22)
                p.Sprite = l.importedTextures[reader.ReadInt16()];
            b = reader.ReadByte();
            if (b == 22)
                p.Name = reader.ReadString();
            return p;
        }
        public static Rust ReadRust(BinaryReader reader, LevelEditState l)
        {
            Rust r = new Rust(new BoundingBox(), l);
            r.MinX = reader.ReadSingle();
            r.MinY = reader.ReadSingle();
            r.MaxX = reader.ReadSingle();
            r.MaxY = reader.ReadSingle();
            r.DisappearLength = reader.ReadDouble();
            byte b = reader.ReadByte();
            if (b == 22)
                r.Sprite = l.importedTextures[reader.ReadInt16()];
            b = reader.ReadByte();
            if (b == 22)
                r.Name = reader.ReadString();
            return r;
        }
        public static Slope ReadSlope(BinaryReader reader, LevelEditState l)
        {
            Slope s = new Slope(l, new Microsoft.Xna.Framework.Point(), new Microsoft.Xna.Framework.Point());
            s.StartX = reader.ReadInt32();
            s.StartY = reader.ReadInt32();
            s.EndX = reader.ReadInt32();
            s.EndY = reader.ReadInt32();
            s.Height = reader.ReadInt32();
            byte b = reader.ReadByte();
            if (b == 22)
                s.Sprite = l.importedTextures[reader.ReadInt16()];
            b = reader.ReadByte();
            if (b == 22)
                s.Name = reader.ReadString();
            return s;
        }
        public static Wall ReadWall(BinaryReader reader, LevelEditState l)
        {
            Wall w = new Wall(l);
            w.MinX = reader.ReadSingle();
            w.MinY = reader.ReadSingle();
            w.MaxX = reader.ReadSingle();
            w.MaxY = reader.ReadSingle();
            byte b = reader.ReadByte();
            if (b == 22)
                w.Sprite = l.importedTextures[reader.ReadInt16()];
            b = reader.ReadByte();
            if (b == 22)
                w.Name = reader.ReadString();
            return w;
        }
    }
}
