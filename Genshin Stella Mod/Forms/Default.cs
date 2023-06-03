using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Runtime.Caching;
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
                MessageBox.Show(string.Format(Resources.Default_Sorry_Background_WasNotFound, _backgroundFiles[bgInt]), Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Log.SaveErrorLog(new Exception(string.Format(Resources.Default_Sorry_Background_WasNotFound, localization, bgInt)));
                return;
            }

            Bitmap backgroundImage = new Bitmap(localization);
            _cache.Add(cacheKey, backgroundImage, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(20) });
            Log.Output(string.Format(Resources.Default_CachedAppBackground_ID, localization, bgInt + 1));

            BackgroundImage = backgroundImage;
            toolTip1.SetToolTip(changeBg_LinkLabel, string.Format(Resources.Default_CurrentBackground, _backgroundFiles[bgInt]));
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
            int bgInt = Program.Settings.ReadInt("Launcher", "Background", 0);

            if (bgInt >= _backgroundFiles.Length)
            {
                bgInt = 0;
                BackgroundImage = Resources.bg_main;

                Program.Settings.WriteInt("Launcher", "Background", 0);
                Program.Settings.Save();

                toolTip1.SetToolTip(changeBg_LinkLabel, Resources.Default_CurrentBackground_Default);

                Log.Output(string.Format(Resources.Default_TheApplicationBackgroundHasBeenChangedToDefault_ID, bgInt.ToString()));
                return;
            }


            Bitmap backgroundImage;
            string cacheKey = $"background_{bgInt}";
            if (_cache.Contains(cacheKey))
            {
                backgroundImage = (Bitmap)_cache.Get(cacheKey);
                Log.Output(string.Format(Resources.Default_SuccessfullyRetrievedAndUpdatedTheCachedAppBackgroundWithID_, bgInt + 1));
            }
            else
            {
                string localization = Path.Combine(Program.AppPath, "data", "images", "launcher", "backgrounds", $"{_backgroundFiles[bgInt]}.png");
                if (!Utils.CheckFileExists(localization))
                {
                    MessageBox.Show(string.Format(Resources.Default_Sorry_Background_WasNotFound, _backgroundFiles[bgInt]), Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Log.SaveErrorLog(new Exception(string.Format(Resources.Default_Sorry_Background_WasNotFound, localization, bgInt)));
                    return;
                }

                backgroundImage = new Bitmap(localization);
                _cache.Add(cacheKey, backgroundImage, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(20) });

                Log.Output(string.Format(Resources.Default_CachedAppBackground_ID, localization, bgInt + 1));
            }

            toolTip1.SetToolTip(changeBg_LinkLabel, string.Format(Resources.Default_CurrentBackground, _backgroundFiles[bgInt]));

            BackgroundImage = backgroundImage;
            bgInt++;
            Program.Settings.WriteInt("Launcher", "Background", bgInt);
            Program.Settings.Save();

            Log.Output(string.Format(Resources.Default_ChangedTheLauncherBackground_ID, bgInt));
        }

        public async Task<int> CheckUpdates()
        {
            updates_LinkLabel.LinkColor = Color.White;
            updates_LinkLabel.Text = Resources.Default_CheckingForUpdates;

            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
            Log.Output(Resources.Default_CheckingForUpdates);

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
                        DialogResult msgReply = MessageBox.Show(Resources.Default_AreYouSureWantToUpdateReShadeConfiguration, Program.AppName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                        if (msgReply == DialogResult.No || msgReply == DialogResult.Cancel)
                        {
                            Log.Output(Resources.Default_TheUpdateOfReShadIniHasBeenCanceledByTheUser);
                            MessageBox.Show(Resources.Default_ForSomeReasonYouDidNotGiveConsentForTheAutomaticUpdateOfTheReShadeFile, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                            return 1;
                        }

                        _ = Cmd.CliWrap(Utils.FirstAppLaunch, null, null, true, false);
                        Environment.Exit(0);

                        return 1;
                    }
                }


                // Not found any new updates
                updates_LinkLabel.Text = Resources.Default_CheckForUpdates;
                updateIco_PictureBox.Image = Resources.icons8_available_updates;

                Log.Output(string.Format(Resources.Default_NotFoundAnyNewUpdates_YourInstalledVersion_, Program.AppVersion));
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                return 0;
            }
            catch (Exception e)
            {
                UpdateIsAvailable = false;

                updates_LinkLabel.LinkColor = Color.Red;
                updates_LinkLabel.Text = Resources.Default_OhhSomethingWentWrong;
                status_Label.Text += $"[x] {e.Message}\n";

                Log.SaveErrorLog(new Exception(string.Format(Resources.Default_SomethingWentWrongWhileCheckingForNewUpdates, e)));
                TaskbarManager.Instance.SetProgressValue(100, 100);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                return -1;
            }
        }

        private async void Main_Shown(object sender, EventArgs e)
        {
            version_LinkLabel.Text = $@"v{Program.AppVersion}";
            Log.Output(string.Format(Resources.Main_LoadedForm_, Text));

            if (!File.Exists(Program.FpsUnlockerExePath) && !Debugger.IsAttached)
                status_Label.Text += $"[x]: {string.Format(Resources.Default_File_WasNotFound, Program.FpsUnlockerExePath)}\n";
                
            if (!File.Exists(Program.InjectorPath) && !Debugger.IsAttached)
                status_Label.Text += $"[x]: {string.Format(Resources.Default_File_WasNotFound, Program.InjectorPath)}\n";

            if (!File.Exists(Program.ReShadePath) && !Debugger.IsAttached)
                status_Label.Text += $"[x]: {string.Format(Resources.Default_File_WasNotFound, Program.ReShadePath)}\n";

            if (!File.Exists(Program.FpsUnlockerCfgPath) && !Debugger.IsAttached)
            {
                status_Label.Text += $"[i] {Resources.Default_DownloadingConfigFileForFPSUnlocker}\n";
                Log.Output(Resources.Default_DownloadingConfigFileForFPSUnlocker);

                try
                {
                    WebClient client = new WebClient();
                    client.Headers.Add("user-agent", Program.UserAgent);
                    await client.DownloadFileTaskAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/unlocker.config.json", Program.FpsUnlockerCfgPath);

                    string fpsUnlockerCfg = File.ReadAllText(Program.FpsUnlockerCfgPath);
                    File.WriteAllText(Program.FpsUnlockerCfgPath, fpsUnlockerCfg.Replace("{GamePath}", @"C:\\Program Files\\Genshin Impact\\Genshin Impact game\\GenshinImpact.exe"));

                    status_Label.Text += $"[✓] {Resources.Default_Success}\n";
                    Log.Output("Done.");
                }
                catch (Exception ex)
                {
                    status_Label.Text += $"[✖] {ex.Message}\n";
                    Log.SaveErrorLog(new Exception($"{Resources.Default_FailedToDownloadUnlockerConfigJson}\n{ex}"));
                }
            }

            if (status_Label.Text.Length > 0) Log.SaveErrorLog(new Exception(status_Label.Text));

            if (File.Exists(NormalRelease.SetupPathExe))
            {
                File.Delete(NormalRelease.SetupPathExe);
                status_Label.Text += $"[i] {Resources.Default_DeletedOldSetupFromTempDirectory}\n";
                Log.Output(string.Format(Resources.Default_DeletedOldSetupFromTempFolder, NormalRelease.SetupPathExe));
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
                status_Label.Text += $"[x]: {Resources.Default_TheSoundFileWithMusicWasNotFound}\n";
                Log.SaveErrorLog(new Exception(string.Format(Resources.Default_TheSoundFileWithMusicWasNotFoundInTheLocalization_, wavPath)));
                return;
            }

            try
            {
                new SoundPlayer { SoundLocation = wavPath }.Play();
                Log.Output(string.Format(Resources.Default_PlayingSoundFile_, wavPath));
            }
            catch (Exception ex)
            {
                status_Label.Text += $"[x]: {ex.Message}\n";
                Log.SaveErrorLog(ex);
            }

            int firstMsgBox = Program.Settings.ReadInt("Launcher", "FirstMsgBox", 1);
            if (firstMsgBox != 1) return;

            MessageBox.Show(Resources.Default_ItAppersThatIsYourFirstTimeLaunchingTheLauncher, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Program.Settings.WriteInt("Launcher", "FirstMsgBox", 0);
            status_Label.Text += $"[i] {Resources.Default_ClickStartGameButtonToInjectReShadeAndUseFPSUnlock}\n";
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
                MessageBox.Show(Resources.Default_GameLauncherWasNotFound, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Log.SaveErrorLog(new Exception(string.Format(Resources.Default_GameLauncherWasNotFoundIn, path)));
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
            MessageBox.Show(Resources.Default_ItsJustText_WhatMoreDoYouWant, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Question);
            Utils.OpenUrl("https://www.youtube.com/watch?v=RpDf3XFHVNI");
        }

        public async void CheckUpdates_Click(object sender, EventArgs e)
        {
            int update = await CheckUpdates();
            if (update == -2 || update == -3)
            {
                DialogResult msgBoxResult = MessageBox.Show(Resources.Default_TheReShadeIniFileCouldNotBeLocatedInYourGameFiles, Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                string gameDir = await Utils.GetGame("giGameDir");
                string reShadePath = Path.Combine(gameDir, "ReShade.ini");

                switch (msgBoxResult)
                {
                    case DialogResult.Yes:
                        try
                        {
                            updates_LinkLabel.LinkColor = Color.DodgerBlue;
                            updates_LinkLabel.Text = Resources.Default_Downloading;
                            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);

                            WebClient client = new WebClient();
                            client.Headers.Add("user-agent", Program.UserAgent);
                            await client.DownloadFileTaskAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/reshade/ReShade.ini", reShadePath);

                            if (File.Exists(reShadePath))
                            {
                                status_Label.Text += $"[✓] {Resources.Default_SuccessfullyDownloadedReShadeIni}\n";
                                Log.Output(string.Format(Resources.Default_SuccessfullyDownloadedReShadeIniAndSavedIn, reShadePath));

                                await CheckUpdates();
                            }
                            else
                            {
                                status_Label.Text += $"[x] {Resources.Default_FileWasNotFound}\n";
                                Log.SaveErrorLog(new Exception(string.Format(Resources.Default_DownloadedReShadeIniWasNotFoundIn_, reShadePath)));

                                TaskbarManager.Instance.SetProgressValue(100, 100);
                                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            status_Label.Text += $"[x] {Resources.Default_Meeow_FailedToDownloadReShadeIni_TryAgain}\n";
                            updates_LinkLabel.LinkColor = Color.Red;
                            updates_LinkLabel.Text = Resources.Default_FailedToDownload;

                            Log.SaveErrorLog(ex);
                            if (!File.Exists(reShadePath)) Log.Output(Resources.Default_TheReShadeIniFileStillDoesNotExist);

                            TaskbarManager.Instance.SetProgressValue(100, 100);
                            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                        }

                        break;
                    case DialogResult.No:
                        status_Label.Text += $"[i] {Resources.Default_CanceledByTheUser_AreYouSureOfWhatYoureDoing}\n";
                        Log.Output(Resources.Default_FileDownloadHasBeenCanceledByTheUser);

                        if (!File.Exists(reShadePath)) Log.Output(Resources.Default_TheReShadeIniFileStillDoesNotExist);

                        TaskbarManager.Instance.SetProgressValue(100, 100);
                        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
                        break;
                }

                return;
            }

            if (update != 0) return;

            updates_LinkLabel.LinkColor = Color.LawnGreen;
            updates_LinkLabel.Text = Resources.Default_YouHaveTheLatestVersion;
            updateIco_PictureBox.Image = Resources.icons8_available_updates;
        }

        private void W_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Os.RegionCode == "PL")
            {
                WebViewWindow viewer = new WebViewWindow { Location = Location, Icon = Resources.icon_52x52 };
                viewer.Navigate("https://www.youtube.com/embed/2F2DdXUNyaQ?autoplay=1");
                viewer.Show();

                MessageBox.Show(@"Pamiętaj by nie grać w lola, gdyż to grzech ciężki.");
            }
            else
            {
                WebViewWindow viewer = new WebViewWindow { Location = Location, Icon = Resources.icon_52x52 };
                viewer.Navigate("https://www.youtube.com/embed/L3ky4gZU5gY?autoplay=1");
                viewer.Show();
            }
        }

        private void Paimon_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<RandomImages>().Any()) return;
            new RandomImages { Location = Location, Icon = Resources.icon_52x52 }.Show();
        }
    }
}
