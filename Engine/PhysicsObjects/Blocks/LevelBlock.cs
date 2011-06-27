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
    /// Levelblock is the abstract representation of an obstacle which cannot be moved through. Platforms are the quintessential LevelBlock.
    /// NOTE: NEED TO ADD SPRITE SUPPORT TO ALL CHILDREN OF LEVELBLOCK
    /// </summary>
    public abstract class LevelBlock : Drawable
    {
        #region Instance Variables
        protected BoundingBox dimensions;  //The hitbox of the level block.
        protected LevelState level;        //The level to which this level block belongs.
        protected Boolean repeatX = false;
        protected Boolean repeatY = false;
        protected Boolean isCollided = false;
        protected int xReps = -1;
        protected int yReps = -1;
        protected float rotation = 0;
        #endregion
        #region Static Object Pool
        //These variables are available for temporary use and are declared in order to assist in controlling garbage collection.
        public static Point position = new Point(); //Usually a player's position.
        public static Vector2 collision = new Vector2(); //Usually the velocity vector calculated by collision detection.
        public static Vector2 postCollision = new Vector2(); //Usually a second vector for use in collision detection.
        public static Rectangle drawRect = new Rectangle();
        public static Rectangle spriteRect = new Rectangle();
        #endregion
        #region Properties
        /// <summary>
        /// Gets or sets the hit box for this
        /// </summary>
        [Browsable(false)]
        public BoundingBox Dimensions
        {
            get { return dimensions; }
            set { dimensions = value; }
        }
        /// <summary>
        /// Gets or sets the x component of the minimum point of this object's hitbox.
        /// </summary>
        [Browsable(false)]
        public virtual float MinX
        {
            get { return dimensions.Min.X; }
            set { dimensions.Min.X = value; }
        }
        /// <summary>
        /// Gets or sets the x component of the maximum point of this object's hitbox
        /// </summary>
        [Browsable(false)]
        public virtual float MaxX
        {
            get { return dimensions.Max.X; }
            set { dimensions.Max.X = value; }
        }
        /// <summary>
        /// Gets or sets the Y component of the minimum point of this object's hitbox
        /// </summary>
        [Browsable(false)]
        public virtual float MinY
        {
            get { return dimensions.Min.Y; }
            set { dimensions.Min.Y = value; }
        }
        /// <summary>
        /// Gets or sets the Y component of the maximum point of this object's hitbox
        /// </summary>
        [Browsable(false)]
        public virtual float MaxY
        {
            get { return dimensions.Max.Y; }
            set { dimensions.Max.Y = value; }
        }
        public virtual float _X
        {
            get { return dimensions.Min.X; }
            set
            {
                float width = dimensions.Max.X - dimensions.Min.X;
                dimensions.Min.X = value;
                dimensions.Max.X = dimensions.Min.X + width;
                recalculateDraw();
            }
        }
        public virtual float _Y
        {
            get { return dimensions.Min.Y; }
            set
            {
                float height = dimensions.Max.Y - dimensions.Min.Y;
                dimensions.Min.Y = value;
                dimensions.Max.Y = dimensions.Min.Y + height;
                recalculateDraw();
            }
        }
        public virtual float _Width
        {
            get { return dimensions.Max.X - dimensions.Min.X; }
            set 
            { 
                dimensions.Max.X = dimensions.Min.X + value;
                recalculateDraw();
            }
        }
        public virtual float _Height
        {
            get { return dimensions.Max.Y - dimensions.Min.Y; }
            set 
            { 
                dimensions.Max.Y = dimensions.Min.Y + value;
                recalculateDraw();
            }
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
        public override void drawThis(drawPacket pack) { }
        /// <summary>
        /// Override is optional. This method is called on any block which must have some internal logic completed at any time a collision is not detected.
        /// </summary>
        /// <param name="pack">See doPacked</param>
        /// <param name="player">The current player</param>
        public override void doThis(doPacket pack) { }
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
        public virtual Boolean RepeatX
        {
            get { return repeatX; }
            set 
            { 
                repeatX = value;
                recalculateDraw();
            }
        }
        public virtual Boolean RepeatY
        {
            get { return repeatY; }
            set 
            { 
                repeatY = value;
                recalculateDraw();
            }
        }
        public virtual float Rotation
        {
            get { return rotation; }
            set 
            { 
                rotation = value;
                recalculateDraw();
            }
        }
        protected void recalculateDraw()
        {
            try
            {
                xReps = -1;
                yReps = -1;
                if (repeatX)
                    xReps = (int)(dimensions.Max.X - dimensions.Min.X) / sprite.Width;
                if (repeatY)
                    yReps = (int)(dimensions.Max.Y - dimensions.Min.Y) / sprite.Height;
            }
            catch
            {}
        }
    }
}
