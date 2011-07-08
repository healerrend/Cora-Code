using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

/* This file is the master game state for the game.
 */

namespace CORA
{
    /// <summary>
    /// This is the master game state class. This class is the workhorse for the game.
    /// All of the global variables are held here, and any top-level management methods are handled here.
    /// </summary>
    public class GameState
    {
        #region Instance Variables
        public RenderTarget2D gameWorld; //A temporary render target for the game world
        public RenderTarget2D effects; //A temporary render target for applying effects to the game world
        public RenderTarget2D UI; //A temporary render target for the game UI
        public RenderTarget2D pauseBG; //A temporary render target for the background of a game pause

        public doPacket doPack; //The doPacket, used to propagate the doThis method tree.
        public drawPacket drawPack; //The drawPacket, used to propagate the drawThis method tree.
        public ContentManager content; //The content manager, used to load assets.
        public ContentManager phaseOutContent;
        public ContentManager standByContent;

        public Player player; //The current object controlled by the player.
        public CollisionPoint playerPosition; //The current position of the player.
        public Point cameraPosition; //The current position of the camera.
        public float cameraScale; //The scale to display the camera

        public State state; //The game's current state.
        public State tempState; //A temporary state overriding the game's current state, such as a pause menu or in-game cinematic.
        public State newState; //A state waiting to be switched to
        public State standByState; //A state that is [being] loaded for a seamless transition
        public State phaseOutState;
        public Boolean finishedSeamlessLoad;
        public Boolean doDrawPhaseOutState;
        public Boolean doDrawStandByState;

        SpriteBatch sb; //The sprite batch, used to draw things.
        public Controller controller; //The controller, updated every frame.
        public Boolean exitGame; //True if the game needs to exit, otherwise false.

        public Boolean paused; //True if the game is paused, otherwise false.
        public Boolean frameCaptured; //True if the frame has been captured for display during pause, otherwise false.
        public Boolean acceptPlayerInput = true;

        public Color tint = Color.White;

        public Microsoft.Xna.Framework.Storage.StorageContainer storageContainer;
        public Microsoft.Xna.Framework.Storage.StorageDevice storageDevice;
        #endregion
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sb">The sprite batch</param>
        /// <param name="c">The content manager</param>
        public GameState(SpriteBatch sb, ContentManager c) 
        {
            GameTime time = new GameTime();
            controller = new Controller();
            doPack = new doPacket(this, time, controller);
            drawPack = new drawPacket(this, time, sb);
            content = c;
            gameWorld = new RenderTarget2D(sb.GraphicsDevice, 1280, 720);
            effects = new RenderTarget2D(sb.GraphicsDevice, 1280, 720);
            UI = new RenderTarget2D(sb.GraphicsDevice, 1280, 720);
            pauseBG = new RenderTarget2D(sb.GraphicsDevice, 1280, 720);
            exitGame = false;
            playerPosition = new CollisionPoint();
            cameraPosition = new Point();
            paused = false;
            frameCaptured = false;
            finishedSeamlessLoad = false;
            doDrawPhaseOutState = false;
            doDrawStandByState = false;
        }
        /// <summary>
        /// This method fires to begin the game.
        /// </summary>
        public void initialize()
        {
            cameraScale = 1;
            state = new IntroSplashState();
            state.loadState(this, content);
        }
        /// <summary>
        /// This is the method where the do chain begins. This will call the state's doThis.
        /// </summary>
        /// <param name="gameTime">The game time</param>
        public void doThis(GameTime gameTime)
        {
            if (newState != null)
            {
                content.Unload();
                newState.loadState(this, content);
                state = newState;
                newState = null;
                TextureLoader.ArBonnie = content.Load<SpriteFont>("realassets\\arbonnie");
                TextureLoader.CourierNew = content.Load<SpriteFont>("realassets\\couriernew");
                TextureLoader.QuartzMS = content.Load<SpriteFont>("realassets\\quartzms");
                TextureLoader.RussellSquare = content.Load<SpriteFont>("realassets\\russellsquare");
                TextureLoader.SketchFlowPrint = content.Load<SpriteFont>("realassets\\sketchflowprint");
                GC.Collect(); //After unloading content, collect the garbage. This will probably be done anyway, but we want to ensure it.
            }
            controller.updateController(); //Always update the controller and the time
            doPack.time = gameTime;
            if (!paused) //THIS LOGIC NEEDS TO BE REVISED
            {
                state.doThis(doPack);
                if (doDrawPhaseOutState)
                    phaseOutState.doThis(doPack);
                if (doDrawStandByState)
                    standByState.doThis(doPack);
            }
            else
            {
                tempState.doThis(doPack);
            }
        }
        /// <summary>
        /// This is the method from which the draw chain begins. It will begin the 4-step draw.
        /// </summary>
        /// <param name="gameTime">The game time</param>
        public void drawThis(GameTime gameTime)
        {
            if (paused && !frameCaptured) //IF: Game is paused but frame is not yet captured
            {
                drawPack.time = gameTime;
                state.drawWorld(drawPack);
                if (doDrawPhaseOutState)
                    phaseOutState.drawWorld(drawPack);
                if (doDrawStandByState)
                    standByState.drawWorld(drawPack);
                state.drawEffects(drawPack);
                if (doDrawPhaseOutState)
                    phaseOutState.drawEffects(drawPack);
                if (doDrawStandByState)
                    standByState.drawEffects(drawPack);
                state.drawUI(drawPack);
                if (doDrawPhaseOutState)
                    phaseOutState.drawUI(drawPack);
                if (doDrawStandByState)
                    standByState.drawUI(drawPack);
                state.drawToPause(drawPack);
                if (doDrawPhaseOutState)
                    phaseOutState.drawToPause(drawPack);
                if (doDrawStandByState)
                    standByState.drawToPause(drawPack);
                drawPack.sb.GraphicsDevice.SetRenderTarget(null);
                frameCaptured = true;
            }
            else if (paused && frameCaptured) //IF: Game is paused and the frame has been captured
            {
                tempState.drawPause(drawPack);
            }
            else //IF: Game is unpaused
            {
                drawPack.time = gameTime;
                state.drawWorld(drawPack);
                if (doDrawPhaseOutState)
                    phaseOutState.drawWorld(drawPack);
                if (doDrawStandByState)
                    standByState.drawWorld(drawPack);
                state.drawEffects(drawPack);
                if (doDrawPhaseOutState)
                    phaseOutState.drawEffects(drawPack);
                if (doDrawStandByState)
                    standByState.drawEffects(drawPack);
                state.drawUI(drawPack);
                if (doDrawPhaseOutState)
                    phaseOutState.drawUI(drawPack);
                if (doDrawStandByState)
                    standByState.drawUI(drawPack);
                state.drawToScreen(drawPack);
                if (doDrawPhaseOutState)
                    phaseOutState.drawToScreen(drawPack);
                if (doDrawStandByState)
                    standByState.drawToScreen(drawPack);
            }
        }
        /// <summary>
        /// This method will flip the switched used to pause the game
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public void pause(doPacket pack)
        {
            frameCaptured = false;
            paused = true;
        }
        /// <summary>
        /// This method will flip the switch(es) to unpause the game.
        /// </summary>
        /// <param name="pack">see dopacket</param>
        public void unpause(doPacket pack)
        {
            paused = false;
        }
        /// <summary>
        /// This method will display an options menu over the current screen during pause.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public void displayOptions(doPacket pack)
        {
            tempState = new OptionsMenuState();
            tempState.loadState(this, content);
        }
        /// <summary>
        /// This method will display a pause menu over the current screen during pause.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public void displayPause(doPacket pack)
        {
         //FILL ME UP   
        }
        /// <summary>
        /// This method will cease displaying and unload an options menu during pause. This should be called before unpause when an options menu is displayed.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public void removeOptions(doPacket pack)
        {
            tempState.unloadState(content);
            tempState = null;
            unpause(pack);
        }
        /// <summary>
        /// This method will cease displaying and unload a pause menu during pause. This should be called before unpause when a pause menu is displayed.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public void removePause(doPacket pack)
        {
        }
        /// <summary>
        /// This utility method will load a state using the content manager.
        /// </summary>
        /// <param name="newState">The state to be loaded</param>
        public void loadState(State newState)
        {
            this.newState = newState;
        }
        /// <summary>
        /// This will set the controller after the start button
        /// </summary>
        /// <param name="c"></param>
        public void setController(Controller c)
        {
            controller = c;
            doPack.controller = c;
        }
        public void initiateLoadStandByState(LevelState s)
        {
            Thread loadStandByStateThread = new Thread(loadStandByState);
            loadStandByStateThread.Start(s);
        }
        public void loadStandByState(object s)
        {
            standByContent = new ContentManager(content.ServiceProvider, content.RootDirectory);

            ((LevelState)s).loadState(this, standByContent);
            finishedSeamlessLoad = true;
        }
        public void activateStandByState()
        {
            doDrawStandByState = true;
        }
        public void loadSeamlessly()
        {
            phaseOutState = state;
            phaseOutContent = content;
            state = standByState;
            content = standByContent;
            doDrawPhaseOutState = true;
            doDrawStandByState = false;
        }
        public void phaseOutOldContent()
        {
            doDrawPhaseOutState = false;
            phaseOutContent.Dispose();
            phaseOutState = null;
        }
    }
}
