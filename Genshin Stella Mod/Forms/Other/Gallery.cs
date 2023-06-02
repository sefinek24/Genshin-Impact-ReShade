using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using StellaLauncher.Properties;
using StellaLauncher.Scripts;

namespace StellaLauncher.Forms.Other
{
    public partial class Gallery : Form
    {
        public Gallery()
        {
            InitializeComponent();
        }

        private void Gallery_Load(object sender, EventArgs e)
        {
            InitBrowser();

            Discord.SetStatus(Resources.Gallery_BrowsingTheGallery);

            Log.Output(string.Format(Resources.Main_LoadedForm_, Text));
        }

        private async void InitBrowser()
        {
            CoreWebView2Environment coreWeb = await CoreWebView2Environment.CreateAsync(null, Program.AppData, new CoreWebView2EnvironmentOptions());
            await webView21.EnsureCoreWebView2Async(coreWeb);

            webView21.CoreWebView2.Navigate($"{Program.AppWebsiteFull}/gallery?page=1");
        }

        private void Gallery_FormClosed(object sender, FormClosedEventArgs e)
        {
            Discord.SetStatus(Resources.Gallery_ExitedTheGallery);

            Log.Output(string.Format(Resources.Main_ClosedForm_, Text));
        }
    }
}
