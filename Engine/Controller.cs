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
    public class Controller
    {
        PlayerIndex playerIndex;
        //Old and new states for keyboard and controller
        private GamePadState cOld;
        private GamePadState cNew;
        private KeyboardState kOld;
        private KeyboardState kNew;
        //Button enumerations for the gamepad
        #region Gamepad Buttons
        private Buttons btnConfirm = Buttons.A;
        private Buttons btnCancel = Buttons.B;
        private Buttons btnUp = Buttons.DPadUp;
        private Buttons btnDown = Buttons.DPadDown;
        private Buttons btnLeft = Buttons.DPadLeft;
        private Buttons btnRight = Buttons.DPadRight;
        private Buttons btnJump = Buttons.A;
        private Buttons btnRun = Buttons.X;
        private Buttons btnClimb = Buttons.X;
        private Buttons btnUse = Buttons.B;
        private Buttons btnRelease = Buttons.Y;
        private Buttons btnDash = Buttons.RightShoulder;
        #endregion
        //Button enumerations for the keyboard
        #region Keyboard Keys
        private Keys keyConfirm = Keys.Enter;
        private Keys keyCancel = Keys.Escape;
        private Keys keyUp = Keys.Up;
        private Keys keyDown = Keys.Down;
        private Keys keyLeft = Keys.Left;
        private Keys keyRight = Keys.Right;
        private Keys keyJump = Keys.Space;
        private Keys keyRun = Keys.LeftShift;
        private Keys keyClimb = Keys.Up;
        private Keys keyUse = Keys.Enter;
        private Keys keyRelease = Keys.A;
        private Keys keyDash = Keys.D;
        #endregion
        /// <summary>
        /// Controller will always look for player 1. Both keyboard and controller states are initialized such that new and old are identical.
        /// </summary>
        public Controller() 
        {
            playerIndex = PlayerIndex.One;
            cOld = GamePad.GetState(playerIndex);
            cNew = GamePad.GetState(playerIndex);
            kOld = Keyboard.GetState();
            kNew = Keyboard.GetState();
        }
        /// <summary>
        /// This constructor has a player index passed in after the current player has been established.
        /// </summary>
        /// <param name="p">The index of the current player, found by pressing the start button.</param>
        public Controller(PlayerIndex p)
        {
            playerIndex = p;
            cOld = GamePad.GetState(playerIndex);
            cNew = GamePad.GetState(playerIndex);
            kOld = Keyboard.GetState();
            kNew = Keyboard.GetState();
        }
        /// <summary>
        /// This is called before each update. It will make the previous new state into the old state, and update the new state.
        /// </summary>
        public void updateController()
        {
            cOld = cNew;
            cNew = GamePad.GetState(playerIndex);
            kOld = kNew;
            kNew = Keyboard.GetState();
        }
        /// <summary>
        /// This helper method will return true if a button has been pressed since the last update.
        /// </summary>
        /// <param name="b">A button on the gamepad</param>
        /// <returns>True if the button has been pressed since the last update, otherwise false.</returns>
        private Boolean isButton(Buttons b)
        {
            return (cOld.IsButtonUp(b) && cNew.IsButtonDown(b));
        }
        /// <summary>
        /// This helper method will return true if a key has been pressed since the last update.
        /// </summary>
        /// <param name="k">A key on the keyboard</param>
        /// <returns>True if the key has been pressed since the last update, otherwise false.</returns>
        private Boolean isKey(Keys k)
        {
            return (kOld.IsKeyUp(k) && kNew.IsKeyDown(k));
        }
        #region Controller/Keyboard Interface Methods
        /// <summary>
        /// Returns true if confirm has been pressed since last update.
        /// </summary>
        /// <returns></returns>
        public Boolean confirm()
        {
            return (isButton(btnConfirm) || isKey(keyConfirm));
        }
        /// <summary>
        /// Returns true if cancel has been pressed since last update.
        /// </summary>
        /// <returns></returns>
        public Boolean cancel()
        {
            return (isButton(btnCancel) || isKey(keyCancel));
        }
        public Boolean dash()
        {
            return (isButton(btnDash) || isKey(keyDash));
        }
        /// <summary>
        /// Returns true if up has been pressed since last update. For thumb stick, the cut-off is .5
        /// </summary>
        /// <returns></returns>
        public Boolean up()
        {
            return (isButton(btnUp) ||
                (cOld.ThumbSticks.Left.Y < .5 && cNew.ThumbSticks.Left.Y >= .5) || isKey(keyUp));
        }
        /// <summary>
        /// Returns true if down has been pressed since last update. For thumb stick, the cut-off is .5
        /// </summary>
        /// <returns></returns>
        public Boolean down()
        {
            return (isButton(btnDown) ||
                (cOld.ThumbSticks.Left.Y > -.5 && cNew.ThumbSticks.Left.Y <= -.5) || isKey(keyDown));
        }
        /// <summary>
        /// Returns true if left has been pressed since last update. For thumb stick, the cut-off is .5
        /// </summary>
        /// <returns></returns>
        public Boolean left()
        {
            return (isButton(btnLeft) ||
                (cOld.ThumbSticks.Left.X > .5 && cNew.ThumbSticks.Left.X <= .5) || isKey(keyLeft));
        }
        /// <summary>
        /// Returns true if right has been pressed since last update. For thumb stick, the cut-off is .5
        /// </summary>
        /// <returns></returns>
        public Boolean right()
        {
            return (isButton(btnRight) ||
                (cOld.ThumbSticks.Left.X < -.5 && cNew.ThumbSticks.Left.X >= -.5) || isKey(keyRight));
        }
        /// <summary>
        /// Returns a value between -1 and 1 representing the x position of the left thumb stick.
        /// </summary>
        /// <returns></returns>
        public float moveStickHoriz()
        {
            return cNew.ThumbSticks.Left.X;
        }
        /// <summary>
        /// Returns a value between -1 and 1 representing the y position of the left thumb stick.
        /// </summary>
        /// <returns></returns>
        public float moveStickVert()
        {
            return cNew.ThumbSticks.Left.Y;
        }
        /// <summary>
        /// Returns true if jump has been pressed since last update.
        /// </summary>
        /// <returns></returns>
        public Boolean jump()
        {
            return isButton(btnJump);
        }
        /// <summary>
        /// Returns true if the run button is currently pressed. NO KEYBOARD FUNCTIONALITY YET.
        /// </summary>
        /// <returns></returns>
        public Boolean run()
        {
            return cNew.IsButtonDown(btnRun);
        }
        /// <summary>
        /// Returns true if any button is being pressed. NO KEYBOARD FUNCTIONALITY YET.
        /// </summary>
        /// <returns></returns>
        public Boolean any()
        {
            return (cNew.Buttons != cOld.Buttons);
        }
        /// <summary>
        /// Returns true if the player has changed directions since last update. NO KEYBOARD FUNCTIONALITY YET.
        /// </summary>
        /// <returns></returns>
        public Boolean changeDirection()
        {
            return (cOld.ThumbSticks.Left.X <= 0 && cNew.ThumbSticks.Left.X > 0
                || cOld.ThumbSticks.Left.X >= 0 && cNew.ThumbSticks.Left.X < 0);
        }
        /// <summary>
        /// Returns 1 if the left thumb stick's x component is greater than 0, -1 if it is less than 0, and 0 if it is equal to 0.
        /// </summary>
        /// <returns></returns>
        public int isRight()
        {
            if (cNew.ThumbSticks.Left.X > 0)
                return 1;
            else if (cNew.ThumbSticks.Left.X < 0)
                return -1;
            else
                return 0;
        }
        /// <summary>
        /// Returns true if climb has been pressed since last update.
        /// </summary>
        /// <returns></returns>
        public Boolean climb()
        {
            return (isButton(btnClimb) || isKey(keyClimb));
        }
        /// <summary>
        /// Returns true if use has been pressed since last update.
        /// </summary>
        /// <returns></returns>
        public Boolean use()
        {
            return (isButton(btnUse) || isKey(keyUse));
        }
        /// <summary>
        /// Returns true if release has been pressed since last update.
        /// </summary>
        /// <returns></returns>
        public Boolean release()
        {
            return (isButton(btnRelease) || isKey(keyRelease));
        }
        #endregion
    }
}
