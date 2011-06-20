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
        public float _Width
        {
            get { return hitBox.Max.X - hitBox.Min.X; }
            set { hitBox.Max.X = hitBox.Min.X + value; }
        }
        public float _Height
        {
            get { return hitBox.Max.Y - hitBox.Min.Y; }
            set { hitBox.Max.Y = hitBox.Min.Y + value; }
        }
        public float _X
        {
            get { return hitBox.Min.X; }
            set
            {
                float w = _Width;
                hitBox.Min.X = value;
                hitBox.Max.X = hitBox.Min.X + w;
            }
        }
        public float _Y
        {
            get { return hitBox.Min.Y; }
            set
            {
                float h = _Height;
                hitBox.Min.Y = value;
                hitBox.Max.Y = hitBox.Min.Y + h;
            }
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
            if(visible && sprite != null)
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
        public void moveThis(float x, float y)
        {
            _X = x;
            _Y = y;
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
}
