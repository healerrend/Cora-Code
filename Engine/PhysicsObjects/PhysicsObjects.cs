using System;
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
        public virtual void WriteToFile(BinaryWriter writer, LevelEditState l)
        {}
    }
}