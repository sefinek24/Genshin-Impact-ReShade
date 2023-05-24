using System;
using System.Collections.Specialized;
using System.ComponentModel;
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
using ByteSizeLib;
using Genshin_Stella_Mod.Forms.Errors;
using Genshin_Stella_Mod.Forms.Other;
using Genshin_Stella_Mod.Models;
using Genshin_Stella_Mod.Properties;
using Genshin_Stella_Mod.Scripts;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.WindowsAPICodePack.Taskbar;
using Newtonsoft.Json;

// 1 = ReShade + FPS Unlocker
// 2 = ReShade
// 3 = FPS Unlocker

namespace Genshin_Stella_Mod.Forms
{
    public partial class Default : Form
    {
        // Files
        private static readonly string SetupPathExe = Path.Combine(Path.GetTempPath(), "Stella-Mod-Update.exe");
        private static readonly string CmdOutputLogs = Path.Combine(Program.AppData, "logs", "cmd.output.log");
        private static IniFile _reShadeIni;

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
        private double _downloadSpeed;
        private long _lastBytesReceived;
        private DateTime _lastUpdateTime = DateTime.Now;

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

        private void Default_Load(object sender, EventArgs e)
        {
            // Path
            string reShadePath = Path.Combine(Utils.GetGame("giGameDir"), "ReShade.ini");
            if (File.Exists(reShadePath)) _reShadeIni = new IniFile(reShadePath);

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
            toolTip1.SetToolTip(ChangeBackground, $"Current background: {_backgroundFiles[bgInt]}");
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

                toolTip1.SetToolTip(ChangeBackground, "Current background: Default");

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

            toolTip1.SetToolTip(ChangeBackground, $"Current background: {_backgroundFiles[bgInt]}");

            BackgroundImage = backgroundImage;
            bgInt++;

            Program.Settings.WriteInt("Launcher", "Background", bgInt);
            Program.Settings.Save();

            Log.Output($"Changed the launcher background. ID: {bgInt}");
        }


        private async Task<int> CheckUpdates()
        {
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);

            updates_Label.LinkColor = Color.White;
            updates_Label.Text = @"Checking for updates...";
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
                    version_Label.Text = $@"v{Program.AppVersion} → v{remoteVersion}";
                    updates_Label.LinkColor = Color.Cyan;
                    updates_Label.Text = @"Major version is available";
                    update_Icon.Image = Resources.icons8_download_from_the_cloud;
                    Log.Output($"New major version from {remoteVerDate} is available: v{Program.AppVersion} → v{remoteVersion}");

                    UpdateIsAvailable = true;

                    TaskbarManager.Instance.SetProgressValue(100, 100);
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
                    new NotCompatible { Icon = Resources.icon_52x52 }.ShowDialog();

                    Environment.Exit(0);
                }

                // Normal release
                if (Program.AppVersion != remoteVersion)
                {
                    // 1
                    UpdateIsAvailable = true;
                    version_Label.Text = $@"v{Program.AppVersion} → v{remoteVersion}";

                    // 2
                    updates_Label.LinkColor = Color.Cyan;
                    updates_Label.Text = @"Click here to update";
                    update_Icon.Image = Resources.icons8_download_from_the_cloud;
                    Utils.RemoveClickEvent(updates_Label);
                    updates_Label.Click += Update_Event;

                    // ToastContentBuilder
                    try
                    {
                        new ToastContentBuilder()
                            .AddText("📥 We found new updates")
                            .AddText("New release is available. Download now!")
                            .Show();
                    }
                    catch (Exception e)
                    {
                        Log.SaveErrorLog(e);
                    }

                    // WebClient
                    WebClient wc = new WebClient();
                    wc.Headers.Add("user-agent", Program.UserAgent);
                    await wc.OpenReadTaskAsync("https://github.com/sefinek24/Genshin-Impact-ReShade/releases/latest/download/Stella-Mod-Setup.exe");
                    string updateSize = ByteSize.FromBytes(Convert.ToInt64(wc.ResponseHeaders["Content-Length"])).MegaBytes.ToString("00.00");
                    status_Label.Text += $"[i] New version from {remoteVerDate} is available.\n[i] Update size: {updateSize} MB\n";

                    Log.Output($"New release from {remoteVerDate} is available: v{Program.AppVersion} → v{remoteVersion} [{updateSize} MB]");

                    // Hide and show elements
                    progressBar1.Hide();
                    PreparingPleaseWait.Hide();
                    PreparingPleaseWait.Text = @"Preparing... If process is stuck, reopen launcher.";
                    pictureBox3.Show();
                    settings_Label.Show();
                    pictureBox6.Show();
                    createShortcut_Label.Show();
                    pictureBox11.Show();
                    linkLabel5.Show();
                    pictureBox4.Show();
                    website_Label.Show();
                    progressBar1.Value = 0;

                    // TaskbarManager
                    TaskbarManager.Instance.SetProgressValue(100, 100);
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
                    return 1;
                }


                string reShadePath = Path.Combine(Utils.GetGame("giGameDir"), "ReShade.ini");
                // Check new updates for ReShade.ini file
                if (!File.Exists(reShadePath))
                {
                    UpdateIsAvailable = false;

                    updates_Label.LinkColor = Color.OrangeRed;
                    updates_Label.Text = @"Download the required file";
                    status_Label.Text += "[x] File ReShade.ini was not found in your game directory.\n";
                    update_Icon.Image = Resources.icons8_download_from_the_cloud;

                    TaskbarManager.Instance.SetProgressValue(100, 100);
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
                    Log.Output($"ReShade.ini was not found in: {reShadePath}");
                    return -2;
                }


                WebClient webClient = new WebClient();
                webClient.Headers.Add("user-agent", Program.UserAgent);
                string content = await webClient.DownloadStringTaskAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/reshade/ReShade.ini");
                NameValueCollection iniData = new NameValueCollection();
                using (StringReader reader = new StringReader(content))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                        if (line.Contains("="))
                        {
                            int separatorIndex = line.IndexOf("=", StringComparison.Ordinal);
                            string key = line.Substring(0, separatorIndex).Trim();
                            string value = line.Substring(separatorIndex + 1).Trim();
                            iniData.Add(key, value);
                        }
                }

                string localIniVersion = _reShadeIni.ReadString("STELLA", "ConfigVersion", null);
                if (localIniVersion == null || localIniVersion.Length <= 0)
                {
                    UpdateIsAvailable = false;

                    updates_Label.LinkColor = Color.Cyan;
                    updates_Label.Text = @"Download the required file";
                    status_Label.Text += "[x] The version of ReShade config was not found.\n";
                    update_Icon.Image = Resources.icons8_download_from_the_cloud;

                    Log.Output("STELLA.ConfigVersion is null in ReShade.ini.");
                    TaskbarManager.Instance.SetProgressValue(100, 100);
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                    return -2;
                }

                string remoteIniVersion = iniData["ConfigVersion"];
                if (localIniVersion != remoteIniVersion)
                {
                    UpdateIsAvailable = true;

                    updates_Label.LinkColor = Color.DodgerBlue;
                    updates_Label.Text = @"Update ReShade config";
                    update_Icon.Image = Resources.icons8_download_from_the_cloud;
                    version_Label.Text = $@"v{localIniVersion} → v{remoteIniVersion}";
                    status_Label.Text += "[i] New ReShade config version is available. Update is required.\n";
                    Log.Output($"New ReShade config version is available: v{localIniVersion} → v{remoteIniVersion}");

                    WebClient wc = new WebClient();
                    wc.Headers.Add("user-agent", Program.UserAgent);
                    await wc.OpenReadTaskAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/reshade/ReShade.ini");
                    string updateSize = ByteSize.FromBytes(Convert.ToInt64(wc.ResponseHeaders["Content-Length"])).KiloBytes.ToString("0.00");
                    status_Label.Text += $"[i] Update size: {updateSize} KB\n";

                    Utils.RemoveClickEvent(updates_Label);
                    updates_Label.Click += UpdateConfig_Event;

                    Log.Output($"Update size: {updateSize} KB");
                    TaskbarManager.Instance.SetProgressValue(100, 100);
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
                    return 1;
                }

                // Not found any new updates
                updates_Label.Text = @"Check for updates";
                update_Icon.Image = Resources.icons8_available_updates;

                Log.Output($"Not found any new updates. Your installed version: v{Program.AppVersion}");
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                return 0;
            }
            catch (Exception e)
            {
                UpdateIsAvailable = false;

                updates_Label.LinkColor = Color.Red;
                updates_Label.Text = @"Ohh, something went wrong";
                status_Label.Text += $"[x] {e.Message}\n";

                Log.SaveErrorLog(new Exception($"Something went wrong while checking for new updates.\n\n{e}"));
                TaskbarManager.Instance.SetProgressValue(100, 100);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                return -1;
            }
        }

        private async void UpdateConfig_Event(object sender, EventArgs e)
        {
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);

            DialogResult msgReply = MessageBox.Show(
                "Are you sure you want to update ReShade configuration?\n\nThis action will result in the loss of any custom configurations you have made. If you do not have any custom configurations, you may proceed by clicking Yes. However, if you have made changes, please ensure that you have backed up the previous ReShade file in your game files.",
                Program.AppName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (msgReply == DialogResult.No || msgReply == DialogResult.Cancel)
            {
                Log.Output("The update of ReShade.ini has been cancelled by the user.");
                MessageBox.Show(
                    @"For some reason, you did not give consent for the automatic update of the ReShade file. Please note that older versions of this file may not be compatible with newer versions of Stella Mod. I hope you know what you're doing.",
                    Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            Log.Output("Downloading ReShade.ini...");

            try
            {
                string reShadePath = Path.Combine(Utils.GetGame("giGameDir"), "ReShade.ini");

                WebClient client = new WebClient();
                client.Headers.Add("user-agent", Program.UserAgent);
                await client.DownloadFileTaskAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/reshade/ReShade.ini", reShadePath);

                int update = await CheckUpdates();
                if (update == 0)
                {
                    Utils.RemoveClickEvent(updates_Label);
                    updates_Label.Click += CheckUpdates_Click;

                    status_Label.Text += "[✓] ReShade.ini was successfully updated to the latest version!\n";
                    updates_Label.LinkColor = Color.LimeGreen;
                    updates_Label.Text = @"Nya~~! Successfully!";
                    update_Icon.Image = Resources.icons8_available_updates;

                    UpdateIsAvailable = false;
                    Log.Output("Done.");
                }
                else
                {
                    UpdateIsAvailable = true;

                    status_Label.Text += "[x] Something went wrong. Sad cat...\n";
                    updates_Label.LinkColor = Color.Red;
                    updates_Label.Text = @"Failed to update...";
                    update_Icon.Image = Resources.icons8_download_from_the_cloud;

                    TaskbarManager.Instance.SetProgressValue(100, 100);
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                    Log.Output("Failed to update ReShade.ini.");
                }
            }
            catch (Exception ex)
            {
                UpdateIsAvailable = false;
                status_Label.Text += $"[x] {ex.Message}\n";

                TaskbarManager.Instance.SetProgressValue(100, 100);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                Log.SaveErrorLog(new Exception($"Failed to download ReShade.ini!\n{ex}"));
            }
        }

        private async void Main_Shown(object sender, EventArgs e)
        {
            version_Label.Text = $@"v{Program.AppVersion}";
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

            if (File.Exists(SetupPathExe))
            {
                File.Delete(SetupPathExe);
                status_Label.Text += "[i] Deleted old setup file from temp directory.\n";
                Log.Output($"Deleted old setup file from temp folder: {SetupPathExe}");
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
        private async void StartGame_Click(object sender, EventArgs e)
        {
            // Run cmd file
            await Cmd.CliWrap("wt.exe", $"{Path.Combine(Program.AppPath, "data", "cmd", "run.cmd")} 1 {Utils.GetGameVersion()} \"{CmdOutputLogs}\"", Program.AppPath, false, false);

            // Exit Stella with status code 0
            Environment.Exit(0);
        }

        private async void OnlyReShade_Click(object sender, EventArgs e)
        {
            // Find game path
            string path = Utils.GetGame("giLauncher");
            if (path == null) return;

            // Open Genshin Launcher
            _ = Cmd.CliWrap(path, null, null, true, false);

            // Run cmd file
            await Cmd.CliWrap("wt.exe", $"{Path.Combine(Program.AppPath, "data", "cmd", "run.cmd")} 2 {Utils.GetGameVersion()} \"{CmdOutputLogs}\"", Program.AppPath, false, false);

            // Exit Stella with status code 0
            Environment.Exit(0);
        }

        private async void OnlyUnlocker_Click(object sender, EventArgs e)
        {
            // Run cmd file
            await Cmd.CliWrap("wt.exe", $"{Path.Combine(Program.AppPath, "data", "cmd", "run.cmd")} 3 {Utils.GetGameVersion()} \"{CmdOutputLogs}\"", Program.AppPath, false, false);

            // Exit Stella with status code 0
            Environment.Exit(0);
        }

        private async void OpenGILauncher_Click(object sender, EventArgs e)
        {
            string path = Utils.GetGame("giLauncher");
            if (path == null) return;

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

        private async void CheckUpdates_Click(object sender, EventArgs e)
        {
            // The integer value returned from the CheckUpdates() function:
            // -3 = The file ReShade.ini does not exist in the player's game files.
            // -2 = The file ReShade.ini does not contain a reference to the version in the properties.
            // -1 = An error occurred.
            // 0  = No new updates found.
            // 1  = Found a minor update.

            int update = await CheckUpdates();
            if (update == -2 || update == -3)
            {
                DialogResult msgBoxResult = MessageBox.Show(
                    "The ReShade.ini file could not be located in your game files, or it may not be compatible with the current version.\n\nWould you like to download this file now to prevent future errors and manual configuration? Recommended.",
                    Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                string reShadePath = Path.Combine(Utils.GetGame("giGameDir"), "ReShade.ini");

                switch (msgBoxResult)
                {
                    case DialogResult.Yes:
                        try
                        {
                            updates_Label.LinkColor = Color.DodgerBlue;
                            updates_Label.Text = "Downloading...";
                            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);

                            WebClient client = new WebClient();
                            client.Headers.Add("user-agent", Program.UserAgent);
                            await client.DownloadFileTaskAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/reshade/ReShade.ini", reShadePath);

                            if (File.Exists(reShadePath))
                            {
                                await CheckUpdates();

                                status_Label.Text += "[✓] Successfully downloaded ReShade.ini!\n";
                                Log.Output($"Successfully downloaded ReShade.ini and saved in: {reShadePath}");
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
                            updates_Label.LinkColor = Color.Red;
                            updates_Label.Text = "Failed to download...";

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

            updates_Label.LinkColor = Color.LawnGreen;
            updates_Label.Text = @"You have the latest version";
            update_Icon.Image = Resources.icons8_available_updates;
        }

        private async void Update_Event(object sender, EventArgs e)
        {
            Log.Output("Preparing to download new update...");
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);

            updates_Label.LinkColor = Color.DodgerBlue;
            updates_Label.Text = @"Updating. Please wait...";
            Utils.RemoveClickEvent(updates_Label);

            progressBar1.Show();
            PreparingPleaseWait.Show();

            pictureBox3.Hide();
            settings_Label.Hide();
            pictureBox6.Hide();
            createShortcut_Label.Hide();
            pictureBox11.Hide();
            linkLabel5.Hide();
            pictureBox4.Hide();
            website_Label.Hide();

            try
            {
                Log.Output("Starting...");
                await StartDownload();
            }
            catch (Exception ex)
            {
                PreparingPleaseWait.Text = @"😥 Something went wrong???";
                Log.ThrowError(ex);
            }

            Log.Output($"Output: {SetupPathExe}");
        }

        private async Task StartDownload()
        {
            if (File.Exists(SetupPathExe))
            {
                File.Delete(SetupPathExe);
                status_Label.Text += "[✓] Deleted old setup file from temp directory.\n";
                Log.Output($"Deleted od setup file from: {SetupPathExe}");
            }

            Log.Output("Downloading in progress...");
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("user-agent", Program.UserAgent);
                client.DownloadProgressChanged += Client_DownloadProgressChanged;
                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                await client.DownloadFileTaskAsync(new Uri("https://github.com/sefinek24/Genshin-Impact-ReShade/releases/latest/download/Stella-Mod-Setup.exe"), SetupPathExe);
            }
        }

        private async void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            int progress = (int)Math.Floor(e.BytesReceived * 100.0 / e.TotalBytesToReceive);
            progressBar1.Value = progress;
            TaskbarManager.Instance.SetProgressValue(progress, 100);

            DateTime currentTime = DateTime.Now;
            TimeSpan elapsedTime = currentTime - _lastUpdateTime;
            long bytesReceived = e.BytesReceived - _lastBytesReceived;

            if (!(elapsedTime.TotalMilliseconds > 1000)) return;

            _lastUpdateTime = currentTime;
            _lastBytesReceived = e.BytesReceived;

            double bytesReceivedMb = ByteSize.FromBytes(e.BytesReceived).MegaBytes;
            double bytesReceiveMb = ByteSize.FromBytes(e.TotalBytesToReceive).MegaBytes;
            PreparingPleaseWait.Text = $@"Downloading... {bytesReceivedMb:00.00} MB of {bytesReceiveMb:000.00} MB";

            _downloadSpeed = bytesReceived / elapsedTime.TotalSeconds;
            double downloadSpeedInMb = _downloadSpeed / (1024 * 1024);
            PreparingPleaseWait.Text += $@" [{downloadSpeedInMb:00.00} MB/s]";

            Log.Output($"Downloading new update... {bytesReceivedMb:000.00} MB of {bytesReceiveMb:000.00} MB / {downloadSpeedInMb:00.00} MB/s");
            await Task.Delay(1000);
        }


        private async void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string logDir = Path.Combine(Log.Folder, "updates");
            if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);

            for (int i = 4; i > 0; i--)
            {
                PreparingPleaseWait.Text = $@"Just a moment. Please wait {i}s...";
                Log.Output($"Waiting {i}s...");
                await Task.Delay(1000);
            }

            int numFilesDeleted = 0;
            long spaceSaved = 0;
            DirectoryInfo cacheDir = new DirectoryInfo(Path.Combine(Program.AppPath, "data", "reshade", "cache"));
            if (cacheDir.Exists)
                foreach (FileInfo file in cacheDir.EnumerateFiles())
                    if (file.Name != "null")
                    {
                        spaceSaved += file.Length;
                        file.Delete();
                        numFilesDeleted++;
                    }

            status_Label.Text += $@"[✓] Deleted {numFilesDeleted} cache files and saved {spaceSaved / 1000000} MB.";
            PreparingPleaseWait.Text = @"Everything is okay! Starting setup...";

            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
            _ = Cmd.CliWrap(SetupPathExe, $"/UPDATE /NORESTART /LOG=\"{logDir}\\{DateTime.Now:yyyy-dd-M...HH-mm-ss}.log\"", null, true, true);

            pictureBox9.Visible = false;
            DiscordServer.Visible = false;
            pictureBox10.Visible = false;
            SupportMe.Visible = false;
            pictureBox8.Visible = false;
            YouTube.Visible = false;

            for (int i = 20; i > 0; i--)
            {
                PreparingPleaseWait.Text = $@"Install a new version in the wizard. Closing launcher in {i}s...";
                Log.Output($"Closing launcher in {i}s...");
                await Task.Delay(1000);
            }

            Log.Output("Closing...");
            Environment.Exit(0);
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
            if (Application.OpenForms.OfType<RandomThings>().Any()) return;
            new RandomThings { Location = Location, Icon = Resources.icon_52x52 }.Show();
        }
    }
}
