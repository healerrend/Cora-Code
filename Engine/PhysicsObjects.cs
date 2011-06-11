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
    //*******************************************************************
    // LEVEL BLOCK AND CHILDREN
    //*******************************************************************
    /// <summary>
    /// Levelblock is the abstract representation of an obstacle which cannot be moved through. Platforms are the quintessential LevelBlock.
    /// NOTE: NEED TO ADD SPRITE SUPPORT TO ALL CHILDREN OF LEVELBLOCK
    /// </summary>
    public abstract class LevelBlock : Drawable
    {
        #region Instance Variables
        protected BoundingBox dimensions;  //The hitbox of the level block.
        protected LevelState level;        //The level to which this level block belongs.
        #endregion
        #region Static Object Pool
        //These variables are available for temporary use and are declared in order to assist in controlling garbage collection.
        public static Point position = new Point(); //Usually a player's position.
        public static Vector2 collision = new Vector2(); //Usually the velocity vector calculated by collision detection.
        public static Vector2 postCollision = new Vector2(); //Usually a second vector for use in collision detection.
        #endregion
        #region Properties
        /// <summary>
        /// Gets or sets the hit box for this
        /// </summary>
        public BoundingBox Dimensions
        {
            get { return dimensions; }
            set { dimensions = value; }
        }
        /// <summary>
        /// Gets or sets the x component of the minimum point of this object's hitbox.
        /// </summary>
        public virtual float MinX
        {
            get { return dimensions.Min.X; }
            set { dimensions.Min.X = value; }
        }
        /// <summary>
        /// Gets or sets the x component of the maximum point of this object's hitbox
        /// </summary>
        public virtual float MaxX
        {
            get { return dimensions.Max.X; }
            set { dimensions.Max.X = value; }
        }
        /// <summary>
        /// Gets or sets the Y component of the minimum point of this object's hitbox
        /// </summary>
        public virtual float MinY
        {
            get { return dimensions.Min.Y; }
            set { dimensions.Min.Y = value; }
        }
        /// <summary>
        /// Gets or sets the Y component of the maximum point of this object's hitbox
        /// </summary>
        public virtual float MaxY
        {
            get { return dimensions.Max.Y; }
            set { dimensions.Max.Y = value; }
        }

        #endregion
        /// <summary>
        /// Must be overridden. This method is called and performs all collision detection logic. The vector which is returned will REPLACE the velocity vector of the object which it collides with.
        /// For instance, if the player's velocity is {2,9}, and this method returns {0,0}, the player will have zero velocity after this method returns.
        /// </summary>
        /// <param name="positions">A list containing every point an object intends to check for collisions. This parameter is primarily used to perform different logic on different points. 
        /// For instance, a floor may only need to check the bottom points, and for all other points, may skip any logic at all.</param>
        /// <param name="pos">The collision point that is currently being tested for collision.</param>
        /// <param name="trajectory">The velocity vector which represent's where the current point is going.</param>
        /// <param name="nearby">A sphere of variable radius which represents how close an object needs to be to be considered possible for collision.
        /// If the block's hit box does not intersect this sphere, no logic should be applied.</param>
        /// <param name="player">The player against whom the collision is being tested.</param>
        /// <returns>A final velocity. If there is no collision, this will return trajectory. Otherwise, it will return a velocity vector appropriate to the collision which took place.</returns>
        public abstract Vector2 detectCollision(List<CollisionPoint> positions, CollisionPoint pos, Vector2 trajectory, BoundingSphere nearby, Player player);
        /// <summary>
        /// Must be overridden. This method will return true if the bounding box passed in intersects the block.
        /// </summary>
        /// <param name="b">A bounding box to check for intersection.</param>
        /// <returns></returns>
        public abstract Boolean intersects(BoundingBox b);
        /// <summary>
        /// Override is optional. This method is called on any block which must be drawn to the screen. All drawing logic should be contained within this method. It should not update any variables or states.
        /// This Draw will be called during the Draw World phase of drawing.
        /// </summary>
        /// <param name="pack">See drawPacket</param>
        public virtual void drawThis(drawPacket pack) { }
        /// <summary>
        /// Override is optional. This method is called on any block which must have some internal logic completed at any time a collision is not detected.
        /// </summary>
        /// <param name="pack">See doPacked</param>
        /// <param name="player">The current player</param>
        public virtual void doThis(doPacket pack, Player player) { }
        /// <summary>
        /// Returns the X and Y coordinates of the minimum point on the hitbox of this block.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (name != "")
                return name;
            return "X: " + dimensions.Min.X + " Y: " + dimensions.Min.Y;
        }
        /// <summary>
        /// This method will move the block by the vector parameter. The default version will move the hit box.
        /// </summary>
        /// <param name="trajectory">A vector by which to move this block.</param>
        public virtual void moveThis(Vector2 trajectory)
        {
            dimensions.Min.X += trajectory.X;
            dimensions.Max.X += trajectory.X;
            dimensions.Min.Y += trajectory.Y;
            dimensions.Max.Y += trajectory.Y;
        }
    }
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
    /// <summary>
    /// Rust is a type of wall which disappears soon after it is stepped on. Rust will not regenerate (unless we end up deciding it should). Otherwise, it behaves exactly like a wall.
    /// </summary>
    public class Rust : Wall
    {
        #region Instance Variables
        public Boolean disappearing = false; //True if this is in the process of disappearing, otherwise false.
        protected double disappearTime = 0; //Number of milliseconds since this started disappearing.
        protected double disappearLength; //Number of milliseconds from when this is collided with until it disappears.
        #endregion
        #region Properties
        public double DisappearLength
        {
            get { return disappearLength; }
            set { disappearLength = value; }
        }
        #endregion
        /// <summary>
        /// Default constructor. This will initialize disappearLength to 1500 milliseconds.
        /// </summary>
        /// <param name="b">The dimensions of the block.</param>
        /// <param name="l">The level to which this block belongs.</param>
        public Rust(BoundingBox b, LevelState l)
            : base(b, l)
        {
            disappearLength = 1500;
        }
        /// <summary>
        /// This constructor allows you to set the length of time the rust will remain on screen before disappearing.
        /// </summary>
        /// <param name="b">The dimensions of the block.</param>
        /// <param name="l">The level to which this block belongs.</param>
        /// <param name="t">The number of milliseconds from when this is collided with until it disappears.</param>
        public Rust(BoundingBox b, LevelState l, double t)
            : base(b, l)
        {
            disappearTime = t;
        }
        /// <summary>
        /// This method primarily calls the collision detection of wall, but if a collision is detected, it will also initiate the object's disappearance.
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
            postCollision = base.detectCollision(positions, pos, trajectory, nearby, player);
            if (postCollision.X != trajectory.X || postCollision.Y != trajectory.Y) //IF: trajectory is different than the return velocity (IE, collision has happened)
                disappearing = true; //Initiate disappearance
            return postCollision; //Pass the final velocity along
        }
        /// <summary>
        /// This method only applies its logic if this is disappearing. If it is, it will handle that logic.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        /// <param name="player">The current player</param>
        public override void doThis(doPacket pack, Player player)
        {
            if (disappearing)
            {
                disappearTime += pack.time.ElapsedGameTime.TotalMilliseconds; //Increments the timer by the number of milliseconds since the last update.
                if (disappearTime >= disappearLength) //IF: The timer is up
                {
                    //NOTE: This should probably be changed to DEACTIVATE it instead of removing it, otherwise we'll probably have trouble when we have to load the same level more than once per session.
                    level.walls.Remove(this); //Remove this from the level
                }
            }
        }
        /// <summary>
        /// Returns rust plus the minimum dimensions of the hit box
        /// </summary>
        /// <returns>Returns rust plus the minimum dimensions of the hit box</returns>
        public override string ToString()
        {
            return "Rust - " + base.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="l"></param>
        public override void WriteToFile(BinaryWriter writer, LevelEditState l)
        {
            writer.Write((byte)11);
            writer.Write((float)MinX);
            writer.Write((float)MinY);
            writer.Write((float)MaxX);
            writer.Write((float)MaxY);
            writer.Write((double)DisappearLength);
            writer.Write((Int16)l.importedTextures.IndexOf(Sprite));
        }
    }
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
        public int BeginX
        {
            get { return begin.X; }
            set { begin.X = value; }
        }
        /// <summary>
        /// Gets or sets the Y coponent of the beginning point of this object's path
        /// </summary>
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
        public float Height
        {
            get { return height; }
            set
            {
                height = value;
                dimensions.Max.Y = dimensions.Min.Y + height;
            }
        }
        public float Width
        {
            get { return width; }
            set
            {
                width = value;
                dimensions.Max.X = dimensions.Min.X + width;
            }
        }
        public float SecondsPerCycle
        {
            get { return secondsPerCycle; }
            set { secondsPerCycle = value; }
        }
        public MovingPlatformRotationType RotationType
        {
            get { return rotationType; }
            set { rotationType = value; }
        }
        public Boolean IsRight
        {
            get { return isRight; }
        }
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
        public MovingPlatform(BoundingBox dimensions, LevelState l, Point begin, Point end, MovingPlatformRotationType rotationType) : base(l)
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
        public MovingPlatform(BoundingBox dimensions, LevelState l, Point begin, Point end, MovingPlatformRotationType rotationType, float secondsPerCycle, Boolean runningHoriz, Boolean runningVert) : base(dimensions, l)
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
            postCollision = base.detectCollision(positions, pos, trajectory, nearby, player); //Use the collision detection from Wall.
            if (postCollision.X != trajectory.X || postCollision.Y != trajectory.Y && !trajectorySet) //IF: A collision was detected 
            {
                if(dimensions.Min.X < pos.X && pos.X < dimensions.Max.X) //IF: pos.x is within the X dimensions of the hitbox
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
        public override void doThis(doPacket pack, Player player)
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
                curSpeed.X = (end.X-begin.X) / (secondsPerCycle * 60);
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
            writer.Write((Int16)l.importedTextures.IndexOf(Sprite));
        }
    }
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
            set { 
                    start.X = value;
                    calculateSlopeIntercept();
                }
        }
        public int StartY
        {
            get { return start.Y; }
            set { 
                    start.Y = value;
                    calculateSlopeIntercept();
                }
        }
        public int EndX
        {
            get { return end.X; }
            set { 
                    end.X = value;
                    calculateSlopeIntercept();
                }
        }
        public int EndY
        {
            get { return end.Y; }
            set { 
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
            if(dimensions.Intersects(nearby)) //If it is nearby
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
    //*******************************************************************
    // HIT BOX INTERACTABLE AND CHILDREN
    //*******************************************************************
    /// <summary>
    /// HitBoxInteractable represents any object in a level which is interacted with by the player using a hit box.
    /// </summary>
    public abstract class HitBoxInteractable : Drawable
    {
        #region Instance Variables
        private Boolean isCollided; //True if this has collided with the player, otherwise false.
        protected BoundingBox hitBox; //This object's hit box
        protected LevelState level; //The level this belongs to
        private Rectangle drawRect; //The rectangle to which this object should be drawn
        #endregion
        #region Properties
        public BoundingBox HitBox
        {
            get { return hitBox; }
            set { hitBox = value; }
        }
        public float MinX
        {
            get { return hitBox.Min.X; }
            set { hitBox.Min.X = value; }
        }
        public float MinY
        {
            get { return hitBox.Min.Y; }
            set { hitBox.Min.Y = value; }
        }
        public float MaxX
        {
            get { return hitBox.Max.X; }
            set { hitBox.Max.X = value; }
        }
        public float MaxY
        {
            get { return hitBox.Max.Y; }
            set { hitBox.Max.Y = value; }
        }
        #endregion
        /// <summary>
        /// Standard constructor.
        /// </summary>
        /// <param name="b">The hit box for this object.</param>
        /// <param name="l">The level this object belongs to.</param>
        /// <param name="s">The sprite for this object.</param>
        public HitBoxInteractable(BoundingBox b, LevelState l, Texture2D s)
        {
            hitBox = b;
            level = l;
            sprite = s;
            drawRect = new Rectangle((int)b.Min.X, (int)b.Min.Y, (int)(b.Max.X - b.Min.X), (int)(b.Max.Y - b.Min.Y)); //DrawRect defaults to the hit box
        }
        /// <summary>
        /// Override is not required. This method executes collision detection logic. It will return true if a collision is detected.
        /// </summary>
        /// <param name="b">The hit box of the other object.</param>
        /// <returns>True if the objects collide, otherwise false.</returns>
        public virtual Boolean detectCollision(BoundingBox b)
        {
            isCollided = b.Intersects(hitBox);
            return isCollided;
        }
        /// <summary>
        /// Override is not required. This method will execute any internal logic for the interactable.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public virtual void doThis(doPacket pack) { }
        /// <summary>
        /// Override required. This method will draw this object.
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public virtual void drawThis(drawPacket pack) 
        {
            pack.sb.Draw(sprite, drawRect, Color.White);
        }
        /// <summary>
        /// Override is required. This method will handle the logic when a collision is detected.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        /// <param name="p">The player which is affected</param>
        /// <returns>True if the player has been affected, otherwise false.</returns>
        public virtual Boolean effectPlayer(doPacket pack, Player p)
        {
            return false;
        }
        /// <summary>
        /// This method will move this interactable by the given trajectory. The default will only move the hit box and the draw rectangle.
        /// </summary>
        /// <param name="trajectory">The vector by which to move this interactable.</param>
        public virtual void moveThis(Vector2 trajectory)
        {
            hitBox.Min.X += trajectory.X;
            hitBox.Max.X += trajectory.X;
            hitBox.Min.Y += trajectory.Y;
            hitBox.Max.Y += trajectory.Y;
            drawRect.X += (int)trajectory.X;
            drawRect.Y += (int)trajectory.Y;
        }
        /// <summary>
        /// Returns the minimum coordinate of the hitbox.
        /// </summary>
        /// <returns>Returns the minimum coordinate of the hitbox.</returns>
        public override string ToString()
        {
            if (name != "")
                return name;
            return "X: " + hitBox.Min.X + " Y: " + hitBox.Min.Y;
        }
    }
    /// <summary>
    /// The hanging ledge is an area which will catch a falling player who has missed a ledge, and allow them to either climb up or jump off.
    /// </summary>
    public class HangingLedge : HitBoxInteractable
    {
        #region Instance Variables
        protected Point hangPoint; //The point at which the player will hang.
        protected Boolean isRight; //True if the hanging point is on the left side of the wall, otherwise false.
        #endregion
        #region Properties
        public Point HangPoint
        {
            get { return hangPoint; }
            set { hangPoint = value; }
        }
        public int PointX
        {
            get { return hangPoint.X; }
            set { hangPoint.X = value; }
        }
        public int PointY
        {
            get { return hangPoint.Y; }
            set { hangPoint.Y = value; }
        }
        public Boolean IsRight
        {
            get { return isRight; }
            set { isRight = value; }
        }
        #endregion
        /// <summary>
        /// Standard constructor.
        /// </summary>
        /// <param name="b">The area where the player can begin hanging from.</param>
        /// <param name="l">The level this belongs to.</param>
        /// <param name="s">The sprite for this object.</param>
        /// <param name="p">The hanging point.</param>
        /// <param name="r">True if this is on the left side of a wall, otherwise false.</param>
        public HangingLedge(BoundingBox b, LevelState l, Texture2D s, Point p, Boolean r) : base(b,l,s)
        {
            level = l;
            hitBox = b;
            hangPoint = p;
            isRight = r;
        }
        /// <summary>
        /// No internal logic is required for this. This method can probably be removed.
        /// </summary>
        /// <param name="pack"></param>
        public override void doThis(doPacket pack){}
        /// <summary>
        /// This method will fire when the player has collided with this object. It will catch them on the hanging point and send the player into hanging mode.
        /// Hanging mode is not handled by this object.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        /// <param name="p">The player</param>
        /// <returns>True if the player has been effected. Otherwise false.</returns>
        public override Boolean effectPlayer(doPacket pack, Player p)
        {
            if(detectCollision(p.hitBox))
            {
                p.hang(pack, this, isRight);
                return true;
            }
            return false;
        }
        /// <summary>
        /// This will move the hitbox of this interactable, as well as all rectangles in it, and the hang point.
        /// </summary>
        /// <param name="trajectory">The vector by which to move this interactable.</param>
        public override void moveThis(Vector2 trajectory)
        {
            base.moveThis(trajectory);
            hangPoint.X += (int)trajectory.X;
            hangPoint.Y += (int)trajectory.Y;
        }
        /// <summary>
        /// Returns Hanging Ledge plus the minimum coordinate of the hitbox.
        /// </summary>
        /// <returns>Returns Hanging Ledge plus the minimum coordinate of the hitbox.</returns>
        public override string ToString()
        {
            return "Hanging Ledge - " + base.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="l"></param>
        public override void WriteToFile(BinaryWriter writer, LevelEditState l)
        {
            writer.Write((byte)7);
            writer.Write((float)MinX);
            writer.Write((float)MinY);
            writer.Write((float)MaxX);
            writer.Write((float)MaxY);
            writer.Write((int)PointX);
            writer.Write((int)PointY);
            writer.Write((Boolean)IsRight);
            writer.Write((Int16)l.importedTextures.IndexOf(Sprite));
        }
    }
    /// <summary>
    /// This is a hanging ledge that moves. It will be like a normal hanging ledge, except that it will alter the player's velocity so that the player moves with the ledge.
    /// </summary>
    public class MovingHangingLedge : HangingLedge
    {
        #region Instance Variables
        protected Vector2 position; //The position of this object
        protected Vector2 velocity; //The velocity of this object
        #endregion
        #region Properties
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public float PosX
        {
            get { return position.X; }
            set { position.X = value; }
        }
        public float PosY
        {
            get { return position.Y; }
            set { position.Y = value; }
        }
        public float VelX
        {
            get { return velocity.X; }
            set { velocity.X = value; }
        }
        public float VelY
        {
            get { return velocity.Y; }
            set { velocity.Y = value; }
        }
        #endregion
        /// <summary>
        /// Standard constructor.
        /// </summary>
        /// <param name="b">This object's hit box.</param>
        /// <param name="l">The level this belongs to.</param>
        /// <param name="s">The sprite to use to draw this object.</param>
        /// <param name="p">The hanging point.</param>
        /// <param name="r">True if this object is on the left side of something, otherwise false.</param>
        public MovingHangingLedge(BoundingBox b, LevelState l, Texture2D s, Point p, Boolean r) 
            : base(b, l, s, p, r)
        {
            position = new Vector2();
            position.X = p.X;
            position.Y = p.Y;
            velocity = new Vector2(0, 0);
        }
        /// <summary>
        /// This method calls the other setVectors method, with a reduced parameter list.
        /// </summary>
        /// <param name="vX">A horizontal velocity.</param>
        /// <param name="vY">A vertical velocity.</param>
        public void setVectors(float vX, float vY)
        {
            setVectors(position.X, position.Y, vX, vY);
        }
        /// <summary>
        /// This method will set the position and velocity vectors of this object, given the x and y components of each vector. It is primarily a utility method.
        /// </summary>
        /// <param name="pX">The X-component of the position</param>
        /// <param name="pY">The Y-component of the position</param>
        /// <param name="vX">The X-component of the velocity</param>
        /// <param name="vY">The Y-component of the velocity</param>
        public void setVectors(float pX, float pY, float vX, float vY)
        {
            position.X = pX;
            position.Y = pY;
            velocity.X = vX;
            velocity.Y = vY;
        }
        /// <summary>
        /// This method will call the player's hang function if a collision is detected.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        /// <param name="p">The current player</param>
        /// <returns>True if the player is affected, otherwise false.</returns>
        public override bool effectPlayer(doPacket pack, Player p)
        {
            if (detectCollision(p.hitBox))
            {
                p.hang(pack, this, isRight, velocity);
                return true;
            }
            return false;
        }
        /// <summary>
        /// This method will update the position of the moving hanging ledge.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public override void doThis(doPacket pack)
        {
            if(!pack.state.paused) //If the game is not paused
            {
                //STUFF
            }
        }
        /// <summary>
        /// This will move the draw rectangle, hit box, hanging point, and position of this interactable.
        /// </summary>
        /// <param name="trajectory">The vector by which to move this.</param>
        public override void moveThis(Vector2 trajectory)
        {
            base.moveThis(trajectory);
            position.X += trajectory.X;
            position.Y += trajectory.Y;
        }
        /// <summary>
        /// Returns Moving Hanging Ledge plus the minimum coordinate of the hitbox.
        /// </summary>
        /// <returns>Returns Moving Hanging Ledge plus the minimum coordinate of the hitbox.</returns>
        public override string ToString()
        {
            return "Moving " + base.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="l"></param>
        public override void WriteToFile(BinaryWriter writer, LevelEditState l)
        {
            writer.Write((byte)8);
            writer.Write((float)MinX);
            writer.Write((float)MinY);
            writer.Write((float)MaxX);
            writer.Write((float)MaxY);
            writer.Write((int)PointX);
            writer.Write((int)PointY);
            writer.Write((Boolean)IsRight);
            writer.Write((Int16)l.importedTextures.IndexOf(Sprite));
        }
    }
    /// <summary>
    /// This represents a pressure plate which is activated whenever the player or a minibot intersects the hit box.
    /// </summary>
    public class PressurePlate : HitBoxInteractable
    {
        #region Instance Variables
        Delegate target; //The method that interacting with this object will invoke.
        #endregion
        /// <summary>
        /// Standard constructor.
        /// </summary>
        /// <param name="b">The hit box of this object.</param>
        /// <param name="l">The level this belongs to.</param>
        /// <param name="s">The sprite for this object</param>
        /// <param name="target">The method this object will dynamically invoke.</param>
        /// <param name="sprite">Why is there another sprite? Was this an oversight? Investigate.</param>
        public PressurePlate(BoundingBox b, LevelState l, Texture2D s, Delegate target)
            : base(b, l, s)
        {
            level = l;
            hitBox = b;
            this.target = target;
            this.sprite = s;
        }
        /// <summary>
        /// This method will check to see if a collision has occurred. If one has, then it will call its delegate method.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        /// <param name="p">The object interacting with this.</param>
        public override Boolean effectPlayer(doPacket pack, Player p)
        {
            //METHOD CALL TO DETECT COLLISIONS HERE
            target.DynamicInvoke();
            return false;
        }
        /// <summary>
        /// Returns pressure plate plus the minimum coordinate of the hit box
        /// </summary>
        /// <returns>Returns pressure plate plus the minimum coordinate of the hit box</returns>
        public override string ToString()
        {
            return "Pressure Plate - " + base.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="l"></param>
        public override void WriteToFile(BinaryWriter writer, LevelEditState l)
        {
            writer.Write((byte)10);
            writer.Write((float)MinX);
            writer.Write((float)MinY);
            writer.Write((float)MaxX);
            writer.Write((float)MaxY);
            writer.Write((Int16)l.importedTextures.IndexOf(Sprite));
        }
    }
    /// <summary>
    /// This represents an interactable object for which the player must either push a button or assign a minibot to activate. It does not activate automatically, like the pressure plate.
    /// </summary>
    public class ControlPanel : HitBoxInteractable
    {
        #region Instance Variables
        Delegate target; //The method which this interactable will dynamically invoke
        #endregion
        /// <summary>
        /// Standard constructor.
        /// </summary>
        /// <param name="b">The hit box for this object</param>
        /// <param name="l">The level this belongs to</param>
        /// <param name="s">The sprite for this object</param>
        /// <param name="target">The method this will dynamically invoke</param>
        /// <param name="sprite">Is this required? See pressureplate</param>
        public ControlPanel(BoundingBox b, LevelState l, Texture2D s, Delegate target)
            : base(b, l, s)
        {
            hitBox = b;
            level = l;
            this.target = target;
            this.Sprite = s;
        }
        /// <summary>
        /// This method will check for collisions. If a collision is detected, then check to see if this is being activated. If it is, affect the player.
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="p"></param>
        public override Boolean effectPlayer(doPacket pack, Player p)
        {
            //This needs to activate only on collision
            if (pack.controller.use() || p.type == InteractorType.toolbot)
                target.DynamicInvoke();
            if (p.type == InteractorType.toolbot)
            {
                int x = 0; //Toolbot interaction animation wot wot
            }
            return false;
        }
        /// <summary>
        /// Returns control panel plus the minimum coordinate of the hit box
        /// </summary>
        /// <returns>Returns control panel plus the minimum coordinate of the hit box</returns>
        public override string ToString()
        {
            return "Control Panel - " + base.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="l"></param>
        public override void WriteToFile(BinaryWriter writer, LevelEditState l)
        {
            writer.Write((byte)5);
            writer.Write((float)MinX);
            writer.Write((float)MinY);
            writer.Write((float)MaxX);
            writer.Write((float)MaxY);
            writer.Write((Int16)l.importedTextures.IndexOf(Sprite));
        }
    }
    /// <summary>
    /// This represents a surface onto which an elevator bot can attach and form an elevator.
    /// </summary>
    public class ElevatorSurface : HitBoxInteractable
    {
        #region Instance Variables
        public Boolean isRight; //True if the surface is on the left side of the wall, otherwise false.
        protected Vector2 start; //The start point of the elevating path.
        protected Vector2 end; //The end point of the elevating path.
        #endregion
        #region Properties
        public Vector2 Start
        {
            get { return start; }
            set { start = value; }
        }
        public Vector2 End
        {
            get { return end; }
            set { end = value; }
        }
        public float StartX
        {
            get { return start.X; }
            set { start.X = value; }
        }
        public float StartY
        {
            get { return start.Y; }
            set { start.Y = value; }
        }
        public float EndX
        {
            get { return end.X; }
            set { end.X = value; }
        }
        public float EndY
        {
            get { return end.Y; }
            set { end.Y = value; }
        }
        #endregion
        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="b">The hit box</param>
        /// <param name="l">The level this object belongs to</param>
        /// <param name="s">The sprite for this object</param>
        /// <param name="r">True if this object is on the left side of a wall, otherwise false.</param>
        /// <param name="start">The starting point of the elevating path</param>
        /// <param name="end">The ending point of the elevating path.</param>
        public ElevatorSurface(BoundingBox b, LevelState l, Texture2D s, Boolean r, Vector2 start, Vector2 end)
            : base(b, l, s)
        {
            hitBox = b;
            level = l;
            isRight = r;
            this.start.X = start.X;
            this.start.Y = start.Y;
            this.end.X = end.X;
            this.end.Y = end.Y;
        }
        /// <summary>
        /// This method handles the interaction when an elevator bot collides with the elevator surface.
        /// </summary>
        /// <param name="pack">See doPacket</param>
        /// <param name="p">The elevator bot colliding with this.</param>
        /// <returns>True if the elevator bot was effected, otherwise false.</returns>
        public override bool effectPlayer(doPacket pack, Player p)
        {
            //Need the collision check
            if (p.type == InteractorType.elevatorbot)
            {
                ((Elevatorbot)p).elevate(this);
            }
            return false;
        }
        /// <summary>
        /// This will move the hit box and the start and end points of this interactable.
        /// </summary>
        /// <param name="trajectory">The vector by which to move this.</param>
        public override void moveThis(Vector2 trajectory)
        {
            base.moveThis(trajectory);
            start.X += trajectory.X;
            end.X += trajectory.X;
            start.Y += trajectory.Y;
            end.Y += trajectory.Y;
        }
        /// <summary>
        /// Returns elevator surface plus the minimum coordinate of the hit box
        /// </summary>
        /// <returns>Returns elevator surface plus the minimum coordinate of the hit box</returns>
        public override string ToString()
        {
            return "Elevator Surface - " + base.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="l"></param>
        public override void WriteToFile(BinaryWriter writer, LevelEditState l)
        {
            writer.Write((byte)14);
            writer.Write((float)MinX);
            writer.Write((float)MinY);
            writer.Write((float)MaxX);
            writer.Write((float)MaxY);
            writer.Write((float)StartX);
            writer.Write((float)StartY);
            writer.Write((float)EndX);
            writer.Write((float)EndY);
            writer.Write((Boolean)isRight);
            writer.Write((Int16)l.importedTextures.IndexOf(Sprite));
        }
    }
}