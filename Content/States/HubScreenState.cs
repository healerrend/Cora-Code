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

/* This file contains the hub screen state, where a player has loaded the game, but before they have decided to play.
 */

namespace CORA
{
    /// <summary>
    /// The hub screen represents the menu after a player has loaded the game, but when they have not entered gameplay. From here the player can access the database, options, or the game itself.
    /// </summary>
    class HubScreenState : MenuState
    {
        #region Textures
        private Texture2D background; //The background for the hub screen
        private Texture2D goToDatabase; //The icon for going to the database
        private Texture2D playGame; //The icon for playing the game
        private Texture2D goToOptions; //The icon for the options menu
        private Texture2D exit; //The icon to exit the game
        #endregion
        //All constants declared in loadState
        #region State Constants
        private Vector2 POSITION_PLAY_GAME;
        private Vector2 POSITION_GO_TO_DATABASE;
        private Vector2 POSITION_GO_TO_OPTIONS;
        private Vector2 POSITION_EXIT;
        #endregion
        #region Static Object Pool
        public static Color c; //A color to tint the menu items with
        #endregion
        /// <summary>
        /// Constructor. IT DOES NOSSSING.
        /// </summary>
        public HubScreenState() 
            : base(4)
        {}
        /// <summary>
        /// This handles the logic of detecting and making sure the elements on the hub screen are animated.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public override void doThis(doPacket pack)
        {
            base.doThis(pack); //Handle the selection logic
            if (pack.controller.confirm()) //If selected, find the index and apply logic.
            {
                switch (selectedIndex)
                {
                    case 0:
                        pack.state.loadState(new Level1State());
                        break;
                    case 1:
                        pack.state.loadState(new DatabaseState());
                        break;
                    case 2:
                        pack.state.pause(pack);
                        pack.state.displayOptions(pack);
                        break;
                    case 3:
                        pack.state.exitGame = true;
                        break;
                }
            }
        }
        /// <summary>
        /// No effects are necessary on the hub screen, only one draw is needed.
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawToScreen(drawPacket pack)
        {
            pack.sb.GraphicsDevice.SetRenderTarget(null);
            drawThis(pack);
        }
        /// <summary>
        /// This is invoked if the options screen is selected, since options screen only appears on pause.
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawToPause(drawPacket pack)
        {
            pack.sb.GraphicsDevice.SetRenderTarget(pack.state.pauseBG);
            drawThis(pack);
        }
        /// <summary>
        /// This logic is for when the menu is paused, and thus, the options menu is shown.
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawPause(drawPacket pack)
        {
            pack.sb.GraphicsDevice.SetRenderTarget(null);
            pack.sb.Begin();
            pack.sb.Draw(pack.state.pauseBG, Vector2.Zero, Color.Gray);

        }
        /// <summary>
        /// This is the actual normal draw for the hub screen.
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        private void drawThis(drawPacket pack)
        {
            pack.sb.Begin();
            pack.sb.Draw(background, Vector2.Zero, Color.White);
            for (int i = 0; i < totalItems; i++)
            {
                if (i == selectedIndex)
                    c = Color.White;
                else
                    c = Color.Gray;
                switch (i)
                {
                    case 0:
                        pack.sb.Draw(playGame, POSITION_PLAY_GAME, c);
                        break;
                    case 1:
                        pack.sb.Draw(goToDatabase, POSITION_GO_TO_DATABASE, c);
                        break;
                    case 2:
                        pack.sb.Draw(goToOptions, POSITION_GO_TO_OPTIONS, c);
                        break;
                    case 3:
                        pack.sb.Draw(exit, POSITION_EXIT, c);
                        break;
                }
            }
            pack.sb.End();
        }
        /// <summary>
        /// This method will load all A/V assets for this state.
        /// </summary>
        /// <param name="content">The content manager, to load the assets.</param>
        public override void loadState(GameState state, ContentManager content)
        {
            this.state = state;
            POSITION_PLAY_GAME = new Vector2(600, 100);
            POSITION_GO_TO_DATABASE = new Vector2(600, 300);
            POSITION_EXIT = new Vector2(0, 100);
            POSITION_GO_TO_OPTIONS = new Vector2(0, 500);
            background = content.Load<Texture2D>("junk\\junkhub");
            playGame = content.Load<Texture2D>("junk\\junkplaygame");
            goToDatabase = content.Load<Texture2D>("junk\\junkgotodatabase");
            goToOptions = content.Load<Texture2D>("junk\\junkgotooptions");
            exit = content.Load<Texture2D>("junk\\junkexitgame");
        }
    }
}
