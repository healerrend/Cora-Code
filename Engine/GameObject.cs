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

/* This file contains all classes which are ostensibly game objects.
 */

namespace CORA
{
    /// <summary>
    /// This is the basic GameObject class. No instances of this class may be created.
    /// </summary>
    public abstract class GameObject
    {
        #region Instance Variables
        public Vector2 position; //This object's position
        public Vector2 velocity; //This object's velocity
        public Vector2 acceleration; //This object's acceleration
        public Boolean accelerationEnabled; //True if this object's acceleration should affect its velocity, otherwise false.
        public Texture2D sprite; //This object's sprite
        public BoundingBox hitBox; //This object's hit box.
        public float GRAVITY; //The acceleration due to gravity for this object, if any.
        #endregion
        /// <summary>
        /// Standard constructor.
        /// </summary>
        public GameObject()
        {
            position = new Vector2();
            velocity = new Vector2();
            acceleration = new Vector2();
            accelerationEnabled = true;
        }
        /// <summary>
        /// Must be overridden. This handles the object's internal logic.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public virtual void doThis(doPacket pack) { }
        /// <summary>
        /// Must be overridden. This method will draw the object into the game world.
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public virtual void drawThis(drawPacket pack) { }
        /// <summary>
        /// This method will apply gravity if acceleration is enabled.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public virtual void doPhysics(doPacket pack)
        {
            if (accelerationEnabled)
            {
                acceleration.Y = GRAVITY;
                acceleration.X = 0;
            }
        }
        /// <summary>
        /// This method will move this object by the specified vector. By default it will move the hitbox and position.
        /// </summary>
        /// <param name="trajectory">The vector by which to move this object.</param>
        public virtual void moveThis(Vector2 trajectory)
        {
            position.X += trajectory.X;
            position.Y += trajectory.Y;
            hitBox.Min.X += trajectory.X;
            hitBox.Max.X += trajectory.X;
            hitBox.Min.Y += trajectory.Y;
            hitBox.Max.Y += trajectory.Y;
        }
    }
    /// <summary>
    /// This class represents any object which is directly controlled by the player. Most commonly this will mean CORA.
    /// </summary>
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

                        if(pack.controller.dash() && !isAirborne) {
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
    /// <summary>
    /// This basic class represents a minibot the player can release.
    /// </summary>
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
    /// <summary>
    /// This is the toolbot. He is responsible for flipping switches, and is the "golden child" of the minibots.
    /// </summary>
    public class Toolbot : Minibot
    {
        public Toolbot(Texture2D s, List<LevelBlock> walls, LevelState l) : base(s, walls, l) { }
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
    /// <summary>
    /// This is the rocketbot. He is responsible for boosting CORA up and over.
    /// </summary>
    public class Rocketbot : Minibot
    {
        public Rocketbot(Texture2D s, List<LevelBlock> walls, LevelState l) : base(s, walls, l) { }
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
    /// <summary>
    /// This is the bucket bot. He can swim, transport, and spray water.
    /// </summary>
    public class Bucketbot : Minibot
    {
        public Bucketbot(Texture2D s, List<LevelBlock> walls, LevelState l) : base(s, walls, l) { }
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
    /// <summary>
    /// This is the batterybot. He can heal CORA, as well as acting as a fuse, or other electrical component.
    /// </summary>
    public class Batterybot : Minibot
    {
        public Batterybot(Texture2D s, List<LevelBlock> walls, LevelState l) : base(s, walls, l) { }
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
    /// <summary>
    /// This is the elevatorbot. He can attach to elevator surfaces and creates an elevator.
    /// </summary>
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
            if(isRight)
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
    /// <summary>
    /// This is the cutterbot. He can cut through weak materials, and is incredibly heavy. He has a fire inside of him. (literally!)
    /// </summary>
    public class Cutterbot : Minibot
    {
        public Cutterbot(Texture2D s, List<LevelBlock> walls, LevelState l) : base(s, walls, l) { }
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
    /// <summary>
    /// This is the swarmbot. They can eat garbage and ___?
    /// </summary>
    public class Swarmbot : Minibot
    {
        public Swarmbot(Texture2D s, List<LevelBlock> walls, LevelState l) : base(s, walls, l) { }
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
    /// <summary>
    /// This class is a point, except it uses floating point numbers for the X and Y instead of integers.
    /// It represents one of the 12 points of collision detection around a player sprite.
    /// </summary>
    public class CollisionPoint
    {
        public float X; //The x component
        public float Y; //The y component
        public Point p; //A point
        public CollisionPoint()
        {
            X = 0;
            Y = 0;
            p = new Point(0, 0);
        }
        public CollisionPoint(int x, int y)
        {
            this.X = x;
            this.X = y;
            p = new Point(x, y);
        }
        
    }
    /// <summary>
    /// This class represents any AI controlled moving hostile. It should be rarely used.
    /// </summary>
    public class Enemy : GameObject
    {

    }
}