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
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Taskbar;
using Newtonsoft.Json;
using StellaLauncher.Forms.Other;
using StellaLauncher.Models;
using StellaLauncher.Properties;
using StellaLauncher.Scripts;
using StellaLauncher.Scripts.Download;

namespace StellaLauncher.Forms
{
    public partial class Default : Form
    {
        // Files
        private static readonly string CmdOutputLogs = Path.Combine(Program.AppData, "logs", "cmd.output.log");

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
        private static LinkLabel _startGame_LinkLabel;
        private static LinkLabel _injectReShade_LinkLabel;
        private static LinkLabel _runFpsUnlocker_LinkLabel;
        private static LinkLabel _only3DMigoto_LinkLabel;
        private static LinkLabel _runGiLauncher_LinkLabel;
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
        private static string _resPath;

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
            progressBar1.Value = 10;
        }

        private async void Default_Load(object sender, EventArgs e)
        {
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

            // Registry
            using (RegistryKey key2 = Registry.CurrentUser.CreateSubKey(Program.RegistryPath, true))
            {
                key2?.SetValue("LastRunTime", DateTime.Now);
            }

            progressBar1.Value = 15;


            // User is my patron?
            string mainPcKey = Secret.GetTokenFromRegistry();
            if (mainPcKey != null)
            {
                string data = await Secret.VerifyToken(mainPcKey);
                if (data == null)
                {
                    if (Directory.Exists(Program.PatronsDir)) Directory.Delete(Program.PatronsDir, true);
                    return;
                }

                GetToken remote = JsonConvert.DeserializeObject<GetToken>(data);
                if (remote.Status == 200)
                {
                    Secret.IsMyPatron = true;
                    label1.Text = Resources.Default_GenshinStellaModForPatrons;
                    label1.TextAlign = ContentAlignment.MiddleRight;
                }
                else if (Directory.Exists(Program.PatronsDir))
                {
                    Directory.Delete(Program.PatronsDir, true);
                }
            }

            progressBar1.Value = 25;

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

        public static async Task<int> CheckForUpdates()
        {
            _updates_LinkLabel.LinkColor = Color.White;
            _updates_LinkLabel.Text = Resources.Default_CheckingForUpdates;

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
                _progressBar1.Value = 40;

                // Major release
                if (Program.AppVersion[0] != remoteVersion[0])
                {
                    UpdateIsAvailable = true;

                    MajorRelease.Run(remoteVersion, remoteVerDate, _version_LinkLabel, _updates_LinkLabel, _updateIco_PictureBox);
                    return 1;
                }

                _progressBar1.Value = 60;

                // Normal release
                if (Program.AppVersion != remoteVersion)
                {
                    UpdateIsAvailable = true;

                    NormalRelease.Run(remoteVersion, remoteVerDate);
                    return 1;
                }

                _progressBar1.Value = 80;

                // Check new updates of resources
                string resSfn = Path.Combine(Program.AppData, "resources-path.sfn");
                string resourcesPath;
                if (File.Exists(resSfn))
                {
                    resourcesPath = File.ReadAllText(resSfn);
                    if (!Directory.Exists(resourcesPath))
                    {
                        _status_Label.Text += $"{string.Format(Resources.Default_Directory_WasNotFound, resourcesPath)}\n";
                        Log.SaveErrorLog(new Exception(string.Format(Resources.Default_Directory_WasNotFound, resourcesPath)));

                        Utils.HideProgressBar(true);
                        return -1;
                    }

                    _resPath = resourcesPath;

                    string jsonFile = Path.Combine(resourcesPath, "data.json");
                    if (!File.Exists(jsonFile))
                    {
                        _status_Label.Text += $"{string.Format(Resources.Default_File_WasNotFound, jsonFile)}\n";
                        Log.SaveErrorLog(new Exception(string.Format(Resources.Default_File_WasNotFound, jsonFile)));

                        Utils.HideProgressBar(true);
                        return -1;
                    }

                    _progressBar1.Value = 80;

                    string jsonContent = File.ReadAllText(jsonFile);
                    LocalResources data = JsonConvert.DeserializeObject<LocalResources>(jsonContent);

                    WebClient resClient = new WebClient();
                    resClient.Headers.Add("user-agent", Program.UserAgent);
                    string resJson = await resClient.DownloadStringTaskAsync("https://api.sefinek.net/api/v4/genshin-stella-mod/version/app/launcher/resources");
                    StellaResources resourcesRes = JsonConvert.DeserializeObject<StellaResources>(resJson);

                    if (data.Version != resourcesRes.Message)
                    {
                        UpdateIsAvailable = true;

                        DownloadResources.Run(resourcesPath, data.Version, resourcesRes.Message, resourcesRes.Date);
                        return 1;
                    }
                }
                else
                {
                    _status_Label.Text += $"{string.Format(Resources.Default_File_WasNotFound, resSfn)}\n";
                    Log.SaveErrorLog(new Exception(string.Format(Resources.Default_File_WasNotFound, resSfn)));

                    Utils.HideProgressBar(true);
                    return -1;
                }

                _progressBar1.Value = 90;

                // Check new updates for ReShade.ini file
                int resultInt = await ReShadeIni.CheckForUpdates();
                switch (resultInt)
                {
                    case -2:
                    {
                        DialogResult msgBoxResult = MessageBox.Show(Resources.Default_TheReShadeIniFileCouldNotBeLocatedInYourGameFiles, Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        int number = await ReShadeIni.Download(resultInt, resourcesPath, msgBoxResult);
                        return number;
                    }

                    case 1:
                    {
                        DialogResult msgReply = MessageBox.Show(Resources.Default_AreYouSureWantToUpdateReShadeConfiguration, Program.AppName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                        if (msgReply == DialogResult.No || msgReply == DialogResult.Cancel)
                        {
                            Log.Output(Resources.Default_TheUpdateOfReShadIniHasBeenCanceledByTheUser);
                            MessageBox.Show(Resources.Default_ForSomeReasonYouDidNotGiveConsentForTheAutomaticUpdateOfTheReShadeFile, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                            Utils.HideProgressBar(true);
                            return 1;
                        }

                        int number = await ReShadeIni.Download(resultInt, resourcesPath, DialogResult.Yes);
                        return number;
                    }
                }


                // Not found any new updates
                _updates_LinkLabel.Text = Resources.Default_CheckForUpdates;
                _updateIco_PictureBox.Image = Resources.icons8_available_updates;

                Utils.RemoveClickEvent(_updates_LinkLabel);
                _updates_LinkLabel.Click += CheckUpdates_Click;

                UpdateIsAvailable = false;
                Log.Output(string.Format(Resources.Default_NotFoundAnyNewUpdates_YourInstalledVersion_, Program.AppVersion));

                _startGame_LinkLabel.Visible = true;
                _injectReShade_LinkLabel.Visible = true;
                _runFpsUnlocker_LinkLabel.Visible = true;
                _only3DMigoto_LinkLabel.Visible = true;
                _runGiLauncher_LinkLabel.Visible = true;
                if (!Secret.IsMyPatron) _becomeMyPatron_LinkLabel.Visible = true;

                _progressBar1.Value = 100;
                Utils.HideProgressBar(false);
                return 0;
            }
            catch (Exception e)
            {
                UpdateIsAvailable = false;

                _updates_LinkLabel.LinkColor = Color.Red;
                _updates_LinkLabel.Text = Resources.Default_OhhSomethingWentWrong;
                _status_Label.Text += $"[x] {e.Message}\n";

                Log.SaveErrorLog(new Exception(string.Format(Resources.Default_SomethingWentWrongWhileCheckingForNewUpdates, e)));
                Utils.HideProgressBar(true);
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

            if (!File.Exists(Program.FpsUnlockerCfgPath) && !Debugger.IsAttached) FpsUnlockerCfg.Run(status_Label);

            if (status_Label.Text.Length > 0) Log.SaveErrorLog(new Exception(status_Label.Text));

            if (File.Exists(NormalRelease.SetupPathExe))
                try
                {
                    File.Delete(NormalRelease.SetupPathExe);
                    status_Label.Text += $"[i] {Resources.Default_DeletedOldSetupFromTempDirectory}\n";
                    Log.Output(string.Format(Resources.Default_DeletedOldSetupFromTempFolder, NormalRelease.SetupPathExe));
                }
                catch (Exception ex)
                {
                    status_Label.Text += $"[x] {ex.Message}\n";
                    Log.SaveErrorLog(ex);
                }


            await CheckForUpdates();

            int data = Program.Settings.ReadInt("Launcher", "DiscordRPC", 1);
            if (data == 1) Discord.InitRpc();

            if (Debugger.IsAttached) return;
            Telemetry.Opened();

            // Music
            int muteBgMusic = Program.Settings.ReadInt("Launcher", "EnableMusic", 1);
            if (muteBgMusic == 1)
            {
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
            }


            // Launch count
            RegistryKey key = Registry.CurrentUser.CreateSubKey(Program.RegistryPath);
            int launchCount = (int)(key?.GetValue("LaunchCount") ?? 0);
            launchCount++;
            key?.SetValue("LaunchCount", launchCount);

            switch (launchCount)
            {
                case 5:
                case 20:
                case 30:
                    DialogResult discordResult = MessageBox.Show(Resources.Program_DoYouWantToJoinOurDiscord, Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    Log.Output(string.Format(Resources.Program_QuestionMessageBox_DoYouWantToJoinOurDiscord_, discordResult));
                    if (discordResult == DialogResult.Yes) Utils.OpenUrl(Discord.Invitation);
                    break;

                case 2:
                case 12:
                case 40:
                    DialogResult feedbackResult = MessageBox.Show(Resources.Program_WouldYouShareOpinionAboutStellaMod, Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    Log.Output(string.Format(Resources.Program_QuestionMessageBox_WouldYouShareOpinionAboutStellaMod, feedbackResult));
                    if (feedbackResult == DialogResult.Yes) Utils.OpenUrl("https://www.trustpilot.com/review/genshin.sefinek.net");
                    break;

                case 3:
                case 10:
                case 25:
                case 35:
                case 45:
                    if (!Secret.IsMyPatron) Application.Run(new SupportMe { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) });
                    return;

                case 28:
                case 70:
                case 100:
                case 200:
                case 300:
                    DialogResult logFilesResult = MessageBox.Show(Resources.Program_DoYouWantToSendUsanonymousLogFiles, Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    Log.Output(string.Format(Resources.Program_QuestionMessageBox_DoYouWantToSendUsanonymousLogFiles_, logFilesResult));
                    if (logFilesResult == DialogResult.Yes)
                    {
                        Telemetry.SendLogFiles();

                        DialogResult showFilesResult = MessageBox.Show(Resources.Program_IfYouWishToSendLogsToTheDeveloperPleaseSendThemToMeOnDiscord, Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (showFilesResult == DialogResult.Yes)
                        {
                            Process.Start(Log.Folder);
                            Log.Output($"Opened: {Log.Folder}");
                        }
                    }

                    break;
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
                $"{(Secret.IsMyPatron ? RunCmdPatrons : RunCmd)} {(Secret.IsMyPatron ? 1 : 6)} {await Utils.GetGameVersion()} \"{CmdOutputLogs}\" {(Secret.IsMyPatron ? $"\"{_resPath}\\3DMigoto\"" : "0")} \"{Program.AppPath}\"", Program.AppPath,
                false, false);

            // Exit Stella with status code 0
            if (res) Environment.Exit(0);
        }

        /* 3 */
        private async void OnlyReShade_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Run cmd file
            bool res = await Cmd.CliWrap("wt.exe", $"{(Secret.IsMyPatron ? RunCmdPatrons : RunCmd)} 3 {await Utils.GetGameVersion()} \"{CmdOutputLogs}\"", Program.AppPath, false, false);
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
            bool res = await Cmd.CliWrap("wt.exe", $"{(Secret.IsMyPatron ? RunCmdPatrons : RunCmd)} 4 {await Utils.GetGameVersion()} \"{CmdOutputLogs}\"", Program.AppPath, false, false);

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
            await Cmd.CliWrap("wt.exe", $"{(Secret.IsMyPatron ? RunCmdPatrons : RunCmd)} 5 {await Utils.GetGameVersion()} \"{CmdOutputLogs}\" \"{_resPath}\\3DMigoto\" \"{Program.AppPath}\"", Program.AppPath, false, false);

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
            new Tools { DesktopLocation = DesktopLocation, Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) }.Show();
        }

        private void Gameplay_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Application.OpenForms.OfType<Gameplay>().Any()) return;
            new Gameplay { DesktopLocation = DesktopLocation, Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) }.Show();
        }

        private void Links_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Application.OpenForms.OfType<Links>().Any()) return;
            new Links { DesktopLocation = DesktopLocation, Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) }.Show();
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
            CheckUpdates_Click(sender, e);
        }

        private static async void CheckUpdates_Click(object sender, EventArgs e)
        {
            int update = await CheckForUpdates();
            if (update != 0) return;

            _updates_LinkLabel.LinkColor = Color.LawnGreen;
            _updates_LinkLabel.Text = Resources.Default_YouHaveTheLatestVersion;
            _updateIco_PictureBox.Image = Resources.icons8_available_updates;
        }

        private void W_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (ComputerInfo.GetSystemRegion() == "PL")
            {
                WebViewWindow viewer = new WebViewWindow { DesktopLocation = DesktopLocation, Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) };
                viewer.Navigate("https://www.youtube.com/embed/2F2DdXUNyaQ?autoplay=1");
                viewer.Show();

                MessageBox.Show(@"Pamiętaj by nie grać w lola, gdyż to grzech ciężki.");
            }
            else
            {
                WebViewWindow viewer = new WebViewWindow { DesktopLocation = DesktopLocation, Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) };
                viewer.Navigate("https://www.youtube.com/embed/L3ky4gZU5gY?autoplay=1");
                viewer.Show();
            }
        }

        private void Paimon_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<RandomImages>().Any()) return;

            RandomImages randomImagesForm = new RandomImages { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) };
            randomImagesForm.Show();
        }
    }
}
