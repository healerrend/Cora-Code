using System;
using System.ComponentModel;
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

namespace CORA
{
    public class Elevator : LevelBlock
    {
        public List<MovingPlatform> walls;
        public LevelState level;
        public Elevator(LevelState level) : base()
        {
            this.level = level;
        }
        public override Vector2 detectCollision(List<CollisionPoint> positions, CollisionPoint pos, Vector2 trajectory, BoundingSphere nearby, Player player)
        {
            throw new NotImplementedException();
        }
        public override bool intersects(BoundingBox b)
        {
            throw new NotImplementedException();
        }
        public override bool intersects(BoundingSphere s)
        {
            throw new NotImplementedException();
        }
    }
}
