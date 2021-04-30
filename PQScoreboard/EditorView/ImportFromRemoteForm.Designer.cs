
namespace PQScoreboard
{
    partial class ImportFromRemoteForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportFromRemoteForm));
            this.LabelUrl = new System.Windows.Forms.Label();
            this.TextBoxUrl = new System.Windows.Forms.TextBox();
            this.ButtonOk = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.LabelName = new System.Windows.Forms.Label();
            this.LabelPassword = new System.Windows.Forms.Label();
            this.TextBoxName = new System.Windows.Forms.TextBox();
            this.TextBoxPassword = new System.Windows.Forms.TextBox();
            this.ProgressBarImport = new System.Windows.Forms.ProgressBar();
            this.CheckBoxExistingAuth = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // LabelUrl
            // 
            this.LabelUrl.AutoSize = true;
            this.LabelUrl.Location = new System.Drawing.Point(12, 9);
            this.LabelUrl.Name = "LabelUrl";
            this.LabelUrl.Size = new System.Drawing.Size(23, 13);
            this.LabelUrl.TabIndex = 0;
            this.LabelUrl.Text = "Url:";
            // 
            // TextBoxUrl
            // 
            this.TextBoxUrl.Location = new System.Drawing.Point(74, 6);
            this.TextBoxUrl.Name = "TextBoxUrl";
            this.TextBoxUrl.Size = new System.Drawing.Size(159, 20);
            this.TextBoxUrl.TabIndex = 1;
            this.TextBoxUrl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxUrl_KeyDown);
            // 
            // ButtonOk
            // 
            this.ButtonOk.Location = new System.Drawing.Point(74, 107);
            this.ButtonOk.Name = "ButtonOk";
            this.ButtonOk.Size = new System.Drawing.Size(75, 23);
            this.ButtonOk.TabIndex = 4;
            this.ButtonOk.Text = "Ok";
            this.ButtonOk.UseVisualStyleBackColor = true;
            this.ButtonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Location = new System.Drawing.Point(158, 107);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 5;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            this.ButtonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // LabelName
            // 
            this.LabelName.AutoSize = true;
            this.LabelName.Location = new System.Drawing.Point(12, 35);
            this.LabelName.Name = "LabelName";
            this.LabelName.Size = new System.Drawing.Size(38, 13);
            this.LabelName.TabIndex = 0;
            this.LabelName.Text = "Name:";
            // 
            // LabelPassword
            // 
            this.LabelPassword.AutoSize = true;
            this.LabelPassword.Location = new System.Drawing.Point(12, 61);
            this.LabelPassword.Name = "LabelPassword";
            this.LabelPassword.Size = new System.Drawing.Size(56, 13);
            this.LabelPassword.TabIndex = 0;
            this.LabelPassword.Text = "Password:";
            // 
            // TextBoxName
            // 
            this.TextBoxName.Location = new System.Drawing.Point(74, 32);
            this.TextBoxName.Name = "TextBoxName";
            this.TextBoxName.Size = new System.Drawing.Size(159, 20);
            this.TextBoxName.TabIndex = 2;
            // 
            // TextBoxPassword
            // 
            this.TextBoxPassword.Location = new System.Drawing.Point(74, 58);
            this.TextBoxPassword.Name = "TextBoxPassword";
            this.TextBoxPassword.Size = new System.Drawing.Size(159, 20);
            this.TextBoxPassword.TabIndex = 3;
            this.TextBoxPassword.UseSystemPasswordChar = true;
            // 
            // ProgressBarImport
            // 
            this.ProgressBarImport.Location = new System.Drawing.Point(12, 107);
            this.ProgressBarImport.Name = "ProgressBarImport";
            this.ProgressBarImport.Size = new System.Drawing.Size(221, 21);
            this.ProgressBarImport.TabIndex = 6;
            // 
            // CheckBoxExistingAuth
            // 
            this.CheckBoxExistingAuth.AutoSize = true;
            this.CheckBoxExistingAuth.Location = new System.Drawing.Point(79, 84);
            this.CheckBoxExistingAuth.Name = "CheckBoxExistingAuth";
            this.CheckBoxExistingAuth.Size = new System.Drawing.Size(154, 17);
            this.CheckBoxExistingAuth.TabIndex = 7;
            this.CheckBoxExistingAuth.Text = "Use existing Authentication";
            this.CheckBoxExistingAuth.UseVisualStyleBackColor = true;
            this.CheckBoxExistingAuth.CheckedChanged += new System.EventHandler(this.CheckBoxExistingAuth_CheckedChanged);
            // 
            // ImportFromRemoteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(245, 140);
            this.Controls.Add(this.CheckBoxExistingAuth);
            this.Controls.Add(this.ProgressBarImport);
            this.Controls.Add(this.TextBoxPassword);
            this.Controls.Add(this.TextBoxName);
            this.Controls.Add(this.LabelPassword);
            this.Controls.Add(this.LabelName);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.ButtonOk);
            this.Controls.Add(this.TextBoxUrl);
            this.Controls.Add(this.LabelUrl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ImportFromRemoteForm";
            this.ShowInTaskbar = false;
            this.Text = "Import from Remote";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LabelUrl;
        private System.Windows.Forms.TextBox TextBoxUrl;
        private System.Windows.Forms.Button ButtonOk;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Label LabelName;
        private System.Windows.Forms.Label LabelPassword;
        private System.Windows.Forms.TextBox TextBoxName;
        private System.Windows.Forms.TextBox TextBoxPassword;
        private System.Windows.Forms.ProgressBar ProgressBarImport;
        private System.Windows.Forms.CheckBox CheckBoxExistingAuth;
    }
}