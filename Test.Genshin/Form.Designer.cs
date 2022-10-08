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
				System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form));
				label1 = new Label();
				SuspendLayout();
				// 
				// label1
				// 
				label1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				label1.BackColor = Color.Transparent;
				label1.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
				label1.ForeColor = Color.Navy;
				label1.Location = new Point(12, 4);
				label1.Name = "label1";
				label1.Size = new Size(356, 50);
				label1.TabIndex = 129;
				label1.Text = "This app is solely for testing the Genshin Stella Mod. It has no other functionality. However, enjoy the cute anime cat girl at the bottom :)";
				label1.TextAlign = ContentAlignment.TopCenter;
				// 
				// Form
				// 
				AutoScaleDimensions = new SizeF(96F, 96F);
				AutoScaleMode = AutoScaleMode.Dpi;
				BackColor = Color.Black;
				BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
				BackgroundImageLayout = ImageLayout.Stretch;
				ClientSize = new Size(380, 565);
				Controls.Add(label1);
				Cursor = Cursors.Cross;
				DoubleBuffered = true;
				ForeColor = Color.White;
				MaximizeBox = false;
				Name = "Form";
				ShowIcon = false;
				StartPosition = FormStartPosition.Manual;
				Text = "Genshin Impact test file";
				Load += Form_Load;
				ResumeLayout(false);
		  }

		  #endregion
		  private Label label1;
	 }
}
