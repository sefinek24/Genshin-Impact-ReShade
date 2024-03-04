namespace StellaModLauncher.Forms
{
    sealed partial class Gameplay
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
				components = new System.ComponentModel.Container();
				System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Gameplay));
				panel2 = new Panel();
				panel1 = new Panel();
				linkLabel1 = new LinkLabel();
				label1 = new Label();
				linkLabel2 = new LinkLabel();
				linkLabel3 = new LinkLabel();
				linkLabel4 = new LinkLabel();
				label2 = new Label();
				toolTip1 = new ToolTip(components);
				pictureBox3 = new PictureBox();
				pictureBox2 = new PictureBox();
				webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
				panel1.SuspendLayout();
				((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
				((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
				((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
				SuspendLayout();
				// 
				// panel2
				// 
				resources.ApplyResources(panel2, "panel2");
				panel2.BackColor = Color.Transparent;
				panel2.Cursor = Cursors.Hand;
				panel2.ForeColor = Color.Transparent;
				panel2.Name = "panel2";
				toolTip1.SetToolTip(panel2, resources.GetString("panel2.ToolTip"));
				panel2.Click += Exit_Click;
				// 
				// panel1
				// 
				panel1.BackColor = Color.Transparent;
				resources.ApplyResources(panel1, "panel1");
				panel1.Controls.Add(panel2);
				panel1.Cursor = Cursors.SizeAll;
				panel1.ForeColor = Color.Transparent;
				panel1.Name = "panel1";
				panel1.MouseDown += MouseDown_Event;
				panel1.MouseMove += MouseMove_Event;
				panel1.MouseUp += MouseUp_Event;
				// 
				// linkLabel1
				// 
				linkLabel1.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(linkLabel1, "linkLabel1");
				linkLabel1.BackColor = Color.Transparent;
				linkLabel1.Cursor = Cursors.Hand;
				linkLabel1.LinkBehavior = LinkBehavior.HoverUnderline;
				linkLabel1.LinkColor = Color.White;
				linkLabel1.Name = "linkLabel1";
				linkLabel1.TabStop = true;
				linkLabel1.LinkClicked += OpenInBrowser_LinkClicked;
				// 
				// label1
				// 
				resources.ApplyResources(label1, "label1");
				label1.BackColor = Color.Transparent;
				label1.ForeColor = Color.White;
				label1.Name = "label1";
				// 
				// linkLabel2
				// 
				linkLabel2.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(linkLabel2, "linkLabel2");
				linkLabel2.BackColor = Color.Transparent;
				linkLabel2.Cursor = Cursors.Hand;
				linkLabel2.LinkBehavior = LinkBehavior.HoverUnderline;
				linkLabel2.LinkColor = Color.White;
				linkLabel2.Name = "linkLabel2";
				linkLabel2.TabStop = true;
				toolTip1.SetToolTip(linkLabel2, resources.GetString("linkLabel2.ToolTip"));
				linkLabel2.LinkClicked += Documentation_LinkLabel;
				// 
				// linkLabel3
				// 
				linkLabel3.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(linkLabel3, "linkLabel3");
				linkLabel3.BackColor = Color.Transparent;
				linkLabel3.Cursor = Cursors.Hand;
				linkLabel3.LinkBehavior = LinkBehavior.HoverUnderline;
				linkLabel3.LinkColor = Color.White;
				linkLabel3.Name = "linkLabel3";
				linkLabel3.TabStop = true;
				toolTip1.SetToolTip(linkLabel3, resources.GetString("linkLabel3.ToolTip"));
				linkLabel3.LinkClicked += Gallery_LinkLabel;
				// 
				// linkLabel4
				// 
				linkLabel4.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(linkLabel4, "linkLabel4");
				linkLabel4.BackColor = Color.Transparent;
				linkLabel4.Cursor = Cursors.Hand;
				linkLabel4.LinkBehavior = LinkBehavior.HoverUnderline;
				linkLabel4.LinkColor = Color.White;
				linkLabel4.Name = "linkLabel4";
				linkLabel4.TabStop = true;
				toolTip1.SetToolTip(linkLabel4, resources.GetString("linkLabel4.ToolTip"));
				linkLabel4.LinkClicked += Videos_LinkLabel;
				// 
				// label2
				// 
				resources.ApplyResources(label2, "label2");
				label2.BackColor = Color.Transparent;
				label2.Cursor = Cursors.Help;
				label2.ForeColor = Color.White;
				label2.Name = "label2";
				// 
				// pictureBox3
				// 
				resources.ApplyResources(pictureBox3, "pictureBox3");
				pictureBox3.BackColor = Color.Transparent;
				pictureBox3.Cursor = Cursors.Help;
				pictureBox3.Image = Properties.Resources.uk_flag;
				pictureBox3.Name = "pictureBox3";
				pictureBox3.TabStop = false;
				toolTip1.SetToolTip(pictureBox3, resources.GetString("pictureBox3.ToolTip"));
				// 
				// pictureBox2
				// 
				resources.ApplyResources(pictureBox2, "pictureBox2");
				pictureBox2.BackColor = Color.Transparent;
				pictureBox2.Cursor = Cursors.Help;
				pictureBox2.Image = Properties.Resources.poland_flag;
				pictureBox2.Name = "pictureBox2";
				pictureBox2.TabStop = false;
				toolTip1.SetToolTip(pictureBox2, resources.GetString("pictureBox2.ToolTip"));
				// 
				// webView21
				// 
				webView21.AllowExternalDrop = true;
				resources.ApplyResources(webView21, "webView21");
				webView21.BackColor = Color.Black;
				webView21.CreationProperties = null;
				webView21.DefaultBackgroundColor = Color.Transparent;
				webView21.ForeColor = Color.Transparent;
				webView21.Name = "webView21";
				webView21.ZoomFactor = 1D;
				// 
				// Gameplay
				// 
				resources.ApplyResources(this, "$this");
				AutoScaleMode = AutoScaleMode.Dpi;
				BackColor = Color.Black;
				BackgroundImage = Properties.Resources.bg_gameplay;
				Controls.Add(linkLabel1);
				Controls.Add(webView21);
				Controls.Add(pictureBox3);
				Controls.Add(pictureBox2);
				Controls.Add(label2);
				Controls.Add(linkLabel4);
				Controls.Add(linkLabel3);
				Controls.Add(linkLabel2);
				Controls.Add(label1);
				Controls.Add(panel1);
				FormBorderStyle = FormBorderStyle.None;
				Name = "Gameplay";
				Load += Gameplay_Load;
				Shown += Tutorial_Shown;
				panel1.ResumeLayout(false);
				((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
				((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
				((System.ComponentModel.ISupportInitialize)webView21).EndInit();
				ResumeLayout(false);
				PerformLayout();
		  }

		  #endregion

		  private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}
