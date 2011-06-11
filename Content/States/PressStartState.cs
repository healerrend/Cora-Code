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

/* This file contains the press start state which verifies which controller slot the player is connected to
 */
namespace CORA.Content.States
{
    /// <summary>
    /// The Press Start State is the state before the player has pressed start on the title screen. This will determine what controller the player is using.
    /// </summary>
    public class PressStartState : SplashState
    {
        /// <summary>
        /// Constructor: Does Nothing.
        /// </summary>
        public PressStartState()
            : base() { }
        public override void nextState(doPacket pack)
        {
            pack.state.loadState(new PressStartState());
        }
        public override void loadState(GameState state, ContentManager C)
        {
            this.state = state;
            this.background = C.Load<Texture2D>("junk\\splash");
        }
        /// <summary>
        /// This handles the logic behind checking which controller is used and setting it to the used controller.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public override void doThis(doPacket pack)
        {
            base.doThis(pack);
            GamePadState PL1 = GamePad.GetState(PlayerIndex.One);
            GamePadState PL2 = GamePad.GetState(PlayerIndex.Two);
            GamePadState PL3 = GamePad.GetState(PlayerIndex.Three);
            GamePadState PL4 = GamePad.GetState(PlayerIndex.Four);
            
            if (PL1.IsButtonDown(Buttons.Start) || PL1.IsButtonDown(Buttons.A))
            {
                state.setController(new Controller(PlayerIndex.One));
            }
            else if (PL2.IsButtonDown(Buttons.Start) || PL2.IsButtonDown(Buttons.A))
            {
                state.setController(new Controller(PlayerIndex.Two));
            }
            else if (PL3.IsButtonDown(Buttons.Start) || PL3.IsButtonDown(Buttons.A))
            {
                state.setController(new Controller(PlayerIndex.Three));
            }
            else if (PL4.IsButtonDown(Buttons.Start) || PL4.IsButtonDown(Buttons.A))
            {
                state.setController(new Controller(PlayerIndex.Four));
            }
            else {}
        }
    }
}