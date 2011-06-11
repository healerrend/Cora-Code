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

namespace CORA
{
    /// <summary>
    /// This is the options menu. This menu is a version of the pause menu in which only the options menu is displayed. The options menu will only display during pause.
    /// </summary>
    public class OptionsMenu
    {
        #region Instance Variables
        public Rectangle drawRect; //The rectangle over which the menu should be drawn
        public double animator; //The animation timer
        State returnState; //The state to which the game should return when the options menu is finished.
        ActivationState activationState; //The activation state for the options menu.
        #endregion
        /// <summary>
        /// Standard constructor.
        /// </summary>
        public OptionsMenu()
        {
            animator = 0;
            activationState = ActivationState.inactive;
            drawRect = new Rectangle();
        }
        /// <summary>
        /// This method will activate the options menu. It should be called after the game has been paused.
        /// </summary>
        /// <param name="s">The state to return to once options are completed.</param>
        /// <param name="pack">see doPacket</param>
        public void activate(State s, doPacket pack)
        {
            activationState = ActivationState.activating;
            returnState = s;
            animator = 0;
        }
        /// <summary>
        /// This method will deactivate the options menu. It should be called just before unpausing.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public void deactivate(doPacket pack)
        {
            //Stuff
        }
        /// <summary>
        /// This method will handle the logic of the options menu. It will only be called when an options menu exists, and all logic will skip when it is inactive.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public void doThis(doPacket pack)
        {
            double t;
            switch (activationState)
            {
                case ActivationState.activating: //When this is in the middle of activating
                    animator += pack.time.ElapsedGameTime.TotalMilliseconds; //Animation stuff
                    if (animator < 250)
                    {
                        t = (animator / (double)250);
                        if (t > 1)
                            t = 1;
                        drawRect.X = 640;
                        drawRect.Y = 360 - (int)(5 * t);
                        drawRect.Width = 1;
                        drawRect.Height = (int)(10 * t);
                    }
                    else if (animator < 650)
                    {
                        t = ((animator - 250) / (double)100);
                        if (t > 1)
                            t = 1;
                        drawRect.X = 640 - (int)((float)620 * t);
                        drawRect.Y = 355;
                        drawRect.Width = (int)((float)1240 * t);
                        drawRect.Height = 10;
                    }
                    break;
                case ActivationState.active: //When this is active
                    break;
                case ActivationState.deactivating: //When this is in the middle of deactivating
                    break;
                case ActivationState.inactive: //When this has successfully deactivated
                    break;
            }
        }
        /// <summary>
        /// This method will draw the options menu.
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public void drawThis(drawPacket pack)
        {
            pack.sb.Draw(pack.state.pauseBG, Vector2.Zero, Color.Gray);
            pack.sb.Draw(TextureLoader.redsquare, drawRect, null, Color.LightGray);
        }
    }
}
