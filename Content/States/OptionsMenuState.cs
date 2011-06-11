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
    public class OptionsMenuState : MenuState
    {
        #region Instance Variables
        public Rectangle drawRect; //The rectangle over which the menu should be drawn
        public double animator; //The animation timer
        ActivationState activationState; //The activation state for the options menu.
        #endregion
        #region State Constants
        private Vector2 POSITION_RETURN; //The position of the return menu item
        #endregion
        #region Assets
        Texture2D background; //The background image for the menu
        SpriteFont font; //The font to write the items in
        #endregion
        /// <summary>
        /// Standard constructor.
        /// </summary>
        public OptionsMenuState() : base(1)
        {
            animator = 0;
            activationState = ActivationState.inactive;
            drawRect = new Rectangle();
        }
        /// <summary>
        /// This will load the assets for this state
        /// </summary>
        /// <param name="content">The content manager to load the assets</param>
        public override void loadState(GameState state, ContentManager content)
        {
            this.state = state;
            background = content.Load<Texture2D>("junk\\junkoptions");
            font = content.Load<SpriteFont>("realassets\\russellsquare");
            drawRect = new Rectangle(100, 100, 960, 540);
            POSITION_RETURN = new Vector2(200, 200);
            activationState = ActivationState.activating;
            animator = 0;
        }
        /// <summary>
        /// This method will handle the logic of the options menu. It will only be called when an options menu exists, and all logic will skip when it is inactive.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public override void doThis(doPacket pack)
        {
            double t;
            switch (activationState)
            {
                case ActivationState.activating: //When this is in the middle of activating
                    activationState = ActivationState.active;
                    break;
                case ActivationState.active: //When this is active
                    if (pack.controller.confirm())
                    {
                        switch (selectedIndex)
                        {
                            case 0:
                                pack.state.removeOptions(pack);
                                break;
                        }
                    }
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
        public override void drawPause(drawPacket pack)
        {
            pack.sb.GraphicsDevice.SetRenderTarget(null);
            pack.sb.Begin();
            pack.sb.Draw(pack.state.pauseBG, Vector2.Zero, Color.Gray);
            pack.sb.Draw(background, drawRect, null, Color.White);
            pack.sb.DrawString(font, "Return", POSITION_RETURN, Color.Black);
            pack.sb.End();
        }
    }
}
