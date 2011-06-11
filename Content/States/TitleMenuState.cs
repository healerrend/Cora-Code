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

/* This file contains the Title Menu state.
 * TO DO:
 * 1. Load final visual and audio asset(s)
 */

namespace CORA
{
    /// <summary>
    /// This is the state which appears immediately after the logo and introductory splash screens.
    /// </summary>
    class TitleMenuState : MenuState
    {
        #region State Constants
        private Vector2 POSITION_PLAY_GAME;
        private Vector2 POSITION_CREDITS;
        private Vector2 POSITION_OPTIONS;
        private Vector2 POSITION_EXIT;
        private Vector2 POSITION_LEVEL_EDIT;
        #endregion
        #region Instance Variables
        #endregion
        #region Static Object Pool
        static Color c; //The color to display menu options in
        static String s; //The menu item to currently display
        static Vector2 v; //The vector to draw the item at
        #endregion
        #region Assets
        Texture2D bgImage; //The background image for the title screen
        SpriteFont font;
        Song bgMusic; //The title screen music
        #endregion
        /// <summary>
        /// Constructor. Impotent.
        /// </summary>
        public TitleMenuState() : base(5) { }
        /// <summary>
        /// This is called to initialize the state itself, and to load any assets needed through the content manager.
        /// </summary>
        public override void loadState(GameState state, ContentManager content)
        {
            this.state = state;
            POSITION_PLAY_GAME = new Vector2(400, 300);
            POSITION_CREDITS = new Vector2(400, 340);
            POSITION_OPTIONS = new Vector2(400, 380);
            POSITION_EXIT = new Vector2(400, 420);
            POSITION_LEVEL_EDIT = new Vector2(400, 460);
            bgImage = content.Load<Texture2D>("junk\\menubg");
            font = content.Load<SpriteFont>("realassets\\arbonnie");


        }
        /// <summary>
        /// This calls the selection logic and handles the logic for when an item is selected.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public override void doThis(doPacket pack)
        {
            base.doThis(pack); //Handle selected index
            if(pack.controller.confirm()) //If selected, handle that.
            {
                switch(selectedIndex)
                {
                    case 0:
                        pack.state.loadState(new HubScreenState());
                        break;
                    case 1:
                        pack.state.loadState(new CreditsState());
                        break;
                    case 2:
                        pack.state.pause(pack);
                        pack.state.displayOptions(pack);
                        break;
                    case 3:
                        pack.state.exitGame = true;
                        break;
                    case 4:
                        pack.state.loadState(new LevelEditState());
                        break;
                }
            }
        }
        /// <summary>
        /// This draws the title menu to the screen
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawToScreen(drawPacket pack)
        {
            pack.sb.Begin();
            pack.sb.Draw(bgImage, Vector2.Zero, Color.White);
            for(int i=0;i<totalItems;i++)
            {
                if(selectedIndex == i)
                    c = Color.White;
                else
                    c = Color.Gray;
                switch(i)
                {
                    case 0:
                        s = "Play Game";
                        v = POSITION_PLAY_GAME;
                        break;
                    case 1:
                        s = "Credits";
                        v = POSITION_CREDITS;
                        break;
                    case 2:
                        s = "Options";
                        v = POSITION_OPTIONS;
                        break;
                    case 3:
                        s = "Exit";
                        v = POSITION_EXIT;
                        break;
                    case 4:
                        s = "Level Editor";
                        v = POSITION_LEVEL_EDIT;
                        break;
                }
                pack.sb.DrawString(TextureLoader.ArBonnie, s, v, c);
            }
            pack.sb.End();
        }
    }
}
