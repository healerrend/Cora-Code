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
        public HangingLedge(BoundingBox b, LevelState l, Texture2D s, Point p, Boolean r)
            : base(b, l, s)
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
        public override void doThis(doPacket pack) { }
        /// <summary>
        /// This method will fire when the player has collided with this object. It will catch them on the hanging point and send the player into hanging mode.
        /// Hanging mode is not handled by this object.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        /// <param name="p">The player</param>
        /// <returns>True if the player has been effected. Otherwise false.</returns>
        public override Boolean effectPlayer(doPacket pack, Player p)
        {
            if (detectCollision(p.hitBox))
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
            mainString.AppendLine("this.interactables.Add(new HangingLedge(new BoundingBox(new Vector3(" + MinX + ", " + MinY + ", 0), new Vector3(" + MaxX + ", " + MaxY + ", 0)), this, " + path + ", new Point(" + hangPoint.X + ", " + hangPoint.Y + "), " + isRight + "));");
        }
    }
}
