using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

/* This file contains the form that goes along with the level editor.
 */
namespace CORA
{
    public partial class LevelEditForm : Form
    {
        public LevelEditState state;
        public LevelEditForm(LevelEditState state)
        {
            InitializeComponent();
            this.state = state;
        }
        /// <summary>
        /// This button will set the level's underlay.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUnderlay_Click(object sender, EventArgs e)
        {
            try
            {
                state.underlay = state.importedTextures[lstTextures.SelectedIndex];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// This button will set the level's background.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBackground_Click(object sender, EventArgs e)
        {
            try
            {
                Doodad d = new Doodad(null, new Vector2(0, 0));
                lstBackgrounds.Items.Add(d);
                state.background.Add(d);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// When the text of the text box changes, change the level's width along with it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtLevelWidth_TextChanged(object sender, EventArgs e)
        {
            try
            {
                state.levelSize.X = float.Parse(txtLevelWidth.Text);
            }
            catch { }
        }
        /// <summary>
        /// when the text of the text box changes, change the level's height along with it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtLevelHeight_TextChanged(object sender, EventArgs e)
        {
            try
            {
                state.levelSize.Y = float.Parse(txtLevelHeight.Text);
            }
            catch { }
        }
        /// <summary>
        /// This will import a texture, adding its name to the list box and the texture itself to the level edit state's collection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportTexture_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult d = openFileDialog1.ShowDialog();
                if (d == DialogResult.OK)
                {
                    FileStream stream = new FileStream(openFileDialog1.FileName,FileMode.Open);
                    state.importedTextures.Add(Texture2D.FromStream(state.graphics, stream));
                    lstTextures.Items.Add(openFileDialog1.FileName);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// This will load the texture preview when the list box's selected index changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstTextures_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                picTexture.Load(lstTextures.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// This will remove a texture which has been imported. It will remove the selected texture.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveTexture_Click(object sender, EventArgs e)
        {
            try
            {
                state.removeTexture(state.importedTextures[lstTextures.SelectedIndex]);
                lstTextures.Items.RemoveAt(lstTextures.SelectedIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCreateBlock_Click(object sender, EventArgs e)
        {
            switch ((DrawableType)cbxBlockTypes.SelectedItem)
            {
                case DrawableType.controlPanel:
                    ControlPanel controlPanel = new ControlPanel(new BoundingBox(new Vector3(0, 0, 0), new Vector3(20, 20, 0)), state, null, null, null);
                    lstBlocks.Items.Add(controlPanel);
                    state.interactables.Add(controlPanel);
                    break;
                case DrawableType.elevatorSurface:
                    ElevatorSurface elevatorSurface = new ElevatorSurface(new BoundingBox(new Vector3(0, 0, 0), new Vector3(20, 50, 0)), state, null, true, new Vector2(10, 50), new Vector2(10, 0));
                    lstBlocks.Items.Add(elevatorSurface);
                    state.interactables.Add(elevatorSurface);
                    break;
                case DrawableType.hangingLedge:
                    HangingLedge hangingLedge = new HangingLedge(new BoundingBox(new Vector3(0, 0, 0), new Vector3(20, 20, 0)), state, null, new Microsoft.Xna.Framework.Point(20, 0), true);
                    lstBlocks.Items.Add(hangingLedge);
                    state.interactables.Add(hangingLedge);
                    break;
                case DrawableType.movingHangingLedge:
                    MovingHangingLedge movingHangingLedge = new MovingHangingLedge(new BoundingBox(new Vector3(0, 0, 0), new Vector3(20, 20, 0)), state, null, new Microsoft.Xna.Framework.Point(20, 0), true);
                    lstBlocks.Items.Add(movingHangingLedge);
                    state.interactables.Add(movingHangingLedge);
                    break;
                case DrawableType.movingPlatform:
                    MovingPlatform movingPlatform = new MovingPlatform(new BoundingBox(new Vector3(0, 0, 0), new Vector3(50, 50, 0)), state, new Microsoft.Xna.Framework.Point(25, 25), new Microsoft.Xna.Framework.Point(75, 25), MovingPlatformRotationType.Bouncing);
                    lstBlocks.Items.Add(movingPlatform);
                    state.walls.Add(movingPlatform);
                    break;
                case DrawableType.pressurePlate:
                    PressurePlate pressurePlate = new PressurePlate(new BoundingBox(new Vector3(0, 0, 0), new Vector3(50, 25, 0)), state, null, null);
                    lstBlocks.Items.Add(pressurePlate);
                    state.interactables.Add(pressurePlate);
                    break;
                case DrawableType.rust:
                    Rust rust = new Rust(new BoundingBox(new Vector3(0, 0, 0), new Vector3(50, 50, 0)), state);
                    lstBlocks.Items.Add(rust);
                    state.walls.Add(rust);
                    break;
                case DrawableType.slope:
                    Slope slope = new Slope(state, new Microsoft.Xna.Framework.Point(0, 50), new Microsoft.Xna.Framework.Point(50, 0));
                    lstBlocks.Items.Add(slope);
                    state.walls.Add(slope);
                    break;
                case DrawableType.wall:
                    Wall wall = new Wall(new BoundingBox(new Vector3(0, 0, 0), new Vector3(50, 50, 0)), state);
                    wall.Sprite = state.wall;
                    lstBlocks.Items.Add(wall);
                    state.walls.Add(wall);
                    break;
                case DrawableType.doodad:
                    Doodad doodad = new Doodad(null, new Vector2(0, 0));
                    lstBlocks.Items.Add(doodad);
                    state.doodads.Add(doodad);
                    break;
                case DrawableType.animatedDoodad:
                    AnimatedDoodad animatedDoodad = new AnimatedDoodad(null, 50, 50, 2, 1, true, 1000, new Vector2(0, 0));
                    lstBlocks.Items.Add(animatedDoodad);
                    state.doodads.Add(animatedDoodad);
                    break;
            }
        }

        private void LevelEditForm_Load(object sender, EventArgs e)
        {
            cbxBlockTypes.Items.Add(DrawableType.animatedDoodad);
            cbxBlockTypes.Items.Add(DrawableType.controlPanel);
            cbxBlockTypes.Items.Add(DrawableType.doodad);
            cbxBlockTypes.Items.Add(DrawableType.elevatorSurface);
            cbxBlockTypes.Items.Add(DrawableType.hangingLedge);
            cbxBlockTypes.Items.Add(DrawableType.movingHangingLedge);
            cbxBlockTypes.Items.Add(DrawableType.movingPlatform);
            cbxBlockTypes.Items.Add(DrawableType.pressurePlate);
            cbxBlockTypes.Items.Add(DrawableType.rust);
            cbxBlockTypes.Items.Add(DrawableType.slope);
            cbxBlockTypes.Items.Add(DrawableType.wall);
            CheckForIllegalCrossThreadCalls = false;
        }

        private void lstBlocks_SelectedIndexChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = lstBlocks.SelectedItem;
            propertyGrid1.Update();
        }
        public void updateProperties()
        {
            propertyGrid1.Refresh();
            backgroundProperties.Refresh();
        }

        private void btnSetSprite_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Drawable p in lstBlocks.Items)
                {
                    if (p == lstBlocks.SelectedItem)
                        p.Sprite = state.importedTextures[lstTextures.SelectedIndex];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            int i = lstBlocks.Items.Count;
            int sel = lstBlocks.SelectedIndex;
            while (i > 0)
            {
                Object o = lstBlocks.Items[0];
                lstBlocks.Items.RemoveAt(0);
                lstBlocks.Items.Add(o);
                i--;
            }
            lstBlocks.SelectedIndex = sel;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            backgroundProperties.SelectedObject = lstBackgrounds.SelectedItem;
        }

        private void btnSetBGSprite_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Doodad d in lstBackgrounds.Items)
                    if (d == lstBackgrounds.SelectedItem)
                        d.Sprite = state.importedTextures[lstTextures.SelectedIndex];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRemoveBlock_Click(object sender, EventArgs e)
        {
            if(state.walls.Contains(lstBlocks.SelectedItem))
                state.walls.Remove((LevelBlock)lstBlocks.SelectedItem);
            if(state.interactables.Contains(lstBlocks.SelectedItem))
                state.interactables.Remove((HitBoxInteractable)lstBlocks.SelectedItem);
            if(state.objects.Contains(lstBlocks.SelectedItem))
                state.objects.Remove((GameObject)lstBlocks.SelectedItem);
            lstBlocks.Items.Remove(lstBlocks.SelectedItem);
        }

        private void btnRemoveBackground_Click(object sender, EventArgs e)
        {
            state.background.Remove((Doodad)lstBackgrounds.SelectedItem);
            lstBackgrounds.Items.Remove(lstBackgrounds.SelectedItem);
        }

        private void btnSaveLevel_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult d = saveFileDialog1.ShowDialog();
                if (d == System.Windows.Forms.DialogResult.OK)
                {
                    if (!saveFileDialog1.FileName.EndsWith(".lvl"))
                        saveFileDialog1.FileName += ".lvl";
                    if (File.Exists(saveFileDialog1.FileName))
                        File.Delete(saveFileDialog1.FileName);
                    LevelSaveLoad.SaveLevel(state, saveFileDialog1.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLoadLevel_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult d = openFileDialog1.ShowDialog();
                if (d == System.Windows.Forms.DialogResult.OK)
                {
                    if (!openFileDialog1.FileName.EndsWith(".lvl"))
                        openFileDialog1.FileName += ".lvl";
                    LevelSaveLoad.LoadLevel(state, openFileDialog1.FileName, state.graphics);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnNewLevel_Click(object sender, EventArgs e)
        {

        }
        public void ClearAllCollections()
        {
            lstBackgrounds.Items.Clear();
            lstBlocks.Items.Clear();
            lstTextures.Items.Clear();
        }
        public void AddToTextures(String tex)
        {
            lstTextures.Items.Add(tex);
        }
        public void AddToBackground(Doodad d)
        {
            lstBackgrounds.Items.Add(d);
        }
        public void AddToBlocks(Drawable d)
        {
            lstBlocks.Items.Add(d);
        }

        private void txtZoomLevel_TextChanged(object sender, EventArgs e)
        {
            try
            {
                float zoom = float.Parse(txtZoomLevel.Text);
                state.state.cameraScale = zoom;
            }
            catch(Exception ex)
            {
            }
        }

        private void btnExportLevel_Click(object sender, EventArgs e)
        {
            DialogResult d = saveFileDialog1.ShowDialog();
            if (d == System.Windows.Forms.DialogResult.OK)
            {
                LevelSaveLoad.ExportLevel(state, saveFileDialog1.FileName);
            }
        }
    }
}
