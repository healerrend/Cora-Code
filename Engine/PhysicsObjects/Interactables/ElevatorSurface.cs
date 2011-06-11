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
            mainString.AppendLine("this.interactables.Add(new ElevatorSurface(new BoundingBox(new Vector3(" + MinX + ", " + MinY + ", 0), new Vector3(" + MaxX + ", " + MaxY + ", 0)), this, " + path + ", " + isRight + ", new Vector2(" + start.X + ", " + start.Y + "), new Vector2(" + end.X + ", " + end.Y + ")));");
        }
    }
}
