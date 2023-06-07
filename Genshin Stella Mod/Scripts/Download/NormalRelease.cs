using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using ByteSizeLib;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.WindowsAPICodePack.Taskbar;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts.Download
{
    internal class NormalRelease
    {
        // Files
        public static readonly string SetupPathExe = Path.Combine(Path.GetTempPath(), "Stella_Mod_Update.exe");

        // LinkLabel, Label, ProgressBar etc.
        private static LinkLabel _updatesLabel;
        private static ProgressBar _progressBar1;
        private static Label _preparingPleaseWait;
        private static PictureBox _pictureBox3;
        private static Label _settingsLabel;
        private static PictureBox _pictureBox6;
        private static Label _createShortcutLabel;
        private static PictureBox _pictureBox11;
        private static LinkLabel _linkLabel5;
        private static PictureBox _pictureBox4;
        private static Label _websiteLabel;
        private static Label _statusLabel;

        private static PictureBox _pictureBox8;
        private static PictureBox _pictureBox9;
        private static PictureBox _pictureBox10;
        private static LinkLabel _discordServer;
        private static LinkLabel _youTube;
        private static LinkLabel _supportMe;

        // Download
        private static double _downloadSpeed;
        private static long _lastBytesReceived;
        private static DateTime _lastUpdateTime = DateTime.Now;

        public static async void Run(
            string remoteVersion,
            DateTime remoteVerDate,
            LinkLabel versionLabel,
            Label statusLabel,
            LinkLabel updatesLabel,
            PictureBox updateIcon,
            ProgressBar progressBar1,
            Label preparingPleaseWait,
            PictureBox pictureBox3,
            Label settingsLabel,
            PictureBox pictureBox6,
            Label createShortcutLabel,
            PictureBox pictureBox11,
            LinkLabel linkLabel5,
            PictureBox pictureBox4,
            Label websiteLabel,
            PictureBox pictureBox8,
            PictureBox pictureBox9,
            PictureBox pictureBox10,
            LinkLabel discordServer,
            LinkLabel youTube,
            LinkLabel supportMe
        )
        {
            _updatesLabel = updatesLabel;
            _progressBar1 = progressBar1;
            _preparingPleaseWait = preparingPleaseWait;
            _pictureBox3 = pictureBox3;
            _settingsLabel = settingsLabel;
            _pictureBox6 = pictureBox6;
            _createShortcutLabel = createShortcutLabel;
            _pictureBox11 = pictureBox11;
            _linkLabel5 = linkLabel5;
            _pictureBox4 = pictureBox4;
            _websiteLabel = websiteLabel;
            _statusLabel = statusLabel;

            _pictureBox8 = pictureBox8;
            _pictureBox9 = pictureBox9;
            _pictureBox10 = pictureBox10;
            _discordServer = discordServer;
            _youTube = youTube;
            _supportMe = supportMe;

            // 1
            versionLabel.Text = $@"v{Program.AppVersion} → v{remoteVersion}";

            // 2
            updatesLabel.LinkColor = Color.Cyan;
            updatesLabel.Text = Resources.NormalRelease_ClickHereToUpdate;
            updateIcon.Image = Resources.icons8_download_from_the_cloud;
            Utils.RemoveClickEvent(updatesLabel);
            updatesLabel.Click += Update_Event;

            // Hide and show elements
            progressBar1.Hide();
            preparingPleaseWait.Hide();
            preparingPleaseWait.Text = Resources.NormalRelease_Preparing_IfProcessIsStuckReopenLauncher;
            pictureBox3.Show();
            settingsLabel.Show();
            pictureBox6.Show();
            createShortcutLabel.Show();
            pictureBox11.Show();
            linkLabel5.Show();
            pictureBox4.Show();
            websiteLabel.Show();
            progressBar1.Value = 0;

            // ToastContentBuilder
            try
            {
                new ToastContentBuilder()
                    .AddText($"📥 {Resources.NormalRelease_WeFoundNewUpdates}")
                    .AddText(Resources.NormalRelease_NewReleaseIsAvailableDownloadNow)
                    .Show();
            }
            catch (Exception e)
            {
                Log.SaveErrorLog(e);
            }

            // Log
            Log.Output(string.Format(Resources.NormalRelease_NewReleaseFrom_IsAvailable_v_, remoteVerDate, Program.AppVersion, remoteVersion));

            // Taskbar
            TaskbarManager.Instance.SetProgressValue(100, 100);

            // Check update size
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("user-agent", Program.UserAgent);
                await wc.OpenReadTaskAsync("https://github.com/sefinek24/Genshin-Impact-ReShade/releases/latest/download/Stella-Mod-Setup.exe");
                string updateSize = ByteSize.FromBytes(Convert.ToInt64(wc.ResponseHeaders["Content-Length"])).MegaBytes.ToString("00.00");
                statusLabel.Text += $"{string.Format(Resources.NormalRelease_NewVersionFrom_IsAvailable_UpdateSize, remoteVerDate, updateSize)}\n";

                // Final
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
                Log.Output(string.Format(Resources.NormalRelease_UpdateSize, updateSize));
            }
        }

        private static async void Update_Event(object sender, EventArgs e)
        {
            Log.Output(Resources.NormalRelease_PreparingToDownloadNewUpdate);
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);

            _updatesLabel.LinkColor = Color.DodgerBlue;
            _updatesLabel.Text = Resources.NormalRelease_UpdatingPleaseWait;
            Utils.RemoveClickEvent(_updatesLabel);

            _progressBar1.Show();
            _preparingPleaseWait.Show();

            _pictureBox3.Hide();
            _settingsLabel.Hide();
            _pictureBox6.Hide();
            _createShortcutLabel.Hide();
            _pictureBox11.Hide();
            _linkLabel5.Hide();
            _pictureBox4.Hide();
            _websiteLabel.Hide();

            try
            {
                Log.Output(Resources.NormalRelease_Starting);
                await StartDownload();
            }
            catch (Exception ex)
            {
                _preparingPleaseWait.Text = $@"😥 {Resources.NormalRelease_SomethingWentWrong}";
                Log.ThrowError(ex);
            }

            Log.Output(string.Format(Resources.NormalRelease_Output_, SetupPathExe));
        }


        private static async Task StartDownload()
        {
            if (File.Exists(SetupPathExe))
            {
                File.Delete(SetupPathExe);
                _statusLabel.Text += $"[✓] {Resources.NormalRelease_DeletedOldSetupFileFromTempDir}\n";
                Log.Output(string.Format(Resources.NormalRelease_DeletedOldSetupFireFrom_, SetupPathExe));
            }

            Log.Output(Resources.NormalRelease_DownloadingInProgress);
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);

            using (WebClient client = new WebClient())
            {
                client.Headers.Add("user-agent", Program.UserAgent);
                client.DownloadProgressChanged += Client_DownloadProgressChanged;
                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                await client.DownloadFileTaskAsync(new Uri("https://github.com/sefinek24/Genshin-Impact-ReShade/releases/latest/download/Stella-Mod-Setup.exe"), SetupPathExe);
            }
        }

        private static void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            int progress = (int)Math.Floor(e.BytesReceived * 100.0 / e.TotalBytesToReceive);
            _progressBar1.Value = progress;
            TaskbarManager.Instance.SetProgressValue(progress, 100);

            DateTime currentTime = DateTime.Now;
            TimeSpan elapsedTime = currentTime - _lastUpdateTime;
            long bytesReceived = e.BytesReceived - _lastBytesReceived;

            if (!(elapsedTime.TotalMilliseconds > 1000)) return;

            _lastUpdateTime = currentTime;
            _lastBytesReceived = e.BytesReceived;

            double bytesReceivedMb = ByteSize.FromBytes(e.BytesReceived).MegaBytes;
            double bytesReceiveMb = ByteSize.FromBytes(e.TotalBytesToReceive).MegaBytes;
            _preparingPleaseWait.Text = string.Format(Resources.NormalRelease_DownloadingUpdate_, $"{bytesReceivedMb:00.00}", $"{bytesReceiveMb:000.00}");

            _downloadSpeed = bytesReceived / elapsedTime.TotalSeconds;
            double downloadSpeedInMb = _downloadSpeed / (1024 * 1024);
            _preparingPleaseWait.Text += $@" [{downloadSpeedInMb:00.00} MB/s]";

            Log.Output(string.Format(Resources.NormalRelease_DownloadingNewUpdate_, $"{bytesReceivedMb:00.00}", $"{bytesReceiveMb:000.00}", $"{downloadSpeedInMb:00.00}"));
        }

        private static async void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string logDir = Path.Combine(Log.Folder, "updates");
            if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);

            Log.Output(string.Format(Resources.NormalRelease_Waiting_, 4));

            _preparingPleaseWait.Text = string.Format(Resources.NormalRelease_JustAMoment_PleaseWait, 4);
            await Task.Delay(1000);

            _preparingPleaseWait.Text = string.Format(Resources.NormalRelease_JustAMoment_PleaseWait, 3);
            await Task.Delay(1000);

            _preparingPleaseWait.Text = string.Format(Resources.NormalRelease_JustAMoment_PleaseWait, 2);
            await Task.Delay(1000);

            _preparingPleaseWait.Text = string.Format(Resources.NormalRelease_JustAMoment_PleaseWait, 1);
            await Task.Delay(1000);

            _preparingPleaseWait.Text = Resources.NormalRelease_EverythingIsOkay_StartingSetup;
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
            await Task.Delay(500);

            _ = Cmd.CliWrap(SetupPathExe, $"/UPDATE /NORESTART /LOG=\"{logDir}\\{DateTime.Now:yyyy-dd-M...HH-mm-ss}.log\"", null, true, true);

            _pictureBox9.Visible = false;
            _discordServer.Visible = false;
            _pictureBox10.Visible = false;
            _supportMe.Visible = false;
            _pictureBox8.Visible = false;
            _youTube.Visible = false;

            for (int i = 15; i > 0; i--)
            {
                _preparingPleaseWait.Text = string.Format(Resources.NormalRelease_InstallANewVersionInTheWizard_ClosingLauncherIn_, i);
                Log.Output(string.Format(Resources.NormalRelease_ClosingLauncherIn_, i));
                await Task.Delay(1000);
            }

            Log.Output(Resources.NormalRelease_Closing);
            Environment.Exit(0);
        }
    }
}
