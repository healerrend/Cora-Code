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
    public class FadeEvent : HandledEvent
    {
        public Color begin;
        public Color end;
        public Color current;
        public double duration;
        public double timer;
        public byte rBegin;
        public byte gBegin;
        public byte bBegin;
        public byte rEnd;
        public byte gEnd;
        public byte bEnd;
        Boolean rFadeIn = true;
        Boolean gFadeIn = true;
        Boolean bFadeIn = true;
        public FadeEvent(GameState gameState, LevelState level, GameEvent parent, string begin, string end, double duration) 
            : base(gameState, level, parent)
        {
            this.duration = duration;
            timer = 0;
            String[] color = begin.Split(',');
            this.begin = new Color(byte.Parse(color[0]), byte.Parse(color[1]), byte.Parse(color[2]));
            color = end.Split(',');
            this.end = new Color(byte.Parse(color[0]), byte.Parse(color[1]), byte.Parse(color[2]));
            rBegin = this.begin.R;
            gBegin = this.begin.G;
            bBegin = this.begin.B;
            rEnd = this.end.R;
            gEnd = this.end.G;
            bEnd = this.end.B;
            current = this.begin;
            if (rEnd < rBegin)
                rFadeIn = false;
            if (gEnd < gBegin)
                gFadeIn = false;
            if (bEnd < bBegin)
                bFadeIn = false;
        }
        public override void doThis(doPacket pack)
        {
            double t = timer / duration;
            timer += pack.time.ElapsedGameTime.TotalMilliseconds;
            if (rFadeIn)
                current.R = rBegin + (t * (rEnd - rBegin));
            else
                current.R = rEnd + (t * (rBegin - rEnd));
            if (gFadeIn)
                current.G = gBegin + (t * (gEnd - gBegin));
            else
                current.G = gEnd + (t * (gBegin - gEnd));
            if (bFadeIn)
                current.B = bBegin + (t * (bEnd - bBegin));
            else
                current.B = bEnd + (t * (bBegin - bEnd));

                if (timer >= duration)
                {
                    current.R = end.R;
                    current.G = end.G;
                    current.B = end.B;
                    gameState.tint = current;
                    end();
                }
        }
    }
}
