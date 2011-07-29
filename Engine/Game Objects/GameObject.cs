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

/* This file contains all classes which are ostensibly game objects.
 */

namespace CORA
{
    /// <summary>
    /// This is the basic GameObject class. No instances of this class may be created.
    /// </summary>
    public abstract class GameObject
    {
        #region Instance Variables
        public Vector2 position; //This object's position
        public Vector2 velocity; //This object's velocity
        public Vector2 acceleration; //This object's acceleration
        public Boolean accelerationEnabled; //True if this object's acceleration should affect its velocity, otherwise false.
        public Texture2D sprite; //This object's sprite
        public BoundingBox hitBox; //This object's hit box.
        public Boolean enabled = true;
        public Boolean visible = true;
        #endregion
        /// <summary>
        /// Standard constructor.
        /// </summary>
        public GameObject()
        {
            position = new Vector2();
            velocity = new Vector2();
            acceleration = new Vector2();
            accelerationEnabled = true;
        }
        /// <summary>
        /// Must be overridden. This handles the object's internal logic.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public virtual void doThis(doPacket pack) { }
        /// <summary>
        /// Must be overridden. This method will draw the object into the game world.
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public virtual void drawThis(drawPacket pack) { }
        /// <summary>
        /// This method will apply gravity if acceleration is enabled.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public virtual void doPhysics(doPacket pack)
        {
            if (accelerationEnabled)
            {
                acceleration.Y = pack.state.GRAVITY;
                acceleration.X = 0;
            }
        }
        /// <summary>
        /// This method will move this object by the specified vector. By default it will move the hitbox and position.
        /// </summary>
        /// <param name="trajectory">The vector by which to move this object.</param>
        public virtual void moveThis(Vector2 trajectory)
        {
            position.X += trajectory.X;
            position.Y += trajectory.Y;
            hitBox.Min.X += trajectory.X;
            hitBox.Max.X += trajectory.X;
            hitBox.Min.Y += trajectory.Y;
            hitBox.Max.Y += trajectory.Y;
        }
        public virtual void moveThis(float x, float y)
        {
            moveThis(new Vector2(x, y));
        }
    }
}