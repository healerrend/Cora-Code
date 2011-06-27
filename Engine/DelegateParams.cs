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
        public int eventID;
        public EventParams(LevelState level, int eventID) : base(level)
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
}