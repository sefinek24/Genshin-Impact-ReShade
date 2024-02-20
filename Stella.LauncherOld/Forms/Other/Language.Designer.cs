namespace StellaLauncher.Forms.Other
{
	sealed partial class Language
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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.ChangeAppLang = new System.Windows.Forms.Label();
            this.Version = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.comboBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(61)))), ((int)(((byte)(61)))));
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.ForeColor = System.Drawing.Color.White;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "English (default)",
            "Polski"});
            this.comboBox1.Location = new System.Drawing.Point(52, 47);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(264, 21);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.Lang_SelectedIndexChanged);
            // 
            // ChangeAppLang
            // 
            this.ChangeAppLang.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ChangeAppLang.Font = new System.Drawing.Font("Arial", 12F);
            this.ChangeAppLang.ForeColor = System.Drawing.Color.White;
            this.ChangeAppLang.Location = new System.Drawing.Point(12, 16);
            this.ChangeAppLang.Name = "ChangeAppLang";
            this.ChangeAppLang.Size = new System.Drawing.Size(345, 22);
            this.ChangeAppLang.TabIndex = 1;
            this.ChangeAppLang.Text = "Change the application language";
            this.ChangeAppLang.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Version
            // 
            this.Version.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Version.Font = new System.Drawing.Font("Arial", 9F);
            this.Version.ForeColor = System.Drawing.Color.White;
            this.Version.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Version.Location = new System.Drawing.Point(12, 84);
            this.Version.Name = "Version";
            this.Version.Size = new System.Drawing.Size(345, 17);
            this.Version.TabIndex = 2;
            this.Version.Text = "Status: Waiting...";
            this.Version.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Language
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.ClientSize = new System.Drawing.Size(369, 110);
            this.Controls.Add(this.Version);
            this.Controls.Add(this.ChangeAppLang);
            this.Controls.Add(this.comboBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Language";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Language";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label ChangeAppLang;
        private System.Windows.Forms.Label Version;
    }
}
