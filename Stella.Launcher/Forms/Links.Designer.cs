namespace StellaModLauncher.Forms
{
	sealed partial class Links
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
				System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Links));
				panel1 = new Panel();
				panel3 = new Panel();
				pictureBox7 = new PictureBox();
				createShortcut_Label = new LinkLabel();
				pictureBox1 = new PictureBox();
				linkLabel1 = new LinkLabel();
				pictureBox2 = new PictureBox();
				linkLabel2 = new LinkLabel();
				pictureBox3 = new PictureBox();
				linkLabel3 = new LinkLabel();
				label1 = new Label();
				label2 = new Label();
				toolTip1 = new ToolTip(components);
				label3 = new Label();
				pictureBox4 = new PictureBox();
				linkLabel4 = new LinkLabel();
				pictureBox5 = new PictureBox();
				linkLabel5 = new LinkLabel();
				pictureBox6 = new PictureBox();
				linkLabel6 = new LinkLabel();
				panel2 = new Panel();
				panel1.SuspendLayout();
				((System.ComponentModel.ISupportInitialize)pictureBox7).BeginInit();
				((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
				((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
				((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
				((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
				((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
				((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
				SuspendLayout();
				// 
				// panel1
				// 
				panel1.BackColor = Color.Transparent;
				resources.ApplyResources(panel1, "panel1");
				panel1.Controls.Add(panel3);
				panel1.Cursor = Cursors.SizeAll;
				panel1.ForeColor = SystemColors.ControlText;
				panel1.Name = "panel1";
				panel1.MouseDown += MouseDown_Event;
				panel1.MouseMove += MouseMove_Event;
				panel1.MouseUp += MouseUp_Event;
				// 
				// panel3
				// 
				resources.ApplyResources(panel3, "panel3");
				panel3.BackColor = Color.Transparent;
				panel3.Cursor = Cursors.Hand;
				panel3.Name = "panel3";
				toolTip1.SetToolTip(panel3, resources.GetString("panel3.ToolTip"));
				panel3.Click += Exit_Click;
				// 
				// pictureBox7
				// 
				resources.ApplyResources(pictureBox7, "pictureBox7");
				pictureBox7.BackColor = Color.Transparent;
				pictureBox7.Image = Properties.Resources.icons8_shortcut;
				pictureBox7.Name = "pictureBox7";
				pictureBox7.TabStop = false;
				// 
				// createShortcut_Label
				// 
				createShortcut_Label.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(createShortcut_Label, "createShortcut_Label");
				createShortcut_Label.BackColor = Color.Transparent;
				createShortcut_Label.LinkBehavior = LinkBehavior.HoverUnderline;
				createShortcut_Label.LinkColor = Color.White;
				createShortcut_Label.Name = "createShortcut_Label";
				createShortcut_Label.TabStop = true;
				createShortcut_Label.LinkClicked += GenshinMap1_LinkClicked;
				// 
				// pictureBox1
				// 
				resources.ApplyResources(pictureBox1, "pictureBox1");
				pictureBox1.BackColor = Color.Transparent;
				pictureBox1.Image = Properties.Resources.icons8_shortcut;
				pictureBox1.Name = "pictureBox1";
				pictureBox1.TabStop = false;
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
				linkLabel1.LinkClicked += GenshinMap2_LinkClicked;
				// 
				// pictureBox2
				// 
				resources.ApplyResources(pictureBox2, "pictureBox2");
				pictureBox2.BackColor = Color.Transparent;
				pictureBox2.Image = Properties.Resources.icons8_shortcut;
				pictureBox2.Name = "pictureBox2";
				pictureBox2.TabStop = false;
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
				linkLabel2.LinkClicked += TierList1_LinkClicked;
				// 
				// pictureBox3
				// 
				resources.ApplyResources(pictureBox3, "pictureBox3");
				pictureBox3.BackColor = Color.Transparent;
				pictureBox3.Image = Properties.Resources.icons8_shortcut;
				pictureBox3.Name = "pictureBox3";
				pictureBox3.TabStop = false;
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
				linkLabel3.LinkClicked += TierList2_LinkClicked;
				// 
				// label1
				// 
				resources.ApplyResources(label1, "label1");
				label1.BackColor = Color.Transparent;
				label1.ForeColor = Color.DeepSkyBlue;
				label1.Name = "label1";
				// 
				// label2
				// 
				resources.ApplyResources(label2, "label2");
				label2.BackColor = Color.Transparent;
				label2.ForeColor = Color.DeepSkyBlue;
				label2.Name = "label2";
				// 
				// label3
				// 
				resources.ApplyResources(label3, "label3");
				label3.BackColor = Color.Transparent;
				label3.ForeColor = Color.DeepSkyBlue;
				label3.Name = "label3";
				// 
				// pictureBox4
				// 
				resources.ApplyResources(pictureBox4, "pictureBox4");
				pictureBox4.BackColor = Color.Transparent;
				pictureBox4.Image = Properties.Resources.icons8_shortcut;
				pictureBox4.Name = "pictureBox4";
				pictureBox4.TabStop = false;
				// 
				// linkLabel4
				// 
				linkLabel4.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(linkLabel4, "linkLabel4");
				linkLabel4.BackColor = Color.Transparent;
				linkLabel4.LinkBehavior = LinkBehavior.HoverUnderline;
				linkLabel4.LinkColor = Color.White;
				linkLabel4.Name = "linkLabel4";
				linkLabel4.TabStop = true;
				linkLabel4.LinkClicked += Api_LinkClicked;
				// 
				// pictureBox5
				// 
				resources.ApplyResources(pictureBox5, "pictureBox5");
				pictureBox5.BackColor = Color.Transparent;
				pictureBox5.Image = Properties.Resources.icons8_shortcut;
				pictureBox5.Name = "pictureBox5";
				pictureBox5.TabStop = false;
				// 
				// linkLabel5
				// 
				linkLabel5.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(linkLabel5, "linkLabel5");
				linkLabel5.BackColor = Color.Transparent;
				linkLabel5.LinkBehavior = LinkBehavior.HoverUnderline;
				linkLabel5.LinkColor = Color.White;
				linkLabel5.Name = "linkLabel5";
				linkLabel5.TabStop = true;
				linkLabel5.LinkClicked += Cdn_LinkClicked;
				// 
				// pictureBox6
				// 
				resources.ApplyResources(pictureBox6, "pictureBox6");
				pictureBox6.BackColor = Color.Transparent;
				pictureBox6.Image = Properties.Resources.icons8_shortcut;
				pictureBox6.Name = "pictureBox6";
				pictureBox6.TabStop = false;
				// 
				// linkLabel6
				// 
				linkLabel6.ActiveLinkColor = Color.DodgerBlue;
				resources.ApplyResources(linkLabel6, "linkLabel6");
				linkLabel6.BackColor = Color.Transparent;
				linkLabel6.LinkBehavior = LinkBehavior.HoverUnderline;
				linkLabel6.LinkColor = Color.White;
				linkLabel6.Name = "linkLabel6";
				linkLabel6.TabStop = true;
				linkLabel6.LinkClicked += Status_LinkClicked;
				// 
				// panel2
				// 
				resources.ApplyResources(panel2, "panel2");
				panel2.BackColor = Color.Transparent;
				panel2.Name = "panel2";
				panel2.MouseClick += Flower_MouseClick;
				// 
				// Links
				// 
				resources.ApplyResources(this, "$this");
				AutoScaleMode = AutoScaleMode.Dpi;
				BackColor = Color.Black;
				BackgroundImage = Properties.Resources.bg_links;
				Controls.Add(panel2);
				Controls.Add(pictureBox6);
				Controls.Add(linkLabel6);
				Controls.Add(pictureBox5);
				Controls.Add(linkLabel5);
				Controls.Add(pictureBox4);
				Controls.Add(linkLabel4);
				Controls.Add(label3);
				Controls.Add(label2);
				Controls.Add(label1);
				Controls.Add(pictureBox3);
				Controls.Add(linkLabel3);
				Controls.Add(pictureBox2);
				Controls.Add(linkLabel2);
				Controls.Add(pictureBox1);
				Controls.Add(linkLabel1);
				Controls.Add(pictureBox7);
				Controls.Add(createShortcut_Label);
				Controls.Add(panel1);
				FormBorderStyle = FormBorderStyle.None;
				Name = "Links";
				Load += Links_Load;
				Shown += URLs_Shown;
				panel1.ResumeLayout(false);
				((System.ComponentModel.ISupportInitialize)pictureBox7).EndInit();
				((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
				((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
				((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
				((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
				((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
				((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
				ResumeLayout(false);
				PerformLayout();
		  }

		  #endregion
		  private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox7;
        private System.Windows.Forms.LinkLabel createShortcut_Label;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.LinkLabel linkLabel5;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.LinkLabel linkLabel6;
        private System.Windows.Forms.Panel panel2;
    }
}
