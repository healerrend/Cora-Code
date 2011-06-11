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
        public Texture2D corasprite;
        public delegate void act(DelegateParams parameters);
        act spawnToolbot = SpawnToolbot;
        public static void SpawnToolbot(DelegateParams parameters)
        {
            SpawnParams p = (SpawnParams)parameters;
            p.level.objects.Add(new Toolbot(TextureLoader.redsquare, p.level.walls, p.level, p.position));
        }

        public Level1State() { }

        public override void loadState(GameState state, ContentManager content)
        {
            this.state = state;

            TextureLoader.junkSpaceBG = content.Load<Texture2D>("junk\\space-1280x720");
            corasprite = content.Load<Texture2D>("junk\\walksheet");
            TextureLoader.grayblock = content.Load<Texture2D>("junk\\graysquare");
            TextureLoader.redsquare = content.Load<Texture2D>("RealAssets\\redsquare");

            levelSize = new Vector2(5000, 3000);
            walls.Add(new Wall(new BoundingBox(new Vector3(0, 1000, 0), new Vector3(1000, 1050, 0)), this));
            walls.Add(new Wall(new BoundingBox(new Vector3(1150, 1000, 0), new Vector3(5000, 1050, 0)), this));
            walls.Add(new Wall(new BoundingBox(new Vector3(1300, 950, 0), new Vector3(1500, 1000, 0)), this));
            walls.Add(new Wall(new BoundingBox(new Vector3(1350, 850, 0), new Vector3(1500, 950, 0)), this));
            
            interactables.Add(new ControlPanel(new BoundingBox(new Vector3(500, 900, 0), new Vector3(550, 950, 0)), this, TextureLoader.redsquare, spawnToolbot, new SpawnParams(this, new Vector2(500,500))));

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
            if (pack.state.playerPosition.X - pack.state.cameraPosition.X > 1080)
                pack.state.cameraPosition.X = (int)pack.state.playerPosition.X - 1080;
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
            pack.sb.Draw(TextureLoader.junkSpaceBG, Vector2.Zero, Color.White);
            pack.sb.End();

            pack.sb.GraphicsDevice.SetRenderTarget(pack.state.gameWorld);
            pack.sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Matrix.CreateTranslation(new Vector3(-pack.state.cameraPosition.X, -pack.state.cameraPosition.Y, 0)) * Matrix.CreateScale(1));
            foreach (Doodad d in background)
                d.drawThis(pack);
            foreach (LevelBlock w in walls)
                w.drawThis(pack);
            foreach (HitBoxInteractable h in interactables)
                h.drawThis(pack);
            foreach (GameObject o in objects)
                o.drawThis(pack);
            pack.sb.End();
        }
    }
}
