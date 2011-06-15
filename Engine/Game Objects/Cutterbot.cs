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
    public class Cutterbot : Minibot
    {
        public Cutterbot(Texture2D s, List<LevelBlock> walls, LevelState l, Vector2 position) : base(s, walls, l, position) { }
        public override void Dock(Player p)
        {
            throw new NotImplementedException();
        }
        public override void LaunchAir(Player p)
        {
            throw new NotImplementedException();
        }
        public override void LaunchGround(Player p)
        {
            throw new NotImplementedException();
        }
    }
}
