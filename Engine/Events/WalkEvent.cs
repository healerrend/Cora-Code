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
    public class WalkEvent : HandledEvent
    {
        public CSLObjectType objectType;
        public object o;
        public Vector2 trajectory;
        public float beginX;
        float endX;
        float beginY;
        float endY;
        public double duration;
        public double timer;
        public WalkEvent(GameState gameState, LevelState level, GameEvent parent, CSLObjectType objectType, object o, Vector2 trajectory, double duration, Boolean relative)
            : base(gameState, level, parent)
        {
            this.objectType = objectType;
            this.o = o;
            this.trajectory = trajectory;
            this.duration = duration;
            switch (objectType)
            {
                case CSLObjectType.doodad:
                case CSLObjectType.animateddoodad:
                    beginX = ((Doodad)o).PosX;
                    beginY = ((Doodad)o).PosY;
                    break;
                case CSLObjectType.controlpanel:
                case CSLObjectType.elevatorsurface:
                case CSLObjectType.hangingledge:
                case CSLObjectType.movinghangingledge:
                case CSLObjectType.pressureplate:
                    beginX = ((HitBoxInteractable)o)._X;
                    beginY = ((HitBoxInteractable)o)._Y;
                    break;
                case CSLObjectType.door:
                case CSLObjectType.movingplatform:
                case CSLObjectType.rust:
                case CSLObjectType.slope:
                case CSLObjectType.wall:
                    beginX = ((LevelBlock)o)._X;
                    beginY = ((LevelBlock)o)._Y;
                    break;
                default:
                    beginX = ((GameObject)o).position.X;
                    beginY = ((GameObject)o).position.Y;
                    break;
            }
            if (!relative)
            {
                trajectory.X -= beginX;
                trajectory.Y -= beginY;
            }
            endX = beginX + trajectory.X;
            endY = beginY + trajectory.Y;
        }
        public override void doThis(doPacket pack)
        {
            timer += pack.time.ElapsedGameTime.TotalMilliseconds;
            float t = (float)(timer / duration);
            if (t > 1)
                t = 1;
            trajectory.X = (t * (endX - beginX));
            trajectory.Y = (t * (endY - beginY));
            switch (objectType)
            {
                case CSLObjectType.doodad:
                case CSLObjectType.animateddoodad:
                    ((Doodad)o).PosX = beginX;
                    ((Doodad)o).PosY = beginY;
                    ((Doodad)o).moveThis(trajectory);
                    break;
                case CSLObjectType.controlpanel:
                case CSLObjectType.elevatorsurface:
                case CSLObjectType.hangingledge:
                case CSLObjectType.movinghangingledge:
                case CSLObjectType.pressureplate:
                    ((HitBoxInteractable)o)._X = beginX + trajectory.X;
                    ((HitBoxInteractable)o)._Y = beginY + trajectory.Y;
                    break;
                case CSLObjectType.door:
                case CSLObjectType.movingplatform:
                case CSLObjectType.rust:
                case CSLObjectType.slope:
                case CSLObjectType.wall:
                    ((LevelBlock)o)._X = beginX + trajectory.X;
                    ((LevelBlock)o)._Y = beginY + trajectory.Y;
                    break;
                default:
                    ((GameObject)o).moveThis(beginX + trajectory.X, beginY + trajectory.Y);
                    break;
            }
            if (timer >= duration)
                end();
        }
    }
}
