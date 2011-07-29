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
    public class Particle
    {
        #region Instance Variables
        private ParticleType type; //The type of particle
        private Vector2 position; //The position of the particle
        private Vector2 velocity; //The velocity of the particle
        private Vector2 acceleration; //The acceleration of the particle
        private BoundingSphere sphere; //The bounding sphere used for collision detection
        private Texture2D sprite;
        private float radius; //The radius of this particle
        #endregion
        #region Sprites
        private Texture2D waterSprite;
        #endregion
        /// <summary>
        /// Standard constructor. This is never actually called in game, it is only used for initialization.
        /// </summary>
        public Particle()
        {
            type = ParticleType.uninitialized;
        }
        public void loadAssets(ContentManager content)
        {

        }
        /// <summary>
        /// This method will convert the particle into a certain particle type.
        /// </summary>
        /// <param name="type">The type of particle to convert to</param>
        /// <param name="pX">The X-component of the position</param>
        /// <param name="pY">The Y-component of the position.</param>
        /// <param name="vX">The X-component of the velocity</param>
        /// <param name="vY">The Y-component of the velocity</param>
        /// <param name="aX">The X-component of the acceleration</param>
        /// <param name="aY">The Y-component of the acceleration</param>
        /// <param name="radius">The radius of the particle</param>
        public void convertParticle(ParticleType type, float pX, float pY, float vX, float vY, float aX, float aY, float radius)
        {
            this.type = type;
            Particle part = new Particle();
            switch (type)
            {
                case ParticleType.firework:
                    pX = RandomNumber(0, 50);
                    pY = RandomNumber(0, 25);
                    vX = RandomNumber(0, 100);
                    vY = RandomNumber(0, 75);
                    aX = 0;
                    aY = 10;
                    radius = RandomNumber(0, 35);
                    part.convertParticle(type, pX, pY, vX, vY, aX, aY, radius);

                    position.X = pX;
                    position.Y = pY;
                    velocity.X = vX;
                    velocity.Y = vY;
                    acceleration.X = aX;
                    acceleration.Y = aY;
                    this.radius = radius;
                    sphere.Center.X = pX;
                    sphere.Center.Y = pY;
                    sphere.Radius = radius;
                    break;
                default:
                    break;
            }
        }
        // Each of these convertParticle methods completes the conversion to a particular particle type
        #region convertParticle Specific Methods
        /// <summary>
        /// This method will complete the conversion to a garbage particle.
        /// </summary>
        private void convertToGarbage()
        {
            //Stuff

        }
        private void convertToWater(Vector2 trajectory)
        {
            this.velocity = trajectory;
            this.sprite = waterSprite;
        }
        #endregion
        /// <summary>
        /// This is the basic do method for the particle. Every active particle will call this every update.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public void doThis(doPacket pack)
        {
            switch (type)
            {
                case ParticleType.water:
                    doWater(pack);
                    break;
            }
        }
        private int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
        //Each of these doThis methods handles the logic of a particlar particle type
        #region doThis Specific Methods
        /// <summary>
        /// This method handles the logic for garbage particles.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        private void doGarbage(doPacket pack)
        {
            //Stuff
        }
        private void doWater(doPacket pack)
        {
            velocity.Y += pack.state.GRAVITY;
            //Detect collisions
            foreach (LevelBlock b in ((LevelState)pack.state.state).walls)
                if (b.intersects(sphere))
                    b.Name = "hi";
        }
        #endregion
        /// <summary>
        /// This is the basic draw method for every particle
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public void drawThis(drawPacket pack)
        {
            //Stuff
        }
        //Each of these drawThis methods handles the drawing of a particular particle type.
        #region drawThis Specific Methods
        /// <summary>
        /// This handles the drawing of garbage particles
        /// </summary>
        /// <param name="pack"></param>
        private void drawGarbage(drawPacket pack)
        {
            //Stuff
        }
        #endregion
        /// <summary>
        /// This is the basic relevancy check for particles. It will return true if the particle still needs to be displayed, or false if it can be recycled.
        /// </summary>
        /// <returns>True if the particle is still relevant. Otherwise, false.</returns>
        public Boolean checkRelevancy()
        {
            //Stuff
            return true;
        }
        //Each of these checkRelevancy methods handles the relevancy logic of a particular particle type.
        #region checkRelevancy Specific Methods
        /// <summary>
        /// This handles the relevancy logic for garbage particles.
        /// </summary>
        private void checkGarbageRelevancy()
        {
            //Stuff
        }
        #endregion
    }
}
