namespace CORA
{
    partial class LevelEditForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnRemoveBackground = new System.Windows.Forms.Button();
            this.btnSetBGSprite = new System.Windows.Forms.Button();
            this.backgroundProperties = new System.Windows.Forms.PropertyGrid();
            this.lstBackgrounds = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtLevelHeight = new System.Windows.Forms.TextBox();
            this.txtLevelWidth = new System.Windows.Forms.TextBox();
            this.btnBackground = new System.Windows.Forms.Button();
            this.btnUnderlay = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnRemoveBlock = new System.Windows.Forms.Button();
            this.btnSetSprite = new System.Windows.Forms.Button();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.lstBlocks = new System.Windows.Forms.ListBox();
            this.btnCreateBlock = new System.Windows.Forms.Button();
            this.cbxBlockTypes = new System.Windows.Forms.ComboBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.picTexture = new System.Windows.Forms.PictureBox();
            this.btnRemoveTexture = new System.Windows.Forms.Button();
            this.btnImportTexture = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.btnPlayLevel = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnLoadLevel = new System.Windows.Forms.Button();
            this.btnSaveLevel = new System.Windows.Forms.Button();
            this.btnNewLevel = new System.Windows.Forms.Button();
            this.lstTextures = new System.Windows.Forms.ListBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.txtZoomLevel = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnExportLevel = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTexture)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(13, 13);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(399, 323);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnRemoveBackground);
            this.tabPage1.Controls.Add(this.btnSetBGSprite);
            this.tabPage1.Controls.Add(this.backgroundProperties);
            this.tabPage1.Controls.Add(this.lstBackgrounds);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.txtLevelHeight);
            this.tabPage1.Controls.Add(this.txtLevelWidth);
            this.tabPage1.Controls.Add(this.btnBackground);
            this.tabPage1.Controls.Add(this.btnUnderlay);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(391, 297);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Environment";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnRemoveBackground
            // 
            this.btnRemoveBackground.Location = new System.Drawing.Point(126, 97);
            this.btnRemoveBackground.Name = "btnRemoveBackground";
            this.btnRemoveBackground.Size = new System.Drawing.Size(88, 23);
            this.btnRemoveBackground.TabIndex = 9;
            this.btnRemoveBackground.Text = "Remove";
            this.btnRemoveBackground.UseVisualStyleBackColor = true;
            this.btnRemoveBackground.Click += new System.EventHandler(this.btnRemoveBackground_Click);
            // 
            // btnSetBGSprite
            // 
            this.btnSetBGSprite.Location = new System.Drawing.Point(6, 126);
            this.btnSetBGSprite.Name = "btnSetBGSprite";
            this.btnSetBGSprite.Size = new System.Drawing.Size(208, 23);
            this.btnSetBGSprite.TabIndex = 8;
            this.btnSetBGSprite.Text = "Set Sprite";
            this.btnSetBGSprite.UseVisualStyleBackColor = true;
            this.btnSetBGSprite.Click += new System.EventHandler(this.btnSetBGSprite_Click);
            // 
            // backgroundProperties
            // 
            this.backgroundProperties.Location = new System.Drawing.Point(220, 6);
            this.backgroundProperties.Name = "backgroundProperties";
            this.backgroundProperties.Size = new System.Drawing.Size(165, 285);
            this.backgroundProperties.TabIndex = 7;
            // 
            // lstBackgrounds
            // 
            this.lstBackgrounds.FormattingEnabled = true;
            this.lstBackgrounds.Location = new System.Drawing.Point(9, 155);
            this.lstBackgrounds.Name = "lstBackgrounds";
            this.lstBackgrounds.Size = new System.Drawing.Size(205, 134);
            this.lstBackgrounds.TabIndex = 6;
            this.lstBackgrounds.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Level Height:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Level Width:";
            // 
            // txtLevelHeight
            // 
            this.txtLevelHeight.Location = new System.Drawing.Point(82, 34);
            this.txtLevelHeight.Name = "txtLevelHeight";
            this.txtLevelHeight.Size = new System.Drawing.Size(132, 20);
            this.txtLevelHeight.TabIndex = 3;
            this.txtLevelHeight.TextChanged += new System.EventHandler(this.txtLevelHeight_TextChanged);
            // 
            // txtLevelWidth
            // 
            this.txtLevelWidth.Location = new System.Drawing.Point(82, 6);
            this.txtLevelWidth.Name = "txtLevelWidth";
            this.txtLevelWidth.Size = new System.Drawing.Size(132, 20);
            this.txtLevelWidth.TabIndex = 2;
            this.txtLevelWidth.TextChanged += new System.EventHandler(this.txtLevelWidth_TextChanged);
            // 
            // btnBackground
            // 
            this.btnBackground.Location = new System.Drawing.Point(6, 97);
            this.btnBackground.Name = "btnBackground";
            this.btnBackground.Size = new System.Drawing.Size(114, 23);
            this.btnBackground.TabIndex = 1;
            this.btnBackground.Text = "Add Background";
            this.btnBackground.UseVisualStyleBackColor = true;
            this.btnBackground.Click += new System.EventHandler(this.btnBackground_Click);
            // 
            // btnUnderlay
            // 
            this.btnUnderlay.Location = new System.Drawing.Point(6, 68);
            this.btnUnderlay.Name = "btnUnderlay";
            this.btnUnderlay.Size = new System.Drawing.Size(208, 23);
            this.btnUnderlay.TabIndex = 0;
            this.btnUnderlay.Text = "Set Underlay";
            this.btnUnderlay.UseVisualStyleBackColor = true;
            this.btnUnderlay.Click += new System.EventHandler(this.btnUnderlay_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnRemoveBlock);
            this.tabPage2.Controls.Add(this.btnSetSprite);
            this.tabPage2.Controls.Add(this.propertyGrid1);
            this.tabPage2.Controls.Add(this.lstBlocks);
            this.tabPage2.Controls.Add(this.btnCreateBlock);
            this.tabPage2.Controls.Add(this.cbxBlockTypes);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(391, 297);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Blocks";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnRemoveBlock
            // 
            this.btnRemoveBlock.Location = new System.Drawing.Point(110, 34);
            this.btnRemoveBlock.Name = "btnRemoveBlock";
            this.btnRemoveBlock.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveBlock.TabIndex = 5;
            this.btnRemoveBlock.Text = "Remove";
            this.btnRemoveBlock.UseVisualStyleBackColor = true;
            this.btnRemoveBlock.Click += new System.EventHandler(this.btnRemoveBlock_Click);
            // 
            // btnSetSprite
            // 
            this.btnSetSprite.Location = new System.Drawing.Point(7, 267);
            this.btnSetSprite.Name = "btnSetSprite";
            this.btnSetSprite.Size = new System.Drawing.Size(178, 23);
            this.btnSetSprite.TabIndex = 4;
            this.btnSetSprite.Text = "Set Sprite";
            this.btnSetSprite.UseVisualStyleBackColor = true;
            this.btnSetSprite.Click += new System.EventHandler(this.btnSetSprite_Click);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Location = new System.Drawing.Point(191, 7);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.propertyGrid1.Size = new System.Drawing.Size(194, 283);
            this.propertyGrid1.TabIndex = 3;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // lstBlocks
            // 
            this.lstBlocks.FormattingEnabled = true;
            this.lstBlocks.Location = new System.Drawing.Point(7, 65);
            this.lstBlocks.Name = "lstBlocks";
            this.lstBlocks.Size = new System.Drawing.Size(178, 199);
            this.lstBlocks.TabIndex = 2;
            this.lstBlocks.SelectedIndexChanged += new System.EventHandler(this.lstBlocks_SelectedIndexChanged);
            // 
            // btnCreateBlock
            // 
            this.btnCreateBlock.Location = new System.Drawing.Point(7, 35);
            this.btnCreateBlock.Name = "btnCreateBlock";
            this.btnCreateBlock.Size = new System.Drawing.Size(97, 23);
            this.btnCreateBlock.TabIndex = 1;
            this.btnCreateBlock.Text = "Create";
            this.btnCreateBlock.UseVisualStyleBackColor = true;
            this.btnCreateBlock.Click += new System.EventHandler(this.btnCreateBlock_Click);
            // 
            // cbxBlockTypes
            // 
            this.cbxBlockTypes.FormattingEnabled = true;
            this.cbxBlockTypes.Location = new System.Drawing.Point(7, 7);
            this.cbxBlockTypes.Name = "cbxBlockTypes";
            this.cbxBlockTypes.Size = new System.Drawing.Size(178, 21);
            this.cbxBlockTypes.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.picTexture);
            this.tabPage3.Controls.Add(this.btnRemoveTexture);
            this.tabPage3.Controls.Add(this.btnImportTexture);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(391, 297);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Visual Assets";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // picTexture
            // 
            this.picTexture.Location = new System.Drawing.Point(138, 3);
            this.picTexture.MaximumSize = new System.Drawing.Size(250, 250);
            this.picTexture.Name = "picTexture";
            this.picTexture.Size = new System.Drawing.Size(250, 250);
            this.picTexture.TabIndex = 3;
            this.picTexture.TabStop = false;
            // 
            // btnRemoveTexture
            // 
            this.btnRemoveTexture.Location = new System.Drawing.Point(4, 32);
            this.btnRemoveTexture.Name = "btnRemoveTexture";
            this.btnRemoveTexture.Size = new System.Drawing.Size(116, 23);
            this.btnRemoveTexture.TabIndex = 2;
            this.btnRemoveTexture.Text = "Remove Texture";
            this.btnRemoveTexture.UseVisualStyleBackColor = true;
            this.btnRemoveTexture.Click += new System.EventHandler(this.btnRemoveTexture_Click);
            // 
            // btnImportTexture
            // 
            this.btnImportTexture.Location = new System.Drawing.Point(3, 3);
            this.btnImportTexture.Name = "btnImportTexture";
            this.btnImportTexture.Size = new System.Drawing.Size(117, 23);
            this.btnImportTexture.TabIndex = 1;
            this.btnImportTexture.Text = "Import Texture";
            this.btnImportTexture.UseVisualStyleBackColor = true;
            this.btnImportTexture.Click += new System.EventHandler(this.btnImportTexture_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.btnPlayLevel);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(391, 297);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Player";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // btnPlayLevel
            // 
            this.btnPlayLevel.Location = new System.Drawing.Point(301, 3);
            this.btnPlayLevel.Name = "btnPlayLevel";
            this.btnPlayLevel.Size = new System.Drawing.Size(87, 65);
            this.btnPlayLevel.TabIndex = 0;
            this.btnPlayLevel.Text = "Play Level!";
            this.btnPlayLevel.UseVisualStyleBackColor = true;
            // 
            // btnLoadLevel
            // 
            this.btnLoadLevel.Location = new System.Drawing.Point(499, 344);
            this.btnLoadLevel.Name = "btnLoadLevel";
            this.btnLoadLevel.Size = new System.Drawing.Size(75, 23);
            this.btnLoadLevel.TabIndex = 6;
            this.btnLoadLevel.Text = "Load Level";
            this.btnLoadLevel.UseVisualStyleBackColor = true;
            this.btnLoadLevel.Click += new System.EventHandler(this.btnLoadLevel_Click);
            // 
            // btnSaveLevel
            // 
            this.btnSaveLevel.Location = new System.Drawing.Point(418, 344);
            this.btnSaveLevel.Name = "btnSaveLevel";
            this.btnSaveLevel.Size = new System.Drawing.Size(75, 23);
            this.btnSaveLevel.TabIndex = 7;
            this.btnSaveLevel.Text = "Save Level";
            this.btnSaveLevel.UseVisualStyleBackColor = true;
            this.btnSaveLevel.Click += new System.EventHandler(this.btnSaveLevel_Click);
            // 
            // btnNewLevel
            // 
            this.btnNewLevel.Location = new System.Drawing.Point(13, 344);
            this.btnNewLevel.Name = "btnNewLevel";
            this.btnNewLevel.Size = new System.Drawing.Size(75, 23);
            this.btnNewLevel.TabIndex = 8;
            this.btnNewLevel.Text = "New Level";
            this.btnNewLevel.UseVisualStyleBackColor = true;
            this.btnNewLevel.Click += new System.EventHandler(this.btnNewLevel_Click);
            // 
            // lstTextures
            // 
            this.lstTextures.FormattingEnabled = true;
            this.lstTextures.Location = new System.Drawing.Point(418, 13);
            this.lstTextures.Name = "lstTextures";
            this.lstTextures.Size = new System.Drawing.Size(156, 316);
            this.lstTextures.TabIndex = 0;
            this.lstTextures.SelectedIndexChanged += new System.EventHandler(this.lstTextures_SelectedIndexChanged);
            // 
            // txtZoomLevel
            // 
            this.txtZoomLevel.Location = new System.Drawing.Point(207, 346);
            this.txtZoomLevel.Name = "txtZoomLevel";
            this.txtZoomLevel.Size = new System.Drawing.Size(38, 20);
            this.txtZoomLevel.TabIndex = 9;
            this.txtZoomLevel.Text = "1";
            this.txtZoomLevel.TextChanged += new System.EventHandler(this.txtZoomLevel_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(164, 349);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Zoom:";
            // 
            // btnExportLevel
            // 
            this.btnExportLevel.Location = new System.Drawing.Point(337, 344);
            this.btnExportLevel.Name = "btnExportLevel";
            this.btnExportLevel.Size = new System.Drawing.Size(75, 23);
            this.btnExportLevel.TabIndex = 11;
            this.btnExportLevel.Text = "Export Level";
            this.btnExportLevel.UseVisualStyleBackColor = true;
            this.btnExportLevel.Click += new System.EventHandler(this.btnExportLevel_Click);
            // 
            // LevelEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 379);
            this.Controls.Add(this.btnExportLevel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtZoomLevel);
            this.Controls.Add(this.btnNewLevel);
            this.Controls.Add(this.btnSaveLevel);
            this.Controls.Add(this.lstTextures);
            this.Controls.Add(this.btnLoadLevel);
            this.Controls.Add(this.tabControl1);
            this.Name = "LevelEditForm";
            this.Text = "LevelEditForm";
            this.Load += new System.EventHandler(this.LevelEditForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picTexture)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBackground;
        private System.Windows.Forms.Button btnUnderlay;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnLoadLevel;
        private System.Windows.Forms.Button btnSaveLevel;
        private System.Windows.Forms.Button btnNewLevel;
        private System.Windows.Forms.Button btnRemoveTexture;
        private System.Windows.Forms.Button btnImportTexture;
        private System.Windows.Forms.PictureBox picTexture;
        private System.Windows.Forms.Button btnCreateBlock;
        private System.Windows.Forms.ComboBox cbxBlockTypes;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Button btnSetSprite;
        private System.Windows.Forms.PropertyGrid backgroundProperties;
        private System.Windows.Forms.Button btnSetBGSprite;
        private System.Windows.Forms.Button btnRemoveBlock;
        private System.Windows.Forms.Button btnRemoveBackground;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        public System.Windows.Forms.ListBox lstTextures;
        public System.Windows.Forms.ListBox lstBlocks;
        public System.Windows.Forms.TextBox txtLevelHeight;
        public System.Windows.Forms.TextBox txtLevelWidth;
        private System.Windows.Forms.TextBox txtZoomLevel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button btnPlayLevel;
        private System.Windows.Forms.Button btnExportLevel;
        public System.Windows.Forms.ListBox lstBackgrounds;
    }
}