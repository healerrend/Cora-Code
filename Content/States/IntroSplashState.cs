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

/* This file contains the intro splash state.
 * TO DO:
 * 1. Load the final visual asset
 * 2. Fade in-out
 */

namespace CORA
{
    /// <summary>
    /// This is the introduction splash state for CORA
    /// </summary>
    public class IntroSplashState : SplashState
    {
        /// <summary>
        /// This is a standard splash state.
        /// </summary>
        public IntroSplashState()
            : base() { }
        public override void nextState(doPacket pack)
        {
            pack.state.loadState(new LogoSplashState());
        }
        public override void loadState(GameState state, ContentManager C)
        {
            this.state = state;
            this.background = C.Load<Texture2D>("junk\\splash");
        }
        public override void unloadState(ContentManager C) //Not sure if we really need the content manager to unload. We'll see.
        {
        }
    }
}