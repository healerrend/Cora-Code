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
    /// <summary>
    /// This is the level state. All levels will be children of this state.
    /// </summary>
    public class LevelState : State
    {
        #region Instance Variables
        public Vector2 levelSize; //The size, in pixels, of the level.
        public List<Doodad> background;
        protected Song bgMusic; //The background music, if any, of the level.
        public Player player; //The object the player is controlling.
        public List<GameObject> objects; //A container of all GameObjects for this level
        public List<LevelBlock> walls; //A container for all LevelBlocks for this level
        public List<HitBoxInteractable> interactables; //A container for all HitBoxInteractables for this level
        public List<Doodad> doodads;
        #endregion
        public LevelState()
        {
            objects = new List<GameObject>();
            walls = new List<LevelBlock>();
            interactables = new List<HitBoxInteractable>();
            doodads = new List<Doodad>();
            background = new List<Doodad>();
        }
        /// <summary>
        /// The do method of a level state should propagate the do chain throughout all objects the level contains.
        /// Additionally, any logic which is above the object level but below the master level should be conducted here.
        /// </summary>
        /// <param name="pack">see doPacket</param>
        public override void doThis(doPacket pack)
        {
            foreach (LevelBlock l in walls)
                l.doThis(pack, player);
            foreach (HitBoxInteractable i in interactables)
                i.doThis(pack);
            foreach (GameObject o in objects)
                o.doThis(pack);
        }

        //EACH OF THE DRAW METHODS ARE DEFAULTS. Sometimes you may not want to draw all layers.
        //These defaults will pass the current frame through the layers unchanged.

        /// <summary>
        /// Draw the world.
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawWorld(drawPacket pack)
        {
            pack.sb.GraphicsDevice.SetRenderTarget(pack.state.gameWorld);
            pack.sb.Begin();
            foreach (Doodad d in background)
                d.drawThis(pack);
            foreach (GameObject o in objects)
                o.drawThis(pack);
            pack.sb.End();
        }
        /// <summary>
        /// Draw the effects.
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawEffects(drawPacket pack)
        {
            pack.sb.GraphicsDevice.SetRenderTarget(pack.state.effects);
            pack.sb.Begin();
            pack.sb.Draw(pack.state.gameWorld, Vector2.Zero, Color.White);
            pack.sb.End();
        }
        /// <summary>
        /// Draw the UI
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawUI(drawPacket pack)
        {
            pack.sb.GraphicsDevice.SetRenderTarget(pack.state.UI);
            pack.sb.Begin();
            pack.sb.Draw(pack.state.effects, Vector2.Zero, Color.White);
            pack.sb.End();
        }
        /// <summary>
        /// Draw to the screen.
        /// </summary>
        /// <param name="pack">see drawPacket</param>
        public override void drawToScreen(drawPacket pack)
        {
            pack.sb.GraphicsDevice.SetRenderTarget(null);
            pack.sb.Begin();
            pack.sb.Draw(pack.state.UI, Vector2.Zero, Color.White);
            pack.sb.End();
        }
    }
}
