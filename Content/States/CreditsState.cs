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

/* This file contains the credits state.
 * TO DO:
 * 1. Put in the correct credits for the crew
 * 2. Animate the credits, if that is desired
 * 3. Load the correct background
 * 4. Load a song
 */

namespace CORA
{
    /// <summary>
    /// This state will roll the credits.
    /// </summary>
    public class CreditsState : State
    {
        public double animator; //Used to animate the credits
        public Texture2D background;
        //public SpriteFont font; //Will need this, don't yet
        /// <summary>
        /// Constructor
        /// </summary>
        public CreditsState() : base()
        {
        }
        public override void loadState(GameState state, ContentManager content)
        {
            this.state = state;
            background = content.Load<Texture2D>("junk\\junkcredits");
        }
        /// <summary>
        /// Handles the positioning of the credits, if any
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public override void doThis(doPacket pack)
        { }
        /// <summary>
        /// Draws to the screen
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawToScreen(drawPacket pack)
        {
            pack.sb.Begin();
            pack.sb.Draw(background, Vector2.Zero, Color.White);
            pack.sb.End();
        }
    }
}
