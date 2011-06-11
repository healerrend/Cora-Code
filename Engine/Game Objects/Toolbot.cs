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
    public class Toolbot : Minibot
    {
        public Boolean isReleased;
        public Boolean canWalk = true;
        public Toolbot(Texture2D s, List<LevelBlock> walls, LevelState l, Vector2 position) : base(s, walls, l, position) 
        {
            isReleased = false;
            type = InteractorType.toolbot;
            drawBox = new Rectangle(0, 0, 100, 100);
            hitBox = new BoundingBox(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
            animBox = new Rectangle(0, 0, 100, 100);

            points[0].X = position.X;
            points[9].X = position.X;
            points[10].X = position.X;
            points[11].X = position.X;
            points[1].X = position.X + 16;
            points[8].X = position.X + 16;
            points[2].X = position.X + 33;
            points[7].X = position.X + 33;
            points[3].X = position.X + 50;
            points[6].X = position.X + 50;
            points[4].X = position.X + 50;
            points[5].X = position.X + 50;

            points[0].Y = position.Y;
            points[1].Y = position.Y;
            points[2].Y = position.Y;
            points[3].Y = position.Y;
            points[4].Y = position.Y + 16;
            points[11].Y = position.Y + 16;
            points[5].Y = position.Y + 33;
            points[10].Y = position.Y + 33;
            points[6].Y = position.Y + 50;
            points[7].Y = position.Y + 50;
            points[8].Y = position.Y + 50;
            points[9].Y = position.Y + 50;
        }
        public override void Dock(Player p)
        {
            throw new NotImplementedException();
        }
        public override void LaunchAir(Player p)
        {
            throw new NotImplementedException();
        }
        public override void LaunchGround(Player p)
        {
            throw new NotImplementedException();
        }
        public override void doThis(doPacket pack)
        {
            //Do physics
            doPhysics(pack);
            velocity += acceleration;

            doAI(pack);
            //Collision Detection
            nearby.Center.X = points[6].X; //Set coords of nearby
            nearby.Center.Y = points[6].Y;
            nearby.Radius = 150f; //Should put some logic here

            //Generic collision detection
            detectCollisions(pack);
            //Finalize movement
            finalizeMovement(pack);
            position.X = points[0].X;
            position.Y = points[0].Y;
        }
        public override void drawThis(drawPacket pack)
        {
            pack.sb.Draw(sprite, new Rectangle((int)position.X, (int)position.Y, 50, 50), Color.White);
        }
        private void doAI(doPacket pack)
        {
            //Follow player
            if (!isAirborne)
            {
                float xDiff = position.X + 25 - level.player.position.X;
                if (xDiff < -250)
                {
                    velocity.X = 7;
                    isRight = true;
                }
                else if (xDiff > 250)
                {
                    velocity.X = -7;
                    isRight = false;
                }
                else if (xDiff < -7)
                {
                    velocity.X = 5;
                    isRight = true;
                }
                else if (xDiff > 7)
                {
                    velocity.X = -5;
                    isRight = false;
                }
                else
                    velocity.X = 0;
                //AI and future sight
                //Find gaps
                CollisionPoint danger;
                if (isRight)
                {
                    danger = points[8];
                    poolBox.Min.X = danger.X;
                    poolBox.Max.X = danger.X + 25;
                }
                else
                {
                    danger = points[7];
                    poolBox.Min.X = danger.X - 25;
                    poolBox.Max.X = danger.X;
                }

                poolBox.Min.Y = danger.Y;
                poolBox.Max.Y = danger.Y + 60;
                canWalk = false;
                foreach (LevelBlock w in walls)
                {
                    if (w.intersects(poolBox))
                        canWalk = true;
                }
                if (!canWalk)
                {
                    //Can we jump over it?
                    if (isRight)
                        poolBox.Max.X += 125;
                    else
                        poolBox.Min.X -= 125;
                    foreach (LevelBlock w in walls)
                    {
                        if (w.intersects(poolBox))
                            canWalk = true;
                    }
                    if (canWalk && ((isRight && xDiff < -175) || (!isRight && xDiff > 175)))
                    {                        
                        velocity.Y = -12;
                        if (isRight)
                            velocity.X = 7;
                        else
                            velocity.X = -7;
                        isAirborne = true;
                    }
                    else
                        velocity.X = 0;
                }
                if (isRight)
                {
                    danger = points[6];
                    poolBox.Min.X = danger.X;
                    poolBox.Max.X = danger.X + 15;
                }
                else
                {
                    danger = points[9];
                    poolBox.Min.X = danger.X - 15;
                    poolBox.Max.X = danger.X;
                }
                poolBox.Min.Y = danger.Y - 50;
                poolBox.Max.Y = danger.Y - 1;
                canWalk = true;
                foreach (LevelBlock w in walls)
                {
                    if (w.intersects(poolBox))
                        canWalk = false;
                }
                if (!canWalk)
                {
                    //Can we jump over it?
                    poolBox.Min.Y = danger.Y - 60;
                    poolBox.Max.Y = danger.Y - 51;
                    canWalk = true;
                    foreach (LevelBlock w in walls)
                    {
                        if (w.intersects(poolBox))
                            canWalk = false;
                    }
                    if (canWalk)
                    {
                        velocity.Y = -12;
                        isAirborne = true;
                    }
                }
            }
            else
            {
                if (isRight && velocity.X < 1)
                    velocity.X = 1;
                else if (!isRight && velocity.X > -1)
                    velocity.X = -1;
            }
        }
    }
}
