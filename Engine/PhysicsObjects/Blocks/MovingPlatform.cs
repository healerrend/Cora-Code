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
    /// <summary>
    /// This is a wall which moves, and which will take the player along with it if they are standing on it, or push the player if they are in its way.
    /// </summary>
    public class MovingPlatform : Wall
    {
        #region Instance Variables
        protected Point begin; //The beginning point of the moving platform. Always leftmost, and then topmost.
        protected Point end; //The ending point of the moving platform. Always rightmost, and then bottommost.
        protected float height; //The vertical height of the moving platform.
        protected float width; //The horizontal width of the moving platform.
        protected float length; //The magnitude of the space traveled by the moving platform.
        protected float secondsPerCycle; //The number of seconds it takes for the moving platform to go from begin to end.
        protected double animator; //Timer used for animating the moving platform.
        protected MovingPlatformRotationType rotationType; //The rotation type. See Enumerations.
        protected Vector2 curSpeed; //The current velocity of the moving platform.
        protected Boolean isRight; //True if this moving platform is currently moving right.
        protected Boolean isDown; //True if this moving platform is currently moving down.
        protected Boolean setAnimator; //True if animator needs to be reset.
        protected Boolean trajectorySet; //True if the player's movement has been corrected this cycle.
        #endregion
        #region Static Object Pool
        public static Vector2 difference; //Never used, probably don't need this?
        public static float t; //Used for parametized math. Goes from 0 to 1, represents the percent of the way to complete for a given task.
        public static Vector2 playerSpeed; //Used to move the player during a collision.
        #endregion
        #region Properties
        /// <summary>
        /// Gets or sets the x component of the beginning point of this object's path
        /// </summary>
        [Browsable(false)]
        public int BeginX
        {
            get { return begin.X; }
            set { begin.X = value; }
        }
        /// <summary>
        /// Gets or sets the Y coponent of the beginning point of this object's path
        /// </summary>
        [Browsable(false)]
        public int BeginY
        {
            get { return begin.Y; }
            set { begin.Y = value; }
        }
        /// <summary>
        /// Gets or sets the x component of the end point of this object's path
        /// </summary>
        public int EndX
        {
            get { return end.X; }
            set { end.X = value; }
        }
        /// <summary>
        /// Gets or sets the Y component of the end point of this object's path
        /// </summary>
        public int EndY
        {
            get { return end.Y; }
            set { end.Y = value; }
        }
        [Browsable(false)]
        public override float MinX
        {
            get
            {
                return base.MinX;
            }
            set
            {
                base.MinX = value;
                width = dimensions.Max.X - dimensions.Min.X;
            }
        }
        [Browsable(false)]
        public override float MinY
        {
            get
            {
                return base.MinY;
            }
            set
            {
                base.MinY = value;
                height = dimensions.Max.Y - dimensions.Min.Y;
            }
        }
        [Browsable(false)]
        public override float MaxX
        {
            get
            {
                return base.MaxX;
            }
            set
            {
                base.MaxX = value;
                width = dimensions.Max.X - dimensions.Min.X;
            }
        }
        [Browsable(false)]
        public override float MaxY
        {
            get
            {
                return base.MaxY;
            }
            set
            {
                base.MaxY = value;
                height = dimensions.Max.Y - dimensions.Min.Y;
            }
        }
        public override float _X
        {
            get
            {
                return begin.X;
            }
            set
            {
                begin.X = (int)value;
            }
        }
        public override float _Y
        {
            get
            {
                return begin.Y;
            }
            set
            {
                begin.Y = (int)value;
            }
        }
        public float SecondsPerCycle
        {
            get { return secondsPerCycle; }
            set { secondsPerCycle = value; }
        }
        [Browsable(false)]
        public MovingPlatformRotationType RotationType
        {
            get { return rotationType; }
            set { rotationType = value; }
        }
        [Browsable(false)]
        public Boolean IsRight
        {
            get { return isRight; }
        }
        [Browsable(false)]
        public Boolean IsDown
        {
            get { return isDown; }
        }

        #endregion
        /// <summary>
        /// Basic constructor. This one does not contain any of the variables which I doubt are necessary. SecondsPerCycle defaults to 5. Those variables are: runningHoriz, and runningVert
        /// </summary>
        /// <param name="dimensions">The hitbox of this block, and its dimensions.</param>
        /// <param name="l">The level to which this block belongs.</param>
        /// <param name="begin">The point where the movement of this block begins.</param>
        /// <param name="end">The point where the movement of this block ends.</param>
        /// <param name="rotationType">The type of rotation for this moving platform.</param>
        public MovingPlatform(BoundingBox dimensions, LevelState l, Point begin, Point end, MovingPlatformRotationType rotationType)
            : base(l)
        {
            trajectorySet = false;
            this.dimensions = dimensions;
            difference = new Vector2();
            this.begin = begin;
            this.end = end;
            this.rotationType = rotationType;
            isRight = true;
            isDown = true;
            setAnimator = true;
            calculateLength();
            calculateSpeed();
            height = dimensions.Max.Y - dimensions.Min.Y;
            width = dimensions.Max.X - dimensions.Min.X;
            secondsPerCycle = 5;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dimensions">The hitbox of this block, and its dimensions.</param>
        /// <param name="l">The level to which this block belongs.</param>
        /// <param name="begin">The point where the movement of this block begins.</param>
        /// <param name="end">The point where the movement of this block ends.</param>
        /// <param name="rotationType">The type of rotation for this moving platform.</param>
        /// <param name="secondsPerCycle">Number of seconds from point a to b</param>
        /// <param name="runningHoriz">True if there is a horizontal component to this platform's speed.</param>
        /// <param name="runningVert">True if there is a vertical component to this platform's speed.</param>
        public MovingPlatform(BoundingBox dimensions, LevelState l, Point begin, Point end, MovingPlatformRotationType rotationType, float secondsPerCycle, Boolean runningHoriz, Boolean runningVert)
            : base(dimensions, l)
        {
            trajectorySet = false;
            this.dimensions = dimensions;
            difference = new Vector2();
            this.secondsPerCycle = secondsPerCycle;
            this.begin = begin;
            this.end = end;
            this.rotationType = rotationType;
            isDown = true;
            isRight = true;
            setAnimator = true;
            calculateLength();
            calculateSpeed();
            height = dimensions.Max.Y - dimensions.Min.Y;
            width = dimensions.Max.X - dimensions.Min.X;
        }
        public MovingPlatform(BoundingBox dimensions, LevelState l, Point begin, Point end, MovingPlatformRotationType rotationType, float secondsPerCycle, Boolean runningHoriz, Boolean runningVert, Texture2D sprite)
            : base(dimensions, l)
        {
            trajectorySet = false;
            this.dimensions = dimensions;
            difference = new Vector2();
            this.secondsPerCycle = secondsPerCycle;
            this.begin = begin;
            this.end = end;
            this.rotationType = rotationType;
            isDown = true;
            isRight = true;
            setAnimator = true;
            calculateLength();
            calculateSpeed();
            height = dimensions.Max.Y - dimensions.Min.Y;
            width = dimensions.Max.X - dimensions.Min.X;
            this.sprite = sprite;
        }
        /// <summary>
        /// This method primarily calls the collision detection from Wall. Then, if there is a collision, it will move the player with it.
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
            if (enabled)
            {
                postCollision = base.detectCollision(positions, pos, trajectory, nearby, player); //Use the collision detection from Wall.
                if (postCollision.X != trajectory.X || postCollision.Y != trajectory.Y && !trajectorySet) //IF: A collision was detected 
                {
                    if (dimensions.Min.X < pos.X && pos.X < dimensions.Max.X) //IF: pos.x is within the X dimensions of the hitbox
                    {
                        playerSpeed.X = curSpeed.X; //Move the player
                        playerSpeed.Y = curSpeed.Y;
                        player.movePlayer(playerSpeed);
                        player.onGround();
                        trajectorySet = true; //We have now moved the player
                    }
                }
                if (pos == positions[11]) //IF: We are done detecting collision
                    trajectorySet = false; //Reset the switch
            }
            return postCollision;
        }
        /// <summary>
        /// This calls the base class's draw.
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawThis(drawPacket pack)
        {
            base.drawThis(pack);
        }
        /// <summary>
        /// This method handles the movement of the moving platform.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        /// <param name="player">The current player</param>
        public override void doThis(doPacket pack)
        {
            if (!pack.state.paused) //If the game is not paused
            {
                if (setAnimator) //If the animator needs to be reset...
                {
                    animator = 0; //...reset it.
                    setAnimator = false;
                }
                animator += pack.time.ElapsedGameTime.TotalMilliseconds; //Increment animator

                t = (float)(animator) / (secondsPerCycle * 1000); //Calculate t (0-1)
                //Set the position of the moving platform.
                float width = _Width;
                float height = _Height;
                if (isRight)
                    dimensions.Min.X = begin.X + ((end.X - begin.X) * t);
                else
                    dimensions.Min.X = end.X - ((end.X - begin.X) * t);
                if (isDown)
                    dimensions.Min.Y = begin.Y + ((end.Y - begin.Y) * t);
                else
                    dimensions.Min.Y = end.Y - ((end.Y - begin.Y) * t);
                dimensions.Max.X = dimensions.Min.X + width;
                dimensions.Max.Y = dimensions.Min.Y + height;
                if (t > 1) //If the cycle is finished...
                {
                    setAnimator = true; //...reverse the platform.
                    isRight = !isRight;
                    isDown = !isDown;
                    calculateSpeed();
                }
            }
        }
        /// <summary>
        /// This method will calculate the magnitude of the length from begin to end.
        /// </summary>
        public void calculateLength()
        {
            postCollision.X = end.X - begin.X;
            postCollision.Y = end.Y - begin.Y;
            length = postCollision.Length();
        }
        /// <summary>
        /// This method will calculate the current speed of the moving platform using secondsPerCycle and the two end points.
        /// </summary>
        public void calculateSpeed()
        {
            if (isRight)
                curSpeed.X = (end.X - begin.X) / (secondsPerCycle * 60);
            else
                curSpeed.X = -(end.X - begin.X) / (secondsPerCycle * 60);


            if (isDown)
                curSpeed.Y = (end.Y - begin.Y) / (secondsPerCycle * 60);
            else
                curSpeed.Y = -(end.Y - begin.Y) / (secondsPerCycle * 60);
        }
        /// <summary>
        /// This override will move the hit box of this, as well as moving the begin and end points.
        /// </summary>
        /// <param name="trajectory">A vector by which to move this block.</param>
        public override void moveThis(Vector2 trajectory)
        {
            base.moveThis(trajectory);
            begin.X += (int)trajectory.X;
            end.X += (int)trajectory.X;
            begin.Y += (int)trajectory.Y;
            end.Y += (int)trajectory.Y;
        }
        /// <summary>
        /// Returns moving platform plus the coordinates of the platform's start location
        /// </summary>
        /// <returns>Returns moving platform plus the coordinates of the platform's start location</returns>
        public override string ToString()
        {
            if (name != "")
                return "Moving Platform - " + name;
            return "Moving Platform - X: " + begin.X.ToString() + " Y : " + begin.Y.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="l"></param>
        public override void WriteToFile(BinaryWriter writer, LevelEditState l)
        {
            writer.Write((byte)9);
            writer.Write((float)MinX);
            writer.Write((float)MinY);
            writer.Write((float)MaxX);
            writer.Write((float)MaxY);
            writer.Write((int)BeginX);
            writer.Write((int)BeginY);
            writer.Write((int)EndX);
            writer.Write((int)EndY);
            writer.Write((float)SecondsPerCycle);
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
            mainString.AppendLine("this.walls.Add(new MovingPlatform(new BoundingBox(new Vector3(" + _X + ", " + _Y + ", 0), new Vector3(" + (_X + _Width) + ", " + (_Y + _Height)+ ", 0)), this, new Point(" + begin.X + ", " + begin.Y + "), new Point(" + end.X + ", " + end.Y + "), MovingPlatformRotationType.Bouncing, " + secondsPerCycle + ", false, false, " + path + "));");
        }
    }
}
