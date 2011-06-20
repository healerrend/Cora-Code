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
    public class DisplayTextEvent : HandledEvent
    {
        private SpriteFont font;
        private String message;
        private double duration;
        private double timer;
        private Vector2 position;
        private Color color;
        public DisplayTextEvent(GameState gameState, LevelState level, GameEvent parent, SpriteFont font, String message, double duration, Vector2 position, string colorString)
            : base(gameState, level, parent)
        {
            this.font = font;
            this.message = message;
            this.duration = duration;
            timer = 0;
            this.position = position;
            string[] totalColor = colorString.Split(',');
            this.color = new Color(byte.Parse(totalColor[0]), byte.Parse(totalColor[1]), byte.Parse(totalColor[2]));
        }
        public override void doThis(doPacket pack)
        {
            timer += pack.time.ElapsedGameTime.TotalMilliseconds;
            if (timer >= duration)
                end();
        }
        public override void drawThis(drawPacket pack)
        {
            pack.sb.DrawString(font, message, position, color);
        }
    }
}
