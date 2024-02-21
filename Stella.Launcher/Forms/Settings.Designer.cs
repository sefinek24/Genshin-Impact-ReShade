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
				components = new System.ComponentModel.Container();
				System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
				panel1 = new Panel();
				panel2 = new Panel();
				label1 = new Label();
				SettingsAndUtils = new Label();
				pictureBox7 = new PictureBox();
				CreateShortcut = new LinkLabel();
				Launcher = new Label();
				toolTip1 = new ToolTip(components);
				MuteMusicOnStart = new LinkLabel();
				DisableDiscordRPC = new LinkLabel();
				ChangeLanguage = new LinkLabel();
				linkLabel1 = new LinkLabel();
				linkLabel3 = new LinkLabel();
				linkLabel2 = new LinkLabel();
				pictureBox11 = new PictureBox();
				pictureBox12 = new PictureBox();
				pictureBox14 = new PictureBox();
				pictureBox15 = new PictureBox();
				label2 = new Label();
				pictureBox13 = new PictureBox();
				pictureBox1 = new PictureBox();
				label3 = new Label();
				release_Label = new Label();
				panel1.SuspendLayout();
				((System.ComponentModel.ISupportInitialize)pictureBox7).BeginInit();
				((System.ComponentModel.ISupportInitialize)pictureBox11).BeginInit();
				((System.ComponentModel.ISupportInitialize)pictureBox12).BeginInit();
				((System.ComponentModel.ISupportInitialize)pictureBox14).BeginInit();
				((System.ComponentModel.ISupportInitialize)pictureBox15).BeginInit();
				((System.ComponentModel.ISupportInitialize)pictureBox13).BeginInit();
				((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
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
				// pictureBox7
				// 
				resources.ApplyResources(pictureBox7, "pictureBox7");
				pictureBox7.BackColor = Color.Transparent;
				pictureBox7.Cursor = Cursors.Hand;
				pictureBox7.Image = Properties.Resources.icons8_shortcut;
				pictureBox7.Name = "pictureBox7";
				pictureBox7.TabStop = false;
				// 
				// CreateShortcut
				// 
				CreateShortcut.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(CreateShortcut, "CreateShortcut");
				CreateShortcut.BackColor = Color.Transparent;
				CreateShortcut.LinkBehavior = LinkBehavior.HoverUnderline;
				CreateShortcut.LinkColor = Color.White;
				CreateShortcut.Name = "CreateShortcut";
				CreateShortcut.TabStop = true;
				toolTip1.SetToolTip(CreateShortcut, resources.GetString("CreateShortcut.ToolTip"));
				CreateShortcut.LinkClicked += CreateShortcut_LinkClicked;
				// 
				// Launcher
				// 
				resources.ApplyResources(Launcher, "Launcher");
				Launcher.BackColor = Color.Transparent;
				Launcher.ForeColor = Color.White;
				Launcher.Name = "Launcher";
				// 
				// MuteMusicOnStart
				// 
				MuteMusicOnStart.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(MuteMusicOnStart, "MuteMusicOnStart");
				MuteMusicOnStart.BackColor = Color.Transparent;
				MuteMusicOnStart.LinkBehavior = LinkBehavior.HoverUnderline;
				MuteMusicOnStart.LinkColor = Color.White;
				MuteMusicOnStart.Name = "MuteMusicOnStart";
				MuteMusicOnStart.TabStop = true;
				toolTip1.SetToolTip(MuteMusicOnStart, resources.GetString("MuteMusicOnStart.ToolTip"));
				MuteMusicOnStart.LinkClicked += MuteMusic_LinkClicked;
				// 
				// DisableDiscordRPC
				// 
				DisableDiscordRPC.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(DisableDiscordRPC, "DisableDiscordRPC");
				DisableDiscordRPC.BackColor = Color.Transparent;
				DisableDiscordRPC.LinkBehavior = LinkBehavior.HoverUnderline;
				DisableDiscordRPC.LinkColor = Color.White;
				DisableDiscordRPC.Name = "DisableDiscordRPC";
				DisableDiscordRPC.TabStop = true;
				toolTip1.SetToolTip(DisableDiscordRPC, resources.GetString("DisableDiscordRPC.ToolTip"));
				DisableDiscordRPC.LinkClicked += DisableRPC_LinkClicked;
				// 
				// ChangeLanguage
				// 
				ChangeLanguage.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(ChangeLanguage, "ChangeLanguage");
				ChangeLanguage.BackColor = Color.Transparent;
				ChangeLanguage.LinkBehavior = LinkBehavior.HoverUnderline;
				ChangeLanguage.LinkColor = Color.White;
				ChangeLanguage.Name = "ChangeLanguage";
				ChangeLanguage.TabStop = true;
				toolTip1.SetToolTip(ChangeLanguage, resources.GetString("ChangeLanguage.ToolTip"));
				ChangeLanguage.LinkClicked += ChangeLang_LinkClicked;
				// 
				// linkLabel1
				// 
				linkLabel1.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(linkLabel1, "linkLabel1");
				linkLabel1.BackColor = Color.Transparent;
				linkLabel1.LinkBehavior = LinkBehavior.HoverUnderline;
				linkLabel1.LinkColor = Color.White;
				linkLabel1.Name = "linkLabel1";
				linkLabel1.TabStop = true;
				toolTip1.SetToolTip(linkLabel1, resources.GetString("linkLabel1.ToolTip"));
				linkLabel1.LinkClicked += OpenConfWindow_LinkClicked;
				// 
				// linkLabel3
				// 
				linkLabel3.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(linkLabel3, "linkLabel3");
				linkLabel3.BackColor = Color.Transparent;
				linkLabel3.LinkBehavior = LinkBehavior.HoverUnderline;
				linkLabel3.LinkColor = Color.White;
				linkLabel3.Name = "linkLabel3";
				linkLabel3.TabStop = true;
				toolTip1.SetToolTip(linkLabel3, resources.GetString("linkLabel3.ToolTip"));
				linkLabel3.LinkClicked += ConfReShade_LinkClicked;
				// 
				// linkLabel2
				// 
				linkLabel2.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(linkLabel2, "linkLabel2");
				linkLabel2.BackColor = Color.Transparent;
				linkLabel2.LinkBehavior = LinkBehavior.HoverUnderline;
				linkLabel2.LinkColor = Color.White;
				linkLabel2.Name = "linkLabel2";
				linkLabel2.TabStop = true;
				toolTip1.SetToolTip(linkLabel2, resources.GetString("linkLabel2.ToolTip"));
				linkLabel2.LinkClicked += ChangeInjectionMethod_LinkClicked;
				// 
				// pictureBox11
				// 
				resources.ApplyResources(pictureBox11, "pictureBox11");
				pictureBox11.BackColor = Color.Transparent;
				pictureBox11.Cursor = Cursors.Hand;
				pictureBox11.Image = Properties.Resources.icons8_gear;
				pictureBox11.Name = "pictureBox11";
				pictureBox11.TabStop = false;
				// 
				// pictureBox12
				// 
				resources.ApplyResources(pictureBox12, "pictureBox12");
				pictureBox12.BackColor = Color.Transparent;
				pictureBox12.Cursor = Cursors.Hand;
				pictureBox12.Image = Properties.Resources.icons8_gear;
				pictureBox12.Name = "pictureBox12";
				pictureBox12.TabStop = false;
				// 
				// pictureBox14
				// 
				resources.ApplyResources(pictureBox14, "pictureBox14");
				pictureBox14.BackColor = Color.Transparent;
				pictureBox14.Cursor = Cursors.Hand;
				pictureBox14.Image = Properties.Resources.icons8_gear;
				pictureBox14.Name = "pictureBox14";
				pictureBox14.TabStop = false;
				// 
				// pictureBox15
				// 
				resources.ApplyResources(pictureBox15, "pictureBox15");
				pictureBox15.BackColor = Color.Transparent;
				pictureBox15.Cursor = Cursors.Hand;
				pictureBox15.Image = Properties.Resources.icons8_gear;
				pictureBox15.Name = "pictureBox15";
				pictureBox15.TabStop = false;
				// 
				// label2
				// 
				resources.ApplyResources(label2, "label2");
				label2.BackColor = Color.Transparent;
				label2.ForeColor = Color.White;
				label2.Name = "label2";
				// 
				// pictureBox13
				// 
				resources.ApplyResources(pictureBox13, "pictureBox13");
				pictureBox13.BackColor = Color.Transparent;
				pictureBox13.Cursor = Cursors.Hand;
				pictureBox13.Image = Properties.Resources.icons8_support;
				pictureBox13.Name = "pictureBox13";
				pictureBox13.TabStop = false;
				// 
				// pictureBox1
				// 
				resources.ApplyResources(pictureBox1, "pictureBox1");
				pictureBox1.BackColor = Color.Transparent;
				pictureBox1.Cursor = Cursors.Hand;
				pictureBox1.Image = Properties.Resources.icons8_support;
				pictureBox1.Name = "pictureBox1";
				pictureBox1.TabStop = false;
				// 
				// label3
				// 
				resources.ApplyResources(label3, "label3");
				label3.BackColor = Color.Transparent;
				label3.ForeColor = Color.White;
				label3.Name = "label3";
				// 
				// release_Label
				// 
				resources.ApplyResources(release_Label, "release_Label");
				release_Label.BackColor = Color.Transparent;
				release_Label.ForeColor = Color.White;
				release_Label.Name = "release_Label";
				// 
				// Settings
				// 
				resources.ApplyResources(this, "$this");
				AutoScaleMode = AutoScaleMode.Dpi;
				BackColor = Color.Black;
				BackgroundImage = Properties.Resources.bg_settings;
				Controls.Add(release_Label);
				Controls.Add(pictureBox1);
				Controls.Add(linkLabel2);
				Controls.Add(label3);
				Controls.Add(pictureBox13);
				Controls.Add(linkLabel3);
				Controls.Add(label2);
				Controls.Add(pictureBox15);
				Controls.Add(linkLabel1);
				Controls.Add(pictureBox14);
				Controls.Add(ChangeLanguage);
				Controls.Add(pictureBox12);
				Controls.Add(DisableDiscordRPC);
				Controls.Add(pictureBox11);
				Controls.Add(MuteMusicOnStart);
				Controls.Add(Launcher);
				Controls.Add(pictureBox7);
				Controls.Add(CreateShortcut);
				Controls.Add(SettingsAndUtils);
				Controls.Add(label1);
				Controls.Add(panel1);
				ForeColor = SystemColors.ControlText;
				FormBorderStyle = FormBorderStyle.None;
				Name = "Settings";
				Load += Tools_Load;
				Shown += Utils_Shown;
				panel1.ResumeLayout(false);
				((System.ComponentModel.ISupportInitialize)pictureBox7).EndInit();
				((System.ComponentModel.ISupportInitialize)pictureBox11).EndInit();
				((System.ComponentModel.ISupportInitialize)pictureBox12).EndInit();
				((System.ComponentModel.ISupportInitialize)pictureBox14).EndInit();
				((System.ComponentModel.ISupportInitialize)pictureBox15).EndInit();
				((System.ComponentModel.ISupportInitialize)pictureBox13).EndInit();
				((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
				ResumeLayout(false);
				PerformLayout();
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
		  private Label release_Label;
	 }
}
