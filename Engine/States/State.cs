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

/* This file contains the base classes for all of the states, as well as the abstract state class.
 */

namespace CORA
{
    /// <summary>
    /// This is the basic state class from which all other states are wrought. It is the alpha and the omega state.
    /// </summary>
    public abstract class State
    {
        public GameState state;
        /// <summary>
        /// Constructor
        /// </summary>
        public State() 
        {}
        /// <summary>
        /// Must be overridden. This method should be called any time this state is loaded into the master game state class.
        /// This method will handle logic which will initialize the state and prepare it for play.
        /// </summary>
        /// <param name="content">The content manager used to load visual assets</param>
        public virtual void loadState(GameState state, ContentManager content) 
        {
            this.state = state;
        }
        /// <summary>
        /// This method should be called any time the state is discarded. It will handle logic which will manually release resources and content.
        /// </summary>
        /// <param name="content">The content manager used to unload visual assets</param>
        public virtual void unloadState(ContentManager content) { }
        /// <summary>
        /// Must be overridden. This method handles all logic for the current game state.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public virtual void doThis(doPacket pack) { }
        /// <summary>
        /// This method will draw all objects in the game world, and then return it to the master game state.
        /// NOTE: ALL DRAW WORLD METHODS MUST SET THE RENDER TARGET TO pack.state.gameWorld
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public virtual void drawWorld(drawPacket pack) { }
        /// <summary>
        /// This method will draw all postprocessing effects on the game world, and then return it to the master game state.
        /// NOTE: ALL DRAW EFFECTS METHODS MUST SET THE RENDER TARGET TO pack.state.effects
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public virtual void drawEffects(drawPacket pack) { }
        /// <summary>
        /// This method will draw all UI components on the effects layer, and then return it to the master game state.
        /// NOTE: ALL DRAW UI METHODS MUST SET THE RENDER TARGET TO pack.state.UI
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public virtual void drawUI(drawPacket pack) { }
        /// <summary>
        /// This method will draw the combined layers to the screen.
        /// NOTE: ALL DRAW TO SCREEN METHODS MUST SET THE RENDER TARGET TO null
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public virtual void drawToScreen(drawPacket pack)
        {
            pack.sb.GraphicsDevice.SetRenderTarget(null);
            pack.sb.Begin();
            pack.sb.Draw(pack.state.UI, Vector2.Zero, Color.White);
            pack.sb.End();
        }
        /// <summary>
        /// This method will be called once before each pause, to take a "screenshot" which the pause menu will be drawn over.
        /// NOTE: ALL DRAW TO PAUSE METHODS MUST SET THE RENDER TARGET TO pack.state.pauseBG
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public virtual void drawToPause(drawPacket pack)
        {
            pack.sb.GraphicsDevice.SetRenderTarget(pack.state.pauseBG);
            pack.sb.Begin();
            pack.sb.Draw(pack.state.UI, Vector2.Zero, Color.Gray);
            pack.sb.End();
        }
        /// <summary>
        /// This method will draw the pause menu of a state. The draw of the pause menu itself should be delegated to the pause menu state.
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public virtual void drawPause(drawPacket pack) { }
        /// <summary>
        /// This method will unpause the state.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public virtual void unpause(doPacket pack) { }

    }
}
