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

/* This file contains the database state, for displaying unlocked art and literature.
 */

namespace CORA
{
    /// <summary>
    /// This state will allow the player to access unlocked art and literature.
    /// </summary>
    public class DatabaseState : State
    {
        #region Instance Variables
        #endregion
        #region Textures
        Texture2D background; //The background for this state
        SpriteFont font; //The font text will be written with
        #endregion
        #region State Constants
        #endregion
        /// <summary>
        /// Constructor
        /// </summary>
        public DatabaseState() : base()
        {
            
        }
        /// <summary>
        /// This method will load all of the A/V assets for this state
        /// </summary>
        /// <param name="content">The content manager</param>
        public override void loadState(GameState state, ContentManager content)
        {
            this.state = state;
            background = content.Load<Texture2D>("junk\\junkdatabase");
        }
        /// <summary>
        /// This will draw the state to the screen
        /// </summary>
        /// <param name="pack">See drawPacket</param>
        public override void drawToScreen(drawPacket pack)
        {
            pack.sb.Begin();
            pack.sb.Draw(background, Vector2.Zero, Color.White);
            pack.sb.End();
        }
        /// <summary>
        /// This will handle all logic for this state
        /// </summary>
        /// <param name="pack">See doPacket</param>
        public override void doThis(doPacket pack)
        {
        }
    }
}
