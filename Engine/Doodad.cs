using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

/* This file contains the doodad class and the animated doodad class, which will draw something on screen.
 */

namespace CORA
{
    /// <summary>
    /// This class will draw or animate something on the screen which does not block or obstruct. It is simply drawn.
    /// </summary>
    public class Doodad : Drawable
    {
        public Vector2 position;
        public BoundingBox hitBox;
        public virtual float PosX
        {
            get { return position.X; }
            set { 
                    position.X = value;
                    hitBox.Min.X = value;
                    hitBox.Max.X = hitBox.Min.X;
                    if (sprite != null)
                        hitBox.Max.X += sprite.Width;
                }
        }
        public virtual float PosY
        {
            get { return position.Y; }
            set { 
                    position.Y = value;
                    hitBox.Min.Y = value;
                    hitBox.Max.Y = hitBox.Min.Y;
                    if (sprite != null)
                        hitBox.Max.Y += sprite.Height;
                }
        }
        public override Texture2D Sprite
        {
            get
            {
                return base.Sprite;
            }
            set
            {
                base.Sprite = value;
                hitBox.Max.X = hitBox.Min.X + sprite.Width;
                hitBox.Max.Y = hitBox.Min.Y + sprite.Height;
            }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sprite">The sprite to draw</param>
        /// <param name="position">The position at which to draw it</param>
        public Doodad(Texture2D sprite, Vector2 position)
        {
            this.sprite = sprite;
            this.position = position;
            if (sprite != null)
            {
                hitBox = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X + sprite.Width, position.Y + sprite.Height, 0));
            }
            else
            {
                hitBox = new BoundingBox(new Vector3(position.X, position.Y, 0), new Vector3(position.X, position.Y, 0));
            }
        }
        /// <summary>
        /// This method will do nothing, since this doodad is not animated.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public override void doThis(doPacket pack)
        {
        }
        /// <summary>
        /// This method will draw the doodad to the screen
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawThis(drawPacket pack)
        {
            pack.sb.Draw(sprite, position, Color.White);
        }
        /// <summary>
        /// This method will move this doodad by the target vector.
        /// </summary>
        /// <param name="trajectory">The vector by which to move this.</param>
        public virtual void moveThis(Vector2 trajectory)
        {
            PosX += trajectory.X;
            PosY += trajectory.Y;
        }
        /// <summary>
        /// This will write this object to a binary file
        /// </summary>
        /// <param name="writer">The binary writer to write with</param>
        /// <param name="l">The state to which this belongs</param>
        public override void WriteToFile(System.IO.BinaryWriter writer, LevelEditState l)
        {
            writer.Write((byte)3);
            writer.Write((float)PosX);
            writer.Write((float)PosY);
            if (sprite != null)
            {
                writer.Write((byte)22);
                writer.Write((Int16)l.importedTextures.IndexOf(Sprite));
            }
            else
                writer.Write((byte)99);
            if (name != null || name != "")
            {
                writer.Write((byte)22);
                writer.Write((String)name);
            }
            else
                writer.Write((byte)99);
        }
        public override void Export(LevelEditState l, System.Text.StringBuilder texturesDec, System.Text.StringBuilder texturesDef, System.Text.StringBuilder mainString)
        {
            string path = l.form.lstTextures.Items[l.importedTextures.IndexOf(this.Sprite)].ToString();
            string[] tokens = path.Split('\\');
            path = tokens.Last();
            path = path.Substring(0, path.IndexOf('.'));
            if (!texturesDec.ToString().Contains(path))
            {
                texturesDec.AppendLine("protected Texture2D " + path + ';');
                texturesDef.AppendLine(path + " = content.Load<Texture2D>(\"realassets\\\\" + path + "\");");
            }
            mainString.AppendLine("this.doodads.Add(new Doodad(path, new Vector2(" + position.X + ", " + position.Y + ")));");
        }
        public void Export(LevelEditState l, System.Text.StringBuilder texturesDec, System.Text.StringBuilder texturesDef, System.Text.StringBuilder mainString, Boolean t)
        {
            string path = l.form.lstTextures.Items[l.importedTextures.IndexOf(this.Sprite)].ToString();
            string[] tokens = path.Split('\\');
            path = tokens.Last();
            path = path.Substring(0, path.IndexOf('.'));
            if (!texturesDec.ToString().Contains(path))
            {
                texturesDec.AppendLine("protected Texture2D " + path + ';');
                texturesDef.AppendLine(path + " = content.Load<Texture2D>(\"realassets\\\\" + path + "\");");
            }
            mainString.AppendLine("this.background.Add(new Doodad(path, new Vector2(" + position.X + ", " + position.Y + ")));");
        }
    }
    public class AnimatedDoodad : Doodad
    {
        public Animation animation; //The animation to display
        #region Properties
        public int Width
        {
            get { return animation.Width; }
            set { animation.Width = value; }
        }
        public int Height
        {
            get { return animation.Height; }
            set { animation.Height = value; }
        }
        public int Frames
        {
            get { return animation.Frames; }
            set { animation.Frames = value; }
        }
        public int Rows
        {
            get { return animation.Rows; }
            set { animation.Rows = value; }
        }
        public Boolean Repeat
        {
            get { return animation.Repeat; }
            set { animation.Repeat = value; }
        }
        public int Milliseconds
        {
            get { return animation.Milliseconds; }
            set { animation.Milliseconds = value; }
        }
        public override float PosX
        {
            get
            {
                return base.PosX;
            }
            set
            {
                base.PosX = value;
                animation.PosX = (int)value;
            }
        }
        public override float PosY
        {
            get
            {
                return base.PosY;
            }
            set
            {
                base.PosY = value;
                animation.PosY = (int)value;
            }
        }
        #endregion
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sprite">The sprite to draw</param>
        /// <param name="width">The width of the sprite</param>
        /// <param name="height">The height of the sprite</param>
        /// <param name="frames">The number of frames per row</param>
        /// <param name="rows">The number of rows per sheet</param>
        /// <param name="repeat">True if the animation should repeat</param>
        /// <param name="milliseconds">Number of milliseconds per frame</param>
        /// <param name="position">The position to draw the animation at</param>
        public AnimatedDoodad(Texture2D sprite, int width, int height, int frames, int rows, bool repeat, int milliseconds, Vector2 position)
            : base(sprite, position)
        {
            animation = new Animation(sprite, width, height, frames, rows, repeat, milliseconds);
            this.sprite = sprite;
            this.position = position;
        }
        /// <summary>
        /// This method will animate the animation.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public override void doThis(doPacket pack)
        {
            animation.doThis(pack);
        }
        /// <summary>
        /// This method will draw the animation
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawThis(drawPacket pack)
        {
            animation.drawThis(pack, position);
        }
        /// <summary>
        /// This will change the sprite for this animated doodad
        /// </summary>
        /// <param name="newSprite">The new sprite</param>
        public void changeSprite(Texture2D newSprite)
        {
            sprite = newSprite;
            hitBox.Max.X = hitBox.Min.X + sprite.Width;
            hitBox.Max.Y = hitBox.Min.Y + sprite.Height;
            animation.setSprite(newSprite);
        }
        public override Texture2D Sprite
        {
            get
            {
                return base.Sprite;
            }
            set
            {
                changeSprite(value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="l"></param>
        public override void WriteToFile(System.IO.BinaryWriter writer, LevelEditState l)
        {
            writer.Write((byte)4);
            writer.Write((float)PosX);
            writer.Write((float)PosY);
            if (sprite != null)
            {
                writer.Write((byte)22);
                writer.Write((Int16)l.importedTextures.IndexOf(Sprite));
            }
            else
                writer.Write((byte)99);
            writer.Write((int)Width);
            writer.Write((int)Height);
            writer.Write((int)Frames);
            writer.Write((int)Rows);
            writer.Write((int)Milliseconds);
            writer.Write((Boolean)Repeat);
            if (name != null || name != "")
            {
                writer.Write((byte)22);
                writer.Write((String)name);
            }
            else
                writer.Write((byte)99);
        }
        public override void Export(LevelEditState l, System.Text.StringBuilder texturesDec, System.Text.StringBuilder texturesDef, System.Text.StringBuilder mainString)
        {
            string path = l.form.lstTextures.Items[l.importedTextures.IndexOf(this.Sprite)].ToString();
            string[] tokens = path.Split('\\');
            path = tokens.Last();
            path = path.Substring(0, path.IndexOf('.'));
            if (!texturesDec.ToString().Contains(path))
            {
                texturesDec.AppendLine("protected Texture2D " + path + ';');
                texturesDef.AppendLine(path + " = content.Load<Texture2D>(\"realassets\\\\" + path + "\");");
            }
            mainString.AppendLine("this.doodads.Add(new AnimatedDoodad(" + path + ", " + Width + ", " + Height + ", " + Frames + ", " + Rows + ", " + Repeat + ", " + Milliseconds + ", new Vector2(" + position.X + ", " + position.Y + ")));");
        }
    }
}
