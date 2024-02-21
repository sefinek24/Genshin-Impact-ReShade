namespace StellaModLauncher.Forms
{
    sealed partial class Tools
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
				System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Tools));
				panel1 = new Panel();
				panel2 = new Panel();
				label1 = new Label();
				SettingsAndUtils = new Label();
				DeleteAllCacheAndLogFiles = new LinkLabel();
				pictureBox3 = new PictureBox();
				ReShadeLogs = new LinkLabel();
				pictureBox1 = new PictureBox();
				LauncherLogs = new LinkLabel();
				pictureBox2 = new PictureBox();
				LogFiles = new Label();
				ConfigFiles = new Label();
				SeeReShadeConfig = new LinkLabel();
				pictureBox4 = new PictureBox();
				SeeFPSUnlockerConfig = new LinkLabel();
				pictureBox5 = new PictureBox();
				ScanAndRepairSysFiles = new LinkLabel();
				pictureBox6 = new PictureBox();
				CacheAndLogs = new Label();
				pictureBox8 = new PictureBox();
				DeleteOnlyWebView2Cache = new LinkLabel();
				toolTip1 = new ToolTip(components);
				panel3 = new Panel();
				pictureBox9 = new PictureBox();
				InstallationLogs = new LinkLabel();
				pictureBox10 = new PictureBox();
				InnoSetupLogs = new LinkLabel();
				madeWith_Label = new Label();
				Misc = new Label();
				panel1.SuspendLayout();
				((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
				((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
				((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
				((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
				((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
				((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
				((System.ComponentModel.ISupportInitialize)pictureBox8).BeginInit();
				((System.ComponentModel.ISupportInitialize)pictureBox9).BeginInit();
				((System.ComponentModel.ISupportInitialize)pictureBox10).BeginInit();
				SuspendLayout();
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
				// panel2
				// 
				resources.ApplyResources(panel2, "panel2");
				panel2.BackColor = Color.Transparent;
				panel2.Cursor = Cursors.Hand;
				panel2.Name = "panel2";
				toolTip1.SetToolTip(panel2, resources.GetString("panel2.ToolTip"));
				panel2.Click += Exit_Click;
				// 
				// label1
				// 
				resources.ApplyResources(label1, "label1");
				label1.BackColor = Color.Transparent;
				label1.ForeColor = Color.DodgerBlue;
				label1.Name = "label1";
				// 
				// SettingsAndUtils
				// 
				resources.ApplyResources(SettingsAndUtils, "SettingsAndUtils");
				SettingsAndUtils.BackColor = Color.Transparent;
				SettingsAndUtils.ForeColor = Color.White;
				SettingsAndUtils.Name = "SettingsAndUtils";
				// 
				// DeleteAllCacheAndLogFiles
				// 
				DeleteAllCacheAndLogFiles.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(DeleteAllCacheAndLogFiles, "DeleteAllCacheAndLogFiles");
				DeleteAllCacheAndLogFiles.BackColor = Color.Transparent;
				DeleteAllCacheAndLogFiles.LinkBehavior = LinkBehavior.HoverUnderline;
				DeleteAllCacheAndLogFiles.LinkColor = Color.White;
				DeleteAllCacheAndLogFiles.Name = "DeleteAllCacheAndLogFiles";
				DeleteAllCacheAndLogFiles.TabStop = true;
				DeleteAllCacheAndLogFiles.LinkClicked += DeleteCache_LinkClicked;
				// 
				// pictureBox3
				// 
				resources.ApplyResources(pictureBox3, "pictureBox3");
				pictureBox3.BackColor = Color.Transparent;
				pictureBox3.Cursor = Cursors.Hand;
				pictureBox3.Image = Properties.Resources.icons8_recycle_bin;
				pictureBox3.Name = "pictureBox3";
				pictureBox3.TabStop = false;
				// 
				// ReShadeLogs
				// 
				ReShadeLogs.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(ReShadeLogs, "ReShadeLogs");
				ReShadeLogs.BackColor = Color.Transparent;
				ReShadeLogs.LinkBehavior = LinkBehavior.HoverUnderline;
				ReShadeLogs.LinkColor = Color.White;
				ReShadeLogs.Name = "ReShadeLogs";
				ReShadeLogs.TabStop = true;
				ReShadeLogs.LinkClicked += ReShadeLogs_LinkClicked;
				// 
				// pictureBox1
				// 
				resources.ApplyResources(pictureBox1, "pictureBox1");
				pictureBox1.BackColor = Color.Transparent;
				pictureBox1.Cursor = Cursors.Hand;
				pictureBox1.Image = Properties.Resources.flaticon_log;
				pictureBox1.Name = "pictureBox1";
				pictureBox1.TabStop = false;
				// 
				// LauncherLogs
				// 
				LauncherLogs.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(LauncherLogs, "LauncherLogs");
				LauncherLogs.BackColor = Color.Transparent;
				LauncherLogs.LinkBehavior = LinkBehavior.HoverUnderline;
				LauncherLogs.LinkColor = Color.White;
				LauncherLogs.Name = "LauncherLogs";
				LauncherLogs.TabStop = true;
				LauncherLogs.LinkClicked += LauncherLogs_LinkClicked;
				// 
				// pictureBox2
				// 
				resources.ApplyResources(pictureBox2, "pictureBox2");
				pictureBox2.BackColor = Color.Transparent;
				pictureBox2.Cursor = Cursors.Hand;
				pictureBox2.Image = Properties.Resources.flaticon_log;
				pictureBox2.Name = "pictureBox2";
				pictureBox2.TabStop = false;
				// 
				// LogFiles
				// 
				resources.ApplyResources(LogFiles, "LogFiles");
				LogFiles.BackColor = Color.Transparent;
				LogFiles.ForeColor = Color.White;
				LogFiles.Name = "LogFiles";
				// 
				// ConfigFiles
				// 
				resources.ApplyResources(ConfigFiles, "ConfigFiles");
				ConfigFiles.BackColor = Color.Transparent;
				ConfigFiles.ForeColor = Color.White;
				ConfigFiles.Name = "ConfigFiles";
				// 
				// SeeReShadeConfig
				// 
				SeeReShadeConfig.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(SeeReShadeConfig, "SeeReShadeConfig");
				SeeReShadeConfig.BackColor = Color.Transparent;
				SeeReShadeConfig.LinkBehavior = LinkBehavior.HoverUnderline;
				SeeReShadeConfig.LinkColor = Color.White;
				SeeReShadeConfig.Name = "SeeReShadeConfig";
				SeeReShadeConfig.TabStop = true;
				toolTip1.SetToolTip(SeeReShadeConfig, resources.GetString("SeeReShadeConfig.ToolTip"));
				SeeReShadeConfig.LinkClicked += ReShadeConfig_LinkClicked;
				// 
				// pictureBox4
				// 
				resources.ApplyResources(pictureBox4, "pictureBox4");
				pictureBox4.BackColor = Color.Transparent;
				pictureBox4.Cursor = Cursors.Hand;
				pictureBox4.Image = Properties.Resources.icons8_edit_property;
				pictureBox4.Name = "pictureBox4";
				pictureBox4.TabStop = false;
				// 
				// SeeFPSUnlockerConfig
				// 
				SeeFPSUnlockerConfig.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(SeeFPSUnlockerConfig, "SeeFPSUnlockerConfig");
				SeeFPSUnlockerConfig.BackColor = Color.Transparent;
				SeeFPSUnlockerConfig.LinkBehavior = LinkBehavior.HoverUnderline;
				SeeFPSUnlockerConfig.LinkColor = Color.White;
				SeeFPSUnlockerConfig.Name = "SeeFPSUnlockerConfig";
				SeeFPSUnlockerConfig.TabStop = true;
				toolTip1.SetToolTip(SeeFPSUnlockerConfig, resources.GetString("SeeFPSUnlockerConfig.ToolTip"));
				SeeFPSUnlockerConfig.LinkClicked += UnlockerConfig_LinkClicked;
				// 
				// pictureBox5
				// 
				resources.ApplyResources(pictureBox5, "pictureBox5");
				pictureBox5.BackColor = Color.Transparent;
				pictureBox5.Cursor = Cursors.Hand;
				pictureBox5.Image = Properties.Resources.icons8_edit_property;
				pictureBox5.Name = "pictureBox5";
				pictureBox5.TabStop = false;
				// 
				// ScanAndRepairSysFiles
				// 
				ScanAndRepairSysFiles.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(ScanAndRepairSysFiles, "ScanAndRepairSysFiles");
				ScanAndRepairSysFiles.BackColor = Color.Transparent;
				ScanAndRepairSysFiles.LinkBehavior = LinkBehavior.HoverUnderline;
				ScanAndRepairSysFiles.LinkColor = Color.White;
				ScanAndRepairSysFiles.Name = "ScanAndRepairSysFiles";
				ScanAndRepairSysFiles.TabStop = true;
				toolTip1.SetToolTip(ScanAndRepairSysFiles, resources.GetString("ScanAndRepairSysFiles.ToolTip"));
				ScanAndRepairSysFiles.LinkClicked += ScanSysFiles_LinkClicked;
				// 
				// pictureBox6
				// 
				resources.ApplyResources(pictureBox6, "pictureBox6");
				pictureBox6.BackColor = Color.Transparent;
				pictureBox6.Cursor = Cursors.Hand;
				pictureBox6.Image = Properties.Resources.icons8_tools;
				pictureBox6.Name = "pictureBox6";
				pictureBox6.TabStop = false;
				// 
				// CacheAndLogs
				// 
				resources.ApplyResources(CacheAndLogs, "CacheAndLogs");
				CacheAndLogs.BackColor = Color.Transparent;
				CacheAndLogs.ForeColor = Color.White;
				CacheAndLogs.Name = "CacheAndLogs";
				// 
				// pictureBox8
				// 
				resources.ApplyResources(pictureBox8, "pictureBox8");
				pictureBox8.BackColor = Color.Transparent;
				pictureBox8.Cursor = Cursors.Hand;
				pictureBox8.Image = Properties.Resources.icons8_recycle_bin;
				pictureBox8.Name = "pictureBox8";
				pictureBox8.TabStop = false;
				// 
				// DeleteOnlyWebView2Cache
				// 
				DeleteOnlyWebView2Cache.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(DeleteOnlyWebView2Cache, "DeleteOnlyWebView2Cache");
				DeleteOnlyWebView2Cache.BackColor = Color.Transparent;
				DeleteOnlyWebView2Cache.LinkBehavior = LinkBehavior.HoverUnderline;
				DeleteOnlyWebView2Cache.LinkColor = Color.White;
				DeleteOnlyWebView2Cache.Name = "DeleteOnlyWebView2Cache";
				DeleteOnlyWebView2Cache.TabStop = true;
				DeleteOnlyWebView2Cache.LinkClicked += DeleteWebViewCache_LinkClicked;
				// 
				// panel3
				// 
				resources.ApplyResources(panel3, "panel3");
				panel3.BackColor = Color.Transparent;
				panel3.Name = "panel3";
				panel3.MouseClick += Notepad_MouseClick;
				// 
				// pictureBox9
				// 
				resources.ApplyResources(pictureBox9, "pictureBox9");
				pictureBox9.BackColor = Color.Transparent;
				pictureBox9.Cursor = Cursors.Hand;
				pictureBox9.Image = Properties.Resources.flaticon_log;
				pictureBox9.Name = "pictureBox9";
				pictureBox9.TabStop = false;
				// 
				// InstallationLogs
				// 
				InstallationLogs.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(InstallationLogs, "InstallationLogs");
				InstallationLogs.BackColor = Color.Transparent;
				InstallationLogs.LinkBehavior = LinkBehavior.HoverUnderline;
				InstallationLogs.LinkColor = Color.White;
				InstallationLogs.Name = "InstallationLogs";
				InstallationLogs.TabStop = true;
				InstallationLogs.LinkClicked += GSModLogs_LinkClicked;
				// 
				// pictureBox10
				// 
				resources.ApplyResources(pictureBox10, "pictureBox10");
				pictureBox10.BackColor = Color.Transparent;
				pictureBox10.Cursor = Cursors.Hand;
				pictureBox10.Image = Properties.Resources.flaticon_open_folder;
				pictureBox10.Name = "pictureBox10";
				pictureBox10.TabStop = false;
				// 
				// InnoSetupLogs
				// 
				InnoSetupLogs.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(InnoSetupLogs, "InnoSetupLogs");
				InnoSetupLogs.BackColor = Color.Transparent;
				InnoSetupLogs.LinkBehavior = LinkBehavior.HoverUnderline;
				InnoSetupLogs.LinkColor = Color.White;
				InnoSetupLogs.Name = "InnoSetupLogs";
				InnoSetupLogs.TabStop = true;
				InnoSetupLogs.LinkClicked += LogDir_LinkClicked;
				// 
				// madeWith_Label
				// 
				resources.ApplyResources(madeWith_Label, "madeWith_Label");
				madeWith_Label.BackColor = Color.Transparent;
				madeWith_Label.ForeColor = Color.White;
				madeWith_Label.Name = "madeWith_Label";
				// 
				// Misc
				// 
				resources.ApplyResources(Misc, "Misc");
				Misc.BackColor = Color.Transparent;
				Misc.ForeColor = Color.White;
				Misc.Name = "Misc";
				// 
				// Tools
				// 
				resources.ApplyResources(this, "$this");
				AutoScaleMode = AutoScaleMode.Dpi;
				BackColor = Color.Black;
				BackgroundImage = Properties.Resources.bg_tools;
				Controls.Add(madeWith_Label);
				Controls.Add(Misc);
				Controls.Add(pictureBox10);
				Controls.Add(InnoSetupLogs);
				Controls.Add(pictureBox9);
				Controls.Add(InstallationLogs);
				Controls.Add(panel3);
				Controls.Add(pictureBox8);
				Controls.Add(DeleteOnlyWebView2Cache);
				Controls.Add(CacheAndLogs);
				Controls.Add(pictureBox6);
				Controls.Add(ScanAndRepairSysFiles);
				Controls.Add(pictureBox5);
				Controls.Add(SeeFPSUnlockerConfig);
				Controls.Add(pictureBox4);
				Controls.Add(SeeReShadeConfig);
				Controls.Add(ConfigFiles);
				Controls.Add(LogFiles);
				Controls.Add(pictureBox2);
				Controls.Add(LauncherLogs);
				Controls.Add(pictureBox1);
				Controls.Add(ReShadeLogs);
				Controls.Add(SettingsAndUtils);
				Controls.Add(pictureBox3);
				Controls.Add(DeleteAllCacheAndLogFiles);
				Controls.Add(label1);
				Controls.Add(panel1);
				ForeColor = SystemColors.ControlText;
				FormBorderStyle = FormBorderStyle.None;
				Name = "Tools";
				Load += Tools_Load;
				Shown += Utils_Shown;
				panel1.ResumeLayout(false);
				((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
				((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
				((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
				((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
				((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
				((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
				((System.ComponentModel.ISupportInitialize)pictureBox8).EndInit();
				((System.ComponentModel.ISupportInitialize)pictureBox9).EndInit();
				((System.ComponentModel.ISupportInitialize)pictureBox10).EndInit();
				ResumeLayout(false);
				PerformLayout();
		  }

		  #endregion

		  private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label SettingsAndUtils;
        private System.Windows.Forms.LinkLabel DeleteAllCacheAndLogFiles;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.LinkLabel ReShadeLogs;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel LauncherLogs;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label LogFiles;
        private System.Windows.Forms.Label ConfigFiles;
        private System.Windows.Forms.LinkLabel SeeReShadeConfig;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.LinkLabel SeeFPSUnlockerConfig;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.LinkLabel ScanAndRepairSysFiles;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.Label CacheAndLogs;
        private System.Windows.Forms.PictureBox pictureBox8;
        private System.Windows.Forms.LinkLabel DeleteOnlyWebView2Cache;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.PictureBox pictureBox9;
        private System.Windows.Forms.LinkLabel InstallationLogs;
        private System.Windows.Forms.PictureBox pictureBox10;
        private System.Windows.Forms.LinkLabel InnoSetupLogs;
        private System.Windows.Forms.Label madeWith_Label;
        private System.Windows.Forms.Label Misc;
    }
}
