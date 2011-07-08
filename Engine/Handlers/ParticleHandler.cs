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

/* This file contains the particle engine, particle handler, and all particles.
 */

namespace CORA
{
    /// <summary>
    /// This class is the particle handler. It will create, store, and handle the execution of all particles in the game, of every type.
    /// This class will be instantiated once in the master Game State.
    /// </summary>
    public class ParticleHandler
    {
        #region Instance Variables
        private const int MAX_PARTICLES = 3000; //CONSTANT VALUE. Maximum number of non-garbage particles allowed at any given time.
        private int inactiveCount; //The number of particles which are not active. (needed?)
        private Boolean lineRunning; //This boolean is used to iterate through each particle in the active queue.
        private Particle handledParticle; //The pointer is the current particle being handled by the particle handler.
        private Particle frontOfLine; //This is a placeholder particle used to delineate the front of the line. It is used to ensure each particle is dequeued once per update.
        #endregion
        #region Particle Containers
        private Queue<Particle> activeParticles; //Contains all active particles
        private Queue<Particle> inactiveParticles; //Contains all inactive particles
        #endregion
        /// <summary>
        /// Constructor. No parameters necessary.
        /// </summary>
        public ParticleHandler()
        {
            activeParticles = new Queue<Particle>();
            inactiveParticles = new Queue<Particle>();
            frontOfLine = new Particle();
            activeParticles.Enqueue(frontOfLine);
            //inactiveParticles.Enqueue(frontOfLine);
        }
        /// <summary>
        /// This method is called every update cycle. It handles the logic for all particles.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public void doThis(doPacket pack)
        {
            lineRunning = true; //Set the switch
            while (lineRunning) //While the switch is set...
            {
                handledParticle = activeParticles.Dequeue(); //Set handled particle
                if (handledParticle == frontOfLine) //If we're done with the queue, finish the method.
                {
                    activeParticles.Enqueue(handledParticle);
                    lineRunning = false;
                }
                else //Otherwise...
                {
                    handledParticle.doThis(pack); //Execute the particle's logic
                    if (handledParticle.checkRelevancy()) //If the particle is still relevant...
                        activeParticles.Enqueue(handledParticle); //...enqueue it back into the active queue.
                    else //Otherwise...
                    {
                        inactiveParticles.Enqueue(handledParticle); //...enqueue it into the inactive queue.
                        inactiveCount++; //Add one inactive particle
                    }
                }
            }
        }
        /// <summary>
        /// This method is responsible for drawing each particle.
        /// </summary>
        /// <param name="pack">See drawPacket</param>
        public void drawThis(drawPacket pack)
        {
            foreach (Particle p in activeParticles)
                p.drawThis(pack);
        }
        /// <summary>
        /// This method will "create" a particle by recycling an inactive particle, or by recycling the oldest active particle.
        /// </summary>
        /// <param name="type">The type of particle to create</param>
        /// <param name="pX">The X-component of the position</param>
        /// <param name="pY">The Y-component of the position</param>
        /// <param name="vX">The X-component of the velocity</param>
        /// <param name="vY">The Y-component of the velocity</param>
        /// <param name="aX">The X-component of the acceleration</param>
        /// <param name="aY">The Y-component of the acceleration</param>
        /// <param name="radius">The radius of the particle</param>
        public void createParticle(ParticleType type, float pX, float pY, float vX, float vY, float aX, float aY, float radius)
        {
            if (inactiveCount > 0) //If there is at least one inactive particle (this should probably be changed to use the .count property of the queue class)
            {
                handledParticle = inactiveParticles.Dequeue(); //Recycle the particle from inactivity
                inactiveCount--;
            }
            else //Otherwise...
                handledParticle = activeParticles.Dequeue(); //The oldest active particle should be used.
            handledParticle.convertParticle(type, pX, pY, vX, vY, aX, aY, radius); //Convert and then enqueue
            activeParticles.Enqueue(handledParticle);
        }
    }
}