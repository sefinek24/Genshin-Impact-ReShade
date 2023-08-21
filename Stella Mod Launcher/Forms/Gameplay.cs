using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using StellaLauncher.Forms.Other;
using StellaLauncher.Properties;
using StellaLauncher.Scripts;
using StellaLauncher.Scripts.Forms;

namespace StellaLauncher.Forms
{
    public partial class Gameplay : Form
    {
        private bool _mouseDown;
        private Point _offset;

        public Gameplay()
        {
            InitializeComponent();
        }

        private async void Tutorial_Shown(object sender, EventArgs e)
        {
            try
            {
                CoreWebView2Environment coreWebView2Env = await CoreWebView2Environment.CreateAsync(null, Program.AppData, new CoreWebView2EnvironmentOptions());
                await webView21.EnsureCoreWebView2Async(coreWebView2Env);

                webView21.CoreWebView2.Navigate("https://www.youtube.com/embed/CjfNy3aPMWs");
            }
            catch (Exception ex)
            {
                WebView2.HandleError(ex);
            }

            Discord.SetStatus(Resources.Gameplay_WatchingGameplay);

            Log.Output(string.Format(Resources.Main_LoadedForm_, Text));
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

        private void Exit_Click(object sender, EventArgs e)
        {
            Log.Output(string.Format(Resources.Main_ClosedForm_, Text));
            Close();

            Discord.Home();
        }


        // Top
        private void OpenInBrowser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl("https://www.youtube.com/watch?v=CjfNy3aPMWs");
        }


        // Bottom
        private void Videos_LinkLabel(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl($"{Program.AppWebsiteFull}/videos");
        }

        private void Documentation_LinkLabel(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl($"{Program.AppWebsiteFull}/docs?page=introduction");
        }

        private void Gallery_LinkLabel(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Application.OpenForms.OfType<Gallery>().Any()) return;
            new Gallery { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) }.Show();
        }
    }
}
