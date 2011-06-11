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

/* This file contains the Animation class
 */

namespace CORA
{
    /// <summary>
    /// This class represents any animation. When the draw method is called on this class, it will animate.
    /// This class will wrap up all of the necessary components of an animation, allowing an animation to seamlessly be placed into the game.
    /// </summary>
    public class Animation
    {
        #region Instance Variables
        private Texture2D sprite; //The sprite sheet
        private Rectangle drawRect; //Where to draw the animation
        private Rectangle spriteRect; //The area of the sprite to draw
        private int width; //The width of the sprite
        private int height; //The height of the sprite
        private int currentFrame; //The index of the current frame in the row
        private int frames; //The total number of frames per row
        private int currentRow; //The index of the current row
        private int rows; //The total number of rows on the sheet
        private int milliseconds; //The number of milliseconds per frame
        public Boolean isComplete; //True during the last frame of an animation, otherwise false
        private Boolean repeat; //True if an animation should reset and repeat, otherwise false
        private double animator; //Used to calculate time until the next frame
        #endregion
        #region Properties
        public Texture2D Sprite
        {
            get { return sprite; }
            set { sprite = value; }
        }
        public int Width
        {
            get { return width; }
            set { 
                    width = value;
                    spriteRect.Width = value;
                    drawRect.Width = value;
                }
        }
        public int Height
        {
            get { return height; }
            set { 
                    height = value;
                    spriteRect.Height = value;
                    drawRect.Height = value;
                }
        }
        public int Frames
        {
            get { return frames; }
            set { frames = value; }
        }
        public int Rows
        {
            get { return rows; }
            set { rows = value; }
        }
        public int Milliseconds
        {
            get { return milliseconds; }
            set { milliseconds = value; }
        }
        public Boolean Repeat
        {
            get { return repeat; }
            set { repeat = value; }
        }
        public int PosX
        {
            get { return drawRect.X; }
            set { drawRect.X = value; }
        }
        public int PosY
        {
            get { return drawRect.Y; }
            set { drawRect.Y = value; }
        }
        #endregion
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sprite">The sprite sheet</param>
        /// <param name="width">The width of the sprite</param>
        /// <param name="height">The height of the sprite</param>
        /// <param name="frames">Total number of frames per row</param>
        /// <param name="rows">Total number of rows on the sheet</param>
        /// <param name="repeat">True if the animation(s) should repeat, otherwise false.</param>
        public Animation(Texture2D sprite, int width, int height, int frames, int rows, Boolean repeat, int milliseconds)
        {
            this.sprite = sprite;
            this.width = width;
            this.height = height;
            this.frames = frames;
            this.rows = rows;
            this.repeat = repeat;
            drawRect = new Rectangle(0, 0, width, height);
            spriteRect = new Rectangle(0, 0, width, height);
            currentFrame = 0;
            currentRow = 0;
            isComplete = false;
            animator = 0;
        }
        /// <summary>
        /// This method will draw the animation at the given position.
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        /// <param name="position">The position to draw the animation at</param>
        public void drawThis(drawPacket pack, Vector2 position)
        {
            drawRect.X = (int)position.X;
            drawRect.Y = (int)position.Y;
            spriteRect.X = (width * currentFrame);
            spriteRect.Y = (height * currentRow);
            pack.sb.Draw(sprite, drawRect, spriteRect, Color.White);
        }
        /// <summary>
        /// This method will keep track of the current frame.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public void doThis(doPacket pack)
        {

            animator += pack.time.ElapsedGameTime.TotalMilliseconds;
            if (animator >= milliseconds)
            {
                if (isComplete)
                {
                    currentFrame = 0;
                    isComplete = false;
                }
                else if (++currentFrame == frames - 1)
                    isComplete = true;
                animator -= milliseconds;
            }
        }
        /// <summary>
        /// This will reset the animation on the given row.
        /// </summary>
        /// <param name="i">The index of the row</param>
        public void setRow(int i)
        {
            currentRow = i;
            currentFrame = 0;
            animator = 0;
        }
        /// <summary>
        /// Mutator method for sprite
        /// </summary>
        /// <param name="sprite">The sprite this animation draws</param>
        public void setSprite(Texture2D sprite)
        {
            this.sprite = sprite;
        }
    }
}
