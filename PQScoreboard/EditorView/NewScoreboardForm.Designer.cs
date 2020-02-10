namespace PQScoreboard
{
    partial class NewScoreboardForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewScoreboardForm));
            this.ButtonOk = new System.Windows.Forms.Button();
            this.LabelNumberOfCategories = new System.Windows.Forms.Label();
            this.LabelNumberOfTeams = new System.Windows.Forms.Label();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.NumericInputNumberOfTeams = new System.Windows.Forms.NumericUpDown();
            this.NumericInputNumberOfCategories = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.NumericInputNumberOfTeams)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericInputNumberOfCategories)).BeginInit();
            this.SuspendLayout();
            // 
            // ButtonOk
            // 
            this.ButtonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonOk.Location = new System.Drawing.Point(175, 64);
            this.ButtonOk.Name = "ButtonOk";
            this.ButtonOk.Size = new System.Drawing.Size(75, 23);
            this.ButtonOk.TabIndex = 2;
            this.ButtonOk.Text = "Ok";
            this.ButtonOk.UseVisualStyleBackColor = true;
            this.ButtonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // LabelNumberOfCategories
            // 
            this.LabelNumberOfCategories.AutoSize = true;
            this.LabelNumberOfCategories.Location = new System.Drawing.Point(12, 40);
            this.LabelNumberOfCategories.Name = "LabelNumberOfCategories";
            this.LabelNumberOfCategories.Size = new System.Drawing.Size(112, 13);
            this.LabelNumberOfCategories.TabIndex = 2;
            this.LabelNumberOfCategories.Text = "Number of Categories:";
            // 
            // LabelNumberOfTeams
            // 
            this.LabelNumberOfTeams.AutoSize = true;
            this.LabelNumberOfTeams.Location = new System.Drawing.Point(12, 14);
            this.LabelNumberOfTeams.Name = "LabelNumberOfTeams";
            this.LabelNumberOfTeams.Size = new System.Drawing.Size(94, 13);
            this.LabelNumberOfTeams.TabIndex = 3;
            this.LabelNumberOfTeams.Text = "Number of Teams:";
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonCancel.Location = new System.Drawing.Point(94, 64);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 3;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            this.ButtonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // NumericInputNumberOfTeams
            // 
            this.NumericInputNumberOfTeams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NumericInputNumberOfTeams.Location = new System.Drawing.Point(130, 12);
            this.NumericInputNumberOfTeams.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericInputNumberOfTeams.Name = "NumericInputNumberOfTeams";
            this.NumericInputNumberOfTeams.Size = new System.Drawing.Size(120, 20);
            this.NumericInputNumberOfTeams.TabIndex = 0;
            this.NumericInputNumberOfTeams.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // NumericInputNumberOfCategories
            // 
            this.NumericInputNumberOfCategories.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NumericInputNumberOfCategories.Location = new System.Drawing.Point(130, 38);
            this.NumericInputNumberOfCategories.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericInputNumberOfCategories.Name = "NumericInputNumberOfCategories";
            this.NumericInputNumberOfCategories.Size = new System.Drawing.Size(120, 20);
            this.NumericInputNumberOfCategories.TabIndex = 1;
            this.NumericInputNumberOfCategories.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // NewScoreboardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(258, 94);
            this.Controls.Add(this.NumericInputNumberOfCategories);
            this.Controls.Add(this.NumericInputNumberOfTeams);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.LabelNumberOfTeams);
            this.Controls.Add(this.LabelNumberOfCategories);
            this.Controls.Add(this.ButtonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "NewScoreboardForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "New Scoreboard";
            ((System.ComponentModel.ISupportInitialize)(this.NumericInputNumberOfTeams)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericInputNumberOfCategories)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonOk;
        private System.Windows.Forms.Label LabelNumberOfCategories;
        private System.Windows.Forms.Label LabelNumberOfTeams;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.NumericUpDown NumericInputNumberOfTeams;
        private System.Windows.Forms.NumericUpDown NumericInputNumberOfCategories;
    }
}