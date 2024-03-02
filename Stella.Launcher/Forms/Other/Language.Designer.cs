namespace StellaModLauncher.Forms.Other
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
				comboBox1 = new ComboBox();
				ChangeAppLang = new Label();
				SuspendLayout();
				// 
				// comboBox1
				// 
				comboBox1.Anchor = AnchorStyles.None;
				comboBox1.BackColor = Color.FromArgb(61, 61, 61);
				comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
				comboBox1.ForeColor = Color.White;
				comboBox1.FormattingEnabled = true;
				comboBox1.Items.AddRange(new object[] { "English (default)", "Polski" });
				comboBox1.Location = new Point(45, 58);
				comboBox1.Name = "comboBox1";
				comboBox1.Size = new Size(282, 23);
				comboBox1.TabIndex = 0;
				comboBox1.SelectedIndexChanged += Lang_SelectedIndexChanged;
				// 
				// ChangeAppLang
				// 
				ChangeAppLang.Anchor = AnchorStyles.None;
				ChangeAppLang.Font = new Font("Arial", 12F);
				ChangeAppLang.ForeColor = Color.White;
				ChangeAppLang.Location = new Point(12, 28);
				ChangeAppLang.Name = "ChangeAppLang";
				ChangeAppLang.Size = new Size(345, 22);
				ChangeAppLang.TabIndex = 1;
				ChangeAppLang.Text = "Change the application language";
				ChangeAppLang.TextAlign = ContentAlignment.MiddleCenter;
				// 
				// Language
				// 
				AutoScaleDimensions = new SizeF(96F, 96F);
				AutoScaleMode = AutoScaleMode.Dpi;
				BackColor = Color.FromArgb(26, 26, 26);
				ClientSize = new Size(369, 110);
				Controls.Add(ChangeAppLang);
				Controls.Add(comboBox1);
				FormBorderStyle = FormBorderStyle.FixedSingle;
				MaximizeBox = false;
				Name = "Language";
				StartPosition = FormStartPosition.CenterParent;
				Text = "Language";
				ResumeLayout(false);
		  }

		  #endregion

		  private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label ChangeAppLang;
    }
}
