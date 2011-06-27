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
        [Browsable(false)]
        public override float _Height
        {
            get
            {
                return base._Height;
            }
        }
        [Browsable(false)]
        public override float _Width
        {
            get
            {
                return base._Width;
            }
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
        public Rust(BoundingBox b, LevelState l, double t, Texture2D sprite) : base(b, l)
        {
            disappearTime = t;
            this.sprite = sprite;
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
        public override void doThis(doPacket pack)
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
        public override void drawThis(drawPacket pack)
        {
            if (visible)
            {
                drawRect.X = (int)_X;
                drawRect.Y = (int)_Y;
                drawRect.Width = (int)_Width;
                drawRect.Height = (int)_Height;
                pack.sb.Draw(sprite, drawRect, null, tint, rotation, origin, effect, depth);
            }
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
            mainString.AppendLine("this.walls.Add(new Rust(new BoundingBox(new Vector3(" + this._X + ", " + this._Y + ", 0), new Vector3(" + this._X + " + " + _Width + ", " + this._Y + " + " + _Height + ", 0)), this, " + this.disappearLength + ", " + path + "));");
        }
    }
}
