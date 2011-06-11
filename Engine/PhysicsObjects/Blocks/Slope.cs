using System;
using System.Text;
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
        [Browsable(false)]
        public override float _Height
        {get { return base._Height; }}
        [Browsable(false)]
        public override float _Width
        {get { return base._Width; }}
        [Browsable(false)]
        public override float _X
        { get { return base._X; } }
        [Browsable(false)]
        public override float _Y
        { get { return base._Y; } }
        [Browsable(false)]
        public override float MaxX
        { get { return base.MaxX; } }
        [Browsable(false)]
        public override float MaxY
        {get{return base.MaxY;}}
        [Browsable(false)]
        public override float MinX
        {get{return base.MinX;}}
        [Browsable(false)]
        public override float MinY
        {get{return base.MinY;}}
        [Browsable(false)]
        public override float OriginX
        {get{return base.OriginX;}}
        [Browsable(false)]
        public override float OriginY
        {get{return base.OriginY;}}
        [Browsable(false)]
        public override float Rotation
        {get{return base.Rotation;}}
        [Browsable(false)]
        public override bool RepeatY
        {get{return base.RepeatY;}}
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
            origin.X = 0;
            origin.Y = 0;
            rotation = (float)Math.Atan(slope);

        }
        /// <summary>
        /// This method contains collision detection logic. It will process the horizontal movement and then find the appropriate vertical position..
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
                else if ((ascendingRight && pos == positions[0]) || (!ascendingRight && pos == positions[3]))
                {
                    if((pos.Y >= height + ((slope * pos.X) + intercept) - 5) && ((pos.Y + trajectory.Y) < (slope * (pos.X + trajectory.X)) + intercept + height))
                    {
                        if (dimensions.Min.X <= pos.X && pos.X <= dimensions.Max.X)
                        {
                            postCollision.X = 0;
                            postCollision.Y = 2;
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
            float length = (float)Math.Sqrt((Math.Pow((end.X - start.X), 2)) + Math.Pow((end.Y - start.Y), 2));
            if (repeatX)
            {
                float progress = 0;
                int counter = 0;
                drawRect.Height = height;
                drawRect.Width = sprite.Width;
                spriteRect.X = 0;
                spriteRect.Y = 0;
                spriteRect.Height = sprite.Height;
                spriteRect.Width = sprite.Width;
                while (progress < length)
                {
                    drawRect.X = (int)(start.X + (counter * (Math.Cos(rotation) * sprite.Width)));
                    drawRect.Y = (int)(start.Y + (counter * (Math.Sin(rotation) * sprite.Width)));
                    if (length - progress < sprite.Width)
                    {
                        drawRect.Width = (int)(length - progress) + 1;
                        spriteRect.Width = (int)(length - progress);
                    }
                    pack.sb.Draw(sprite, drawRect, spriteRect, tint, rotation, origin, effect, depth);
                    progress += sprite.Width;
                    counter++;
                }
            }
            else
            {
                drawRect.X = start.X;
                drawRect.Y = start.Y;
                drawRect.Height = height;
                drawRect.Width = (int)length + 1;
                pack.sb.Draw(sprite, drawRect, null, tint, rotation, origin, effect, depth);
            }
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
        public override void Export(LevelEditState l, StringBuilder texturesDec, StringBuilder texturesDef, StringBuilder mainString)
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
            mainString.AppendLine("this.walls.Add(new Slope(this, new Point(" + start.X + ", " + start.Y + "), new Point(" + end.X + ", " + end.Y + ")));");
        }
    }
}
