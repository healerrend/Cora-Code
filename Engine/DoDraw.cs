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
    /// This is a wrapper class which contains the minimum amount needed for an object to do basic logic on itself and the game world every update.
    /// </summary>
    public class doPacket
    {
        public GameState state; //The master game state
        public Controller controller; //The controller
        public GameTime time; //The GameTime, which includes the time since the game was started, and the time elapsed since last update.
        /// <summary>
        /// Creates a doPacket. Only one of these is needed per update.
        /// </summary>
        /// <param name="s">The master game state</param>
        /// <param name="t">The GameTime, which includes the time since the game was started, and the time elapsed since last update.</param>
        /// <param name="c">The controller</param>
        public doPacket(GameState s, GameTime t, Controller c)
        {
            state = s;
            controller = c;
            time = t;
        }
    }
    /// <summary>
    /// This is a wrapper class which contains the minimum amount needed for an object to do basic drawing of itself every update.
    /// </summary>
    public class drawPacket
    {
        public GameState state; //The master game state
        public GameTime time; //The GameTime, which includes the time since the game was started, and the time elapsed since last update.
        public SpriteBatch sb; //The sprite batch, which is the tool that actually draws.
        /// <summary>
        /// Creates a drawPacket. Only one of these is needed per update.
        /// </summary>
        /// <param name="s">The master game state</param>
        /// <param name="t">The GameTime, which includes the time since the game was started, and the time elapsed since last update.</param>
        /// <param name="c">The sprite batch, which is the tool that actually draws.</param>
        public drawPacket(GameState s, GameTime t, SpriteBatch c)
        {
            state = s;
            sb = c;
            time = t;
        }
    }
}