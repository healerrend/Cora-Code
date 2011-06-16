using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CORA
{
    public class WaitEvent : HandledEvent
    {
        public double duration;
        public double timer;
        public WaitEvent(GameState gameState, LevelState level, GameEvent parent, double duration)
            : base(gameState, level, parent)
        {
            this.duration = duration;
            timer = 0;
        }
        public override void doThis(doPacket pack)
        {
            timer += pack.time.ElapsedGameTime.TotalMilliseconds;
            if (timer >= duration)
                end();
        }
    }
}
