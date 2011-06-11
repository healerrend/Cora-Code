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

namespace CORA
{
    /// <summary>
    /// The wall is the quintessential LevelBlock. It involves a hit box and collision detection for that hitbox.
    /// When a collision is queried against a Wall, it will check for all four of its sides. All walls are rectangles.
    /// </summary>
    public class Wall : LevelBlock
    {
        /// <summary>
        /// Basic constructor. This should probably not be used.
        /// </summary>
        /// <param name="l"></param>
        public Wall(LevelState l)
        {
            dimensions = new BoundingBox();
            level = l;
        }
        /// <summary>
        /// Standard constructor.
        /// </summary>
        /// <param name="b">This bounding box represents the dimensions of the wall.</param>
        /// <param name="l">The level to which this belongs.</param>
        public Wall(BoundingBox b, LevelState l)
        {
            dimensions = b;
            level = level;
        }
        /// <summary>
        /// This collision detection will check all four sides and return a final velocity.
        /// </summary>
        /// <param name="positions">A list containing every point an object intends to check for collisions. This parameter is primarily used to perform different logic on different points. 
        /// For instance, a floor may only need to check the bottom points, and for all other points, may skip any logic at all.</param>
        /// <param name="pos">The collision point that is currently being tested for collision.</param>
        /// <param name="trajectory">The velocity vector which represent's where the current point is going.</param>
        /// <param name="nearby">A sphere of variable radius which represents how close an object needs to be to be considered possible for collision.
        /// If the block's hit box does not intersect this sphere, no logic should be applied.</param>
        /// <param name="player">The player against whom the collision is being tested.</param>
        /// <returns>A final velocity. If there is no collision, this will return trajectory. Otherwise, it will return a velocity vector appropriate to the collision which took place.</returns>
        public override Vector2 detectCollision(List<CollisionPoint> positions, CollisionPoint pos, Vector2 trajectory, BoundingSphere nearby, Player player)
        {
            //If the wall is not nearby, skip all logic and return trajectory.
            if (nearby.Intersects(dimensions))
            {
                postCollision.X = 0; //Initialize postCollision
                postCollision.Y = 0;
                //Check floors and ceilings
                if ((pos.Y <= dimensions.Min.Y && pos.Y + trajectory.Y >= dimensions.Min.Y) //IF: pos.Y is over the top line of the box and travels through the line within the bounds of the box's width
                    && (dimensions.Min.X <= pos.X && pos.X <= dimensions.Max.X))
                {
                    if (pos == positions[6] || pos == positions[7] || pos == positions[8] || pos == positions[9]) //IF: The current position is on the bottom of the player's hitbox
                    {
                        player.onGround(); //Confirm player has touched the ground
                        postCollision.Y = dimensions.Min.Y - pos.Y - 1; //Set postcollision. Give one pixel leeway.
                    }
                }
                else if ((pos.Y >= dimensions.Max.Y && pos.Y + trajectory.Y <= dimensions.Max.Y) //IF: pos.Y is under the bottom line of the box and travels through the line within the bounds of the box's width
                    && (dimensions.Min.X <= pos.X && pos.X <= dimensions.Max.X))
                    if (pos == positions[0] || pos == positions[1] || pos == positions[2] || pos == positions[3]) //IF: The current position is on the top of the player's hitbox
                    {
                        postCollision.Y = dimensions.Max.Y - pos.Y + 2; //Set postcollision. Give 2 pixels leeway.
                    }
                    else { }
                else
                    postCollision.Y = trajectory.Y; //No floor or ceiling collision.

                //Check walls
                if ((pos.X >= dimensions.Max.X && pos.X + trajectory.X <= dimensions.Max.X) //IF: pos.x is right of the rightmost side and travels through the rightmost side within the bounds of the box's height
                    && (dimensions.Min.Y <= pos.Y && pos.Y <= dimensions.Max.Y))
                    postCollision.X = dimensions.Max.X - pos.X + 2; //Set postcollision. Give 2 pixels leeway.
                else if ((pos.X <= dimensions.Min.X && pos.X + trajectory.X >= dimensions.Min.X) //IF: pos.x is left of the leftmost side and travels through the leftmost side within the bounds of the box's height
                    && (dimensions.Min.Y <= pos.Y && pos.Y <= dimensions.Max.Y))
                    postCollision.X = dimensions.Min.X - pos.X - 2; //Set postcollision. Give 2 pixels leeway.
                else
                    postCollision.X = trajectory.X; //No wall collision
                //If, after the collision detection logic happens, position is still going to end up inside the wall, push it out moving the least amount possible.
                if (dimensions.Contains(new Vector3(pos.X + postCollision.X, pos.Y + postCollision.Y, 0)) == ContainmentType.Contains)
                {
                    float x = pos.X + postCollision.X; //Current final x coordinate
                    float y = pos.Y + postCollision.Y; //Current final y coordinate
                    float left = x - dimensions.Min.X; //Distance between final coords and the left side
                    float right = dimensions.Max.X - x; //Distance between final coords and right side
                    float top = dimensions.Min.Y - y; //Distance between final coords and top
                    float bottom = y - dimensions.Min.Y; //Distance between final coords and bottom
                    //Find out which one is least, then push.
                    if (top <= bottom)
                    {
                        if (Math.Abs(left) <= Math.Abs(right))
                        {
                            if (Math.Abs(top) <= Math.Abs(left))
                                postCollision.Y -= (top + 1);
                            else
                                postCollision.X -= (left + 1);
                        }
                        else
                        {
                            if (Math.Abs(top) <= Math.Abs(right))
                                postCollision.Y -= (top + 1);
                            else
                                postCollision.X += (right + 1);
                        }
                    }
                    else
                    {
                        if (left <= right)
                        {
                            if (bottom <= left)
                                postCollision.Y += (bottom + 1);
                            else
                                postCollision.X -= (left + 1);
                        }
                        else
                        {
                            if (bottom <= right)
                                postCollision.Y += (bottom + 1);
                            else
                                postCollision.X += (right + 1);
                        }
                    }
                }
                return postCollision; //Return final velocity
            }
            else
            {
                return trajectory; //Return original velocity
            }
        }
        /// <summary>
        /// Checks intersection between the wall's hitbox and b
        /// </summary>
        /// <param name="b">A bounding box</param>
        /// <returns>True if b intersects the wall, otherwise false.</returns>
        public override Boolean intersects(BoundingBox b)
        {
            return dimensions.Intersects(b);
        }
        /// <summary>
        /// Draws the wall. NOTE: Needs to support different sprites! Steps:
        /// 1. Modify the constructor of Wall to accept a Texture2D
        /// 2. Add an instance variable of type Texture2D
        /// 3. Modify this method to use that variable
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawThis(drawPacket pack)
        {
            pack.sb.Draw(sprite, new Rectangle((int)dimensions.Min.X, (int)dimensions.Min.Y, (int)(dimensions.Max.X - dimensions.Min.X), (int)(dimensions.Max.Y - dimensions.Min.Y)), Color.White);
        }
        /// <summary>
        /// Returns wall + the minimum coordinates of the hit box
        /// </summary>
        /// <returns>Returns wall + the minimum coordinates of the hit box</returns>
        public override string ToString()
        {
            return "Wall - " + base.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="l"></param>
        public override void WriteToFile(BinaryWriter writer, LevelEditState l)
        {
            writer.Write((byte)13);
            writer.Write((float)MinX);
            writer.Write((float)MinY);
            writer.Write((float)MaxX);
            writer.Write((float)MaxY);
            writer.Write((Int16)l.importedTextures.IndexOf(Sprite));
        }
    }
}
