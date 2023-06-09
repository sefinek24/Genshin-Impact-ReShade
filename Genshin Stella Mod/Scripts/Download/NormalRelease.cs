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

        // Main
        private static Label _status_Label;
        private static Label _preparingPleaseWait;
        private static ProgressBar _progressBar1;

        // Left
        private static PictureBox _discordServerIco_Picturebox;
        private static LinkLabel _discordServer_LinkLabel;
        private static PictureBox _supportMeIco_PictureBox;
        private static LinkLabel _supportMe_LinkLabel;
        private static PictureBox _youtubeIco_Picturebox;
        private static LinkLabel _youTube_LinkLabel;

        // Bottom
        private static PictureBox _toolsIco_PictureBox;
        private static LinkLabel _tools_LinkLabel;
        private static PictureBox _shortcutIco_PictureBox;
        private static LinkLabel _links_LinkLabel;
        private static PictureBox _padIco_PictureBox;
        private static LinkLabel _gameplay_LinkLabel;
        private static PictureBox _websiteIco_PictureBox;
        private static LinkLabel _website_LinkLabel;

        // Right
        private static LinkLabel _updates_LinkLabel;

        // Download
        private static double _downloadSpeed;
        private static long _lastBytesReceived;
        private static DateTime _lastUpdateTime = DateTime.Now;

        public static async void Run(
            // Custom
            string remoteVersion,
            DateTime remoteVerDate,

            // Main
            Label status_Label,
            Label PreparingPleaseWait,
            ProgressBar progressBar1,

            // Left
            PictureBox discordServerIco_Picturebox,
            LinkLabel discordServer_LinkLabel,
            PictureBox supportMeIco_PictureBox,
            LinkLabel supportMe_LinkLabel,
            PictureBox youtubeIco_Picturebox,
            LinkLabel youTube_LinkLabel,

            // Bottom
            PictureBox toolsIco_PictureBox,
            LinkLabel tools_LinkLabel,
            PictureBox shortcutIco_PictureBox,
            LinkLabel links_LinkLabel,
            PictureBox padIco_PictureBox,
            LinkLabel gameplay_LinkLabel,
            PictureBox websiteIco_PictureBox,
            LinkLabel website_LinkLabel,

            // Right
            LinkLabel version_LinkLabel,
            LinkLabel updates_LinkLabel,
            PictureBox updateIco_PictureBox
        )
        {
            _status_Label = status_Label;
            _preparingPleaseWait = PreparingPleaseWait;
            _progressBar1 = progressBar1;

            _discordServerIco_Picturebox = discordServerIco_Picturebox;
            _discordServer_LinkLabel = discordServer_LinkLabel;
            _supportMeIco_PictureBox = supportMeIco_PictureBox;
            _supportMe_LinkLabel = supportMe_LinkLabel;
            _youtubeIco_Picturebox = youtubeIco_Picturebox;
            _youTube_LinkLabel = youTube_LinkLabel;

            _toolsIco_PictureBox = toolsIco_PictureBox;
            _tools_LinkLabel = tools_LinkLabel;
            _shortcutIco_PictureBox = shortcutIco_PictureBox;
            _links_LinkLabel = links_LinkLabel;
            _padIco_PictureBox = padIco_PictureBox;
            _gameplay_LinkLabel = gameplay_LinkLabel;
            _websiteIco_PictureBox = websiteIco_PictureBox;
            _website_LinkLabel = website_LinkLabel;

            _updates_LinkLabel = updates_LinkLabel;

            // 1
            version_LinkLabel.Text = $@"v{Program.AppVersion} → v{remoteVersion}";

            // 2
            updates_LinkLabel.LinkColor = Color.Cyan;
            updates_LinkLabel.Text = Resources.NormalRelease_ClickHereToUpdate;
            updateIco_PictureBox.Image = Resources.icons8_download_from_the_cloud;
            Utils.RemoveClickEvent(updates_LinkLabel);
            updates_LinkLabel.Click += Update_Event;

            // Hide and show elements
            progressBar1.Hide();
            PreparingPleaseWait.Hide();
            PreparingPleaseWait.Text = Resources.NormalRelease_Preparing_IfProcessIsStuckReopenLauncher;

            discordServerIco_Picturebox.Show();
            discordServer_LinkLabel.Show();
            supportMeIco_PictureBox.Show();
            supportMe_LinkLabel.Show();
            youtubeIco_Picturebox.Show();

            toolsIco_PictureBox.Show();
            tools_LinkLabel.Show();

            shortcutIco_PictureBox.Show();
            shortcutIco_PictureBox.Show();

            links_LinkLabel.Show();

            padIco_PictureBox.Show();
            gameplay_LinkLabel.Show();
            websiteIco_PictureBox.Show();
            website_LinkLabel.Show();
            version_LinkLabel.Show();
            updates_LinkLabel.Show();
            updateIco_PictureBox.Show();
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
            status_Label.Text += $"[i] {string.Format(Resources.NormalRelease_NewVersionFrom_IsAvailable, remoteVerDate)}\n";
            Log.Output(string.Format(Resources.NormalRelease_NewReleaseFrom_IsAvailable_v_, remoteVerDate, Program.AppVersion, remoteVersion));

            // Taskbar
            TaskbarManager.Instance.SetProgressValue(100, 100);

            // Check update size
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("user-agent", Program.UserAgent);
                await wc.OpenReadTaskAsync("https://github.com/sefinek24/Genshin-Impact-ReShade/releases/latest/download/Stella-Mod-Setup.exe");
                string updateSize = ByteSize.FromBytes(Convert.ToInt64(wc.ResponseHeaders["Content-Length"])).MegaBytes.ToString("00.00");
                status_Label.Text += $"[i] {string.Format(Resources.StellaResources_UpdateSize, $"{updateSize} MB")}\n";

                // Final
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
                Log.Output(string.Format(Resources.NormalRelease_UpdateSize, updateSize));
            }
        }

        private static async void Update_Event(object sender, EventArgs e)
        {
            Log.Output(Resources.NormalRelease_PreparingToDownloadNewUpdate);
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);

            _updates_LinkLabel.LinkColor = Color.DodgerBlue;
            _updates_LinkLabel.Text = Resources.NormalRelease_UpdatingPleaseWait;
            Utils.RemoveClickEvent(_updates_LinkLabel);

            _progressBar1.Show();
            _preparingPleaseWait.Show();

            _progressBar1.Show();
            _preparingPleaseWait.Show();

            _discordServerIco_Picturebox.Hide();
            _discordServer_LinkLabel.Hide();
            _supportMeIco_PictureBox.Hide();
            _supportMe_LinkLabel.Hide();
            _youtubeIco_Picturebox.Hide();
            _youTube_LinkLabel.Hide();

            _toolsIco_PictureBox.Hide();
            _tools_LinkLabel.Hide();
            _shortcutIco_PictureBox.Hide();
            _links_LinkLabel.Hide();
            _padIco_PictureBox.Hide();
            _gameplay_LinkLabel.Hide();
            _websiteIco_PictureBox.Hide();
            _website_LinkLabel.Hide();

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
                _status_Label.Text += $"[✓] {Resources.NormalRelease_DeletedOldSetupFileFromTempDir}\n";
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

            _downloadSpeed = bytesReceived / elapsedTime.TotalSeconds;
            double downloadSpeedInMb = _downloadSpeed / (1024 * 1024);

            _preparingPleaseWait.Text = $@"{string.Format(Resources.NormalRelease_DownloadingUpdate_, $"{bytesReceivedMb:00.00}", $"{bytesReceiveMb:000.00}")} [{downloadSpeedInMb:00.00} MB/s]";

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
