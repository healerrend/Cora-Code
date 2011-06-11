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
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        int frames = 0;
        int maxframes = 0;
        int updates = 0;
        int maxupdates = 0;
        int useconds = 0;
        int fseconds = 0;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameState state;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            InitGraphicsMode(1280, 720, false);
            base.Initialize();
            state = new GameState(spriteBatch, Content);
            state.initialize();
            
        }
        private bool InitGraphicsMode(int iWidth, int iHeight, bool bFullScreen)
        {
            // If we aren't using a full screen mode, the height and width of the window can
            // be set to anything equal to or smaller than the actual screen size.
            if (bFullScreen == false)
            {
                if ((iWidth <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                    && (iHeight <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
                {
                    graphics.PreferredBackBufferWidth = iWidth;
                    graphics.PreferredBackBufferHeight = iHeight;
                    graphics.IsFullScreen = bFullScreen;
                    graphics.ApplyChanges();
                    return true;
                }
            }
            else
            {
                // If we are using full screen mode, we should check to make sure that the display
                // adapter can handle the video mode we are trying to set.  To do this, we will
                // iterate thorugh the display modes supported by the adapter and check them against
                // the mode we want to set.
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    // Check the width and height of each mode against the passed values
                    if ((dm.Width == iWidth) && (dm.Height == iHeight))
                    {
                        // The mode is supported, so set the buffer formats, apply changes and return
                        graphics.PreferredBackBufferWidth = iWidth;
                        graphics.PreferredBackBufferHeight = iHeight;
                        graphics.IsFullScreen = bFullScreen;
                        graphics.ApplyChanges();
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here

            //Fonts
            TextureLoader.ArBonnie = Content.Load<SpriteFont>("realassets\\arbonnie");
            TextureLoader.CourierNew = Content.Load<SpriteFont>("realassets\\couriernew");
            TextureLoader.QuartzMS = Content.Load<SpriteFont>("realassets\\quartzms");
            TextureLoader.RussellSquare = Content.Load<SpriteFont>("realassets\\russellsquare");
            TextureLoader.SketchFlowPrint = Content.Load<SpriteFont>("realassets\\sketchflowprint");
            //Textures
            TextureLoader.introSplash = Content.Load<Texture2D>("junk\\splash");
            TextureLoader.titleMenu = Content.Load<Texture2D>("junk\\menubg");

            TextureLoader.redsquare = Content.Load<Texture2D>("realassets\\redsquare");
            TextureLoader.roughCoraWalk = Content.Load<Texture2D>("junk\\walksheet");
            TextureLoader.junkLevel1 = Content.Load<Texture2D>("junk\\junklevel1");
            TextureLoader.junkSpaceBG = Content.Load<Texture2D>("junk\\space-1280x720");
            TextureLoader.grayblock = Content.Load<Texture2D>("junk\\graysquare");


            state = new GameState(spriteBatch, Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Delete) || state.exitGame)
                this.Exit();

            state.doThis(gameTime);
            if (gameTime.TotalGameTime.Seconds == useconds)
            {
                updates++;
            }
            else
            {
                maxupdates = updates;
                updates = 1;
                useconds = gameTime.TotalGameTime.Seconds;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (gameTime.TotalGameTime.Seconds == fseconds)
            {
                frames++;
            }
            else
            {
                maxframes = frames;
                frames = 1;
                fseconds = gameTime.TotalGameTime.Seconds;
            }

            state.drawThis(gameTime);

            spriteBatch.Begin();
            spriteBatch.DrawString(TextureLoader.QuartzMS, "Updates: " + maxupdates + " - Frames: " + maxframes, new Vector2(550, 25), Color.Black);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
