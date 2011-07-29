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
    public class Door : LevelBlock
    {
        public LevelState level;
        public Vector2 drawPos;
        public Wall hitBox;
        public Animation animation;
        public double openCloseTime;
        public double timer;
        public Boolean isOpen;
        public Boolean isMoving;
        public Boolean closing;
        public Door(LevelState level, Wall hitBox, Animation animation, double openCloseTime)
            : base()
        {
            this.level = level;
            this.hitBox = hitBox;
            this.animation = animation;
            this.openCloseTime = openCloseTime;
            timer = 0;
            closing = false;
            isOpen = false;
            isMoving = false;
            position.X = (int)hitBox._X;
            position.Y = (int)hitBox._Y;
            drawPos = new Vector2(hitBox._X, hitBox._Y);
        }
        public override void doThis(doPacket pack)
        {
            hitBox.doThis(pack);
            animation.doThis(pack);
            if (isMoving)
            {
                timer += pack.time.ElapsedGameTime.TotalMilliseconds;
                if (timer >= openCloseTime)
                    animationDone();
            }
        }
        public override void drawThis(drawPacket pack)
        {
            if (!isOpen)
                animation.drawThis(pack, drawPos);
        }
        public override Vector2 detectCollision(List<CollisionPoint> positions, CollisionPoint pos, Vector2 trajectory, BoundingSphere nearby, Player player)
        {
            if(!isOpen)
                return hitBox.detectCollision(positions, pos, trajectory, nearby, player);
            return trajectory;
        }
        public override bool intersects(BoundingBox b)
        {
            if (!isOpen)
                return hitBox.intersects(b);
            return false;
        }
        public override bool intersects(BoundingSphere s)
        {
            if (!isOpen)
                return hitBox.intersects(s);
            return false;
        }
        public void toggle()
        {
            isMoving = true;
            closing = !closing;
            if (closing)
                animation.setRow(0);
            else
                animation.setRow(1);
        }
        private void resetAnimation()
        {
            animation.setFrame(0);
        }
        private void animationDone()
        {
            resetAnimation();
            isMoving = false;
            isOpen = !isOpen;
        }

    }
}
