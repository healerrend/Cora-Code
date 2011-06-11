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
}
