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
        public delegate void act(DelegateParams parameters);
        act activateRunTutorial = Level1State.ActivateRunTutorial;
        public static void ActivateRunTutorial(DelegateParams parameters)
        {

        }

        public Level1State() { }

        public override void loadState(GameState state, ContentManager content)
        {
            this.state = state;

            TextureLoader.junkSpaceBG = content.Load<Texture2D>("junk\\space-1280x720");
            corasprite = content.Load<Texture2D>("junk\\walksheet");
            TextureLoader.grayblock = content.Load<Texture2D>("junk\\graysquare");
            TextureLoader.redsquare = content.Load<Texture2D>("RealAssets\\redsquare");
            runtutorial = content.Load<Texture2D>("junk\\runtutorial");

            this.levelSize.X = 3350;
            this.levelSize.Y = 720;
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
            this.walls.Add(new Slope(this, new Point(1900, 420), new Point(2500, 330)));
            //this.walls.Add(new Slope(this, new Point(0, 370), new Point(450, 250)));

            interactables.Add(new PressurePlate(new BoundingBox(new Vector3(100,600,0), new Vector3(150,650,0)), this, null, null));

            foreach (LevelBlock w in walls)
            {
                w.Sprite = TextureLoader.grayblock;
            }
            player = new Player(corasprite, walls, this);


            objects.Add(player);
            player.movePlayer(new Point(100, 600));
        }
        public override void doThis(doPacket pack)
        {
            base.doThis(pack);
            if (pack.state.playerPosition.X - pack.state.cameraPosition.X < 400) //This will keep the camera on the player, but inside the level.
                pack.state.cameraPosition.X = (int)pack.state.playerPosition.X - 400;
            if (pack.state.playerPosition.X - pack.state.cameraPosition.X > 870)
                pack.state.cameraPosition.X = (int)pack.state.playerPosition.X - 870;
            if (pack.state.playerPosition.Y - pack.state.cameraPosition.Y < 360)
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
            //pack.sb.Draw(TextureLoader.junkSpaceBG, Vector2.Zero, Color.White);
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
