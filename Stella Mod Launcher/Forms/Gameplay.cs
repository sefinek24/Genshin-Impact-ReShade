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
        private const string VideoId = "0tPwI7uVRxo";

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

                webView21.CoreWebView2.Navigate($"https://www.youtube.com/embed/{VideoId}");
            }
            catch (Exception ex)
            {
                WebView2.HandleError(ex);
            }

            Discord.SetStatus(Resources.Gameplay_WatchingGameplay);

            Program.Logger.Info(string.Format(Resources.Main_LoadedForm_, Text));
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
            Program.Logger.Info(string.Format(Resources.Main_ClosedForm_, Text));
            Close();

            Discord.Home();
        }


        // Top
        private void OpenInBrowser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl($"https://www.youtube.com/watch?v={VideoId}");
        }


        // Bottom
        private void Videos_LinkLabel(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Application.OpenForms.OfType<WebView2Window>().Any()) return;
            new WebView2Window { Title = "Videos - sefinek.net", Url = $"{Program.AppWebsiteFull}/videos" }.Show();
        }

        private void Documentation_LinkLabel(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Application.OpenForms.OfType<WebView2Window>().Any()) return;
            new WebView2Window { WindowState = FormWindowState.Maximized, Title = "Documentation - sefinek.net", Url = $"{Program.AppWebsiteFull}/docs?page=introduction" }.Show();
        }

        private void Gallery_LinkLabel(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Application.OpenForms.OfType<WebView2Window>().Any()) return;
            new WebView2Window { WindowState = FormWindowState.Maximized, Title = "Gallery - sefinek.net", Url = $"{Program.AppWebsiteFull}/gallery?page=1" }.Show();
        }
    }
}
