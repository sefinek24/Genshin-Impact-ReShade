namespace StellaModLauncher.Forms.Errors
{
	 partial class Banned
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
					 _libVlc.Dispose();
					 _mp.Dispose();
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
				System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Banned));
				videoView1 = new LibVLCSharp.WinForms.VideoView();
				label1 = new Label();
				((System.ComponentModel.ISupportInitialize)videoView1).BeginInit();
				SuspendLayout();
				// 
				// videoView1
				// 
				resources.ApplyResources(videoView1, "videoView1");
				videoView1.BackColor = Color.Black;
				videoView1.MediaPlayer = null;
				videoView1.Name = "videoView1";
				// 
				// label1
				// 
				resources.ApplyResources(label1, "label1");
				label1.BackColor = Color.Transparent;
				label1.ForeColor = Color.Transparent;
				label1.Name = "label1";
				// 
				// Banned
				// 
				resources.ApplyResources(this, "$this");
				AutoScaleMode = AutoScaleMode.Font;
				BackColor = Color.FromArgb(136, 149, 132);
				Controls.Add(label1);
				Controls.Add(videoView1);
				Name = "Banned";
				ShowIcon = false;
				FormClosing += Banned_FormClosing;
				Load += Banned_Load;
				((System.ComponentModel.ISupportInitialize)videoView1).EndInit();
				ResumeLayout(false);
		  }

		  #endregion

		  private LibVLCSharp.WinForms.VideoView videoView1;
		  private Label label1;
	 }
}
