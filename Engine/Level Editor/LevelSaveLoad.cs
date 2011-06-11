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
                    String name = path.Split('\\').Last();
                    if(Directory.Exists(Application.ExecutablePath + "Textures"))
                        Directory.CreateDirectory(Application.ExecutablePath + "Textures");
                    if(File.Exists(Application.ExecutablePath + "Textures\\" + name))
                        File.Delete(Application.ExecutablePath + "Textures\\" + name);
                    File.Copy(path, Application.ExecutablePath + "Textures\\" + name);
                    writer.Write((byte)6);
                    writer.Write(name);
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
                    String path = Application.ExecutablePath + "Textures\\";
                    path += reader.ReadString();
                    textureStream = new FileStream(path, FileMode.Open);
                    Texture2D tex = Texture2D.FromStream(graphics, textureStream);
                    l.importedTextures.Add(tex);
                    l.form.AddToTextures(path);
                    b = reader.ReadByte();
                    textureStream.Close();

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
                            d = new Wall(null);
                            break;
                    }
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
        public static Doodad ReadDoodad(BinaryReader reader, LevelEditState l)
        {
            Doodad d = new Doodad(l.importedTextures[0], new Vector2(0,0));
            d.PosX = reader.ReadSingle();
            d.PosY = reader.ReadSingle();
            d.Sprite = l.importedTextures[reader.ReadInt16()];
            return d;
        }
        public static AnimatedDoodad ReadAnimatedDoodad(BinaryReader reader, LevelEditState l)
        {
            AnimatedDoodad d = new AnimatedDoodad(null, 0, 0, 0, 0, true, 0, Vector2.Zero);
            d.PosX = reader.ReadSingle();
            d.PosY = reader.ReadSingle();
            d.Sprite = l.importedTextures[reader.ReadInt16()];
            d.Width = reader.ReadInt16();
            d.Height = reader.ReadInt16();
            d.Frames = reader.ReadInt16();
            d.Rows = reader.ReadInt16();
            d.Milliseconds = reader.ReadInt16();
            d.Repeat = reader.ReadBoolean();
            return d;
        }
        public static ControlPanel ReadControlPanel(BinaryReader reader, LevelEditState l)
        {
            ControlPanel c = new ControlPanel(new BoundingBox(Vector3.Zero, Vector3.Zero), l, null, null, null);
            c.MinX = reader.ReadSingle();
            c.MinY = reader.ReadSingle();
            c.MaxX = reader.ReadSingle();
            c.MaxY = reader.ReadSingle();
            c.Sprite = l.importedTextures[reader.ReadInt16()];
            return c;
        }
        public static ElevatorSurface ReadElevatorSurface(BinaryReader reader, LevelEditState l)
        {
            ElevatorSurface e = new ElevatorSurface(new BoundingBox(Vector3.Zero, Vector3.Zero), l, null, true, Vector2.Zero, Vector2.Zero);
            e.MinX = reader.ReadSingle();
            e.MinY = reader.ReadSingle();
            e.MaxX = reader.ReadSingle();
            e.MaxY = reader.ReadSingle();
            e.StartX = reader.ReadSingle();
            e.StartY = reader.ReadSingle();
            e.EndX = reader.ReadSingle();
            e.EndY = reader.ReadSingle();
            e.isRight = reader.ReadBoolean();
            e.Sprite = l.importedTextures[reader.ReadInt16()];
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
            h.Sprite = l.importedTextures[reader.ReadInt16()];
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
            m.Sprite = l.importedTextures[reader.ReadInt16()];
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
            m.Sprite = l.importedTextures[reader.ReadInt16()];
            return m;
        }
        public static PressurePlate ReadPressurePlate(BinaryReader reader, LevelEditState l)
        {
            PressurePlate p = new PressurePlate(new BoundingBox(), l, null, null);
            p.MinX = reader.ReadSingle();
            p.MinY = reader.ReadSingle();
            p.MaxX = reader.ReadSingle();
            p.MaxY = reader.ReadSingle();
            p.Sprite = l.importedTextures[reader.ReadInt16()];
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
            r.Sprite = l.importedTextures[reader.ReadInt16()];
            return r;
        }
        public static Slope ReadSlope(BinaryReader reader, LevelEditState l)
        {
            Slope s = new Slope(l, new Microsoft.Xna.Framework.Point(), new Microsoft.Xna.Framework.Point());
            s.StartX = reader.ReadInt16();
            s.StartY = reader.ReadInt16();
            s.EndX = reader.ReadInt16();
            s.EndY = reader.ReadInt16();
            s.Height = reader.ReadInt16();
            s.Sprite = l.importedTextures[reader.ReadInt16()];
            return s;
        }
        public static Wall ReadWall(BinaryReader reader, LevelEditState l)
        {
            Wall w = new Wall(l);
            w.MinX = reader.ReadSingle();
            w.MinY = reader.ReadSingle();
            w.MaxX = reader.ReadSingle();
            w.MaxY = reader.ReadSingle();
            w.Sprite = l.importedTextures[reader.ReadInt16()];
            return w;
        }
    }
}
