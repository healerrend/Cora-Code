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
    public class Elevatorbot : Minibot
    {
        #region Instance Variables
        public MovingPlatform platform; //The moving platform this will create while elevating.
        public MovingHangingLedge ledge; //The moving hanging ledge this will create while elevating
        public Boolean animatorSet; //True if the animator needs to be set, otherwise false.
        public Boolean isElevating; //True if this has found an elevator surface, otherwise false.
        public ElevatorSurface attachedSurface; //The surface this is attached to
        public float lengthX; //The x component of the elevator vector
        public float lengthY; //The y component of the elevator vector
        public double time; //Calculated value, how long it takes to make one trip along the elevator surface.
        public double animator; //Used for animating the sprite.
        #endregion
        #region Constants
        public float SPEED_PIXELS_PER_UPDATE = 5; //The horizontal speed while not elevating
        public float ELEVATING_SPEED_PIXELS_PER_SECOND = 100; //The speed while elevating
        #endregion
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="s">The sprite</param>
        /// <param name="walls">The list of level blocks in the level</param>
        /// <param name="l">The level this belongs to</param>
        public Elevatorbot(Texture2D s, List<LevelBlock> walls, LevelState l)
            : base(s, walls, l)
        {
            animatorSet = false;
            isElevating = false;
            attachedSurface = null;
            type = InteractorType.elevatorbot;
            platform = new MovingPlatform(new BoundingBox(), l, new Point(-1, -1), new Point(-1, -1), MovingPlatformRotationType.Bouncing);
            ledge = new MovingHangingLedge(new BoundingBox(), l, TextureLoader.grayblock, new Point(-1, -1), true);
        }
        /// <summary>
        /// Called when this is launched and CORA is on the ground. This should walk forward until it runs out of batteries or finds a surface.
        /// </summary>
        /// <param name="p">The player this belongs to</param>
        public override void LaunchGround(Player p)
        {
            isRight = p.isRight;
            position.X = p.position.X;
            position.Y = p.position.Y;
            updateHitPoints();
            velocity.X += p.velocity.X;
            velocity.Y = 0;
            acceleration.X = 0;
            acceleration.Y = 0;
        }
        /// <summary>
        /// Called when this is launched while CORA is airborne. This should check to see if there is an elevator surface close enough, and if so, launch onto it.
        /// </summary>
        /// <param name="p">The player this belongs to</param>
        public override void LaunchAir(Player p)
        {
            //STUFF
        }
        /// <summary>
        /// This will dock the bot with the player after it is finished for whatever reason.
        /// </summary>
        /// <param name="p">The player this belongs to</param>
        public override void Dock(Player p)
        {
            //base.Dock(p); Method needs to be defined in the abstract?
            isElevating = false;
            attachedSurface = null;

        }
        /// <summary>
        /// This handles the logic of the elevatorbot each update.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public override void doThis(doPacket pack)
        {
            if (isActive) //If this has been released
            {
                if (!isElevating) //If this has not yet found a surface
                {
                    if (isRight) //If this is facing right
                        velocity.X = SPEED_PIXELS_PER_UPDATE;
                    else
                        velocity.X = -SPEED_PIXELS_PER_UPDATE;
                    doPhysics(pack);
                    detectCollisions(pack);
                }
                else //If this has found a surface
                {
                    if (!animatorSet)
                        animator = pack.time.TotalGameTime.TotalMilliseconds;
                    float t = (float)(animator / time); //Calculate t
                    if (isRight) //Set the position
                    {
                        position.X = attachedSurface.StartX + ((attachedSurface.EndX - attachedSurface.StartX) / t);
                        position.Y = attachedSurface.StartY + ((attachedSurface.EndY - attachedSurface.StartY) / t);
                    }
                    else
                    {
                        position.X = attachedSurface.EndX - ((attachedSurface.EndX - attachedSurface.StartX) / t);
                        position.Y = attachedSurface.EndY - ((attachedSurface.EndY - attachedSurface.StartY) / t);
                    }
                }
            }
        }
        /// <summary>
        /// This will draw this bot into the game world.
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawThis(drawPacket pack)
        {

        }
        /// <summary>
        /// This will fire when the elevatorbot collides with an elevator surface, and will initiate the elevation procedure.
        /// It is called from the surface, not from within this bot.
        /// </summary>
        /// <param name="s">The surface it is colliding with.</param>
        public void elevate(ElevatorSurface s)
        {
            attachedSurface = s;
            lengthX = s.EndX - s.StartX;
            lengthY = s.EndY - s.StartY;
            time = lengthY / ELEVATING_SPEED_PIXELS_PER_SECOND;
            platform.SecondsPerCycle = (float)time;
            platform.MinX = hitBox.Min.X;
            platform.MinY = hitBox.Min.Y;
            platform.MaxX = hitBox.Max.X;
            platform.MaxY = hitBox.Max.Y;
            platform.BeginX = (int)attachedSurface.StartX;
            platform.BeginX = (int)attachedSurface.StartY;
            platform.EndX = (int)attachedSurface.StartX;
            platform.EndY = (int)attachedSurface.EndY;
            if (isRight)
            {
                ledge.PosX = hitBox.Min.X;
                ledge.PosY = hitBox.Min.Y;
                ledge.MinX = hitBox.Min.X - 5;
                ledge.MinY = hitBox.Min.Y;
                ledge.MaxX = hitBox.Min.X + 5;
                ledge.MaxY = hitBox.Min.Y + 5;
            }
            else
            {
                ledge.PosX = hitBox.Max.X;
                ledge.PosY = hitBox.Min.Y;
                ledge.MinX = hitBox.Max.X - 5;
                ledge.MinY = hitBox.Min.Y;
                ledge.MaxX = hitBox.Max.X + 5;
                ledge.MaxY = hitBox.Min.Y + 5;
            }
            time *= 1000;
            isRight = true;
        }
    }
}
