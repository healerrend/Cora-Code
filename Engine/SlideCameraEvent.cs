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
    public class SlideCameraEvent : HandledEvent
    {
        public int beginX;
        public int beginY;
        public int endX;
        public int endY;
        public double duration;
        public double timer;
        public SlideCameraEvent(GameState gameState, LevelState level, GameEvent parent, int endX, int endY, double duration)
            : base(gameState, level, parent)
        {
            beginX = gameState.cameraPosition.X;
            beginY = gameState.cameraPosition.Y;
            this.endX = endX;
            this.endY = endY;
            this.duration = duration;
            timer = 0;
        }
        public override void doThis(doPacket pack)
        {
            timer += pack.time.ElapsedGameTime.TotalMilliseconds;
            double t = timer / duration;
            gameState.cameraPosition.X = beginX + (int)(t * (endX - beginX));
            gameState.cameraPosition.Y = beginY + (int)(t * (endY - beginY));
            if (timer >= duration)
                end();
        }
    }
}
