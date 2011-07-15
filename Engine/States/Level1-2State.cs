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
    public class Level2State : LevelState
    {
        #region EventFlags
        public Boolean eventIsHappening = false;
        public Boolean runningTutorial = false;
        #endregion

        public Texture2D corasprite;
        public Texture2D runtutorial;
        public GameEvent loadingEvent;
        public delegate void act(DelegateParams parameters);
        act activateTutorial;
        act openDoor;
        public static void ActivateTutorial(DelegateParams parameters)
        {
            ((EventParams)parameters).level.events[((EventParams)parameters).eventID].execute();
        }
        public static void ToggleDoor(DelegateParams parameters)
        {
            ((OpenDoorParams)parameters).door.toggle();
        }

        public Level2State() { }

        public override void loadState(GameState state, ContentManager content)
        {
            this.state = state;

            TextureLoader.junkSpaceBG = content.Load<Texture2D>("junk\\space-1280x720");
            corasprite = content.Load<Texture2D>("junk\\walksheet");
            TextureLoader.grayblock = content.Load<Texture2D>("junk\\graysquare");
            TextureLoader.redsquare = content.Load<Texture2D>("RealAssets\\redsquare");

            this.levelSize.X = 3500;
            this.levelSize.Y = 1500;
            this.walls.Add(new Wall(new BoundingBox(new Vector3(150, 250, 0), new Vector3(3500, 250, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(150, 250, 0), new Vector3(150, 1100, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(400, 500, 0), new Vector3(450, 800, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(400, 500, 0), new Vector3(1100, 550, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1050, 500, 0), new Vector3(1100, 1350, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1300, 500, 0), new Vector3(1350, 1200, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1300, 500, 0), new Vector3(1600, 550, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(2000, 500, 0), new Vector3(2600, 550, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(2800, 500, 0), new Vector3(3500, 550, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(2800, 500, 0), new Vector3(2850, 1350, 0)), this));

            this.walls.Add(new Wall(new BoundingBox(new Vector3(1050, 1300, 0), new Vector3(2850, 1350, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(2200, 500, 0), new Vector3(2250, 1225, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1600, 1700, 0), new Vector3(2000, 750, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1450, 750, 0), new Vector3(1500, 900, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1450, 850, 0), new Vector3(1700, 900, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1850, 850, 0), new Vector3(1900, 900, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(2000, 900, 0), new Vector3(2200, 950, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1600, 900, 0), new Vector3(1700, 1050, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1600, 1000, 0), new Vector3(1950, 1050, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1900, 900, 0), new Vector3(1950, 1050, 0)), this));

            this.walls.Add(new Wall(new BoundingBox(new Vector3(1600, 1050, 0), new Vector3(2250, 1100, 0)), this));

            this.walls.Add(new Slope(this, new Point(1600,500), new Point(2000,700)));
            this.walls.Add(new Slope(this, new Point(1900, 850), new Point(2000, 900)));
            

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
            
            player = new Player(corasprite, walls, this);


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
