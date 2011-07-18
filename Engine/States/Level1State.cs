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
    /// This is the test level state.
    /// </summary>
    public class Level1State : LevelState
    {
        #region EventFlags
        public Boolean eventIsHappening = false;
        public Boolean runningTutorial = false;
        #endregion

        public Texture2D corasprite;
        public Texture2D runtutorial;
        public GameEvent loadingEvent;
        public delegate void act(DelegateParams parameters);
        act activateTutorial = Level1State.ActivateTutorial;
        act openDoor = Level1State.ToggleDoor;
        act loadNextlevel = Level1State.LoadNextLevel;
        act drawNextLevel = Level1State.DrawNextLevel;
        act engageElevator = Level1State.EngageElevator;
        public static void ActivateTutorial(DelegateParams parameters)
        {
            ((EventParams)parameters).level.events[((EventParams)parameters).eventID].execute();
        }
        public static void ToggleDoor(DelegateParams parameters)
        {
            ((OpenDoorParams)parameters).door.toggle();
        }
        public static void LoadNextLevel(DelegateParams parameters)
        {
            ((GameStateParams)parameters).state.standByState = new Level1_2State();
            ((GameStateParams)parameters).state.initiateLoadStandByState();
        }
        public static void DrawNextLevel(DelegateParams parameters)
        {
            ((GameStateParams)parameters).state.activateStandByState();
        }
        public static void EngageElevator(DelegateParams parameters)
        {
            ((MovingPlatform)((GenericObjectParams)parameters).o).IsActive = true;
        }

        public Level1State() { }

        public override void loadState(GameState state, ContentManager content)
        {
            this.state = state;

            TextureLoader.junkSpaceBG = content.Load<Texture2D>("junk\\space-1280x720");
            corasprite = content.Load<Texture2D>("junk\\walksheet");
            TextureLoader.grayblock = content.Load<Texture2D>("junk\\graysquare");
            TextureLoader.redsquare = content.Load<Texture2D>("RealAssets\\redsquare");
            loadingEvent = new GameEvent(state, this);
            loadingEvent.loadScript("..\\..\\..\\code\\content\\scripts\\level1.csl", "runtutorial", content);
            events.Add(loadingEvent);
            loadingEvent = new GameEvent(state, this);
            loadingEvent.loadScript("..\\..\\..\\code\\content\\scripts\\level1.csl", "jumptutorial", content);
            events.Add(loadingEvent);
            loadingEvent = new GameEvent(state, this);
            loadingEvent.loadScript("..\\..\\..\\code\\content\\scripts\\level1.csl", "doublejumptutorial", content);
            events.Add(loadingEvent);
            loadingEvent = new GameEvent(state, this);
            loadingEvent.loadScript("..\\..\\..\\code\\content\\scripts\\level1.csl", "controlpaneltutorial", content);
            events.Add(loadingEvent);
            /*
            this.levelSize.X = 3350;
            this.levelSize.Y = 720;
             */
            state.maxX += 3350;
            state.minY -= 720;
            this.walls.Add(new Wall(new BoundingBox(new Vector3(-34, -2, 0), new Vector3(-1, 740, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(0, 620, 0), new Vector3(1150, 670, 0)), this));
            //this.walls.Add(new Wall(new BoundingBox(new Vector3(200, 617, 0), new Vector3(400, 622, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1100, 570, 0), new Vector3(1950, 620, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1900, 420, 0), new Vector3(1950, 570, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(2500, 330, 0), new Vector3(3350, 370, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(3300, 0, 0), new Vector3(3350, 330, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(0, 270, 0), new Vector3(650, 320, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(600, 270, 0), new Vector3(650, 420, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(600, 370, 0), new Vector3(1000, 420, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(950, 320, 0), new Vector3(1050, 370, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1000, 270, 0), new Vector3(1100, 320, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1050, 220, 0), new Vector3(1150, 270, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1100, 170, 0), new Vector3(3050, 220, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(3000, 0, 0), new Vector3(3050, 170, 0)), this));
            MovingPlatform m = new MovingPlatform(new BoundingBox(new Vector3(3000, 330, 0), new Vector3(3300, 370, 0)), this, new Point(3000, 330), new Point(3000, -530), MovingPlatformRotationType.Bouncing, 7, false, true);
            m.IsActive = false;
            m.RepeatY = false;
            m.RepeatX = false;
            this.walls.Add(m);
            interactables.Add(new PressurePlate(new BoundingBox(new Vector3(3250, 300, 0), new Vector3(3300, 320, 0)), this, null, engageElevator, new GenericObjectParams(this, m)));

            this.walls.Add(new Slope(this, new Point(1900, 420), new Point(2500, 330)));
            foreach (LevelBlock w in walls)
            {
                w.Sprite = TextureLoader.grayblock;
            }
            //this.walls.Add(new Slope(this, new Point(0, 370), new Point(450, 250)));
            this.walls.Add(new Door(this, new Wall(new BoundingBox(new Vector3(2775, 220, 0), new Vector3(2825, 330, 0)), this, null), new Animation(TextureLoader.grayblock, 50, 110, 1, 2, false, 100), 1500));
            this.interactables.Add(new ControlPanel(new BoundingBox(new Vector3(2600, 250, 0), new Vector3(2630, 280, 0)), this, TextureLoader.redsquare, openDoor, new OpenDoorParams(this, (Door)walls.Last<LevelBlock>()), false));

            interactables.Add(new PressurePlate(new BoundingBox(new Vector3(100, 600, 0), new Vector3(150, 650, 0)), this, null, activateTutorial, new EventParams(this, 0)));
            interactables.Add(new PressurePlate(new BoundingBox(new Vector3(1400, 500, 0), new Vector3(1500, 600, 0)), this, null, activateTutorial, new EventParams(this, 1)));
            interactables.Add(new PressurePlate(new BoundingBox(new Vector3(100, 600, 0), new Vector3(150, 650, 0)), this, null, activateTutorial, new EventParams(this, 2)));
            interactables.Add(new PressurePlate(new BoundingBox(new Vector3(100, 600, 0), new Vector3(150, 650, 0)), this, null, activateTutorial, new EventParams(this, 3)));
            interactables.Add(new PressurePlate(new BoundingBox(new Vector3(100, 600, 0), new Vector3(150, 650, 0)), this, null, loadNextlevel, new GameStateParams(this, state), false));
            interactables.Add(new PressurePlate(new BoundingBox(new Vector3(2000, 200, 0), new Vector3(3000, 600, 0)), this, null, drawNextLevel, new GameStateParams(this, state), false));

            if (state.player == null)
            {
                player = new Player(corasprite, walls, this);
                state.player = player;
            }
            objects.Add(player);
            player.movePlayer(new Point(100, 600));
        }
        public override void doThis(doPacket pack)
        {
            base.doThis(pack);
        }
        public override void drawWorld(drawPacket pack)
        {
            pack.sb.GraphicsDevice.SetRenderTarget(pack.state.gameWorld);
            pack.sb.Begin();
            //pack.sb.Draw(TextureLoader.junkSpaceBG, Vector2.Zero, Color.White);
            pack.sb.End();

            pack.sb.GraphicsDevice.SetRenderTarget(pack.state.gameWorld);
            pack.sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Matrix.CreateTranslation(new Vector3(-pack.state.cameraPosition.X, -pack.state.cameraPosition.Y, 0)) * Matrix.CreateScale(1));
            foreach (Doodad d in background)
                d.drawThis(pack);
            foreach (LevelBlock w in walls)
                w.drawThis(pack);
            foreach (Doodad d in doodads)
                d.drawThis(pack);
            foreach (GameObject o in objects)
                o.drawThis(pack);
            foreach (HitBoxInteractable h in interactables)
                h.drawThis(pack);
            pack.sb.End();
        }
    }
}
