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
    /// This represents an interactable object for which the player must either push a button or assign a minibot to activate. It does not activate automatically, like the pressure plate.
    /// </summary>
    public class ControlPanel : HitBoxInteractable
    {
        #region Instance Variables
        Boolean canRepeat;
        Boolean hasActivated;
        Delegate target; //The method which this interactable will dynamically invoke
        DelegateParams parameters;
        #endregion
        /// <summary>
        /// Standard constructor.
        /// </summary>
        /// <param name="b">The hit box for this object</param>
        /// <param name="l">The level this belongs to</param>
        /// <param name="s">The sprite for this object</param>
        /// <param name="target">The method this will dynamically invoke</param>
        /// <param name="sprite">Is this required? See pressureplate</param>
        public ControlPanel(BoundingBox b, LevelState l, Texture2D s, Delegate target, DelegateParams parameters)
            : base(b, l, s)
        {
            hitBox = b;
            level = l;
            this.target = target;
            this.Sprite = s;
            this.parameters = parameters;
            canRepeat = true;
            hasActivated = false;
        }
        public ControlPanel(BoundingBox b, LevelState l, Texture2D s, Delegate target, DelegateParams parameters, Boolean canRepeat)
            : base(b, l, s)
        {
            hitBox = b;
            level = l;
            this.target = target;
            this.Sprite = s;
            this.parameters = parameters;
            this.canRepeat = canRepeat;
            hasActivated = false;
        }
        public override void drawThis(drawPacket pack)
        {
            if(sprite != null)
                base.drawThis(pack);
        }
        /// <summary>
        /// This method will check for collisions. If a collision is detected, then check to see if this is being activated. If it is, affect the player.
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="p"></param>
        public override Boolean effectPlayer(doPacket pack, Player p)
        {
            //This needs to activate only on collision
            if(!hasActivated || canRepeat)
                if (enabled && p.hitBox.Intersects(hitBox))
                    if (pack.controller.use() || (p.type == InteractorType.toolbot && ((Toolbot)p).isReleased))
                    {
                        target.DynamicInvoke(parameters);
                        hasActivated = true;
                        return true;
                    }
                    else if (p.type == InteractorType.toolbot)
                    {
                        int x = 0; //Toolbot interaction animation wot wot
                        target.DynamicInvoke(parameters);
                        hasActivated = true;
                        return true;
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
            mainString.AppendLine("this.interactables.Add(new ControlPanel(new BoundingBox(new Vector3(" + MinX + ", " + MinY + ", 0), new Vector3(" + MaxX + ", " + MaxY + ", 0)), this, " + path + ", INSERT DELEGATE HERE));");
        }
    }
}
