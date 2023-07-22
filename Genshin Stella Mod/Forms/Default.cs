using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;
using Newtonsoft.Json;
using StellaLauncher.Forms.Other;
using StellaLauncher.Models;
using StellaLauncher.Properties;
using StellaLauncher.Scripts;
using StellaLauncher.Scripts.Forms.MainForm;

namespace StellaLauncher.Forms
{
    public partial class Default : Form
    {
        // Files
        private static readonly string RunCmd = Path.Combine(Program.AppPath, "data", "cmd", "run.cmd");
        private static readonly string RunCmdPatrons = Path.Combine(Program.AppPath, "data", "cmd", "run_patrons.cmd");

        // New update?
        public static bool UpdateIsAvailable;

        // Main
        public static Label _status_Label;
        public static Label _preparingPleaseWait;
        public static ProgressBar _progressBar1;

        // Left
        public static PictureBox _discordServerIco_Picturebox;
        public static LinkLabel _discordServer_LinkLabel;
        public static PictureBox _supportMeIco_PictureBox;
        public static LinkLabel _supportMe_LinkLabel;
        public static PictureBox _youtubeIco_Picturebox;
        public static LinkLabel _youTube_LinkLabel;

        // Start the game
        public static LinkLabel _startGame_LinkLabel;
        public static LinkLabel _injectReShade_LinkLabel;
        public static LinkLabel _runFpsUnlocker_LinkLabel;
        public static LinkLabel _only3DMigoto_LinkLabel;
        public static LinkLabel _runGiLauncher_LinkLabel;
        public static LinkLabel _becomeMyPatron_LinkLabel;

        // Bottom
        // public static PictureBox _toolsIco_PictureBox;
        // public static LinkLabel _tools_LinkLabel;
        // public static PictureBox _shortcutIco_PictureBox;
        // public static LinkLabel _links_LinkLabel;
        // public static PictureBox _padIco_PictureBox;
        // public static LinkLabel _gameplay_LinkLabel;
        // public static PictureBox _websiteIco_PictureBox;
        // public static LinkLabel _website_LinkLabel;

        // Right
        public static LinkLabel _version_LinkLabel;
        public static LinkLabel _updates_LinkLabel;
        public static PictureBox _updateIco_PictureBox;

        // Path
        public static string _resourcesPath;

        // Window
        private bool _mouseDown;
        private Point _offset;

        public Default()
        {
            InitializeComponent();
        }

        private async void Default_Load(object sender, EventArgs e)
        {
            progressBar1.Value = 5;

            // First
            _status_Label = status_Label;
            _preparingPleaseWait = PreparingPleaseWait;
            _progressBar1 = progressBar1;

            _discordServerIco_Picturebox = discordServerIco_Picturebox;
            _discordServer_LinkLabel = discordServer_LinkLabel;
            _supportMeIco_PictureBox = supportMeIco_PictureBox;
            _supportMe_LinkLabel = supportMe_LinkLabel;
            _youtubeIco_Picturebox = youtubeIco_Picturebox;
            _youTube_LinkLabel = youTube_LinkLabel;

            _startGame_LinkLabel = startGame_LinkLabel;
            _injectReShade_LinkLabel = injectReShade_LinkLabel;
            _runFpsUnlocker_LinkLabel = runFpsUnlocker_LinkLabel;
            _only3DMigoto_LinkLabel = only3DMigoto_LinkLabel;
            _runGiLauncher_LinkLabel = runGiLauncher_LinkLabel;
            _becomeMyPatron_LinkLabel = becomeMyPatron_LinkLabel;

            // _toolsIco_PictureBox = toolsIco_PictureBox;
            // _tools_LinkLabel = tools_LinkLabel;
            // _shortcutIco_PictureBox = shortcutIco_PictureBox;
            // _links_LinkLabel = links_LinkLabel;
            // _padIco_PictureBox = padIco_PictureBox;
            // _gameplay_LinkLabel = gameplay_LinkLabel;
            // _websiteIco_PictureBox = websiteIco_PictureBox;
            // _website_LinkLabel = website_LinkLabel;

            _version_LinkLabel = version_LinkLabel;
            _updates_LinkLabel = updates_LinkLabel;
            _updateIco_PictureBox = updateIco_PictureBox;


            // Set background
            Image newBackground = Background.OnStart(toolTip1, changeBg_LinkLabel);
            if (newBackground != null) BackgroundImage = newBackground;

            // Registry
            progressBar1.Value = 15;
            using (RegistryKey key2 = Registry.CurrentUser.CreateSubKey(Program.RegistryPath, true))
            {
                key2?.SetValue("LastRunTime", DateTime.Now);
            }


            // Is user my Patron?
            progressBar1.Value = 35;
            string mainPcKey = Secret.GetTokenFromRegistry();
            if (mainPcKey == null) return;

            label1.Text = @"/ᐠ. ｡.ᐟ\ᵐᵉᵒʷˎˊ˗";

            string data = await Secret.VerifyToken(mainPcKey);
            if (data == null)
            {
                if (Directory.Exists(Program.PatronsDir)) Directory.Delete(Program.PatronsDir, true);
                return;
            }

            GetToken remote = JsonConvert.DeserializeObject<GetToken>(data);
            Log.Output(remote.Status.ToString());
            if (remote.Status == 200)
            {
                Secret.IsMyPatron = true;
                label1.Text = Resources.Default_GenshinStellaModForPatrons;
                label1.TextAlign = ContentAlignment.MiddleRight;
            }
            else
            {
                if (Directory.Exists(Program.PatronsDir)) Directory.Delete(Program.PatronsDir, true);
                label1.Text = @"Oh nooo... Sad cat... ( ̿–ᆺ ̿–)";
            }
        }

        private async void Main_Shown(object sender, EventArgs e)
        {
            // Check if all required files exists
            progressBar1.Value = 45;
            await Files.ScanAsync(Text);

            // Delete setup file from Temp directory
            progressBar1.Value = 50;
            await Files.DeleteSetupAsync();

            // Check for updates
            progressBar1.Value = 55;
            await CheckForUpdatesMain.Analyze();

            // Discord RPC
            Discord.InitRpc();

            if (Debugger.IsAttached) return;
            // Telemetry.Opened();

            // Music
            Music.Play();

            // Launch count
            LaunchCountHelper.CheckLaunchCountAndShowMessages();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Default_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log.Output(string.Format(Resources.Main_ClosingForm_, Text));
        }

        private void Default_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Output(Resources.Main_Closed);
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

        private void ChangeBg_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Image newBackground = Background.Change(BackgroundImage, toolTip1, changeBg_LinkLabel);
            if (newBackground != null) BackgroundImage = newBackground;
        }


        // ------- Body -------
        private void GitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl("https://github.com/sefinek24/Genshin-Impact-ReShade");
        }


        // ------- Start the game -------
        // 6 = ReShade + FPS Unlocker
        // 1 = ReShade + 3DMigoto + FPS Unlocker
        // 2 = ReShade + 3DMigoto
        // 3 = Only ReShade
        // 4 = Only FPS Unlocker
        // 5 = Only 3DMigoto

        /* 1 */
        private async void StartGame_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Run cmd file
            bool res = await Cmd.CliWrap("wt.exe",
                $"{(Secret.IsMyPatron ? RunCmdPatrons : RunCmd)} {(Secret.IsMyPatron ? 1 : 6)} {await Utils.GetGameVersion()} \"{Log.CmdLogs}\" {(Secret.IsMyPatron ? $"\"{_resourcesPath}\\3DMigoto\"" : "0")} \"{Program.AppPath}\"", Program.AppPath,
                false, false);

            // Exit Stella with status code 0
            if (res) Environment.Exit(0);
        }

        /* 3 */
        private async void OnlyReShade_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Run cmd file
            bool res = await Cmd.CliWrap("wt.exe", $"{(Secret.IsMyPatron ? RunCmdPatrons : RunCmd)} 3 {await Utils.GetGameVersion()} \"{Log.CmdLogs}\"", Program.AppPath, false, false);
            if (!res) return;

            // Find game path
            string path = await Utils.GetGame("giLauncher");
            if (path == null) return;

            // Open Genshin Launcher
            _ = Cmd.CliWrap(path, null, null, true, false);
        }

        /* 4 */
        private async void OnlyUnlocker_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Run cmd file
            bool res = await Cmd.CliWrap("wt.exe", $"{(Secret.IsMyPatron ? RunCmdPatrons : RunCmd)} 4 {await Utils.GetGameVersion()} \"{Log.CmdLogs}\"", Program.AppPath, false, false);

            // Exit Stella with status code 0
            if (res) Environment.Exit(0);
        }

        /* 5 */
        private async void Only3DMigoto_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!Secret.IsMyPatron)
            {
                DialogResult result = MessageBox.Show(Resources.Default_ThisFeatureIsAvailableOnlyForMyPatrons, Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) Utils.OpenUrl("https://www.patreon.com/sefinek");
                return;
            }

            // Run cmd file
            await Cmd.CliWrap("wt.exe", $"{(Secret.IsMyPatron ? RunCmdPatrons : RunCmd)} 5 {await Utils.GetGameVersion()} \"{Log.CmdLogs}\" \"{_resourcesPath}\\3DMigoto\" \"{Program.AppPath}\"", Program.AppPath, false, false);

            // Find game path
            string path = await Utils.GetGame("giLauncher");
            if (path == null) return;

            // Open Genshin Launcher
            _ = Cmd.CliWrap(path, null, null, true, false);
        }

        private async void OpenGILauncher_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string path = await Utils.GetGame("giLauncher");
            if (path == string.Empty)
            {
                MessageBox.Show(Resources.Default_GameLauncherWasNotFound, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Log.SaveError(string.Format(Resources.Default_GameLauncherWasNotFoundIn, path));
                return;
            }

            await Cmd.CliWrap(path, null, null, true, false);
        }


        // ------- Footer -------
        private void Patreon_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl("https://www.patreon.com/sefinek");
        }

        private void SupportMe_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl("https://sefinek.net/support-me");
        }

        private void DiscordServer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl(Discord.Invitation);
        }

        private void YouTube_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl("https://www.youtube.com/@sefinek");
        }

        private void Tools_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Application.OpenForms.OfType<Tools>().Any()) return;
            new Tools { DesktopLocation = DesktopLocation, Icon = Program.Ico }.Show();
        }

        private void Gameplay_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Application.OpenForms.OfType<Gameplay>().Any()) return;
            new Gameplay { DesktopLocation = DesktopLocation, Icon = Program.Ico }.Show();
        }

        private void Links_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Application.OpenForms.OfType<Links>().Any()) return;
            new Links { DesktopLocation = DesktopLocation, Icon = Program.Ico }.Show();
        }

        private void Website_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl(Program.AppWebsiteFull);
        }

        private void Version_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl("https://github.com/sefinek24/Genshin-Impact-ReShade/wiki/14.-Changelog-for-v7.x.x");
        }

        private void MadeBySefinek_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Resources.Default_ItsJustText_WhatMoreDoYouWant, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Question);
            Utils.OpenUrl("https://www.youtube.com/watch?v=RpDf3XFHVNI");
        }

        private void CheckUpdates_Worker(object sender, EventArgs e)
        {
            CheckForUpdatesMain.CheckUpdates_Click(sender, e);
        }

        private void W_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (ComputerInfo.GetSystemRegion() == "PL")
            {
                WebViewWindow viewer = new WebViewWindow { DesktopLocation = DesktopLocation, Icon = Program.Ico };
                viewer.Navigate("https://www.youtube.com/embed/2F2DdXUNyaQ?autoplay=1");
                viewer.Show();

                MessageBox.Show(@"Pamiętaj by nie grać w lola, gdyż to grzech ciężki.");
            }
            else
            {
                WebViewWindow viewer = new WebViewWindow { DesktopLocation = DesktopLocation, Icon = Program.Ico };
                viewer.Navigate("https://www.youtube.com/embed/L3ky4gZU5gY?autoplay=1");
                viewer.Show();
            }
        }

        private void Paimon_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<RandomImages>().Any()) return;

            RandomImages randomImagesForm = new RandomImages { Icon = Program.Ico };
            randomImagesForm.Show();
        }

        private void StatusLabel_TextChanged(object sender, EventArgs e)
        {
            status_Label.Visible = !string.IsNullOrEmpty(status_Label.Text);
        }
    }
}
