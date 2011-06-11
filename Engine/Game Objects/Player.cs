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
    public class Player : GameObject
    {
        #region Instance Variables
        Point OldPosition; //Needed?
        public InteractorType type; //The interactor type of this object
        public InteractorType selectedBot; //The interactor type of the selected minibot

        public Boolean isEnableAcceleration; //True if acceleration should act on this, otherwise false.
        public Boolean isHanging; //True if this is hanging, otherwise false.
        public Boolean isClimbing; //True if the player is currently climbing a ledge, otherwise false.
        public Boolean isRight; //True if the player is facing right, otherwise false.
        public Boolean isCrest; //Needed?
        public Boolean isAirborne; //True if the player is airborne, otherwise false.

        public Rectangle drawBox; //The rectangle to draw the sprite to
        public Rectangle animBox; //The area of the sprite to draw

        public List<LevelBlock> walls; //Points to the collection of level blocks in the level
        public List<HitBoxInteractable> interactables; //Points to the collection of interactables in the level
        public List<CollisionPoint> points; //This object's list of collision points.
        public HangingLedge currentLedge = null; //The ledge the player is currently hanging on.
        public Vector2 staticVelocity; //This velocity is used to move the player while on a moving hanging platform
        public Point hangPoint; //The hang point of the current ledge

        public BoundingSphere nearby; //This boundingsphere decides what objects are considered near enough for a possible collision.
        public LevelState level; //The level to which this object belongs.

        public Boolean hasDoubleJumped = false;   //Are we double jumping?
        public Boolean isDashing = false;
        public double elapsedTime = 0;
        public double timeToEndDash = 500; //In miniseconds
        #endregion
        #region Static Object Pool
        public static Point poolPoint; //Point, used in the static object pool
        public static Vector2 poolVector; //Vector2, used in the static object pool.
        public SpriteEffects se; //A sprite Effect
        #endregion
        #region Constants
        public float HORIZONTAL_ACCELERATION; //Like gravity but over instead of up
        public float RUN_MULTIPLIER; //How much faster you sprint than run
        public float JUMP_SPEED; //The velocity you are granted upward when you jump
        public float DASH_VELOCITY_ACCELERATION = 2;
        public float DOUBLE_JUMP_ACCELERATION_Y = -20; //Arbitrary number, based on horizonal acceleration.
        public float DOUBLE_JUMP_ACCELERATION_X = 55;


        #endregion
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="s">Sprite</param>
        /// <param name="walls">List of walls</param>
        /// <param name="l">The level this belongs to</param>
        public Player(Texture2D s, List<LevelBlock> walls, LevelState l)
        {
            selectedBot = InteractorType.toolbot;
            type = InteractorType.player;
            staticVelocity = new Vector2();
            isEnableAcceleration = true;
            isHanging = false;
            isClimbing = false;
            drawBox = new Rectangle(0, 0, 100, 100);
            hitBox = new BoundingBox(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
            animBox = new Rectangle(0, 0, 100, 100);
            poolVector = new Vector2();
            poolPoint = new Point();

            level = l;
            walls = level.walls;
            interactables = level.interactables;
            position = new Vector2(100, 0);
            nearby = new BoundingSphere(new Vector3(position.X, position.Y, 0f), 50);
            sprite = s;
            isCrest = false;
            isAirborne = false;
            HORIZONTAL_ACCELERATION = 2;
            RUN_MULTIPLIER = 2f;
            GRAVITY = 1.5f;
            JUMP_SPEED = -25;
            points = new List<CollisionPoint>();
            for (int i = 0; i < 12; i++) //Initialize the 12 collision points
            {
                points.Add(new CollisionPoint());
            }
            points[0].X = 0;
            points[0].Y = 0;
            points[1].X = 33;
            points[1].Y = 0;
            points[2].X = 66;
            points[2].Y = 0;
            points[3].X = 100;
            points[3].Y = 0;
            points[4].X = 100;
            points[4].Y = 33;
            points[5].X = 100;
            points[5].Y = 66;
            points[6].X = 100;
            points[6].Y = 100;
            points[7].X = 66;
            points[7].Y = 100;
            points[8].X = 33;
            points[8].Y = 100;
            points[9].X = 0;
            points[9].Y = 100;
            points[10].X = 0;
            points[10].Y = 66;
            points[11].X = 0;
            points[11].Y = 33;

            this.walls = walls;
        }
        /// <summary>
        /// This method contains all of the logic for the player each update. It will handle a lot of the game's internal logic.
        /// i. Most of the controls will take place here
        /// ii. Most of the physics will be directly caused from here
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public override void doThis(doPacket pack)
        {
            if (!pack.state.paused) //IF: Not paused
            {
                if (isDashing)
                {
                    //Check ans see if enough time has passed:
                    elapsedTime += pack.time.ElapsedGameTime.TotalMilliseconds;  //Set the elapsed time variable so we can check when to cancel the dash.
                    if (elapsedTime > timeToEndDash)
                    {
                        isDashing = false;
                        isEnableAcceleration = true;
                    }
                }
                if (isEnableAcceleration) //IF: Acceleration is enabled
                {

                    if (!isDashing)
                    {
                        //Release minibots
                        if (pack.controller.release())
                            this.releaseBots(pack);
                        if (pack.controller.isRight() == 1) //Set or clear isRight
                            isRight = true;
                        else if (pack.controller.isRight() == -1)
                            isRight = false;
                        doPhysics(pack); //Do physics.
                        //Horizontal movement
                        acceleration.X = pack.controller.moveStickHoriz() * HORIZONTAL_ACCELERATION;
                        if (isAirborne)
                            acceleration.X *= .25f; //Horizontal air control 25% of normal
                        if (pack.controller.run())
                            acceleration.X *= 2; //This needs to be a variable instead
                        if (!isAirborne && pack.controller.moveStickHoriz() == 0)
                            acceleration.X = velocity.X * -.25f; //Friction

                        if (!hasDoubleJumped)
                        {
                            if (pack.controller.run())
                            {
                                if (velocity.X > 10) //Maximum velocity (NEED A CONSTANT FOR THIS)
                                    velocity.X = 10;
                                if (velocity.X < -10)
                                    velocity.X = -10;
                            }
                            else
                            {
                                if (velocity.X > 7) //Maximum velocity (NEED A CONSTANT FOR THIS)
                                    velocity.X = 7;
                                if (velocity.X < -7)
                                    velocity.X = -7;
                            }
                        }
                        velocity += acceleration; //Apply acceleration to velocity
                        if (velocity.Y > 75)
                            velocity.Y = 75; //Max

                        if (pack.controller.dash() && !isAirborne)
                        {
                            elapsedTime = 0;

                            velocity.X += DASH_VELOCITY_ACCELERATION;
                            isDashing = true;
                        }


                        if (pack.controller.jump() && !isAirborne) //Jumping
                        {
                            velocity.Y = JUMP_SPEED;
                        }
                        else if (pack.controller.jump() && isAirborne && !hasDoubleJumped) //Double Jumping
                        {
                            if (isRight)
                            {
                                velocity.Y = DOUBLE_JUMP_ACCELERATION_Y;
                                velocity.X += DOUBLE_JUMP_ACCELERATION_X;
                                hasDoubleJumped = true;
                            }
                            else
                            {
                                velocity.Y = DOUBLE_JUMP_ACCELERATION_Y;
                                velocity.X -= DOUBLE_JUMP_ACCELERATION_X;
                                hasDoubleJumped = true;
                            }
                        }
                        isAirborne = true;
                    }
                    //Collision Detection

                    nearby.Center.X = points[6].X; //Set coords of nearby
                    nearby.Center.Y = points[6].Y;
                    nearby.Radius = 150f; //Should put some logic here

                    //Generic collision detection
                    detectCollisions(pack);
                    //Finalize movement

                    foreach (CollisionPoint p in points) //Move each collision point to set
                    {
                        p.X += (int)velocity.X;
                        p.Y += (int)velocity.Y;
                    }
                    hitBox.Min.X = points[0].X; //Move the hitbox
                    hitBox.Max.X = points[6].X;
                    hitBox.Min.Y = points[0].Y;
                    hitBox.Max.Y = points[6].Y;
                    foreach (LevelBlock w in walls)
                    {
                        if (w.intersects(hitBox))
                        {
                            //INSERT DEATH/DAMAGE BY CRUSHING HERE
                        }
                    }
                }
                else if (isHanging) //IF: Hanging
                {
                    if (isClimbing) //IF: Climbing
                    {
                        if (points[6].Y <= hangPoint.Y) //Move up and over the ledge
                        {
                            velocity.Y = 0;
                            velocity.X = 10;
                            if (!isRight)
                                velocity.X *= -1;
                            if ((isRight && points[0].X >= hangPoint.X)
                            || !isRight && points[6].X <= hangPoint.X)
                            {
                                isHanging = false;
                                isClimbing = false;
                                isEnableAcceleration = true;
                                currentLedge = null;
                                staticVelocity.X = 0;
                                staticVelocity.Y = 0;
                                velocity.X = 0;
                            }
                        }
                    }
                    else
                    {
                        if (pack.controller.up() || pack.controller.climb()) //If the player climbs the ledge
                        {
                            isClimbing = true;
                            velocity.Y = -10;
                        }
                        else if (pack.controller.jump()) //If the player jumps from the ledge
                        {
                            isHanging = false;
                            if (!isRight)
                                velocity.X = 10;
                            else
                                velocity.X = 10;
                            velocity.Y = JUMP_SPEED;
                            isRight = !isRight;
                            velocity.X += staticVelocity.X;
                            velocity.Y += staticVelocity.Y;
                            staticVelocity.X = 0;
                            staticVelocity.Y = 0;
                            currentLedge = null;
                            isEnableAcceleration = true;
                        }
                    }
                    foreach (CollisionPoint p in points) //Move each collision point
                    {
                        p.X += (int)velocity.X;
                        p.Y += (int)velocity.Y;
                        p.X += (int)staticVelocity.X;
                        p.Y += (int)staticVelocity.Y;
                    }
                }

                //Stay on screen
                pack.state.playerPosition.X = points[6].X;
                pack.state.playerPosition.Y = points[6].Y;

                hitBox.Min.X = points[0].X;
                hitBox.Min.Y = points[0].Y;
                hitBox.Max.X = points[6].X;
                hitBox.Max.Y = points[6].Y;
                drawBox.X = (int)points[0].X;
                drawBox.Y = (int)points[0].Y;
            }
        }
        /// <summary>
        /// This method will update the list of collision points.
        /// </summary>
        protected void updateHitPoints()
        {
            int hSpacing = 33;
            int vSpacing = 33;
            points[0].X = (int)position.X;
            points[0].Y = (int)position.Y;
            points[1].X = points[0].X + hSpacing;
            points[1].Y = points[0].Y;
            points[2].X = points[0].X + (2 * hSpacing);
            points[2].Y = points[0].Y;
            points[3].X = points[0].X + (drawBox.Width);
            points[3].Y = points[0].Y;
            points[4].X = points[0].X + (drawBox.Width);
            points[4].Y = points[0].Y + vSpacing;
            points[5].X = points[0].X + drawBox.Width;
            points[5].Y = points[0].Y + (vSpacing * 2);
            points[6].X = points[0].X + drawBox.Width;
            points[6].Y = points[0].Y + drawBox.Height;
            points[7].X = points[0].X + (hSpacing * 2);
            points[7].Y = points[0].Y + drawBox.Height;
            points[8].X = points[0].X + hSpacing;
            points[8].Y = points[0].Y + drawBox.Height;
            points[9].X = points[0].X;
            points[9].Y = points[0].Y + drawBox.Height;
            points[10].X = points[0].X;
            points[10].Y = points[0].Y + (2 * hSpacing);
            points[11].X = points[0].X;
            points[11].Y = points[0].Y + hSpacing;
        }
        /// <summary>
        /// This method moves each point in the player's collision point list by a trajectory vector.
        /// </summary>
        /// <param name="v">A trajectory vector to move the player by</param>
        public void movePlayer(Vector2 v)
        {
            for (int i = 0; i < 12; i++)
            {
                points[i].X += v.X;
                points[i].Y += v.Y;
            }
        }
        /// <summary>
        /// This will move the player to a point in space.
        /// </summary>
        /// <param name="p">The point to move this to</param>
        public void movePlayer(Point p)
        {
            float x = p.X - points[6].X;
            float y = p.Y - points[6].Y;
            for (int i = 0; i < 12; i++)
            {
                points[i].X += x;
                points[i].Y += y;
            }
        }
        /// <summary>
        /// This utility method will set airborne to false. 
        /// </summary>
        public void onGround()
        {
            isAirborne = false;
            hasDoubleJumped = false;
        }
        /// <summary>
        /// This will draw the player
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawThis(drawPacket pack)
        {
            if (isRight) //If facing left, flip the sprite.
                se = SpriteEffects.None;
            else
                se = SpriteEffects.FlipHorizontally;

            //THIS NEEDS TO GO
            animBox.X = ((DateTime.Now.Millisecond / 100) % 10) * 100;
            animBox.Y = 0;
            animBox.Width = 100;
            animBox.Height = 100;

            pack.sb.Draw(sprite, drawBox, new Rectangle(((DateTime.Now.Millisecond / 100) % 10) * 100, 0, 100, 100), Color.White, 0f, Vector2.Zero, se, 0);
            foreach (CollisionPoint p in points)
            {
                pack.sb.Draw(TextureLoader.redsquare, new Rectangle((int)p.X, (int)p.Y, 1, 1), Color.White);
            }
        }
        /// <summary>
        /// This method sets the variables and flips the switches to engage in a hang.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        /// <param name="h">The ledge this is hanging on</param>
        /// <param name="r">True if the ledge is on the left side of a wall, otherwise false.</param>
        /// <param name="v">A velocity to move the player by while they hang.</param>
        public virtual void hang(doPacket pack, HangingLedge h, Boolean r, Vector2 v)
        {
            isHanging = true;
            isEnableAcceleration = false;
            isRight = r;
            acceleration.X = 0;
            acceleration.Y = 0;
            velocity.X = 0;
            velocity.Y = 0;
            staticVelocity.X = v.X;
            staticVelocity.Y = v.Y;
            hangPoint.X = h.HangPoint.X;
            hangPoint.Y = h.HangPoint.Y;
            movePlayer(hangPoint);
            poolVector.X = 0;
            poolVector.Y = 98;
            if (isRight)
                poolVector.X--;
            else
                poolVector.X++;
            movePlayer(poolVector);
            currentLedge = h;
        }
        /// <summary>
        /// This utility method will call the other hang method with the zero vector.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        /// <param name="h">The ledge this is hanging on</param>
        /// <param name="r">True if the ledge is on the left side of a wall, otherwise false.</param>
        public virtual void hang(doPacket pack, HangingLedge h, Boolean r)
        {
            hang(pack, h, r, Vector2.Zero);
        }
        /// <summary>
        /// This method will handle the release of minibots.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        private void releaseBots(doPacket pack)
        {
            switch (selectedBot)
            {
                case InteractorType.toolbot:
                    if (isAirborne)
                    {

                    }
                    else
                    {

                    }
                    break;
                case InteractorType.swarmbot:
                    if (isAirborne)
                    {

                    }
                    else
                    {

                    }
                    break;
                case InteractorType.rocketbot:
                    if (isAirborne)
                    {

                    }
                    else
                    {

                    }
                    break;
                case InteractorType.elevatorbot:
                    if (isAirborne)
                    {

                    }
                    else
                    {

                    }
                    break;
                case InteractorType.cutterbot:
                    if (isAirborne)
                    {

                    }
                    else
                    {

                    }
                    break;
                case InteractorType.bucketbot:
                    if (isAirborne)
                    {

                    }
                    else
                    {

                    }
                    break;
                case InteractorType.batterybot:
                    if (isAirborne)
                    {

                    }
                    else
                    {

                    }
                    break;
            }
        }
        /// <summary>
        /// This method will handle the player's collision detection.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        protected virtual void detectCollisions(doPacket pack)
        {
            foreach (LevelBlock w in walls) //For each level block, check all 12 points
            {
                foreach (CollisionPoint p in points)
                {
                    velocity = w.detectCollision(points, p, velocity, nearby, this);
                }
            }
            foreach (HitBoxInteractable h in interactables)
            {
                h.effectPlayer(pack, this); //For each interactable, check the hit box.
            }
        }
        /// <summary>
        /// This will move the player. It will call the movePlayer method, as well as moving the drawRect.
        /// </summary>
        /// <param name="trajectory"></param>
        public override void moveThis(Vector2 trajectory)
        {
            base.moveThis(trajectory);
            movePlayer(trajectory);
            drawBox.X += (int)trajectory.X;
            drawBox.Y += (int)trajectory.Y;
        }
    }
}
