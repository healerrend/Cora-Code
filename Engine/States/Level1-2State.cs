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
    public class Level1_2State : LevelState
    {
        #region EventFlags
        public Boolean eventIsHappening = false;
        public Boolean runningTutorial = false;
        #endregion

        public Texture2D corasprite;
        public Texture2D runtutorial;
        public Texture2D toolbotsprite;
        public GameEvent loadingEvent;
        public delegate void act(DelegateParams parameters);
        act executeEvent = GameState.ExecuteEvent;
        act loadSeamlessly = GameState.LoadSeamlessly;
        act phaseOutPrevious = GameState.PhaseOutPrevious;

        public Level1_2State() { }

        public override void loadState(GameState state, ContentManager content)
        {
            this.state = state;

            TextureLoader.junkSpaceBG = content.Load<Texture2D>("junk\\space-1280x720");
            corasprite = content.Load<Texture2D>("junk\\walksheet");
            toolbotsprite = content.Load<Texture2D>("junk\\walksheet");
            TextureLoader.grayblock = content.Load<Texture2D>("junk\\graysquare");
            TextureLoader.redsquare = content.Load<Texture2D>("RealAssets\\redsquare");

            loadingEvent = new GameEvent(state, this, "activatetoolbot");
            loadingEvent.loadScript("..\\..\\..\\code\\content\\scripts\\level1.csl", "activateToolbot", content);
            events.Add(loadingEvent);


            //this.levelSize.X = 3500;
            //this.levelSize.Y = 1500;
            state.maxX += 3350;
            state.minY -= 720;
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1650, 550, 0), new Vector3(1700, 735, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(150, 200, 0), new Vector3(3500, 250, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(100, 250, 0), new Vector3(150, 1100, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(400, 500, 0), new Vector3(450, 1000, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(400, 500, 0), new Vector3(1100, 550, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1100, 500, 0), new Vector3(1300, 550, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1050, 500, 0), new Vector3(1100, 1350, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1300, 500, 0), new Vector3(1350, 1190, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1300, 500, 0), new Vector3(1600, 550, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(2000, 500, 0), new Vector3(2600, 540, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(2800, 500, 0), new Vector3(3500, 550, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(2800, 500, 0), new Vector3(2850, 1350, 0)), this));

            this.walls.Add(new Wall(new BoundingBox(new Vector3(1050, 1300, 0), new Vector3(2850, 1350, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(2200, 500, 0), new Vector3(2250, 1225, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1600, 1700, 0), new Vector3(2000, 750, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1460, 750, 0), new Vector3(1500, 900, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1460, 850, 0), new Vector3(1650, 900, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1850, 850, 0), new Vector3(1900, 900, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(2000, 900, 0), new Vector3(2200, 950, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1600, 900, 0), new Vector3(1650, 1050, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1600, 1000, 0), new Vector3(1950, 1050, 0)), this));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1900, 900, 0), new Vector3(1950, 1050, 0)), this));

            this.walls.Add(new Wall(new BoundingBox(new Vector3(1600, 1050, 0), new Vector3(2250, 1100, 0)), this));

            this.walls.Add(new Slope(this, new Point(1600,500), new Point(2000,700)));
            this.walls.Add(new Wall(new BoundingBox(new Vector3(1650, 700, 0), new Vector3(2000, 735, 0)), this));
            this.walls.Add(new Slope(this, new Point(1900, 850), new Point(2000, 900)));

            //this.doodads.Add(new Doodad(TextureLoader.redsquare, new Vector2(1150, 1250)));
            this.objects.Add(new Toolbot(toolbotsprite, walls, this, new Vector2(1150, 1250), "toolbot"));
            this.interactables.Add(new PressurePlate(new BoundingBox(new Vector3(200,750,0), new Vector3(500,850,0)), this, null, loadSeamlessly, new GenericObjectParams(this, state), false));
            this.interactables.Add(new PressurePlate(new BoundingBox(new Vector3(500, 250, 0), new Vector3(700, 500, 0)), this, null, phaseOutPrevious, new GenericObjectParams(this, state), false)); 
            this.interactables.Add(new ControlPanel(new BoundingBox(new Vector3(1125, 1200, 0), new Vector3(1175, 1200, 0)), this, null, executeEvent, new EventParams(this, "activatetoolbot"), false));
            

            foreach (LevelBlock w in walls)
            {
                w.Sprite = TextureLoader.grayblock;
            }
            //this.walls.Add(new Slope(this, new Point(0, 370), new Point(450, 250)));

            if (state.player == null)
            {
                player = new Player(corasprite, walls, this);
                state.player = player;
            }
            else
                player = state.player;
            objects.Add(player);
        }
        public override void doThis(doPacket pack)
        {
            base.doThis(pack);
        }
        public override void translate()
        {
            base.translate(new Vector2(2900,-920));
            foreach (LevelBlock w in ((LevelState)state.state).walls)
                if (w.Name.Equals("elevator"))
                    this.walls.Add(w);
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
