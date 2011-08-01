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
    public class DelegateParams
    {
        public LevelState level;
        public DelegateParams(LevelState level)
        {
            this.level = level;
        }
    }
    public class SpawnParams : DelegateParams
    {
        public Vector2 position;
        public SpawnParams(LevelState level, Vector2 position) : base(level)
        {
            this.position = position;
        }
    }
    public class EventParams : DelegateParams
    {
        public string eventID;
        public EventParams(LevelState level, string eventID) : base(level)
        {
            this.eventID = eventID;
        }
    }
    public class OpenDoorParams : DelegateParams
    {
        public Door door;
        public OpenDoorParams(LevelState level, Door door)
            : base(level)
        {
            this.door = door;
        }
    }
    public class GameStateParams : DelegateParams
    {
        public GameState state;
        public GameStateParams(LevelState level, GameState state)
            : base(level)
        {
            this.state = state;
        }
    }
    public class GenericObjectParams : DelegateParams
    {
        public Object o;
        public GenericObjectParams(LevelState level, Object o) : base (level)
        {
            this.o = o;
        }
    }
    public class DoubleObjectParams : DelegateParams
    {
        public Object o;
        public Object p;
        public DoubleObjectParams(LevelState level, Object o, Object p)
            : base(level)
        {
            this.o = o;
            this.p = p;
        }
    }
}