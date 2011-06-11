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
    /// This state represents an image or animation that will show on a screen and then load a new state.
    /// In general these can be canceled through a button-press.
    /// </summary>
    public class SplashState : State
    {
        protected Texture2D background; //The image of this splash state
        /// <summary>
        /// Standard constructor.
        /// </summary>
        /// <param name="background">The image of this splash state</param>
        public SplashState()
        {
        }
        /// <summary>
        /// Standard splash state logic is to wait for any button press and then call nextState.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public override void doThis(doPacket pack)
        {
            if (pack.controller.confirm())
            {
                nextState(pack);
            }
        }
        /// <summary>
        /// Only one draw layer is needed for splash state. This will draw directly to screen.
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawToScreen(drawPacket pack)
        {
            pack.sb.Begin();
            pack.sb.Draw(background, Vector2.Zero, Color.White);
            pack.sb.End();
        }
        /// <summary>
        /// Must be overridden. This method will load the next state into the master game state.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public virtual void nextState(doPacket pack) { }
    }
}
