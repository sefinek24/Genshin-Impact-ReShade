namespace StellaModLauncher.Forms.Other
{
	sealed partial class SupportMe
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
				System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SupportMe));
				panel1 = new Panel();
				panel2 = new Panel();
				label2 = new Label();
				linkLabel6 = new LinkLabel();
				linkLabel5 = new LinkLabel();
				label1 = new Label();
				toolTip1 = new ToolTip(components);
				linkLabel1 = new LinkLabel();
				linkLabel3 = new LinkLabel();
				linkLabel4 = new LinkLabel();
				linkLabel7 = new LinkLabel();
				linkLabel8 = new LinkLabel();
				linkLabel2 = new LinkLabel();
				linkLabel9 = new LinkLabel();
				linkLabel10 = new LinkLabel();
				label3 = new Label();
				panel1.SuspendLayout();
				SuspendLayout();
				// 
				// panel1
				// 
				panel1.BackColor = Color.Transparent;
				panel1.Controls.Add(panel2);
				panel1.Cursor = Cursors.SizeAll;
				resources.ApplyResources(panel1, "panel1");
				panel1.Name = "panel1";
				panel1.MouseDown += MouseDown_Event;
				panel1.MouseMove += MouseMove_Event;
				panel1.MouseUp += MouseUp_Event;
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
				// label2
				// 
				resources.ApplyResources(label2, "label2");
				label2.BackColor = Color.Transparent;
				label2.ForeColor = Color.DodgerBlue;
				label2.Name = "label2";
				// 
				// linkLabel6
				// 
				linkLabel6.ActiveLinkColor = Color.Orange;
				resources.ApplyResources(linkLabel6, "linkLabel6");
				linkLabel6.BackColor = Color.Transparent;
				linkLabel6.LinkBehavior = LinkBehavior.HoverUnderline;
				linkLabel6.LinkColor = Color.DarkOrange;
				linkLabel6.Name = "linkLabel6";
				linkLabel6.TabStop = true;
				linkLabel6.LinkClicked += MaybeLater_LinkClicked;
				// 
				// linkLabel5
				// 
				linkLabel5.ActiveLinkColor = Color.Lime;
				resources.ApplyResources(linkLabel5, "linkLabel5");
				linkLabel5.BackColor = Color.Transparent;
				linkLabel5.LinkBehavior = LinkBehavior.HoverUnderline;
				linkLabel5.LinkColor = Color.SpringGreen;
				linkLabel5.Name = "linkLabel5";
				linkLabel5.TabStop = true;
				linkLabel5.LinkClicked += OkayDone_LinkClicked;
				// 
				// label1
				// 
				resources.ApplyResources(label1, "label1");
				label1.BackColor = Color.Transparent;
				label1.ForeColor = Color.LightGreen;
				label1.Name = "label1";
				// 
				// linkLabel1
				// 
				linkLabel1.ActiveLinkColor = Color.LightSkyBlue;
				resources.ApplyResources(linkLabel1, "linkLabel1");
				linkLabel1.BackColor = Color.Transparent;
				linkLabel1.ForeColor = Color.Transparent;
				linkLabel1.LinkBehavior = LinkBehavior.HoverUnderline;
				linkLabel1.LinkColor = Color.DeepSkyBlue;
				linkLabel1.Name = "linkLabel1";
				linkLabel1.TabStop = true;
				toolTip1.SetToolTip(linkLabel1, resources.GetString("linkLabel1.ToolTip"));
				linkLabel1.LinkClicked += SupportMe_LinkClicked;
				// 
				// linkLabel3
				// 
				linkLabel3.ActiveLinkColor = Color.LightSkyBlue;
				resources.ApplyResources(linkLabel3, "linkLabel3");
				linkLabel3.BackColor = Color.Transparent;
				linkLabel3.ForeColor = Color.Transparent;
				linkLabel3.LinkBehavior = LinkBehavior.HoverUnderline;
				linkLabel3.LinkColor = Color.DeepSkyBlue;
				linkLabel3.Name = "linkLabel3";
				linkLabel3.TabStop = true;
				toolTip1.SetToolTip(linkLabel3, resources.GetString("linkLabel3.ToolTip"));
				linkLabel3.LinkClicked += SubscribeMe_LinkClicked;
				// 
				// linkLabel4
				// 
				linkLabel4.ActiveLinkColor = Color.LightSkyBlue;
				resources.ApplyResources(linkLabel4, "linkLabel4");
				linkLabel4.BackColor = Color.Transparent;
				linkLabel4.ForeColor = Color.Transparent;
				linkLabel4.LinkBehavior = LinkBehavior.HoverUnderline;
				linkLabel4.LinkColor = Color.DeepSkyBlue;
				linkLabel4.Name = "linkLabel4";
				linkLabel4.TabStop = true;
				toolTip1.SetToolTip(linkLabel4, resources.GetString("linkLabel4.ToolTip"));
				linkLabel4.LinkClicked += StarTheRepo_LinkClicked;
				// 
				// linkLabel7
				// 
				linkLabel7.ActiveLinkColor = Color.LightSkyBlue;
				resources.ApplyResources(linkLabel7, "linkLabel7");
				linkLabel7.BackColor = Color.Transparent;
				linkLabel7.ForeColor = Color.Transparent;
				linkLabel7.LinkBehavior = LinkBehavior.HoverUnderline;
				linkLabel7.LinkColor = Color.DeepSkyBlue;
				linkLabel7.Name = "linkLabel7";
				linkLabel7.TabStop = true;
				toolTip1.SetToolTip(linkLabel7, resources.GetString("linkLabel7.ToolTip"));
				linkLabel7.LinkClicked += DiscordServer_LinkClicked;
				// 
				// linkLabel8
				// 
				linkLabel8.ActiveLinkColor = Color.LightSkyBlue;
				resources.ApplyResources(linkLabel8, "linkLabel8");
				linkLabel8.BackColor = Color.Transparent;
				linkLabel8.ForeColor = Color.Transparent;
				linkLabel8.LinkBehavior = LinkBehavior.HoverUnderline;
				linkLabel8.LinkColor = Color.DeepSkyBlue;
				linkLabel8.Name = "linkLabel8";
				linkLabel8.TabStop = true;
				toolTip1.SetToolTip(linkLabel8, resources.GetString("linkLabel8.ToolTip"));
				linkLabel8.LinkClicked += DiscordFeedback_LinkClicked;
				// 
				// linkLabel2
				// 
				linkLabel2.ActiveLinkColor = Color.LightSkyBlue;
				resources.ApplyResources(linkLabel2, "linkLabel2");
				linkLabel2.BackColor = Color.Transparent;
				linkLabel2.ForeColor = Color.Transparent;
				linkLabel2.LinkBehavior = LinkBehavior.HoverUnderline;
				linkLabel2.LinkColor = Color.DeepSkyBlue;
				linkLabel2.Name = "linkLabel2";
				linkLabel2.TabStop = true;
				toolTip1.SetToolTip(linkLabel2, resources.GetString("linkLabel2.ToolTip"));
				linkLabel2.LinkClicked += CsGoSkins_LinkClicked;
				// 
				// linkLabel9
				// 
				linkLabel9.ActiveLinkColor = Color.LightSkyBlue;
				resources.ApplyResources(linkLabel9, "linkLabel9");
				linkLabel9.BackColor = Color.Transparent;
				linkLabel9.ForeColor = Color.Transparent;
				linkLabel9.LinkBehavior = LinkBehavior.HoverUnderline;
				linkLabel9.LinkColor = Color.DeepSkyBlue;
				linkLabel9.Name = "linkLabel9";
				linkLabel9.TabStop = true;
				toolTip1.SetToolTip(linkLabel9, resources.GetString("linkLabel9.ToolTip"));
				linkLabel9.LinkClicked += PullRequest_LinkClicked;
				// 
				// linkLabel10
				// 
				linkLabel10.ActiveLinkColor = Color.LightSkyBlue;
				resources.ApplyResources(linkLabel10, "linkLabel10");
				linkLabel10.BackColor = Color.Transparent;
				linkLabel10.ForeColor = Color.Transparent;
				linkLabel10.LinkBehavior = LinkBehavior.HoverUnderline;
				linkLabel10.LinkColor = Color.DeepSkyBlue;
				linkLabel10.Name = "linkLabel10";
				linkLabel10.TabStop = true;
				toolTip1.SetToolTip(linkLabel10, resources.GetString("linkLabel10.ToolTip"));
				linkLabel10.LinkClicked += TrustPilot_LinkClicked;
				// 
				// label3
				// 
				resources.ApplyResources(label3, "label3");
				label3.BackColor = Color.Transparent;
				label3.ForeColor = Color.LightGreen;
				label3.Name = "label3";
				// 
				// SupportMe
				// 
				resources.ApplyResources(this, "$this");
				AutoScaleMode = AutoScaleMode.Dpi;
				BackColor = Color.Black;
				BackgroundImage = Properties.Resources.bg_support_me;
				Controls.Add(linkLabel10);
				Controls.Add(linkLabel9);
				Controls.Add(label3);
				Controls.Add(linkLabel2);
				Controls.Add(linkLabel8);
				Controls.Add(linkLabel7);
				Controls.Add(linkLabel4);
				Controls.Add(linkLabel3);
				Controls.Add(linkLabel1);
				Controls.Add(panel1);
				Controls.Add(label2);
				Controls.Add(linkLabel6);
				Controls.Add(linkLabel5);
				Controls.Add(label1);
				DoubleBuffered = true;
				FormBorderStyle = FormBorderStyle.None;
				Name = "SupportMe";
				Load += SupportMe_Load;
				Shown += SupportMe_Shown;
				panel1.ResumeLayout(false);
				ResumeLayout(false);
				PerformLayout();
		  }

		  #endregion

		  private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkLabel6;
        private System.Windows.Forms.LinkLabel linkLabel5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.LinkLabel linkLabel7;
        private System.Windows.Forms.LinkLabel linkLabel8;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel linkLabel9;
        private System.Windows.Forms.LinkLabel linkLabel10;
    }
}
