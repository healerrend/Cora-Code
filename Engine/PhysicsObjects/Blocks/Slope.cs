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
    /// This block represents a slope.
    /// </summary>
    public class Slope : LevelBlock
    {
        #region Instance Variables
        protected Point start; //The point at which the slope begins. Always leftmost.
        protected Point end; //The point at which the slope ends. Always rightmost.
        protected float slope; //The slope of the line representing the floor of this block.
        protected float intercept; //The y intercept of the line representing the floor of this block.
        protected int height; //The total height of the slope
        #endregion
        #region Static Object Pool
        static float m; //The standard form (y=mx+b) of the line representing the incoming trajectory.
        static float b;
        static float intersect; //The x value of the intersect between this block and the incoming trajectory
        #endregion
        #region Properties
        public int StartX
        {
            get { return start.X; }
            set
            {
                start.X = value;
                calculateSlopeIntercept();
            }
        }
        public int StartY
        {
            get { return start.Y; }
            set
            {
                start.Y = value;
                calculateSlopeIntercept();
            }
        }
        public int EndX
        {
            get { return end.X; }
            set
            {
                end.X = value;
                calculateSlopeIntercept();
            }
        }
        public int EndY
        {
            get { return end.Y; }
            set
            {
                end.Y = value;
                calculateSlopeIntercept();
            }
        }
        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        #endregion
        /// <summary>
        /// Standard constructor.
        /// </summary>
        /// <param name="l">The level to which this block belongs.</param>
        /// <param name="s">The leftmost point of the slope.</param>
        /// <param name="e">The rightmost point of the slope.</param>
        public Slope(LevelState l, Point s, Point e)
        {
            level = l;
            height = 33; //This is the minimum height of a slope. NOTE: This should be made configurable
            start = s;
            end = e;
            calculateSlopeIntercept();
            float X; //Max x
            float x; //Min X
            float Y; //Max y
            float y; //Min y
            if (s.X < e.X)
            {
                x = s.X;
                X = e.X;
            }
            else
            {
                x = e.X;
                X = s.X;
            }
            if (s.Y < e.Y)
            {
                y = s.Y;
                Y = e.Y;
            }
            else
            {
                y = e.Y;
                Y = s.Y;
            }
            dimensions = new BoundingBox(new Vector3(x, y, 0), new Vector3(X, Y, 0));
        }
        /// <summary>
        /// This utility method will calculate the slope of the slope and the intercept
        /// </summary>
        public void calculateSlopeIntercept()
        {
            float rise = end.Y - start.Y; //Construct the variables in this slope's standard form equation (y=mx+b)
            float run = end.X - start.X;
            slope = rise / run; //lololol
            intercept = start.Y - (start.X * slope);
        }
        /// <summary>
        /// This method contains collision detection logic. It will process the horizontal movement and then find the appropriate vertical position.
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
            if (dimensions.Intersects(nearby)) //If it is nearby
            {
                Boolean ascendingRight = false; //Is this slope ascending right?
                if (slope < 0)
                    ascendingRight = true;
                if ((ascendingRight && pos == positions[6]) || //Only process the bottom right corner if the slope ascends right, otherwise only process the bottom left corner.
                    (!ascendingRight && pos == positions[9]))
                {
                    if ((pos.Y <= 5 + ((slope * pos.X) + intercept) && //IF: pos crosses the slope's line, with a 5 pixel leeway. (Without leeway, we have a weird physics effect,
                        ((pos.Y + trajectory.Y) > (slope * (pos.X + trajectory.X)) + intercept))) //where the slope would launch the player into the air.)
                    {
                        //Collision is detected.
                        if (trajectory.X < 1) //If the trajectory is close to vertical...
                        {
                            intersect = pos.X; //..then use a vertical trajectory.
                        }
                        else
                        {
                            m = trajectory.Y / trajectory.X; //Find the standard form equation of the incoming trajectory.
                            b = pos.Y - (m * pos.X);
                            intersect = (intercept - b) / (m - slope); //Find the intersect of the two lines. 
                        }
                        if (dimensions.Min.X <= intersect && intersect <= dimensions.Max.X) //If the collision is detected within the horizontal bounds of the slope
                        {
                            postCollision.X = trajectory.X; //Set postcollision
                            postCollision.Y = (slope * (pos.X + postCollision.X)) + intercept;
                            postCollision.Y -= pos.Y;
                            postCollision.Y--;
                            player.onGround(); //Player has landed
                            return postCollision;
                        }
                    }
                }
            }
            return trajectory;
        }
        /// <summary>
        /// This method will draw the slope. NOTE: This method is currently a placeholder. It needs a better implementation.
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawThis(drawPacket pack)
        {
            pack.sb.Draw(TextureLoader.redsquare, new Vector2(start.X, start.Y), Color.White);
            //pack.sb.Draw(TextureLoader.redsquare, new Vector2(end.X, end.Y), Color.White);
        }
        /// <summary>
        /// This method always returns false.
        /// </summary>
        /// <param name="b">-</param>
        /// <returns></returns>
        public override bool intersects(BoundingBox b)
        {
            /*
            if (b.Max.Y <= (slope * b.Max.X) + intercept)
            {
                if (b.Min.Y <= (slope * b.Min.X) + intercept)
                    if (b.Min.Y <= (slope * b.Max.X) + intercept)
                        if (b.Max.Y <= (slope * b.Min.X) + intercept)
                            return false;
            }
            else if(b.Max.Y >= height + (slope * b.Max.X) + intercept) 
            {
                if (b.Min.Y >= height + (slope * b.Min.X) + intercept)
                    if (b.Max.Y >= height + (slope * b.Min.X) + intercept)
                        if (b.Min.Y >= height + (slope * b.Max.X) + intercept)
                            return false;
            }
            return true;
             */
            return false;
        }
        /// <summary>
        /// This will move the hit box of this block, as well as moving the start and end positions of the slope and re-calculate the Y intercept.
        /// </summary>
        /// <param name="trajectory">The vector by which to move this block</param>
        public override void moveThis(Vector2 trajectory)
        {
            base.moveThis(trajectory);
            start.X += (int)trajectory.X;
            end.X += (int)trajectory.X;
            start.Y += (int)trajectory.Y;
            end.Y += (int)trajectory.Y;
            intercept = start.Y - (start.X * slope);
        }
        /// <summary>
        /// Returns slope plus the coordinates of the start point of this slope.
        /// </summary>
        /// <returns>Returns slope plus the coordinates of the start point of this slope.</returns>
        public override string ToString()
        {
            if (name != "")
                return "Slope - " + name;
            return "Slope - X: " + start.X.ToString() + " Y: " + start.Y.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="l"></param>
        public override void WriteToFile(BinaryWriter writer, LevelEditState l)
        {
            writer.Write((byte)12);
            writer.Write((int)StartX);
            writer.Write((int)StartY);
            writer.Write((int)EndX);
            writer.Write((int)EndY);
            writer.Write((int)Height);
            writer.Write((Int16)l.importedTextures.IndexOf(Sprite));
        }
    }
}
