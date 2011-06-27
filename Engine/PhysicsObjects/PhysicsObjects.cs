using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

/* This file contains all classes on which physics or collisions will act.
 */

namespace CORA
{
    public abstract class Drawable
    {
        protected Texture2D sprite;
        protected String name = "";
        protected float depth = .5f;
        protected SpriteEffects effect = SpriteEffects.None;
        protected Color tint = Color.White;
        protected Vector2 origin = new Vector2(0, 0);
        public Boolean enabled = true;
        public Boolean visible = true;
        public virtual Texture2D Sprite
        {
            get { return sprite; }
            set { sprite = value; }
        }
        public  virtual String Name
        {
            get { return name; }
            set { name = value; }
        }
        public float Depth
        {
            get { return depth; }
            set { depth = value; }
        }
        public SpriteEffects Effect
        {
            get { return effect; }
            set { effect = value; }
        }
        public Color Tint
        {
            get { return tint; }
            set { tint = value; }
        }
        public virtual float OriginX
        {
            get { return origin.X; }
            set { origin.X = value; }
        }
        public virtual float OriginY
        {
            get { return origin.Y; }
            set { origin.Y = value; }
        }
        public virtual void WriteToFile(BinaryWriter writer, LevelEditState l)
        {}
        public virtual void Export(LevelEditState l, StringBuilder texturesDec, StringBuilder texturesDef, StringBuilder mainString)
        {}
        public virtual void drawThis(drawPacket pack)
        {}
        public virtual void doThis(doPacket pack) { }
    }
}