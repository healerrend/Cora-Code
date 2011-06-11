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
    /// <summary>
    /// This state represents an image or animation that will show on a screen and then load a new state.
    /// In general these can be canceled through a button-press.
    /// </summary>
    public class SplashState : State
    {
        protected Texture2D background; //The image of this splash state
        /// <summary>
        /// Standard constructor.
        /// </summary>
        /// <param name="background">The image of this splash state</param>
        public SplashState()
        {
        }
        /// <summary>
        /// Standard splash state logic is to wait for any button press and then call nextState.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public override void doThis(doPacket pack)
        {
            if (pack.controller.confirm())
            {
                nextState(pack);
            }
        }
        /// <summary>
        /// Only one draw layer is needed for splash state. This will draw directly to screen.
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawToScreen(drawPacket pack)
        {
            pack.sb.Begin();
            pack.sb.Draw(background, Vector2.Zero, Color.White);
            pack.sb.End();
        }
        /// <summary>
        /// Must be overridden. This method will load the next state into the master game state.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public virtual void nextState(doPacket pack) { }
    }
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
    /// <summary>
    /// This is the level state. All levels will be children of this state.
    /// </summary>
    public class LevelState : State
    {
        #region Instance Variables
        public Vector2 levelSize; //The size, in pixels, of the level.
        public List<Doodad> background;
        protected Song bgMusic; //The background music, if any, of the level.
        public Player player; //The object the player is controlling.
        public List<GameObject> objects; //A container of all GameObjects for this level
        public List<LevelBlock> walls; //A container for all LevelBlocks for this level
        public List<HitBoxInteractable> interactables; //A container for all HitBoxInteractables for this level
        public List<Doodad> doodads;
        #endregion
        public LevelState()
        {
            objects = new List<GameObject>();
            walls = new List<LevelBlock>();
            interactables = new List<HitBoxInteractable>();
            doodads = new List<Doodad>();
            background = new List<Doodad>();
        }
        /// <summary>
        /// The do method of a level state should propagate the do chain throughout all objects the level contains.
        /// Additionally, any logic which is above the object level but below the master level should be conducted here.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public override void doThis(doPacket pack)
        {
            foreach (LevelBlock l in walls)
                l.doThis(pack, player);
            foreach (HitBoxInteractable i in interactables)
                i.doThis(pack);
            foreach (GameObject o in objects)
                o.doThis(pack);
        }

        //EACH OF THE DRAW METHODS ARE DEFAULTS. Sometimes you may not want to draw all layers.
        //These defaults will pass the current frame through the layers unchanged.

        /// <summary>
        /// Draw the world.
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawWorld(drawPacket pack)
        {
            pack.sb.GraphicsDevice.SetRenderTarget(pack.state.gameWorld);
            pack.sb.Begin();
            foreach (Doodad d in background)
                d.drawThis(pack);
            foreach(GameObject o in objects)
                o.drawThis(pack);
            pack.sb.End();
        }
        /// <summary>
        /// Draw the effects.
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawEffects(drawPacket pack)
        {
            pack.sb.GraphicsDevice.SetRenderTarget(pack.state.effects);
            pack.sb.Begin();
            pack.sb.Draw(pack.state.gameWorld, Vector2.Zero, Color.White);
            pack.sb.End();
        }
        /// <summary>
        /// Draw the UI
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawUI(drawPacket pack)
        {
            pack.sb.GraphicsDevice.SetRenderTarget(pack.state.UI);
            pack.sb.Begin();
            pack.sb.Draw(pack.state.effects, Vector2.Zero, Color.White);
            pack.sb.End();
        }
        /// <summary>
        /// Draw to the screen.
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawToScreen(drawPacket pack)
        {
            pack.sb.GraphicsDevice.SetRenderTarget(null);
            pack.sb.Begin();
            pack.sb.Draw(pack.state.UI, Vector2.Zero, Color.White);
            pack.sb.End();
        }
    }
    /// <summary>
    /// This is the test level state.
    /// </summary>
    public class Level1State : LevelState
    {
        public Texture2D corasprite;

        public Level1State() {}

        public override void loadState(GameState state, ContentManager content)
        {
            this.state = state;

            TextureLoader.junkSpaceBG = content.Load<Texture2D>("junk\\space-1280x720");
            corasprite = content.Load<Texture2D>("junk\\walksheet");
            TextureLoader.grayblock = content.Load<Texture2D>("junk\\graysquare");
            TextureLoader.redsquare = content.Load<Texture2D>("RealAssets\\redsquare");

            levelSize = new Vector2(5000, 3000);
            walls.Add(new Wall(new BoundingBox(new Vector3(0,1000,0), new Vector3(5000, 1020, 0)), this));
            walls.Add(new Wall(new BoundingBox(new Vector3(701, 800, 0), new Vector3(1000, 850, 0)), this));
            walls.Add(new Slope(this, new Point(400, 1000), new Point(700, 800)));
            walls.Add(new MovingPlatform(new BoundingBox(new Vector3(900,800,0), new Vector3(1000,850,0)), this, new Point(900,600), new Point(1000,800), MovingPlatformRotationType.Bouncing, 5, true, true));

            foreach (LevelBlock w in walls)
            {
                w.Sprite = TextureLoader.grayblock;
            }
            player = new Player(corasprite, walls, this);


            objects.Add(player);
        }
        public override void doThis(doPacket pack)
        {
            base.doThis(pack);
            if (pack.state.playerPosition.X - pack.state.cameraPosition.X < 200) //This will keep the camera on the player, but inside the level.
                pack.state.cameraPosition.X = (int)pack.state.playerPosition.X - 200;
            if(pack.state.playerPosition.X - pack.state.cameraPosition.X > 1080)
                pack.state.cameraPosition.X = (int)pack.state.playerPosition.X - 1080;
            if(pack.state.playerPosition.Y - pack.state.cameraPosition.Y < 360)
                pack.state.cameraPosition.Y = (int)pack.state.playerPosition.Y - 360;
            if (pack.state.playerPosition.Y - pack.state.cameraPosition.Y > 620)
                pack.state.cameraPosition.Y = (int)pack.state.playerPosition.Y - 620;

            if (pack.state.cameraPosition.X < 0)
                pack.state.cameraPosition.X = 0;
            if (pack.state.cameraPosition.Y < 0)
                pack.state.cameraPosition.Y = 0;
            if (pack.state.cameraPosition.X > levelSize.X - 1280)
                pack.state.cameraPosition.X = (int)levelSize.X - 1280;
            if (pack.state.cameraPosition.Y > levelSize.Y - 720)
                pack.state.cameraPosition.Y = (int)levelSize.Y - 720;
                
        }
        public override void drawWorld(drawPacket pack)
        {
            pack.sb.GraphicsDevice.SetRenderTarget(pack.state.gameWorld);
            pack.sb.Begin();
            pack.sb.Draw(TextureLoader.junkSpaceBG, Vector2.Zero, Color.White);
            pack.sb.End();

            pack.sb.GraphicsDevice.SetRenderTarget(pack.state.gameWorld);
            pack.sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Matrix.CreateTranslation(new Vector3(-pack.state.cameraPosition.X, -pack.state.cameraPosition.Y, 0)) * Matrix.CreateScale(1));
            foreach (Doodad d in background)
                d.drawThis(pack);
            foreach (LevelBlock w in walls)
                w.drawThis(pack);
            foreach (GameObject o in objects)
                o.drawThis(pack);
            foreach (HitBoxInteractable h in interactables)
                h.drawThis(pack);
            pack.sb.End();
        }
    }
}
