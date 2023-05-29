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

namespace StellaLauncher.Scripts.Updates
{
    internal class NormalRelease
    {
        // Files
        public static readonly string SetupPathExe = Path.Combine(Path.GetTempPath(), "Stella-Mod-Update.exe");

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
            updatesLabel.Text = @"Click here to update";
            updateIcon.Image = Resources.icons8_download_from_the_cloud;
            Utils.RemoveClickEvent(updatesLabel);
            updatesLabel.Click += Update_Event;

            // Hide and show elements
            progressBar1.Hide();
            preparingPleaseWait.Hide();
            preparingPleaseWait.Text = @"Preparing... If process is stuck, reopen launcher.";
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
                    .AddText("📥 We found new updates")
                    .AddText("New release is available. Download now!")
                    .Show();
            }
            catch (Exception e)
            {
                Log.SaveErrorLog(e);
            }

            // Log
            Log.Output($"New release from {remoteVerDate} is available: v{Program.AppVersion} → v{remoteVersion}");

            // Taskbar
            TaskbarManager.Instance.SetProgressValue(100, 100);

            // Check update size
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("user-agent", Program.UserAgent);
                await wc.OpenReadTaskAsync("https://github.com/sefinek24/Genshin-Impact-ReShade/releases/latest/download/Stella-Mod-Setup.exe");
                string updateSize = ByteSize.FromBytes(Convert.ToInt64(wc.ResponseHeaders["Content-Length"])).MegaBytes.ToString("00.00");
                statusLabel.Text += $"[i] New version from {remoteVerDate} is available.\n[i] Update size: {updateSize} MB\n";

                // Final
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
                Log.Output($"Update size: {updateSize} MB");
            }
        }

        private static async void Update_Event(object sender, EventArgs e)
        {
            Log.Output("Preparing to download new update...");
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);

            _updatesLabel.LinkColor = Color.DodgerBlue;
            _updatesLabel.Text = @"Updating. Please wait...";
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
                Log.Output("Starting...");
                await StartDownload();
            }
            catch (Exception ex)
            {
                _preparingPleaseWait.Text = @"😥 Something went wrong???";
                Log.ThrowError(ex);
            }

            Log.Output($"Output: {SetupPathExe}");
        }


        private static async Task StartDownload()
        {
            if (File.Exists(SetupPathExe))
            {
                File.Delete(SetupPathExe);
                _statusLabel.Text += "[✓] Deleted old setup file from temp directory.\n";
                Log.Output($"Deleted od setup file from: {SetupPathExe}");
            }

            Log.Output("Downloading in progress...");
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
            _preparingPleaseWait.Text = $@"Downloading update... {bytesReceivedMb:00.00} MB / {bytesReceiveMb:000.00} MB";

            _downloadSpeed = bytesReceived / elapsedTime.TotalSeconds;
            double downloadSpeedInMb = _downloadSpeed / (1024 * 1024);
            _preparingPleaseWait.Text += $@" [{downloadSpeedInMb:00.00} MB/s]";

            Log.Output($"Downloading new update... {bytesReceivedMb:000.00} MB of {bytesReceiveMb:000.00} MB / {downloadSpeedInMb:00.00} MB/s");
        }

        private static async void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string logDir = Path.Combine(Log.Folder, "updates");
            if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);

            Log.Output("Waiting 4s...");

            _preparingPleaseWait.Text = @"Just a moment. Please wait 4s...";
            await Task.Delay(1000);

            _preparingPleaseWait.Text = @"Just a moment. Please wait 3s...";
            await Task.Delay(1000);

            _preparingPleaseWait.Text = @"Just a moment. Please wait 2s...";
            await Task.Delay(1000);

            _preparingPleaseWait.Text = @"Just a moment. Please wait 1s...";
            await Task.Delay(1000);

            _preparingPleaseWait.Text = @"Everything is okay! Starting setup...";
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
                _preparingPleaseWait.Text = $@"Install a new version in the wizard. Closing launcher in {i}s...";
                Log.Output($"Closing launcher in {i}s...");
                await Task.Delay(1000);
            }

            Log.Output("Closing...");
            Environment.Exit(0);
        }
    }
}
