using System;
using System.Drawing;
using System.Windows.Forms;
using Genshin_Stella_Mod.Properties;
using Genshin_Stella_Mod.Scripts;

namespace Genshin_Stella_Mod.Forms.Other
{
    public partial class SupportMe : Form
    {
        private bool _mouseDown;
        private Point _offset;

        public SupportMe()
        {
            InitializeComponent();
        }

        private void MouseDown_Event(object sender, MouseEventArgs e)
        {
            _offset.X = e.X;
            _offset.Y = e.Y;
            _mouseDown = true;
        }

        private void MouseMove_Event(object sender, MouseEventArgs e)
        {
            if (!_mouseDown) return;
            Point currentScreenPos = PointToScreen(e.Location);
            Location = new Point(currentScreenPos.X - _offset.X, currentScreenPos.Y - _offset.Y);
        }

        private void MouseUp_Event(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
        }

        private void SupportMe_Shown(object sender, EventArgs e)
        {
            Log.Output($"Loaded form '{Text}'.");
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Log.Output($"Closed form '{Text}'.");
            Close();
        }


        // Content
        private void SupportMyWork_Click(object sender, EventArgs e)
        {
            Utils.OpenUrl("https://sefinek.net/support-me");
        }

        private void SubscribeMeOnYT_Click(object sender, EventArgs e)
        {
            Utils.OpenUrl("https://www.youtube.com/channel/UClrAIcAzcqIMbvGXZqK7e0A?sub_confirmation=1");
        }

        private void StarOnGitHub_Click(object sender, EventArgs e)
        {
            Utils.OpenUrl("https://sefinek.net/genshin-impact-reshade/repositories");
        }

        private void Discord_Click(object sender, EventArgs e)
        {
            Utils.OpenUrl(Discord.Invitation);
        }

        private void FeedbackOnDsc_Click(object sender, EventArgs e)
        {
            Utils.OpenUrl(Discord.FeedbackChannel);
        }


        // Footer
        private void WhyNot_Click(object sender, EventArgs e)
        {
            new Default { Location = Location, StartPosition = FormStartPosition.Manual, Icon = Resources.icon_52x52 }.Show();
            MessageBox.Show("Thanks :3", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);

            Log.Output($"Clicked yes in form '{Text}'.");
            Telemetry.SupportMe_AnswYes();
        }

        private void NotThisTime_Click(object sender, EventArgs e)
        {
            new Default { Location = Location, StartPosition = FormStartPosition.Manual, Icon = Resources.icon_52x52 }.Show();

            Log.Output($"Clicked no in form '{Text}'.");
            Telemetry.SupportMe_AnswNo();
        }
    }
}
