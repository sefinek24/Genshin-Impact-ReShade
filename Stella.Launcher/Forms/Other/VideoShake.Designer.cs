namespace StellaModLauncher.Forms.Other
{
    partial class VideoShake
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
				System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoShake));
				videoView1 = new LibVLCSharp.WinForms.VideoView();
				((System.ComponentModel.ISupportInitialize)videoView1).BeginInit();
				SuspendLayout();
				// 
				// videoView1
				// 
				videoView1.BackColor = Color.Black;
				resources.ApplyResources(videoView1, "videoView1");
				videoView1.MediaPlayer = null;
				videoView1.Name = "videoView1";
				// 
				// VideoShake
				// 
				resources.ApplyResources(this, "$this");
				AutoScaleMode = AutoScaleMode.Dpi;
				Controls.Add(videoView1);
				Name = "VideoShake";
				FormClosing += VideoShake_FormClosing;
				Load += WebViewWindow_Load;
				Shown += WebViewWindow_Shown;
				((System.ComponentModel.ISupportInitialize)videoView1).EndInit();
				ResumeLayout(false);
		  }

		  #endregion

		  private LibVLCSharp.WinForms.VideoView videoView1;
	 }
}
