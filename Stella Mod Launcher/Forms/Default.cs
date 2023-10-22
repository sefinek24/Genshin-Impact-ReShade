using System;
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
using StellaLauncher.Scripts.Forms;
using StellaLauncher.Scripts.Forms.MainForm;
using StellaLauncher.Scripts.Patrons;

namespace StellaLauncher.Forms
{
    public partial class Default : Form
    {
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
        private static LinkLabel _becomeMyPatron_LinkLabel;

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
        public static string ResourcesPath;

        // Window
        private bool _mouseDown;
        private Point _offset;

        public Default()
        {
            InitializeComponent();
        }

        private void Default_Load(object sender, EventArgs e)
        {
            // Set background
            Image newBackground = Background.OnStart(toolTip1, changeBg_LinkLabel);
            if (newBackground != null) BackgroundImage = newBackground;
        }

        private async void Main_Shown(object sender, EventArgs e)
        {
            // Registry
            using (RegistryKey key2 = Registry.CurrentUser.CreateSubKey(Program.RegistryPath, true))
            {
                key2?.SetValue("LastRunTime", DateTime.Now);
            }

            progressBar1.Value = 6;

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


            // Get resources path
            string resourcesPath = null;
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Program.RegistryPath))
            {
                if (key != null) resourcesPath = (string)key.GetValue("ResourcesPath");
            }

            if (string.IsNullOrEmpty(resourcesPath))
            {
                Log.SaveError("Path of the resources was not found. Is null or empty.");
                MessageBox.Show(Resources.Default_ResourceDirNotFound, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (!Directory.Exists(resourcesPath))
            {
                Log.SaveError($"Directory with the resources '{resourcesPath}' was not found.");
                MessageBox.Show(string.Format(Resources.Default_Directory_WasNotFound, resourcesPath), Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (string.IsNullOrEmpty(resourcesPath) || !Directory.Exists(resourcesPath))
            {
                _ = Cmd.Execute(new Cmd.CliWrap { App = Program.PrepareLauncher });
                Environment.Exit(997890421);
            }

            ResourcesPath = resourcesPath;


            // App version
            version_LinkLabel.Text = $@"v{Program.AppVersion}";
            progressBar1.Value = 18;


            // Tray
            NotifyIcon trayIcon = new NotifyIcon
            {
                Icon = Program.Ico,
                Text = Program.AppName,
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip()
            };

            Tray trayHandler = new Tray(trayIcon, this);
            trayIcon.ContextMenuStrip.Items.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("Toggle Minimize/Restore", null, trayHandler.ToggleMinimizeRestore),
                new ToolStripMenuItem("Reload window", null, trayHandler.ReloadForm),
                new ToolStripMenuItem("Official website", null, Tray.OfficialWebsite),
                new ToolStripMenuItem("Discord server", null, Tray.DiscordServer),
                new ToolStripMenuItem("Support", null, Tray.Support),
                new ToolStripMenuItem("Donations", null, Tray.Donations),
                new ToolStripMenuItem("Leave your feedback", null, Tray.Feedback),
                new ToolStripMenuItem("Quit", null, Tray.OnQuitClick)
            });


            // Is user my Patron?
            string mainPcKey = Secret.GetTokenFromRegistry();
            progressBar1.Value = 37;
            if (mainPcKey != null)
            {
                label1.Text = @"/ᐠ. ｡.ᐟ\ᵐᵉᵒʷˎˊ˗";

                string data = await Secret.VerifyToken(mainPcKey);
                if (data == null)
                {
                    Secret.IsMyPatron = false;
                    Log.Output("Received null from the server. Deleting benefits in progress...");

                    DeleteBenefits.Run();
                    MessageBox.Show("Zero data received from the server. What happened? I don't know, but Sefinek probably will (:\nReport this error as soon as possible.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(6660666);
                }
                else
                {
                    VerifyToken remote = JsonConvert.DeserializeObject<VerifyToken>(data);
                    Log.Output($"Status: {remote.Status}; Tier {remote.TierId}; Message: {remote.Message ?? "Unknown"};");

                    switch (remote.Status)
                    {
                        case 200:
                            Secret.IsMyPatron = true;
                            Log.Output($"User is my Patron; {Secret.IsMyPatron}; Benefits are enabled;");

                            label1.Text = Resources.Default_GenshinStellaModForPatrons;
                            label1.TextAlign = ContentAlignment.MiddleRight;

                            Secret.BearerToken = remote.Token;
                            break;

                        case 500:
                            Secret.IsMyPatron = false;
                            label1.Text = @"Something went wrong ( ̿–ᆺ ̿–)";

                            DeleteBenefits.Run();
                            MessageBox.Show(
                                $"Unfortunately, there was a server-side error during the verification of your benefits. Please report this error on the Discord server or via email. Remember to provide your `backup code` as well.\nIf you launch the game after closing this message, you will be playing the free version.\n\n{remote.Message}",
                                Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;

                        default:
                            Secret.IsMyPatron = false;
                            label1.Text = @"Oh nooo... Sad cat... ( ̿–ᆺ ̿–)";

                            DeleteBenefits.Run();
                            MessageBox.Show(
                                $"An error occurred while verifying the benefits of your subscription. The server informed the client that it sent an invalid request. If you launch the game after closing this message, you will be playing the free version. Please contact Sefinek for more information. Error details can be found below.\n\n{remote.Message}",
                                Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                    }
                }
            }
            else
            {
                Secret.IsMyPatron = false;
                DeleteBenefits.Run();
            }


            // Check if all required files exists
            progressBar1.Value = 44;
            await Files.ScanAsync();

            // Delete setup file from Temp directory
            progressBar1.Value = 50;
            await Files.DeleteSetupAsync();

            // Loaded form
            Log.Output(string.Format(Resources.Main_LoadedForm_, Text));

            // Launch count
            await LaunchCountHelper.CheckLaunchCountAndShowMessages();
            progressBar1.Value = 57;

            // Telemetry
            Telemetry.Opened();

            // Discord RPC
            Discord.InitRpc();


            // Updated was updated?
            int updatedLauncher = Program.Settings.ReadInt("Updates", "UpdateAvailable", 0);
            string oldVersion = Program.Settings.ReadString("Updates", "OldVersion", null);
            if (updatedLauncher == 1 && oldVersion != Program.AppVersion)
            {
                Program.Settings.WriteInt("Updates", "UpdateAvailable", 0);
                Program.Settings.Save();
                status_Label.Text += $"[✓] {Resources.Default_Congratulations}\n[i] {string.Format(Resources.Default_SMLSuccessfullyUpdatedToVersion_, Program.AppVersion)}\n";
            }


            // Check InjectType
            string injectMode = Program.Settings.ReadString("Injection", "Method", "exe");
            switch (injectMode)
            {
                case "exe":
                    Run.InjectType = "exe";
                    break;
                case "cmd" when Secret.IsMyPatron:
                    Run.InjectType = "cmd";
                    break;
                default:
                {
                    Run.InjectType = "exe";
                    Program.Settings.WriteString("Injection", "Method", "exe");
                    Program.Settings.Save();

                    if (!Secret.IsMyPatron)
                    {
                        status_Label.Text += @"[X] You cannot use batch files in Genshin Stella Mod without being a patron.";
                        Log.SaveError("You cannot use batch files without being a patron.");
                    }

                    break;
                }
            }


            // Check for updates
            progressBar1.Value = 68;
            int found = await CheckForUpdates.Analyze();
            if (found == 1) return;


            progressBar1.Value = 88;

            // Music
            _ = Music.PlayBg();


            // Done (:
            progressBar1.Value = 100;
            startGame_LinkLabel.Visible = true;
            injectReShade_LinkLabel.Visible = true;
            runFpsUnlocker_LinkLabel.Visible = true;
            only3DMigoto_LinkLabel.Visible = true;
            runGiLauncher_LinkLabel.Visible = true;
            if (!Secret.IsMyPatron) _becomeMyPatron_LinkLabel.Visible = true;
            Utils.HideProgressBar(false);
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

            Music.PlaySound("winxp", "menu_command");
        }


        // ------- Body -------
        private void GitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl("https://github.com/sefinek24/Genshin-Impact-ReShade");
        }


        // ------- Start the game -------
        // 1 = ReShade       + 3DMigoto      + FPS Unlocker  = 1 (default for patrons)
        // 2 = ReShade       + 3DMigoto                      = 2
        // 6 = ReShade       + FPS Unlocker                  = 6 (default)
        // 4 = FPS Unlocker                                  = 4
        // 5 = 3DMigoto                                      = 5
        // 3 = ReShade                                       = 3

        /* 1 */
        private async void StartGame_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            await Run.StartGame();
        }

        /* 3 */
        private async void OnlyReShade_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            await Run.ReShade();
        }

        /* 4 */
        private async void OnlyUnlocker_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            await Run.FpsUnlocker();
        }

        /* 5 */
        private async void Only3DMigoto_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            await Run.Migoto();
        }

        private async void OpenGILauncher_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string giLauncher = await Utils.GetGame("giLauncher");
            if (string.IsNullOrEmpty(giLauncher))
            {
                MessageBox.Show(Resources.Default_GameLauncherWasNotFound, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Log.SaveError(string.Format(Resources.Default_GameLauncherWasNotFoundIn, giLauncher));
                return;
            }

            await Cmd.Execute(new Cmd.CliWrap { App = giLauncher });
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
            Music.PlaySound("winxp", "navigation_start");
        }

        private void Settings_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Application.OpenForms.OfType<Settings>().Any()) return;
            new Settings { DesktopLocation = DesktopLocation, Icon = Program.Ico }.Show();
            Music.PlaySound("winxp", "navigation_start");
        }

        private void ViewResources_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Cmd.Start(ResourcesPath);
        }

        private void Gameplay_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Application.OpenForms.OfType<Gameplay>().Any()) return;
            new Gameplay { DesktopLocation = DesktopLocation, Icon = Program.Ico }.Show();
            Music.PlaySound("winxp", "navigation_start");
        }

        private void Links_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Application.OpenForms.OfType<Links>().Any()) return;
            new Links { DesktopLocation = DesktopLocation, Icon = Program.Ico }.Show();
            Music.PlaySound("winxp", "navigation_start");
        }

        private void Website_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl(Program.AppWebsiteFull);
        }

        private void Version_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl("https://sefinek.net/genshin-impact-reshade/docs?page=changelog_v7");
        }

        private void MadeBySefinek_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Resources.Default_ItsJustText_WhatMoreDoYouWant, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Question);
            Utils.OpenUrl("https://www.youtube.com/watch?v=RpDf3XFHVNI");
        }

        private void CheckUpdates_Worker(object sender, EventArgs e)
        {
            CheckForUpdates.CheckUpdates_Click(sender, e);
        }

        private void W_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Music.PlaySound("winxp", "pop-up_blocked");

            if (ComputerInfo.GetSystemRegion() == "PL")
            {
                WebViewWindow viewer = new WebViewWindow { DesktopLocation = DesktopLocation, Icon = Program.Ico };
                viewer.Navigate("https://www.youtube.com/embed/2F2DdXUNyaQ?autoplay=1");
                viewer.Show();

                MessageBox.Show(@"Pamiętaj by nie grać w lola, gdyż to grzech ciężki.", "kurwa");
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
            new RandomImages { Icon = Program.Ico }.Show();
            Music.PlaySound("winxp", "navigation_start");
        }

        private void StatusLabel_TextChanged(object sender, EventArgs e)
        {
            status_Label.Visible = !string.IsNullOrEmpty(status_Label.Text);
            Music.PlaySound("winxp", "balloon");
        }
    }
}
