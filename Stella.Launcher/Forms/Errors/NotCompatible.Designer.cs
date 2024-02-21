namespace StellaModLauncher.Forms.Errors
{
	sealed partial class NotCompatible
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
				System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotCompatible));
				pictureBox1 = new PictureBox();
				label2 = new Label();
				label1 = new Label();
				linkLabel1 = new LinkLabel();
				pictureBox2 = new PictureBox();
				((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
				((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
				SuspendLayout();
				// 
				// pictureBox1
				// 
				resources.ApplyResources(pictureBox1, "pictureBox1");
				pictureBox1.Image = Properties.Resources.new_release;
				pictureBox1.Name = "pictureBox1";
				pictureBox1.TabStop = false;
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
				label1.ForeColor = Color.White;
				label1.Name = "label1";
				// 
				// linkLabel1
				// 
				linkLabel1.ActiveLinkColor = Color.Blue;
				resources.ApplyResources(linkLabel1, "linkLabel1");
				linkLabel1.Cursor = Cursors.Hand;
				linkLabel1.LinkBehavior = LinkBehavior.HoverUnderline;
				linkLabel1.LinkColor = Color.DodgerBlue;
				linkLabel1.Name = "linkLabel1";
				linkLabel1.TabStop = true;
				linkLabel1.Click += DownloadInstaller_Click;
				// 
				// pictureBox2
				// 
				resources.ApplyResources(pictureBox2, "pictureBox2");
				pictureBox2.Image = Properties.Resources.party_popper;
				pictureBox2.Name = "pictureBox2";
				pictureBox2.TabStop = false;
				// 
				// NotCompatible
				// 
				resources.ApplyResources(this, "$this");
				AutoScaleMode = AutoScaleMode.Dpi;
				BackColor = Color.Black;
				Controls.Add(pictureBox2);
				Controls.Add(linkLabel1);
				Controls.Add(label1);
				Controls.Add(label2);
				Controls.Add(pictureBox1);
				FormBorderStyle = FormBorderStyle.FixedSingle;
				MaximizeBox = false;
				Name = "NotCompatible";
				FormClosed += NotCompatible_Closed;
				Shown += NotCompatible_Shown;
				((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
				((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
				ResumeLayout(false);
		  }

		  #endregion

		  private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}
