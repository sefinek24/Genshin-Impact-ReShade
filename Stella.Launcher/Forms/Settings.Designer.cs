namespace StellaModLauncher.Forms
{
    sealed partial class Settings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.SettingsAndUtils = new System.Windows.Forms.Label();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.CreateShortcut = new System.Windows.Forms.LinkLabel();
            this.Launcher = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.MuteMusicOnStart = new System.Windows.Forms.LinkLabel();
            this.DisableDiscordRPC = new System.Windows.Forms.LinkLabel();
            this.ChangeLanguage = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.pictureBox11 = new System.Windows.Forms.PictureBox();
            this.pictureBox12 = new System.Windows.Forms.PictureBox();
            this.pictureBox14 = new System.Windows.Forms.PictureBox();
            this.pictureBox15 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox13 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            // pictureBox7
            // 
            resources.ApplyResources(this.pictureBox7, "pictureBox7");
            this.pictureBox7.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox7.Image = global::StellaModLauncher.Properties.Resources.icons8_shortcut;
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.TabStop = false;
            // 
            // CreateShortcut
            // 
            this.CreateShortcut.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            resources.ApplyResources(this.CreateShortcut, "CreateShortcut");
            this.CreateShortcut.BackColor = System.Drawing.Color.Transparent;
            this.CreateShortcut.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.CreateShortcut.LinkColor = System.Drawing.Color.White;
            this.CreateShortcut.Name = "CreateShortcut";
            this.CreateShortcut.TabStop = true;
            this.toolTip1.SetToolTip(this.CreateShortcut, resources.GetString("CreateShortcut.ToolTip"));
            this.CreateShortcut.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.CreateShortcut_LinkClicked);
            // 
            // Launcher
            // 
            resources.ApplyResources(this.Launcher, "Launcher");
            this.Launcher.BackColor = System.Drawing.Color.Transparent;
            this.Launcher.ForeColor = System.Drawing.Color.White;
            this.Launcher.Name = "Launcher";
            // 
            // MuteMusicOnStart
            // 
            this.MuteMusicOnStart.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            resources.ApplyResources(this.MuteMusicOnStart, "MuteMusicOnStart");
            this.MuteMusicOnStart.BackColor = System.Drawing.Color.Transparent;
            this.MuteMusicOnStart.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.MuteMusicOnStart.LinkColor = System.Drawing.Color.White;
            this.MuteMusicOnStart.Name = "MuteMusicOnStart";
            this.MuteMusicOnStart.TabStop = true;
            this.toolTip1.SetToolTip(this.MuteMusicOnStart, resources.GetString("MuteMusicOnStart.ToolTip"));
            this.MuteMusicOnStart.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.MuteMusic_LinkClicked);
            // 
            // DisableDiscordRPC
            // 
            this.DisableDiscordRPC.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            resources.ApplyResources(this.DisableDiscordRPC, "DisableDiscordRPC");
            this.DisableDiscordRPC.BackColor = System.Drawing.Color.Transparent;
            this.DisableDiscordRPC.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.DisableDiscordRPC.LinkColor = System.Drawing.Color.White;
            this.DisableDiscordRPC.Name = "DisableDiscordRPC";
            this.DisableDiscordRPC.TabStop = true;
            this.toolTip1.SetToolTip(this.DisableDiscordRPC, resources.GetString("DisableDiscordRPC.ToolTip"));
            this.DisableDiscordRPC.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DisableRPC_LinkClicked);
            // 
            // ChangeLanguage
            // 
            this.ChangeLanguage.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            resources.ApplyResources(this.ChangeLanguage, "ChangeLanguage");
            this.ChangeLanguage.BackColor = System.Drawing.Color.Transparent;
            this.ChangeLanguage.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.ChangeLanguage.LinkColor = System.Drawing.Color.White;
            this.ChangeLanguage.Name = "ChangeLanguage";
            this.ChangeLanguage.TabStop = true;
            this.toolTip1.SetToolTip(this.ChangeLanguage, resources.GetString("ChangeLanguage.ToolTip"));
            this.ChangeLanguage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ChangeLang_LinkClicked);
            // 
            // linkLabel1
            // 
            this.linkLabel1.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            resources.ApplyResources(this.linkLabel1, "linkLabel1");
            this.linkLabel1.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel1.LinkColor = System.Drawing.Color.White;
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.TabStop = true;
            this.toolTip1.SetToolTip(this.linkLabel1, resources.GetString("linkLabel1.ToolTip"));
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OpenConfWindow_LinkClicked);
            // 
            // linkLabel3
            // 
            this.linkLabel3.ActiveLinkColor = System.Drawing.Color.DodgerBlue;
            resources.ApplyResources(this.linkLabel3, "linkLabel3");
            this.linkLabel3.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel3.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel3.LinkColor = System.Drawing.Color.White;
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.TabStop = true;
            this.toolTip1.SetToolTip(this.linkLabel3, resources.GetString("linkLabel3.ToolTip"));
            this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ConfReShade_LinkClicked);
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
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ChangeInjectionMethod_LinkClicked);
            // 
            // pictureBox11
            // 
            resources.ApplyResources(this.pictureBox11, "pictureBox11");
            this.pictureBox11.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox11.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox11.Image = global::StellaModLauncher.Properties.Resources.icons8_gear;
            this.pictureBox11.Name = "pictureBox11";
            this.pictureBox11.TabStop = false;
            // 
            // pictureBox12
            // 
            resources.ApplyResources(this.pictureBox12, "pictureBox12");
            this.pictureBox12.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox12.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox12.Image = global::StellaModLauncher.Properties.Resources.icons8_gear;
            this.pictureBox12.Name = "pictureBox12";
            this.pictureBox12.TabStop = false;
            // 
            // pictureBox14
            // 
            resources.ApplyResources(this.pictureBox14, "pictureBox14");
            this.pictureBox14.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox14.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox14.Image = global::StellaModLauncher.Properties.Resources.icons8_gear;
            this.pictureBox14.Name = "pictureBox14";
            this.pictureBox14.TabStop = false;
            // 
            // pictureBox15
            // 
            resources.ApplyResources(this.pictureBox15, "pictureBox15");
            this.pictureBox15.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox15.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox15.Image = global::StellaModLauncher.Properties.Resources.icons8_gear;
            this.pictureBox15.Name = "pictureBox15";
            this.pictureBox15.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Name = "label2";
            // 
            // pictureBox13
            // 
            resources.ApplyResources(this.pictureBox13, "pictureBox13");
            this.pictureBox13.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox13.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox13.Image = global::StellaModLauncher.Properties.Resources.icons8_support;
            this.pictureBox13.Name = "pictureBox13";
            this.pictureBox13.TabStop = false;
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = global::StellaModLauncher.Properties.Resources.icons8_support;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Name = "label3";
            // 
            // Settings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImage = global::StellaModLauncher.Properties.Resources.bg_settings;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pictureBox13);
            this.Controls.Add(this.linkLabel3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox15);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.pictureBox14);
            this.Controls.Add(this.ChangeLanguage);
            this.Controls.Add(this.pictureBox12);
            this.Controls.Add(this.DisableDiscordRPC);
            this.Controls.Add(this.pictureBox11);
            this.Controls.Add(this.MuteMusicOnStart);
            this.Controls.Add(this.Launcher);
            this.Controls.Add(this.pictureBox7);
            this.Controls.Add(this.CreateShortcut);
            this.Controls.Add(this.SettingsAndUtils);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Settings";
            this.Load += new System.EventHandler(this.Tools_Load);
            this.Shown += new System.EventHandler(this.Utils_Shown);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label SettingsAndUtils;
        private System.Windows.Forms.PictureBox pictureBox7;
        private System.Windows.Forms.LinkLabel CreateShortcut;
        private System.Windows.Forms.Label Launcher;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.PictureBox pictureBox11;
        private System.Windows.Forms.LinkLabel MuteMusicOnStart;
        private System.Windows.Forms.PictureBox pictureBox12;
        private System.Windows.Forms.LinkLabel DisableDiscordRPC;
        private System.Windows.Forms.PictureBox pictureBox14;
        private System.Windows.Forms.LinkLabel ChangeLanguage;
        private System.Windows.Forms.PictureBox pictureBox15;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox13;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.Label label3;
    }
}
