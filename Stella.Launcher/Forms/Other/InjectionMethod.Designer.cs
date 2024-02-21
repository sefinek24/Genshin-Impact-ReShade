namespace StellaModLauncher.Forms.Other
{
	sealed partial class InjectionMethod
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
				System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InjectionMethod));
				comboBox1 = new ComboBox();
				ChangeAppLang = new Label();
				SuspendLayout();
				// 
				// comboBox1
				// 
				resources.ApplyResources(comboBox1, "comboBox1");
				comboBox1.BackColor = Color.FromArgb(61, 61, 61);
				comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
				comboBox1.ForeColor = Color.White;
				comboBox1.FormattingEnabled = true;
				comboBox1.Items.AddRange(new object[] { resources.GetString("comboBox1.Items"), resources.GetString("comboBox1.Items1") });
				comboBox1.Name = "comboBox1";
				comboBox1.SelectedIndexChanged += Method_SelectedIndexChanged;
				// 
				// ChangeAppLang
				// 
				resources.ApplyResources(ChangeAppLang, "ChangeAppLang");
				ChangeAppLang.ForeColor = Color.White;
				ChangeAppLang.Name = "ChangeAppLang";
				// 
				// InjectionMethod
				// 
				resources.ApplyResources(this, "$this");
				AutoScaleMode = AutoScaleMode.Dpi;
				BackColor = Color.FromArgb(26, 26, 26);
				Controls.Add(ChangeAppLang);
				Controls.Add(comboBox1);
				FormBorderStyle = FormBorderStyle.FixedSingle;
				MaximizeBox = false;
				Name = "InjectionMethod";
				Load += InjectionMethod_Load;
				ResumeLayout(false);
		  }

		  #endregion

		  private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label ChangeAppLang;
    }
}
