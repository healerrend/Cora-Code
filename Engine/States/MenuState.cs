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
    /// This is the basic menu state. Menu states have the logic for each menu item included, so there is little in this "hollow class"
    /// </summary>
    public abstract class MenuState : State
    {
        #region Instance Variables
        protected int selectedIndex; //The currently selected menu index
        protected int totalItems; //The total number of menu items
        #endregion
        /// <summary>
        /// Standard constructor.
        /// </summary>
        /// <param name="itemCount">Number of items in the menu</param>
        public MenuState(int itemCount)
        {
            totalItems = itemCount;
        }
        /// <summary>
        /// Typical menu state do method will only handle the logic of selecting and changing menu items. 
        /// Method calls will probably be used to perform the menu item logic itself.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public override void doThis(doPacket pack)
        {
            if (pack.controller.up())
            {
                selectedIndex--;
                if (selectedIndex < 0)
                    selectedIndex = totalItems - 1;
            }
            if (pack.controller.down())
            {
                selectedIndex++;
                if (selectedIndex == totalItems)
                    selectedIndex = 0;
            }
        }
    }
}
