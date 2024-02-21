namespace StellaModLauncher.Forms.Errors
{
	sealed partial class ErrorOccurred
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
				System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorOccurred));
				label2 = new Label();
				label1 = new Label();
				button3 = new Button();
				button2 = new Button();
				button1 = new Button();
				pictureBox1 = new PictureBox();
				button4 = new Button();
				((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
				SuspendLayout();
				// 
				// label2
				// 
				resources.ApplyResources(label2, "label2");
				label2.ForeColor = SystemColors.ControlLight;
				label2.Name = "label2";
				// 
				// label1
				// 
				resources.ApplyResources(label1, "label1");
				label1.ForeColor = SystemColors.ControlLight;
				label1.Name = "label1";
				// 
				// button3
				// 
				resources.ApplyResources(button3, "button3");
				button3.Cursor = Cursors.Hand;
				button3.Name = "button3";
				button3.UseVisualStyleBackColor = true;
				button3.Click += SeeLogs_Button;
				// 
				// button2
				// 
				resources.ApplyResources(button2, "button2");
				button2.Cursor = Cursors.Hand;
				button2.Name = "button2";
				button2.UseVisualStyleBackColor = true;
				button2.Click += Discord_Button;
				// 
				// button1
				// 
				resources.ApplyResources(button1, "button1");
				button1.Cursor = Cursors.Hand;
				button1.Name = "button1";
				button1.UseVisualStyleBackColor = true;
				button1.Click += Reinstall_Button;
				// 
				// pictureBox1
				// 
				resources.ApplyResources(pictureBox1, "pictureBox1");
				pictureBox1.BackColor = Color.Transparent;
				pictureBox1.Image = Properties.Resources.error;
				pictureBox1.Name = "pictureBox1";
				pictureBox1.TabStop = false;
				// 
				// button4
				// 
				resources.ApplyResources(button4, "button4");
				button4.Cursor = Cursors.Hand;
				button4.Name = "button4";
				button4.UseVisualStyleBackColor = true;
				button4.Click += SfcScan_Click;
				// 
				// ErrorOccurred
				// 
				resources.ApplyResources(this, "$this");
				AutoScaleMode = AutoScaleMode.Dpi;
				BackColor = Color.Maroon;
				Controls.Add(button4);
				Controls.Add(pictureBox1);
				Controls.Add(button3);
				Controls.Add(button2);
				Controls.Add(button1);
				Controls.Add(label2);
				Controls.Add(label1);
				FormBorderStyle = FormBorderStyle.FixedSingle;
				MaximizeBox = false;
				Name = "ErrorOccurred";
				FormClosed += ErrorOccurred_FormClosed;
				Load += ErrorOccurred_Load;
				Shown += ErrorOccurred_Shown;
				((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
				ResumeLayout(false);
		  }

		  #endregion
		  private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button4;
    }
}
