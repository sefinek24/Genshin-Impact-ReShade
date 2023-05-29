using System;
using System.Drawing;
using System.Windows.Forms;
using Genshin_Stella_Mod.Scripts;
using StellaLauncher.Properties;
using StellaLauncher.Scripts;

namespace StellaLauncher.Forms.Other
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
        private void SupportMe_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl("https://sefinek.net/support-me");
        }

        private void CsGoSkins_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl("https://steamcommunity.com/tradeoffer/new/?partner=1156692850&token=smcSsTMe");
        }

        private void SubscribeMe_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl("https://www.youtube.com/channel/UClrAIcAzcqIMbvGXZqK7e0A?sub_confirmation=1");
        }

        private void StarTheRepo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl("https://sefinek.net/genshin-impact-reshade/repositories");
        }

        private void DiscordServer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl(Discord.Invitation);
        }

        private void LeaveFeedback_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl(Discord.FeedbackChannel);
        }


        // Footer
        private void MaybeLater_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new Default { Location = Location, StartPosition = FormStartPosition.Manual, Icon = Resources.icon_52x52 }.Show();

            Log.Output($"Clicked no in form '{Text}'.");
            Telemetry.SupportMe_AnswNo();
        }

        private void OkayDone_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new Default { Location = Location, StartPosition = FormStartPosition.Manual, Icon = Resources.icon_52x52 }.Show();
            MessageBox.Show("Thanks :3", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);

            Log.Output($"Clicked yes in form '{Text}'.");
            Telemetry.SupportMe_AnswYes();
        }
    }
}
