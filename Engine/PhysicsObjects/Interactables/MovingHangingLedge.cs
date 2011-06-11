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
            if (!pack.state.paused) //If the game is not paused
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
            mainString.AppendLine("this.interactables.Add(new MovingHangingLedge(new BoundingBox(new Vector3(" + this.MinX + ", "+ this.MinY + ", 0), new Vector3(" + this.MaxX + ", " + this.MaxY + ", 0)), this, " + path + ", new Point(" + hangPoint.X + ", " + hangPoint.Y + "), " + isRight + "));");
        }
    }
}
