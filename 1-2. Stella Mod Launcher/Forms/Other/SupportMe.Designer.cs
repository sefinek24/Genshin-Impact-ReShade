namespace StellaModLauncherNET.Forms.Other
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SupportMe));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.linkLabel6 = new System.Windows.Forms.LinkLabel();
            this.linkLabel5 = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.linkLabel7 = new System.Windows.Forms.LinkLabel();
            this.linkLabel8 = new System.Windows.Forms.LinkLabel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.linkLabel9 = new System.Windows.Forms.LinkLabel();
            this.linkLabel10 = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Cursor = System.Windows.Forms.Cursors.SizeAll;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MouseDown_Event);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMove_Event);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MouseUp_Event);
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panel2.ForeColor = System.Drawing.Color.Transparent;
            this.panel2.Name = "panel2";
            this.toolTip1.SetToolTip(this.panel2, resources.GetString("panel2.ToolTip"));
            this.panel2.Click += new System.EventHandler(this.Exit_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label2.Name = "label2";
            // 
            // linkLabel6
            // 
            this.linkLabel6.ActiveLinkColor = System.Drawing.Color.Orange;
            resources.ApplyResources(this.linkLabel6, "linkLabel6");
            this.linkLabel6.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel6.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel6.LinkColor = System.Drawing.Color.DarkOrange;
            this.linkLabel6.Name = "linkLabel6";
            this.linkLabel6.TabStop = true;
            this.linkLabel6.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.MaybeLater_LinkClicked);
            // 
            // linkLabel5
            // 
            this.linkLabel5.ActiveLinkColor = System.Drawing.Color.Lime;
            resources.ApplyResources(this.linkLabel5, "linkLabel5");
            this.linkLabel5.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel5.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel5.LinkColor = System.Drawing.Color.SpringGreen;
            this.linkLabel5.Name = "linkLabel5";
            this.linkLabel5.TabStop = true;
            this.linkLabel5.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OkayDone_LinkClicked);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.LightGreen;
            this.label1.Name = "label1";
            // 
            // linkLabel1
            // 
            this.linkLabel1.ActiveLinkColor = System.Drawing.Color.LightSkyBlue;
            resources.ApplyResources(this.linkLabel1, "linkLabel1");
            this.linkLabel1.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel1.ForeColor = System.Drawing.Color.Transparent;
            this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel1.LinkColor = System.Drawing.Color.DeepSkyBlue;
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.TabStop = true;
            this.toolTip1.SetToolTip(this.linkLabel1, resources.GetString("linkLabel1.ToolTip"));
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SupportMe_LinkClicked);
            // 
            // linkLabel3
            // 
            this.linkLabel3.ActiveLinkColor = System.Drawing.Color.LightSkyBlue;
            resources.ApplyResources(this.linkLabel3, "linkLabel3");
            this.linkLabel3.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel3.ForeColor = System.Drawing.Color.Transparent;
            this.linkLabel3.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel3.LinkColor = System.Drawing.Color.DeepSkyBlue;
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.TabStop = true;
            this.toolTip1.SetToolTip(this.linkLabel3, resources.GetString("linkLabel3.ToolTip"));
            this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SubscribeMe_LinkClicked);
            // 
            // linkLabel4
            // 
            this.linkLabel4.ActiveLinkColor = System.Drawing.Color.LightSkyBlue;
            resources.ApplyResources(this.linkLabel4, "linkLabel4");
            this.linkLabel4.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel4.ForeColor = System.Drawing.Color.Transparent;
            this.linkLabel4.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel4.LinkColor = System.Drawing.Color.DeepSkyBlue;
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.TabStop = true;
            this.toolTip1.SetToolTip(this.linkLabel4, resources.GetString("linkLabel4.ToolTip"));
            this.linkLabel4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.StarTheRepo_LinkClicked);
            // 
            // linkLabel7
            // 
            this.linkLabel7.ActiveLinkColor = System.Drawing.Color.LightSkyBlue;
            resources.ApplyResources(this.linkLabel7, "linkLabel7");
            this.linkLabel7.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel7.ForeColor = System.Drawing.Color.Transparent;
            this.linkLabel7.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel7.LinkColor = System.Drawing.Color.DeepSkyBlue;
            this.linkLabel7.Name = "linkLabel7";
            this.linkLabel7.TabStop = true;
            this.toolTip1.SetToolTip(this.linkLabel7, resources.GetString("linkLabel7.ToolTip"));
            this.linkLabel7.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DiscordServer_LinkClicked);
            // 
            // linkLabel8
            // 
            this.linkLabel8.ActiveLinkColor = System.Drawing.Color.LightSkyBlue;
            resources.ApplyResources(this.linkLabel8, "linkLabel8");
            this.linkLabel8.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel8.ForeColor = System.Drawing.Color.Transparent;
            this.linkLabel8.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel8.LinkColor = System.Drawing.Color.DeepSkyBlue;
            this.linkLabel8.Name = "linkLabel8";
            this.linkLabel8.TabStop = true;
            this.toolTip1.SetToolTip(this.linkLabel8, resources.GetString("linkLabel8.ToolTip"));
            this.linkLabel8.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DiscordFeedback_LinkClicked);
            // 
            // linkLabel2
            // 
            this.linkLabel2.ActiveLinkColor = System.Drawing.Color.LightSkyBlue;
            resources.ApplyResources(this.linkLabel2, "linkLabel2");
            this.linkLabel2.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel2.ForeColor = System.Drawing.Color.Transparent;
            this.linkLabel2.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel2.LinkColor = System.Drawing.Color.DeepSkyBlue;
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.TabStop = true;
            this.toolTip1.SetToolTip(this.linkLabel2, resources.GetString("linkLabel2.ToolTip"));
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.CsGoSkins_LinkClicked);
            // 
            // linkLabel9
            // 
            this.linkLabel9.ActiveLinkColor = System.Drawing.Color.LightSkyBlue;
            resources.ApplyResources(this.linkLabel9, "linkLabel9");
            this.linkLabel9.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel9.ForeColor = System.Drawing.Color.Transparent;
            this.linkLabel9.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel9.LinkColor = System.Drawing.Color.DeepSkyBlue;
            this.linkLabel9.Name = "linkLabel9";
            this.linkLabel9.TabStop = true;
            this.toolTip1.SetToolTip(this.linkLabel9, resources.GetString("linkLabel9.ToolTip"));
            this.linkLabel9.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.PullRequest_LinkClicked);
            // 
            // linkLabel10
            // 
            this.linkLabel10.ActiveLinkColor = System.Drawing.Color.LightSkyBlue;
            resources.ApplyResources(this.linkLabel10, "linkLabel10");
            this.linkLabel10.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel10.ForeColor = System.Drawing.Color.Transparent;
            this.linkLabel10.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel10.LinkColor = System.Drawing.Color.DeepSkyBlue;
            this.linkLabel10.Name = "linkLabel10";
            this.linkLabel10.TabStop = true;
            this.toolTip1.SetToolTip(this.linkLabel10, resources.GetString("linkLabel10.ToolTip"));
            this.linkLabel10.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.TrustPilot_LinkClicked);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.LightGreen;
            this.label3.Name = "label3";
            // 
            // SupportMe
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImage = global::StellaModLauncherNET.Properties.Resources.bg_support_me;
            this.Controls.Add(this.linkLabel10);
            this.Controls.Add(this.linkLabel9);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.linkLabel8);
            this.Controls.Add(this.linkLabel7);
            this.Controls.Add(this.linkLabel4);
            this.Controls.Add(this.linkLabel3);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.linkLabel6);
            this.Controls.Add(this.linkLabel5);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SupportMe";
            this.Load += new System.EventHandler(this.SupportMe_Load);
            this.Shown += new System.EventHandler(this.SupportMe_Shown);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

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
