using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Taskbar;
using Newtonsoft.Json;
using StellaLauncher.Forms.Other;
using StellaLauncher.Models;
using StellaLauncher.Properties;
using StellaLauncher.Scripts;
using StellaLauncher.Scripts.Updates;

namespace StellaLauncher.Forms
{
    public partial class Default : Form
    {
        // Files
        private static readonly string CmdOutputLogs = Path.Combine(Program.AppData, "logs", "cmd.output.log");
        public static IniFile ReShadeIni;

        // New update?
        public static bool UpdateIsAvailable;

        // Background
        private readonly string[] _backgroundFiles =
        {
            @"nahida\1",
            @"yaoyao\1", @"yaoyao\2",
            @"ayaka\1", @"ayaka\2", @"ayaka\3", @"ayaka\4",
            @"hutao\1", @"hutao\2", @"hutao\3", @"hutao\4"
        };

        // Cache
        private readonly ObjectCache _cache = MemoryCache.Default;

        // Window
        private bool _mouseDown;
        private Point _offset;

        public Default()
        {
            int selected = Program.Settings.ReadInt("Launcher", "LanguageID", 0);
            switch (selected)
            {
                case 0:
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                    break;
                case 1:
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("pl");
                    break;
                default:
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
                    break;
            }

            InitializeComponent();
        }

        private async void Default_Load(object sender, EventArgs e)
        {
            // Path
            string mainGameDir = await Utils.GetGame("giGameDir");
            string reShadePath = Path.Combine(mainGameDir, "ReShade.ini");
            if (File.Exists(reShadePath)) ReShadeIni = new IniFile(reShadePath);

            // Background
            int bgInt = Program.Settings.ReadInt("Launcher", "Background", 0);
            if (bgInt == 0) return;

            bgInt--;

            string cacheKey = $"background_{bgInt}";
            string localization = Path.Combine(Program.AppPath, "data", "images", "launcher", "backgrounds", $"{_backgroundFiles[bgInt]}.png");
            if (!Utils.CheckFileExists(localization))
            {
                MessageBox.Show($@"Sorry. Background {_backgroundFiles[bgInt]} was not found.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Log.SaveErrorLog(new Exception($"Background file was not found in '{localization}'. ID: {bgInt}"));
                return;
            }

            Bitmap backgroundImage = new Bitmap(localization);
            _cache.Add(cacheKey, backgroundImage, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(20) });
            Log.Output($"Cached app background: '{localization}'; ID: {bgInt + 1}");

            BackgroundImage = backgroundImage;
            toolTip1.SetToolTip(changeBg_LinkLabel, $"Current background: {_backgroundFiles[bgInt]}");
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Default_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log.Output($"Closing form '{Text}'.");
        }

        private void Default_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Output("Closed.");
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
            int bgInt = Program.Settings.ReadInt("Launcher", "Background", 0);

            if (bgInt >= _backgroundFiles.Length)
            {
                bgInt = 0;
                BackgroundImage = Resources.bg_main;

                Program.Settings.WriteInt("Launcher", "Background", 0);
                Program.Settings.Save();

                toolTip1.SetToolTip(changeBg_LinkLabel, "Current background: Default");

                Log.Output($"The application background has been changed to default. ID: {bgInt}");
                return;
            }


            Bitmap backgroundImage;
            string cacheKey = $"background_{bgInt}";
            if (_cache.Contains(cacheKey))
            {
                backgroundImage = (Bitmap)_cache.Get(cacheKey);
                Log.Output($"Successfully retrieved and updated the cached app background with ID {bgInt + 1}.");
            }
            else
            {
                string localization = Path.Combine(Program.AppPath, "data", "images", "launcher", "backgrounds", $"{_backgroundFiles[bgInt]}.png");
                if (!Utils.CheckFileExists(localization))
                {
                    MessageBox.Show($@"Sorry. Background {_backgroundFiles[bgInt]} was not found.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Log.SaveErrorLog(new Exception($"Background file was not found in '{localization}'. ID: {bgInt}"));
                    return;
                }

                backgroundImage = new Bitmap(localization);
                _cache.Add(cacheKey, backgroundImage, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(20) });

                Log.Output($"Cached app background: '{localization}'; ID: {bgInt + 1}");
            }

            toolTip1.SetToolTip(changeBg_LinkLabel, $"Current background: {_backgroundFiles[bgInt]}");

            BackgroundImage = backgroundImage;
            bgInt++;

            Program.Settings.WriteInt("Launcher", "Background", bgInt);
            Program.Settings.Save();

            Log.Output($"Changed the launcher background. ID: {bgInt}");
        }

        public async Task<int> CheckUpdates()
        {
            updates_LinkLabel.LinkColor = Color.White;
            updates_LinkLabel.Text = @"Checking for updates...";

            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
            Log.Output("Checking for new versions...");

            try
            {
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", Program.UserAgent);
                string json = await client.DownloadStringTaskAsync("https://api.sefinek.net/api/v4/genshin-stella-mod/version/app/launcher");
                StellaApiVersion res = JsonConvert.DeserializeObject<StellaApiVersion>(json);

                string remoteVersion = res.Launcher.Version;
                DateTime remoteVerDate = DateTime.Parse(res.Launcher.ReleaseDate, null, DateTimeStyles.RoundtripKind).ToUniversalTime().ToLocalTime();

                // Major release
                if (Program.AppVersion[0] != remoteVersion[0])
                {
                    UpdateIsAvailable = true;

                    MajorRelease.Run(remoteVersion, remoteVerDate, version_LinkLabel, updates_LinkLabel, updateIco_PictureBox);
                    return 1;
                }

                // Normal release
                if (Program.AppVersion != remoteVersion)
                {
                    UpdateIsAvailable = true;

                    NormalRelease.Run(
                        remoteVersion, remoteVerDate, version_LinkLabel, status_Label, updates_LinkLabel, updateIco_PictureBox, progressBar1, PreparingPleaseWait, toolsIco_PictureBox, tools_LinkLabel, padIco_PictureBox, gameplay_LinkLabel,
                        shortcutIco_PictureBox,
                        links_LinkLabel, websiteIco_PictureBox,
                        website_LinkLabel, pictureBox8, pictureBox9, pictureBox10, discordServer_LinkLabel, youTube_LinkLabel, supportMe_LinkLabel
                    );

                    return 1;
                }

                // Check new updates for ReShade.ini file
                int resultInt = await ReShadeIniUpdate.Run(updates_LinkLabel, status_Label, updateIco_PictureBox, version_LinkLabel);
                switch (resultInt)
                {
                    case -2:
                        return resultInt;

                    case 1:
                    {
                        DialogResult msgReply = MessageBox.Show(
                            "Are you sure you want to update ReShade configuration?\n\nThis action will result in the loss of any custom configurations you have made. If you do not have any custom configurations, you may proceed by clicking Yes. However, if you have made changes, please ensure that you have backed up the previous ReShade file in your game files.",
                            Program.AppName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                        if (msgReply == DialogResult.No || msgReply == DialogResult.Cancel)
                        {
                            Log.Output("The update of ReShade.ini has been cancelled by the user.");
                            MessageBox.Show(
                                @"For some reason, you did not give consent for the automatic update of the ReShade file. Please note that older versions of this file may not be compatible with newer versions of Stella Mod. I hope you know what you're doing.",
                                Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                            return 1;
                        }

                        _ = Cmd.CliWrap(Utils.FirstAppLaunch, null, null, true, false);
                        Environment.Exit(0);

                        return 1;
                    }
                }


                // Not found any new updates
                updates_LinkLabel.Text = @"Check for updates";
                updateIco_PictureBox.Image = Resources.icons8_available_updates;

                Log.Output($"Not found any new updates. Your installed version: v{Program.AppVersion}");
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                return 0;
            }
            catch (Exception e)
            {
                UpdateIsAvailable = false;

                updates_LinkLabel.LinkColor = Color.Red;
                updates_LinkLabel.Text = @"Ohh, something went wrong";
                status_Label.Text += $"[x] {e.Message}\n";

                Log.SaveErrorLog(new Exception($"Something went wrong while checking for new updates.\n\n{e}"));
                TaskbarManager.Instance.SetProgressValue(100, 100);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                return -1;
            }
        }

        private async void Main_Shown(object sender, EventArgs e)
        {
            version_LinkLabel.Text = $@"v{Program.AppVersion}";
            Log.Output($"Loaded form '{Text}'.");

            if (!File.Exists(Program.FpsUnlockerExePath) && !Debugger.IsAttached)
                status_Label.Text += "[WARN]: data/unlocker/unlockfps_clr.exe was not found.\n";
            if (!File.Exists(Program.InjectorPath) && !Debugger.IsAttached)
                status_Label.Text += "[WARN]: data/reshade/inject64.exe was not found.\n";
            if (!File.Exists(Program.ReShadePath) && !Debugger.IsAttached)
                status_Label.Text += "[WARN]: data/reshade/ReShade64.dll was not found.\n";
            if (!File.Exists(Program.FpsUnlockerCfgPath) && !Debugger.IsAttached)
            {
                status_Label.Text += "[i] Downloading config file for FPS Unlocker...\n";
                Log.Output("Downloading config file for FPS Unlocker...");

                try
                {
                    WebClient client = new WebClient();
                    client.Headers.Add("user-agent", Program.UserAgent);
                    await client.DownloadFileTaskAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/unlocker.config.json", Program.FpsUnlockerCfgPath);

                    string fpsUnlockerCfg = File.ReadAllText(Program.FpsUnlockerCfgPath);
                    File.WriteAllText(Program.FpsUnlockerCfgPath, fpsUnlockerCfg.Replace("{GamePath}", @"C:\\Program Files\\Genshin Impact\\Genshin Impact game\\GenshinImpact.exe"));

                    status_Label.Text += "[✓] Success!\n";
                    Log.Output("Done.");
                }
                catch (Exception ex)
                {
                    status_Label.Text += $"[✖] {ex.Message}\n";
                    Log.SaveErrorLog(new Exception($"Failed to download unlocker.config.json.\n{ex}"));
                }
            }

            if (status_Label.Text.Length > 0) Log.SaveErrorLog(new Exception(status_Label.Text));

            if (File.Exists(NormalRelease.SetupPathExe))
            {
                File.Delete(NormalRelease.SetupPathExe);
                status_Label.Text += "[i] Deleted old setup file from temp directory.\n";
                Log.Output($"Deleted old setup file from temp folder: {NormalRelease.SetupPathExe}");
            }

            await CheckUpdates();

            int data = Program.Settings.ReadInt("Launcher", "DiscordRPC", 1);
            if (data == 1) Discord.InitRpc();

            if (Debugger.IsAttached) return;
            Telemetry.Opened();

            // Music
            int muteBgMusic = Program.Settings.ReadInt("Launcher", "EnableMusic", 1);
            if (muteBgMusic == 0) return;

            Random random = new Random();
            string wavPath = Path.Combine(Program.AppPath, "data", "sounds", "bg", $"{random.Next(1, 6 + 1)}.wav");
            if (!File.Exists(wavPath))
            {
                status_Label.Text += "[x]: Background music was not found.\n";
                Log.SaveErrorLog(new Exception($"Background music file '{wavPath}' was not found."));
                return;
            }

            try
            {
                new SoundPlayer { SoundLocation = wavPath }.Play();
                Log.Output($"Playing: {wavPath}");
            }
            catch (Exception ex)
            {
                status_Label.Text += $"[x]: {ex.Message}\n";
                Log.SaveErrorLog(ex);
            }

            int firstMsgBox = Program.Settings.ReadInt("Launcher", "FirstMsgBox", 1);
            if (firstMsgBox != 1) return;

            MessageBox.Show(
                "It appears that this is your first time launching the launcher! Take some time to review the terms of use on the GitHub Wiki for the mod and the rules to avoid any unexpected issues.\n\nREMEMBER NOT TO SHARE YOUR UID WITH VISIBLE GAME SHADERS WITH ANYONE!",
                Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Program.Settings.WriteInt("Launcher", "FirstMsgBox", 0);
            status_Label.Text += "[i] Click 'Start game' button to inject ReShade and use FPS Unlock.\n";
        }

        // ------- Body -------
        private void GitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl("https://github.com/sefinek24/Genshin-Impact-ReShade");
        }

        // ------- Start the game -------
        // 1 = ReShade + FPS Unlocker
        // 2 = Only ReShade
        // 3 = Only FPS Unlocker
        private async void StartGame_Click(object sender, EventArgs e)
        {
            // Run cmd file
            bool res = await Cmd.CliWrap("wt.exe", $"{Path.Combine(Program.AppPath, "data", "cmd", "run.cmd")} 1 {await Utils.GetGameVersion()} \"{CmdOutputLogs}\"", Program.AppPath, false, false);

            // Exit Stella with status code 0
            if (res) Environment.Exit(0);
        }

        private async void OnlyReShade_Click(object sender, EventArgs e)
        {
            // Run cmd file
            bool res = await Cmd.CliWrap("wt.exe", $"{Path.Combine(Program.AppPath, "data", "cmd", "run.cmd")} 2 {await Utils.GetGameVersion()} \"{CmdOutputLogs}\"", Program.AppPath, false, false);
            if (!res) return;

            // Find game path
            string path = await Utils.GetGame("giLauncher");
            if (path == null) return;

            // Open Genshin Launcher
            _ = Cmd.CliWrap(path, null, null, true, false);

            // Exit Stella with status code 0
            Environment.Exit(0);
        }

        private async void OnlyUnlocker_Click(object sender, EventArgs e)
        {
            // Run cmd file
            bool res = await Cmd.CliWrap("wt.exe", $"{Path.Combine(Program.AppPath, "data", "cmd", "run.cmd")} 3 {await Utils.GetGameVersion()} \"{CmdOutputLogs}\"", Program.AppPath, false, false);

            // Exit Stella with status code 0
            if (res) Environment.Exit(0);
        }

        private async void OpenGILauncher_Click(object sender, EventArgs e)
        {
            string path = await Utils.GetGame("giLauncher");
            if (path == string.Empty)
            {
                MessageBox.Show(@"Game launcher was not found.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Log.SaveErrorLog(new Exception($"Game launcher was not found. Result: {path}"));
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
            new Tools { Location = Location, Icon = Resources.icon_52x52 }.Show();
        }

        private void Gameplay_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Application.OpenForms.OfType<Gameplay>().Any()) return;
            new Gameplay { Location = Location, Icon = Resources.icon_52x52 }.Show();
        }

        private void Links_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Application.OpenForms.OfType<Links>().Any()) return;
            new Links { Location = Location, Icon = Resources.icon_52x52 }.Show();
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
            MessageBox.Show(@"It's just text. What more do you want?", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Utils.OpenUrl("https://www.youtube.com/watch?v=RpDf3XFHVNI");
        }

        public async void CheckUpdates_Click(object sender, EventArgs e)
        {
            int update = await CheckUpdates();
            if (update == -2 || update == -3)
            {
                DialogResult msgBoxResult = MessageBox.Show(
                    "The ReShade.ini file could not be located in your game files, or it may not be compatible with the current version.\n\nWould you like to download this file now to prevent future errors and manual configuration? Recommended.",
                    Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                string gameDir = await Utils.GetGame("giGameDir");
                string reShadePath = Path.Combine(gameDir, "ReShade.ini");

                switch (msgBoxResult)
                {
                    case DialogResult.Yes:
                        try
                        {
                            updates_LinkLabel.LinkColor = Color.DodgerBlue;
                            updates_LinkLabel.Text = @"Downloading...";
                            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);

                            WebClient client = new WebClient();
                            client.Headers.Add("user-agent", Program.UserAgent);
                            await client.DownloadFileTaskAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/reshade/ReShade.ini", reShadePath);

                            if (File.Exists(reShadePath))
                            {
                                status_Label.Text += "[✓] Successfully downloaded ReShade.ini!\n";
                                Log.Output($"Successfully downloaded ReShade.ini and saved in: {reShadePath}");

                                await CheckUpdates();
                            }
                            else
                            {
                                status_Label.Text += "[x] File was not found.\n";
                                Log.SaveErrorLog(new Exception($"Downloaded ReShade.ini was not found in: {reShadePath}"));

                                TaskbarManager.Instance.SetProgressValue(100, 100);
                                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            status_Label.Text += "[x] Meeow! Failed to download ReShade.ini. Try again.\n";
                            updates_LinkLabel.LinkColor = Color.Red;
                            updates_LinkLabel.Text = @"Failed to download...";

                            Log.SaveErrorLog(ex);
                            if (!File.Exists(reShadePath)) Log.Output("The ReShade.ini file still does not exist!");
                            TaskbarManager.Instance.SetProgressValue(100, 100);
                            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                        }

                        break;
                    case DialogResult.No:
                        status_Label.Text += "[i] Canceled by the user. Are you sure of what you're doing?\n";
                        Log.Output("File download has been canceled by the user.");

                        if (!File.Exists(reShadePath)) Log.Output("The ReShade.ini file does not exist.");

                        TaskbarManager.Instance.SetProgressValue(100, 100);
                        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
                        break;
                }

                return;
            }

            if (update != 0) return;

            updates_LinkLabel.LinkColor = Color.LawnGreen;
            updates_LinkLabel.Text = @"You have the latest version";
            updateIco_PictureBox.Image = Resources.icons8_available_updates;
        }

        private void W_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Os.RegionCode == "PL")
            {
                Utils.OpenUrl("https://www.youtube.com/watch?v=2F2DdXUNyaQ");
                MessageBox.Show(@"Pamiętaj by nie grać w lola, gdyż to grzech ciężki.");
            }
            else
            {
                Utils.OpenUrl("https://www.youtube.com/watch?v=L3ky4gZU5gY");
            }
        }

        private void Paimon_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<RandomImages>().Any()) return;
            new RandomImages { Location = Location, Icon = Resources.icon_52x52 }.Show();
        }
    }
}
