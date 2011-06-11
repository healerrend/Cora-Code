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

namespace CORA
{
    public class CollisionPoint
    {
        public float X; //The x component
        public float Y; //The y component
        public Point p; //A point
        public CollisionPoint()
        {
            X = 0;
            Y = 0;
            p = new Point(0, 0);
        }
        public CollisionPoint(int x, int y)
        {
            this.X = x;
            this.X = y;
            p = new Point(x, y);
        }

    }
}
