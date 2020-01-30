namespace PQScoreboard
{
    partial class AddCategoryForm
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
            this.ButtonOk = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.LabelCategoryName = new System.Windows.Forms.Label();
            this.TextboxCategoryName = new System.Windows.Forms.TextBox();
            this.DataGridViewScores = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewScores)).BeginInit();
            this.SuspendLayout();
            // 
            // ButtonOk
            // 
            this.ButtonOk.Location = new System.Drawing.Point(315, 97);
            this.ButtonOk.Name = "ButtonOk";
            this.ButtonOk.Size = new System.Drawing.Size(75, 23);
            this.ButtonOk.TabIndex = 2;
            this.ButtonOk.Text = "Ok";
            this.ButtonOk.UseVisualStyleBackColor = true;
            this.ButtonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Location = new System.Drawing.Point(234, 97);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 3;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            this.ButtonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // LabelCategoryName
            // 
            this.LabelCategoryName.AutoSize = true;
            this.LabelCategoryName.Location = new System.Drawing.Point(12, 9);
            this.LabelCategoryName.Name = "LabelCategoryName";
            this.LabelCategoryName.Size = new System.Drawing.Size(83, 13);
            this.LabelCategoryName.TabIndex = 2;
            this.LabelCategoryName.Text = "Category Name:";
            // 
            // TextboxCategoryName
            // 
            this.TextboxCategoryName.Location = new System.Drawing.Point(101, 6);
            this.TextboxCategoryName.Name = "TextboxCategoryName";
            this.TextboxCategoryName.Size = new System.Drawing.Size(100, 20);
            this.TextboxCategoryName.TabIndex = 0;
            this.TextboxCategoryName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextboxCategoryName_KeyDown);
            // 
            // DataGridViewScores
            // 
            this.DataGridViewScores.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridViewScores.Location = new System.Drawing.Point(12, 32);
            this.DataGridViewScores.Name = "DataGridViewScores";
            this.DataGridViewScores.Size = new System.Drawing.Size(378, 59);
            this.DataGridViewScores.TabIndex = 1;
            // 
            // AddCategoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 126);
            this.Controls.Add(this.DataGridViewScores);
            this.Controls.Add(this.TextboxCategoryName);
            this.Controls.Add(this.LabelCategoryName);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.ButtonOk);
            this.Name = "AddCategoryForm";
            this.Text = "Add Category";
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewScores)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonOk;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Label LabelCategoryName;
        private System.Windows.Forms.TextBox TextboxCategoryName;
        private System.Windows.Forms.DataGridView DataGridViewScores;
    }
}