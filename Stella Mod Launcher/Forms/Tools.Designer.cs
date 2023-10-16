namespace StellaLauncher.Forms
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Tools));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.SettingsAndUtils = new System.Windows.Forms.Label();
            this.DeleteAllCacheAndLogFiles = new System.Windows.Forms.LinkLabel();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.ReShadeLogs = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.LauncherLogs = new System.Windows.Forms.LinkLabel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.LogFiles = new System.Windows.Forms.Label();
            this.ConfigFiles = new System.Windows.Forms.Label();
            this.SeeReShadeConfig = new System.Windows.Forms.LinkLabel();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.SeeFPSUnlockerConfig = new System.Windows.Forms.LinkLabel();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.ScanAndRepairSysFiles = new System.Windows.Forms.LinkLabel();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.CacheAndLogs = new System.Windows.Forms.Label();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.DeleteOnlyWebView2Cache = new System.Windows.Forms.LinkLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.pictureBox9 = new System.Windows.Forms.PictureBox();
            this.InstallationLogs = new System.Windows.Forms.LinkLabel();
            this.pictureBox10 = new System.Windows.Forms.PictureBox();
            this.InnoSetupLogs = new System.Windows.Forms.LinkLabel();
            this.madeWith_Label = new System.Windows.Forms.Label();
            this.Misc = new System.Windows.Forms.Label();
            this.pictureBox16 = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox16)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.panel1.ForeColor = System.Drawing.Color.Transparent;
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
            this.panel2.Name = "panel2";
            this.toolTip1.SetToolTip(this.panel2, resources.GetString("panel2.ToolTip"));
            this.panel2.Click += new System.EventHandler(this.Exit_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label1.Name = "label1";
            // 
            // SettingsAndUtils
            // 
            resources.ApplyResources(this.SettingsAndUtils, "SettingsAndUtils");
            this.SettingsAndUtils.BackColor = System.Drawing.Color.Transparent;
            this.SettingsAndUtils.ForeColor = System.Drawing.Color.White;
            this.SettingsAndUtils.Name = "SettingsAndUtils";
            // 
            // DeleteAllCacheAndLogFiles
            // 
            this.DeleteAllCacheAndLogFiles.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            resources.ApplyResources(this.DeleteAllCacheAndLogFiles, "DeleteAllCacheAndLogFiles");
            this.DeleteAllCacheAndLogFiles.BackColor = System.Drawing.Color.Transparent;
            this.DeleteAllCacheAndLogFiles.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.DeleteAllCacheAndLogFiles.LinkColor = System.Drawing.Color.White;
            this.DeleteAllCacheAndLogFiles.Name = "DeleteAllCacheAndLogFiles";
            this.DeleteAllCacheAndLogFiles.TabStop = true;
            this.DeleteAllCacheAndLogFiles.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DeleteCache_LinkClicked);
            // 
            // pictureBox3
            // 
            resources.ApplyResources(this.pictureBox3, "pictureBox3");
            this.pictureBox3.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox3.Image = global::StellaLauncher.Properties.Resources.icons8_recycle_bin;
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.TabStop = false;
            // 
            // ReShadeLogs
            // 
            this.ReShadeLogs.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            resources.ApplyResources(this.ReShadeLogs, "ReShadeLogs");
            this.ReShadeLogs.BackColor = System.Drawing.Color.Transparent;
            this.ReShadeLogs.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.ReShadeLogs.LinkColor = System.Drawing.Color.White;
            this.ReShadeLogs.Name = "ReShadeLogs";
            this.ReShadeLogs.TabStop = true;
            this.ReShadeLogs.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ReShadeLogs_LinkClicked);
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // LauncherLogs
            // 
            this.LauncherLogs.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            resources.ApplyResources(this.LauncherLogs, "LauncherLogs");
            this.LauncherLogs.BackColor = System.Drawing.Color.Transparent;
            this.LauncherLogs.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.LauncherLogs.LinkColor = System.Drawing.Color.White;
            this.LauncherLogs.Name = "LauncherLogs";
            this.LauncherLogs.TabStop = true;
            this.LauncherLogs.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LauncherLogs_LinkClicked);
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // LogFiles
            // 
            resources.ApplyResources(this.LogFiles, "LogFiles");
            this.LogFiles.BackColor = System.Drawing.Color.Transparent;
            this.LogFiles.ForeColor = System.Drawing.Color.White;
            this.LogFiles.Name = "LogFiles";
            // 
            // ConfigFiles
            // 
            resources.ApplyResources(this.ConfigFiles, "ConfigFiles");
            this.ConfigFiles.BackColor = System.Drawing.Color.Transparent;
            this.ConfigFiles.ForeColor = System.Drawing.Color.White;
            this.ConfigFiles.Name = "ConfigFiles";
            // 
            // SeeReShadeConfig
            // 
            this.SeeReShadeConfig.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            resources.ApplyResources(this.SeeReShadeConfig, "SeeReShadeConfig");
            this.SeeReShadeConfig.BackColor = System.Drawing.Color.Transparent;
            this.SeeReShadeConfig.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.SeeReShadeConfig.LinkColor = System.Drawing.Color.White;
            this.SeeReShadeConfig.Name = "SeeReShadeConfig";
            this.SeeReShadeConfig.TabStop = true;
            this.toolTip1.SetToolTip(this.SeeReShadeConfig, resources.GetString("SeeReShadeConfig.ToolTip"));
            this.SeeReShadeConfig.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ReShadeConfig_LinkClicked);
            // 
            // pictureBox4
            // 
            resources.ApplyResources(this.pictureBox4, "pictureBox4");
            this.pictureBox4.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox4.Image = global::StellaLauncher.Properties.Resources.icons8_edit_property;
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.TabStop = false;
            // 
            // SeeFPSUnlockerConfig
            // 
            this.SeeFPSUnlockerConfig.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            resources.ApplyResources(this.SeeFPSUnlockerConfig, "SeeFPSUnlockerConfig");
            this.SeeFPSUnlockerConfig.BackColor = System.Drawing.Color.Transparent;
            this.SeeFPSUnlockerConfig.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.SeeFPSUnlockerConfig.LinkColor = System.Drawing.Color.White;
            this.SeeFPSUnlockerConfig.Name = "SeeFPSUnlockerConfig";
            this.SeeFPSUnlockerConfig.TabStop = true;
            this.toolTip1.SetToolTip(this.SeeFPSUnlockerConfig, resources.GetString("SeeFPSUnlockerConfig.ToolTip"));
            this.SeeFPSUnlockerConfig.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.UnlockerConfig_LinkClicked);
            // 
            // pictureBox5
            // 
            resources.ApplyResources(this.pictureBox5, "pictureBox5");
            this.pictureBox5.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox5.Image = global::StellaLauncher.Properties.Resources.icons8_edit_property;
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.TabStop = false;
            // 
            // ScanAndRepairSysFiles
            // 
            this.ScanAndRepairSysFiles.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            resources.ApplyResources(this.ScanAndRepairSysFiles, "ScanAndRepairSysFiles");
            this.ScanAndRepairSysFiles.BackColor = System.Drawing.Color.Transparent;
            this.ScanAndRepairSysFiles.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.ScanAndRepairSysFiles.LinkColor = System.Drawing.Color.White;
            this.ScanAndRepairSysFiles.Name = "ScanAndRepairSysFiles";
            this.ScanAndRepairSysFiles.TabStop = true;
            this.toolTip1.SetToolTip(this.ScanAndRepairSysFiles, resources.GetString("ScanAndRepairSysFiles.ToolTip"));
            this.ScanAndRepairSysFiles.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ScanSysFiles_LinkClicked);
            // 
            // pictureBox6
            // 
            resources.ApplyResources(this.pictureBox6, "pictureBox6");
            this.pictureBox6.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox6.Image = global::StellaLauncher.Properties.Resources.icons8_tools;
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.TabStop = false;
            // 
            // CacheAndLogs
            // 
            resources.ApplyResources(this.CacheAndLogs, "CacheAndLogs");
            this.CacheAndLogs.BackColor = System.Drawing.Color.Transparent;
            this.CacheAndLogs.ForeColor = System.Drawing.Color.White;
            this.CacheAndLogs.Name = "CacheAndLogs";
            // 
            // pictureBox8
            // 
            resources.ApplyResources(this.pictureBox8, "pictureBox8");
            this.pictureBox8.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox8.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox8.Image = global::StellaLauncher.Properties.Resources.icons8_recycle_bin;
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.TabStop = false;
            // 
            // DeleteOnlyWebView2Cache
            // 
            this.DeleteOnlyWebView2Cache.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            resources.ApplyResources(this.DeleteOnlyWebView2Cache, "DeleteOnlyWebView2Cache");
            this.DeleteOnlyWebView2Cache.BackColor = System.Drawing.Color.Transparent;
            this.DeleteOnlyWebView2Cache.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.DeleteOnlyWebView2Cache.LinkColor = System.Drawing.Color.White;
            this.DeleteOnlyWebView2Cache.Name = "DeleteOnlyWebView2Cache";
            this.DeleteOnlyWebView2Cache.TabStop = true;
            this.DeleteOnlyWebView2Cache.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DeleteWebViewCache_LinkClicked);
            // 
            // linkLabel2
            // 
            this.linkLabel2.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            resources.ApplyResources(this.linkLabel2, "linkLabel2");
            this.linkLabel2.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel2.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel2.LinkColor = System.Drawing.Color.White;
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.TabStop = true;
            this.toolTip1.SetToolTip(this.linkLabel2, resources.GetString("linkLabel2.ToolTip"));
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.RemoveStellaNotifications_LinkClicked);
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.BackColor = System.Drawing.Color.Transparent;
            this.panel3.Name = "panel3";
            this.panel3.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Notepad_MouseClick);
            // 
            // pictureBox9
            // 
            resources.ApplyResources(this.pictureBox9, "pictureBox9");
            this.pictureBox9.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox9.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox9.Name = "pictureBox9";
            this.pictureBox9.TabStop = false;
            // 
            // InstallationLogs
            // 
            this.InstallationLogs.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            resources.ApplyResources(this.InstallationLogs, "InstallationLogs");
            this.InstallationLogs.BackColor = System.Drawing.Color.Transparent;
            this.InstallationLogs.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.InstallationLogs.LinkColor = System.Drawing.Color.White;
            this.InstallationLogs.Name = "InstallationLogs";
            this.InstallationLogs.TabStop = true;
            this.InstallationLogs.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.GSModLogs_LinkClicked);
            // 
            // pictureBox10
            // 
            resources.ApplyResources(this.pictureBox10, "pictureBox10");
            this.pictureBox10.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox10.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox10.Image = global::StellaLauncher.Properties.Resources.flaticon_open_folder;
            this.pictureBox10.Name = "pictureBox10";
            this.pictureBox10.TabStop = false;
            // 
            // InnoSetupLogs
            // 
            this.InnoSetupLogs.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            resources.ApplyResources(this.InnoSetupLogs, "InnoSetupLogs");
            this.InnoSetupLogs.BackColor = System.Drawing.Color.Transparent;
            this.InnoSetupLogs.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.InnoSetupLogs.LinkColor = System.Drawing.Color.White;
            this.InnoSetupLogs.Name = "InnoSetupLogs";
            this.InnoSetupLogs.TabStop = true;
            this.InnoSetupLogs.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LogDir_LinkClicked);
            // 
            // madeWith_Label
            // 
            resources.ApplyResources(this.madeWith_Label, "madeWith_Label");
            this.madeWith_Label.BackColor = System.Drawing.Color.Transparent;
            this.madeWith_Label.ForeColor = System.Drawing.Color.White;
            this.madeWith_Label.Name = "madeWith_Label";
            // 
            // Misc
            // 
            resources.ApplyResources(this.Misc, "Misc");
            this.Misc.BackColor = System.Drawing.Color.Transparent;
            this.Misc.ForeColor = System.Drawing.Color.White;
            this.Misc.Name = "Misc";
            // 
            // pictureBox16
            // 
            resources.ApplyResources(this.pictureBox16, "pictureBox16");
            this.pictureBox16.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox16.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox16.Image = global::StellaLauncher.Properties.Resources.icons8_recycle_bin;
            this.pictureBox16.Name = "pictureBox16";
            this.pictureBox16.TabStop = false;
            // 
            // Tools
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImage = global::StellaLauncher.Properties.Resources.bg_tools;
            this.Controls.Add(this.pictureBox16);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.madeWith_Label);
            this.Controls.Add(this.Misc);
            this.Controls.Add(this.pictureBox10);
            this.Controls.Add(this.InnoSetupLogs);
            this.Controls.Add(this.pictureBox9);
            this.Controls.Add(this.InstallationLogs);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.pictureBox8);
            this.Controls.Add(this.DeleteOnlyWebView2Cache);
            this.Controls.Add(this.CacheAndLogs);
            this.Controls.Add(this.pictureBox6);
            this.Controls.Add(this.ScanAndRepairSysFiles);
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.SeeFPSUnlockerConfig);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.SeeReShadeConfig);
            this.Controls.Add(this.ConfigFiles);
            this.Controls.Add(this.LogFiles);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.LauncherLogs);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.ReShadeLogs);
            this.Controls.Add(this.SettingsAndUtils);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.DeleteAllCacheAndLogFiles);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Tools";
            this.Shown += new System.EventHandler(this.Utils_Shown);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox16)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.PictureBox pictureBox16;
    }
}
