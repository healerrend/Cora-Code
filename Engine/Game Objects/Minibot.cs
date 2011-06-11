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
    public abstract class Minibot : Player
    {
        public Boolean isActive;
        public Minibot(Texture2D s, List<LevelBlock> walls, LevelState l)
            : base(s, walls, l)
        {
            isActive = false;
        }
        public abstract void LaunchGround(Player p);
        public abstract void LaunchAir(Player p);
        public abstract void Dock(Player p);
    }
}
