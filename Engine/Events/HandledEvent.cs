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
    public abstract class HandledEvent
    {
        public GameState gameState;
        public LevelState level;
        public GameEvent parent;
        public HandledEvent(GameState gameState, LevelState level, GameEvent parent)
        {
            this.gameState = gameState;
            this.level = level;
            this.parent = parent;
        }
        public abstract void doThis(doPacket pack);
        public virtual void end()
        {
            parent.instructionHasCompleted = true;
            parent.cleanupEvent(this);
        }
        public virtual void drawThis(drawPacket pack)
        {
        }
    }
}
