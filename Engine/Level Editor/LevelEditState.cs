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
using System.Threading;

/* This file contains the level editor state and all level editing components.
 */

namespace CORA
{
    /// <summary>
    /// This is the level edit state. It will do and draw the current edited level.
    /// It will save and load levels.
    /// </summary>
    public class LevelEditState : LevelState
    {
        #region Textures
        public Texture2D underlay;
        public Texture2D wall;
        #endregion
        #region Instance Variables
        public GraphicsDevice graphics;
        public MouseState mOld;
        public MouseState mNew;
        public List<Texture2D> importedTextures;
        public Object selectedObject;
        public LevelEditForm form;
        public Player nullPlayer;
        #endregion

        public LevelEditState()
        {

        }
        public override void loadState(GameState state, ContentManager content)
        {
            this.state = state;
            importedTextures = new List<Texture2D>();
            mOld = Mouse.GetState();
            mNew = Mouse.GetState();
            TextureLoader.redsquare = content.Load<Texture2D>("realassets\\redsquare");
            TextureLoader.cursor = content.Load<Texture2D>("junk\\levelcreatorcursor");
            wall = content.Load<Texture2D>("junk\\graysquare");
            form = new LevelEditForm(this);
            Thread formThread = new Thread(new ThreadStart(runForm));
            formThread.SetApartmentState(ApartmentState.STA);
            formThread.Start();
            nullPlayer = new Player(null, walls, this);
        }
        /// <summary>
        /// This will handle the do logic of the game world. All do's should be conditional, since null elements will be common.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public override void doThis(doPacket pack)
        {
            updateMouse();
            float xDif = 0;
            float yDif = 0;
            if (mNew.LeftButton == ButtonState.Released) //If the mouse button is released, the selected object clears.
                selectedObject = null;
            if (mOld.LeftButton == ButtonState.Released && mNew.LeftButton == ButtonState.Pressed) //Whenever the mouse button is clicked...
            {
                while(true)
                {
                    bool found = false;
                    Vector3 mouseNow = new Vector3(mNew.X, mNew.Y, 0); //Capture its position
                    mouseNow.X += pack.state.cameraPosition.X;
                    mouseNow.Y += pack.state.cameraPosition.Y;
                    foreach (LevelBlock w in walls) //Crawl each collection to find if there is an object being clicked on
                    {
                        if (w.Dimensions.Contains(mouseNow) == ContainmentType.Contains)
                        {
                            selectedObject = w;
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                    foreach (Doodad d in doodads)
                    {
                        if (d.hitBox.Contains(mouseNow) == ContainmentType.Contains)
                        {
                            selectedObject = d;
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                    foreach (GameObject o in objects)
                    {
                        if (o.hitBox.Contains(mouseNow) == ContainmentType.Contains)
                        {
                            selectedObject = o;
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                    foreach (HitBoxInteractable i in interactables)
                    {
                        if (i.HitBox.Contains(mouseNow) == ContainmentType.Contains)
                        {
                            selectedObject = i;
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                    foreach (Doodad d in background)
                    {
                        if (d.hitBox.Contains(mouseNow) == ContainmentType.Contains)
                        {
                            selectedObject = d;
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                    selectedObject = null; //If no object is being clicked on, then set selected object to null
                    break;
                }
            }
            if ((mOld.LeftButton == ButtonState.Pressed && mNew.LeftButton == ButtonState.Pressed)
                    || (mOld.RightButton == ButtonState.Pressed && mNew.RightButton == ButtonState.Pressed)) //If the mouse remains clicked, capture the mouse position difference
            {
                xDif = mNew.X - mOld.X;
                yDif = mNew.Y - mOld.Y;
                xDif /= state.cameraScale;
                yDif /= state.cameraScale;
            }
            else //Otherwise, dif = 0
            {
                xDif = 0;
                yDif = 0;
            }
            if (selectedObject == null && 
                    pack.state.drawPack.sb.GraphicsDevice.Viewport.X <= mOld.X &&
                    pack.state.drawPack.sb.GraphicsDevice.Viewport.Y <= mOld.Y &&
                    mOld.X <= pack.state.drawPack.sb.GraphicsDevice.Viewport.X + pack.state.drawPack.sb.GraphicsDevice.Viewport.Width &&
                    mOld.Y <= pack.state.drawPack.sb.GraphicsDevice.Viewport.Y + pack.state.drawPack.sb.GraphicsDevice.Viewport.Height) //If no object underneath the mouse, and the mouse is over the window, move the level.
            {
                pack.state.cameraPosition.X -= (int)xDif;
                pack.state.cameraPosition.Y -= (int)yDif;
            }
            else //Else, find the object underneath the mouse and move its position, drag and drop stype
            {
                foreach (Doodad d in background)
                {
                    if (selectedObject == d)
                    {
                        d.moveThis(new Vector2(xDif,yDif));
                        form.updateProperties();
                    }
                }
                foreach (LevelBlock w in walls)
                {
                    if (selectedObject == w)
                    {
                        w.moveThis(new Vector2(xDif, yDif));
                        form.updateProperties();
                    }
                }
                foreach (Doodad d in doodads)
                {
                    if (selectedObject == d)
                    {
                        d.moveThis(new Vector2(xDif, yDif));
                        form.updateProperties();
                    }
                }
                foreach (GameObject o in objects)
                {
                    if (selectedObject == o)
                    {
                        o.moveThis(new Vector2(xDif, yDif));
                        form.updateProperties();
                    }
                }
                foreach (HitBoxInteractable i in interactables)
                {
                    if (selectedObject == i)
                    {
                        i.moveThis(new Vector2(xDif, yDif));
                        form.updateProperties();
                    }
                }
            }
            foreach (LevelBlock w in walls)
                w.doThis(pack);
            foreach (Doodad d in doodads)
                d.doThis(pack);
            foreach (HitBoxInteractable i in interactables)
                i.doThis(pack);
            foreach (GameObject o in objects)
                o.doThis(pack);
        }
        /// <summary>
        /// This will draw the game world. Every draw should be conditional, since null elements will be common.
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawWorld(drawPacket pack)
        {
            if (graphics == null)
                graphics = pack.sb.GraphicsDevice;
            pack.sb.GraphicsDevice.SetRenderTarget(pack.state.gameWorld); //Set render target
            //Draw underlay without translation
            pack.sb.Begin();
            if (underlay != null)
                pack.sb.Draw(underlay, Vector2.Zero, Color.White);
            pack.sb.End();
            //Draw world
            pack.sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Matrix.CreateTranslation(new Vector3(-pack.state.cameraPosition.X, -pack.state.cameraPosition.Y, 0)) * Matrix.CreateScale(pack.state.cameraScale));
            foreach (Doodad d in background)
                if (d.Sprite != null)
                    d.drawThis(pack);
            foreach (LevelBlock w in walls) //Draw the world stuff
                if(w.Sprite != null)
                    w.drawThis(pack);
            foreach (Doodad d in doodads)
                if (d.Sprite != null)
                    d.drawThis(pack);
            foreach (GameObject o in objects)
                if(o.sprite != null)
                    o.drawThis(pack);
            foreach (HitBoxInteractable h in interactables)
                if(h.Sprite != null)
                    h.drawThis(pack);
            pack.sb.Draw(TextureLoader.redsquare, new Rectangle(-1, -1, (int)levelSize.X + 2, 2), Color.White);//Top
            pack.sb.Draw(TextureLoader.redsquare, new Rectangle(-1, -1, 2, (int)levelSize.Y + 2), Color.White);//Left
            pack.sb.Draw(TextureLoader.redsquare, new Rectangle(-1, (int)levelSize.Y, (int)levelSize.X + 2, 2), Color.White);//Bottom
            pack.sb.Draw(TextureLoader.redsquare, new Rectangle((int)levelSize.X, -1, 2, (int)levelSize.Y + 2), Color.White);//Right
            pack.sb.End();
        }
        /// <summary>
        /// Draws the cursor;
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawUI(drawPacket pack)
        {
            pack.sb.GraphicsDevice.SetRenderTarget(pack.state.UI);
            pack.sb.Begin();
            pack.sb.Draw(pack.state.effects, Vector2.Zero, Color.White);
            pack.sb.Draw(TextureLoader.cursor, new Vector2(mNew.X, mNew.Y), Color.White);
            pack.sb.End();
        }
        /// <summary>
        /// This will be called by the form thread and will run the form in parallel to the level.
        /// </summary>
        private void runForm()
        {
            System.Windows.Forms.Application.Run(form);
        }
        /// <summary>
        /// This will update the mouse state
        /// </summary>
        private void updateMouse()
        {
            mOld = mNew;
            mNew = Mouse.GetState();
        }
        /// <summary>
        /// This will crawl all containers in this class and remove the passed texture
        /// </summary>
        /// <param name="texture"></param>
        public void removeTexture(Texture2D texture)
        {
            foreach (Doodad d in background)
                if (d.Sprite == texture)
                    d.Sprite = null;
            if (underlay == texture)
                underlay = null;
            foreach (LevelBlock w in walls)
                if (w.Sprite == texture)
                    w.Sprite = null;
            foreach (GameObject o in objects)
                if (o.sprite == texture)
                    o.sprite = null;
            foreach (HitBoxInteractable h in interactables)
                if (h.Sprite == texture)
                    h.Sprite = null;
        }
    }
}
