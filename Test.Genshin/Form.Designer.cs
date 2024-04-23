namespace GenshinTest
{
	 partial class Form
	 {
		  /// <summary>
		  ///  Required designer variable.
		  /// </summary>
		  private System.ComponentModel.IContainer components = null;

		  /// <summary>
		  ///  Clean up any resources being used.
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
		  ///  Required method for Designer support - do not modify
		  ///  the contents of this method with the code editor.
		  /// </summary>
		  private void InitializeComponent()
		  {
				pictureBox6 = new PictureBox();
				((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
				SuspendLayout();
				// 
				// pictureBox6
				// 
				pictureBox6.BackColor = Color.Transparent;
				pictureBox6.Dock = DockStyle.Fill;
				pictureBox6.Image = Properties.Resources.e062a85fbaf7f4db50d2918a99ba919aae3b0badd001d115c832860791adbc05;
				pictureBox6.ImeMode = ImeMode.NoControl;
				pictureBox6.Location = new Point(0, 0);
				pictureBox6.Name = "pictureBox6";
				pictureBox6.Size = new Size(441, 675);
				pictureBox6.SizeMode = PictureBoxSizeMode.StretchImage;
				pictureBox6.TabIndex = 127;
				pictureBox6.TabStop = false;
				// 
				// Form
				// 
				AutoScaleDimensions = new SizeF(7F, 15F);
				AutoScaleMode = AutoScaleMode.Font;
				ClientSize = new Size(441, 675);
				Controls.Add(pictureBox6);
				DoubleBuffered = true;
				Name = "Form";
				Text = "Genshin Impact test file";
				((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
				ResumeLayout(false);
		  }

		  #endregion

		  private PictureBox pictureBox6;
	 }
}
