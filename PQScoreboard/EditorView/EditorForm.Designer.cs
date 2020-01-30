namespace PQScoreboard
{
    partial class EditorForm
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
            this.DataGridViewScores = new System.Windows.Forms.DataGridView();
            this.ButtonAnimate = new System.Windows.Forms.Button();
            this.MenuStripMain = new System.Windows.Forms.MenuStrip();
            this.MenuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuFileClose = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuEditAddTeam = new System.Windows.Forms.ToolStripMenuItem();
            this.addCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ButtonClose = new System.Windows.Forms.Button();
            this.ComboBoxScreen = new System.Windows.Forms.ComboBox();
            this.NumericInputAnimationLength = new System.Windows.Forms.NumericUpDown();
            this.LabelDisplay = new System.Windows.Forms.Label();
            this.LabelAnimationLength = new System.Windows.Forms.Label();
            this.CheckBoxFireworks = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewScores)).BeginInit();
            this.MenuStripMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericInputAnimationLength)).BeginInit();
            this.SuspendLayout();
            // 
            // DataGridViewScores
            // 
            this.DataGridViewScores.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataGridViewScores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridViewScores.Location = new System.Drawing.Point(12, 27);
            this.DataGridViewScores.Name = "DataGridViewScores";
            this.DataGridViewScores.Size = new System.Drawing.Size(920, 433);
            this.DataGridViewScores.TabIndex = 1;
            this.DataGridViewScores.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewScores_CellValueChanged);
            // 
            // ButtonAnimate
            // 
            this.ButtonAnimate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonAnimate.Location = new System.Drawing.Point(776, 466);
            this.ButtonAnimate.Name = "ButtonAnimate";
            this.ButtonAnimate.Size = new System.Drawing.Size(75, 23);
            this.ButtonAnimate.TabIndex = 2;
            this.ButtonAnimate.Text = "Animate";
            this.ButtonAnimate.UseVisualStyleBackColor = true;
            this.ButtonAnimate.Click += new System.EventHandler(this.ButtonAnimate_Click);
            // 
            // MenuStripMain
            // 
            this.MenuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuFile,
            this.MenuEdit});
            this.MenuStripMain.Location = new System.Drawing.Point(0, 0);
            this.MenuStripMain.Name = "MenuStripMain";
            this.MenuStripMain.Size = new System.Drawing.Size(944, 24);
            this.MenuStripMain.TabIndex = 3;
            this.MenuStripMain.Text = "menuStrip1";
            // 
            // MenuFile
            // 
            this.MenuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuFileNew,
            this.MenuFileOpen,
            this.MenuFileSave,
            this.MenuFileClose,
            this.MenuFileExit});
            this.MenuFile.Name = "MenuFile";
            this.MenuFile.Size = new System.Drawing.Size(37, 20);
            this.MenuFile.Text = "File";
            // 
            // MenuFileNew
            // 
            this.MenuFileNew.Name = "MenuFileNew";
            this.MenuFileNew.Size = new System.Drawing.Size(103, 22);
            this.MenuFileNew.Text = "New";
            this.MenuFileNew.Click += new System.EventHandler(this.MenuFileNew_Click);
            // 
            // MenuFileOpen
            // 
            this.MenuFileOpen.Name = "MenuFileOpen";
            this.MenuFileOpen.Size = new System.Drawing.Size(103, 22);
            this.MenuFileOpen.Text = "Open";
            this.MenuFileOpen.Click += new System.EventHandler(this.MenuFileOpen_Click);
            // 
            // MenuFileSave
            // 
            this.MenuFileSave.Name = "MenuFileSave";
            this.MenuFileSave.Size = new System.Drawing.Size(103, 22);
            this.MenuFileSave.Text = "Save";
            this.MenuFileSave.Click += new System.EventHandler(this.MenuFileSave_Click);
            // 
            // MenuFileClose
            // 
            this.MenuFileClose.Name = "MenuFileClose";
            this.MenuFileClose.Size = new System.Drawing.Size(103, 22);
            this.MenuFileClose.Text = "Close";
            this.MenuFileClose.Click += new System.EventHandler(this.MenuFileClose_Click);
            // 
            // MenuFileExit
            // 
            this.MenuFileExit.Name = "MenuFileExit";
            this.MenuFileExit.Size = new System.Drawing.Size(103, 22);
            this.MenuFileExit.Text = "Exit";
            this.MenuFileExit.Click += new System.EventHandler(this.MenuFileExit_Click);
            // 
            // MenuEdit
            // 
            this.MenuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuEditAddTeam,
            this.addCategoryToolStripMenuItem});
            this.MenuEdit.Name = "MenuEdit";
            this.MenuEdit.Size = new System.Drawing.Size(39, 20);
            this.MenuEdit.Text = "Edit";
            // 
            // MenuEditAddTeam
            // 
            this.MenuEditAddTeam.Name = "MenuEditAddTeam";
            this.MenuEditAddTeam.Size = new System.Drawing.Size(147, 22);
            this.MenuEditAddTeam.Text = "Add Team";
            this.MenuEditAddTeam.Click += new System.EventHandler(this.MenuEditAddTeam_Click);
            // 
            // addCategoryToolStripMenuItem
            // 
            this.addCategoryToolStripMenuItem.Name = "addCategoryToolStripMenuItem";
            this.addCategoryToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.addCategoryToolStripMenuItem.Text = "Add Category";
            this.addCategoryToolStripMenuItem.Click += new System.EventHandler(this.MenuEditAddCategory_Click);
            // 
            // ButtonClose
            // 
            this.ButtonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonClose.Location = new System.Drawing.Point(857, 466);
            this.ButtonClose.Name = "ButtonClose";
            this.ButtonClose.Size = new System.Drawing.Size(75, 23);
            this.ButtonClose.TabIndex = 4;
            this.ButtonClose.Text = "Close";
            this.ButtonClose.UseVisualStyleBackColor = true;
            this.ButtonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // ComboBoxScreen
            // 
            this.ComboBoxScreen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ComboBoxScreen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxScreen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboBoxScreen.FormattingEnabled = true;
            this.ComboBoxScreen.Location = new System.Drawing.Point(649, 466);
            this.ComboBoxScreen.Name = "ComboBoxScreen";
            this.ComboBoxScreen.Size = new System.Drawing.Size(121, 23);
            this.ComboBoxScreen.TabIndex = 5;
            // 
            // NumericInputAnimationLength
            // 
            this.NumericInputAnimationLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.NumericInputAnimationLength.DecimalPlaces = 1;
            this.NumericInputAnimationLength.Location = new System.Drawing.Point(436, 469);
            this.NumericInputAnimationLength.Maximum = new decimal(new int[] {
            6000,
            0,
            0,
            65536});
            this.NumericInputAnimationLength.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            65536});
            this.NumericInputAnimationLength.Name = "NumericInputAnimationLength";
            this.NumericInputAnimationLength.Size = new System.Drawing.Size(61, 20);
            this.NumericInputAnimationLength.TabIndex = 6;
            this.NumericInputAnimationLength.Value = new decimal(new int[] {
            320,
            0,
            0,
            65536});
            // 
            // LabelDisplay
            // 
            this.LabelDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelDisplay.AutoSize = true;
            this.LabelDisplay.Location = new System.Drawing.Point(599, 471);
            this.LabelDisplay.Name = "LabelDisplay";
            this.LabelDisplay.Size = new System.Drawing.Size(44, 13);
            this.LabelDisplay.TabIndex = 7;
            this.LabelDisplay.Text = "Display:";
            // 
            // LabelAnimationLength
            // 
            this.LabelAnimationLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelAnimationLength.AutoSize = true;
            this.LabelAnimationLength.Location = new System.Drawing.Point(338, 471);
            this.LabelAnimationLength.Name = "LabelAnimationLength";
            this.LabelAnimationLength.Size = new System.Drawing.Size(92, 13);
            this.LabelAnimationLength.TabIndex = 8;
            this.LabelAnimationLength.Text = "Animation Length:";
            // 
            // CheckBoxFireworks
            // 
            this.CheckBoxFireworks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CheckBoxFireworks.AutoSize = true;
            this.CheckBoxFireworks.Location = new System.Drawing.Point(512, 470);
            this.CheckBoxFireworks.Name = "CheckBoxFireworks";
            this.CheckBoxFireworks.Size = new System.Drawing.Size(71, 17);
            this.CheckBoxFireworks.TabIndex = 10;
            this.CheckBoxFireworks.Text = "Fireworks";
            this.CheckBoxFireworks.UseVisualStyleBackColor = true;
            // 
            // EditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 501);
            this.Controls.Add(this.CheckBoxFireworks);
            this.Controls.Add(this.LabelAnimationLength);
            this.Controls.Add(this.LabelDisplay);
            this.Controls.Add(this.NumericInputAnimationLength);
            this.Controls.Add(this.ComboBoxScreen);
            this.Controls.Add(this.ButtonClose);
            this.Controls.Add(this.ButtonAnimate);
            this.Controls.Add(this.DataGridViewScores);
            this.Controls.Add(this.MenuStripMain);
            this.Name = "EditorForm";
            this.Text = "Editor";
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewScores)).EndInit();
            this.MenuStripMain.ResumeLayout(false);
            this.MenuStripMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericInputAnimationLength)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView DataGridViewScores;
        private System.Windows.Forms.Button ButtonAnimate;
        private System.Windows.Forms.MenuStrip MenuStripMain;
        private System.Windows.Forms.ToolStripMenuItem MenuFile;
        private System.Windows.Forms.ToolStripMenuItem MenuFileNew;
        private System.Windows.Forms.ToolStripMenuItem MenuFileOpen;
        private System.Windows.Forms.ToolStripMenuItem MenuFileSave;
        private System.Windows.Forms.ToolStripMenuItem MenuFileExit;
        private System.Windows.Forms.ToolStripMenuItem MenuEdit;
        private System.Windows.Forms.ToolStripMenuItem MenuEditAddTeam;
        private System.Windows.Forms.ToolStripMenuItem addCategoryToolStripMenuItem;
        private System.Windows.Forms.Button ButtonClose;
        private System.Windows.Forms.ComboBox ComboBoxScreen;
        private System.Windows.Forms.ToolStripMenuItem MenuFileClose;
        private System.Windows.Forms.NumericUpDown NumericInputAnimationLength;
        private System.Windows.Forms.Label LabelDisplay;
        private System.Windows.Forms.Label LabelAnimationLength;
        private System.Windows.Forms.CheckBox CheckBoxFireworks;
    }
}

