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
        public Color cBegin;
        public Color cEnd;
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
            this.cBegin = new Color(byte.Parse(color[0]), byte.Parse(color[1]), byte.Parse(color[2]));
            color = end.Split(',');
            this.cEnd = new Color(byte.Parse(color[0]), byte.Parse(color[1]), byte.Parse(color[2]));
            rBegin = this.cBegin.R;
            gBegin = this.cBegin.G;
            bBegin = this.cBegin.B;
            rEnd = this.cEnd.R;
            gEnd = this.cEnd.G;
            bEnd = this.cEnd.B;
            current = this.cBegin;
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
                current.R = (byte)(rBegin + (byte)(t * (rEnd - rBegin)));
            else
                current.R = (byte)(rEnd + (byte)(t * (rBegin - rEnd)));
            if (gFadeIn)
                current.G = (byte)(gBegin + (byte)(t * (gEnd - gBegin)));
            else
                current.G = (byte)(gEnd + (byte)(t * (gBegin - gEnd)));
            if (bFadeIn)
                current.B = (byte)(bBegin + (byte)(t * (bEnd - bBegin)));
            else
                current.B = (byte)(bEnd + (byte)(t * (bBegin - bEnd)));

                if (timer >= duration)
                {
                    current.R = cEnd.R;
                    current.G = cEnd.G;
                    current.B = cEnd.B;
                    gameState.tint = current;
                    this.end();
                }
        }
    }
}
