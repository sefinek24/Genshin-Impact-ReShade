using System;
using System.Windows.Forms;
using Genshin_Stella_Mod.Scripts;
using Microsoft.Web.WebView2.Core;

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

            int data = Program.Settings.ReadInt("Launcher", "DiscordRPC", 1);
            if (data == 1)
            {
                Discord.Presence.Details = "Browsing the gallery 📷";
                Discord.Client.SetPresence(Discord.Presence);
            }

            Log.Output($"Loaded form '{Text}'.");
        }

        private async void InitBrowser()
        {
            CoreWebView2Environment coreWeb = await CoreWebView2Environment.CreateAsync(null, Program.AppData, new CoreWebView2EnvironmentOptions());
            await webView21.EnsureCoreWebView2Async(coreWeb);

            webView21.CoreWebView2.Navigate($"{Program.AppWebsiteFull}/gallery?page=1");
        }

        private void Gallery_FormClosed(object sender, FormClosedEventArgs e)
        {
            int data = Program.Settings.ReadInt("Launcher", "DiscordRPC", 1);
            if (data == 1)
            {
                Discord.Presence.Details = "Exited the gallery 🦊";
                Discord.Client.SetPresence(Discord.Presence);
            }

            Log.Output($"Closed form '{Text}'.");
        }
    }
}
