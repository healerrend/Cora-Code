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

/* This file contains the logo splash screen state.
 * TO DO:
 * 1. Load in the final visual asset
 * 2. Fade in-out
 */

namespace CORA
{
    /// <summary>
    /// This class represents the Manticore Games logo splash state.
    /// </summary>
    public class LogoSplashState : SplashState
    {
        /// <summary>
        /// This state is a standard splash state. Additional things can be added.
        /// </summary>
        public LogoSplashState()
         : base() { }
        public override void nextState(doPacket pack)
        {
            pack.state.loadState(new TitleMenuState());
        }
        public override void loadState(GameState state, ContentManager content)
        {
            this.state = state;
            background = content.Load<Texture2D>("junk\\junklogo");
        }
    }
}
