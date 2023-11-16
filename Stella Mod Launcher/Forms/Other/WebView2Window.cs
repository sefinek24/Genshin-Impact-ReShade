using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using StellaLauncher.Properties;
using StellaLauncher.Scripts;

namespace StellaLauncher.Forms.Other
{
    public partial class WebView2Window : Form
    {
        internal string Url;
        internal string Title;

        public WebView2Window()
        {
            InitializeComponent();
        }

        private void Gallery_Load(object sender, EventArgs e)
        {
            Text = Title ?? "sefinek.net";

            InitBrowser();

            Discord.SetStatus(Resources.Gallery_BrowsingTheGallery);

            Program.Logger.Info(string.Format(Resources.Main_LoadedForm_, Text));
        }

        private async void InitBrowser()
        {
            CoreWebView2Environment coreWeb = await CoreWebView2Environment.CreateAsync(null, Program.AppData, new CoreWebView2EnvironmentOptions());
            await webView21.EnsureCoreWebView2Async(coreWeb);

            webView21.CoreWebView2.Navigate(Url);
        }

        private void Gallery_FormClosed(object sender, FormClosedEventArgs e)
        {
            Discord.SetStatus(Resources.Gallery_ExitedTheGallery);

            Program.Logger.Info(string.Format(Resources.Main_ClosedForm_, Text));
        }
    }
}
