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
    }
    public class SpawnParams : DelegateParams
    {
        public LevelState level;
        public Vector2 position;
        public SpawnParams(LevelState level, Vector2 position)
        {
            this.level = level;
            this.position = position;
        }
    }
}