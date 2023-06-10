using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ByteSizeLib;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.WindowsAPICodePack.Taskbar;
using StellaLauncher.Forms;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts.Download
{
    internal class DownloadResources
    {
        // Files
        public static string StellaResZip;

        // Custom
        private static string _resourcesPath;

        // Download
        private static double _downloadSpeed;
        private static long _lastBytesReceived;
        private static DateTime _lastUpdateTime = DateTime.Now;


        public static async void Run(string resourcesPath, string localResVersion, string remoteResVersion, string remoteResDate)
        {
            _resourcesPath = resourcesPath;

            // 1
            Default._version_LinkLabel.Text = $@"v{localResVersion} → v{remoteResVersion}";

            // 2
            Default._updates_LinkLabel.LinkColor = Color.Cyan;
            Default._updates_LinkLabel.Text = Resources.NormalRelease_ClickHereToUpdate;
            Default._updateIco_PictureBox.Image = Resources.icons8_download_from_the_cloud;
            Utils.RemoveClickEvent(Default._updates_LinkLabel);
            Default._updates_LinkLabel.Click += Update_Event;

            // Hide and show elements
            Default._progressBar1.Hide();
            Default._preparingPleaseWait.Hide();
            Default._preparingPleaseWait.Text = Resources.NormalRelease_Preparing_IfProcessIsStuckReopenLauncher;

            Default._discordServerIco_Picturebox.Show();
            Default._discordServer_LinkLabel.Show();
            Default._supportMeIco_PictureBox.Show();
            Default._supportMe_LinkLabel.Show();
            Default._youtubeIco_Picturebox.Show();

            Default._toolsIco_PictureBox.Show();
            Default._tools_LinkLabel.Show();

            Default._shortcutIco_PictureBox.Show();
            Default._links_LinkLabel.Show();

            Default._padIco_PictureBox.Show();
            Default._gameplay_LinkLabel.Show();
            Default._websiteIco_PictureBox.Show();
            Default._website_LinkLabel.Show();
            Default._version_LinkLabel.Show();
            Default._updates_LinkLabel.Show();
            Default._updateIco_PictureBox.Show();
            Default._progressBar1.Value = 0;

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

            // Date
            DateTime date = DateTime.Parse(remoteResDate, null, DateTimeStyles.RoundtripKind).ToUniversalTime().ToLocalTime();

            // Log
            Log.Output($"{string.Format(Resources.StellaResources_NewResourcesUpdateIsAvailable, date)} {remoteResDate}.");

            Default._status_Label.Text += $"[i] {string.Format(Resources.StellaResources_NewResourcesUpdateIsAvailable, date)}\n";
            StellaResZip = Path.Combine(resourcesPath, $"Stella resources - v{remoteResVersion}.zip");


            // Check update size
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("user-agent", Program.UserAgent);
                await wc.OpenReadTaskAsync("https://github.com/sefinek24/Genshin-Stella-Resources/releases/latest/download/resources.zip");
                string updateSize = ByteSize.FromBytes(Convert.ToInt64(wc.ResponseHeaders["Content-Length"])).MegaBytes.ToString("00.00");
                Default._status_Label.Text += $"[i] {string.Format(Resources.StellaResources_UpdateSize, $"{updateSize} MB")}\n";

                // Final
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
                Log.Output(string.Format(Resources.StellaResources_UpdateSize, $"{updateSize} MB"));
            }

            // Taskbar
            TaskbarManager.Instance.SetProgressValue(100, 100);
        }

        private static async void Update_Event(object sender, EventArgs e)
        {
            Log.Output(Resources.NormalRelease_PreparingToDownloadNewUpdate);
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);

            Default._updates_LinkLabel.LinkColor = Color.DodgerBlue;
            Default._updates_LinkLabel.Text = Resources.NormalRelease_UpdatingPleaseWait;
            Utils.RemoveClickEvent(Default._updates_LinkLabel);

            Default._progressBar1.Show();
            Default._preparingPleaseWait.Show();

            Default._discordServerIco_Picturebox.Hide();
            Default._discordServer_LinkLabel.Hide();
            Default._supportMeIco_PictureBox.Hide();
            Default._supportMe_LinkLabel.Hide();
            Default._youtubeIco_Picturebox.Hide();
            Default._youTube_LinkLabel.Hide();

            Default._toolsIco_PictureBox.Hide();
            Default._tools_LinkLabel.Hide();
            Default._shortcutIco_PictureBox.Hide();
            Default._links_LinkLabel.Hide();
            Default._padIco_PictureBox.Hide();
            Default._gameplay_LinkLabel.Hide();
            Default._websiteIco_PictureBox.Hide();
            Default._website_LinkLabel.Hide();

            try
            {
                Log.Output(Resources.NormalRelease_Starting);
                await StartDownload();
            }
            catch (Exception ex)
            {
                Default._preparingPleaseWait.Text = $@"😥 {Resources.NormalRelease_SomethingWentWrong}";
                Log.ThrowError(ex);
            }

            Log.Output(string.Format(Resources.NormalRelease_Output_, StellaResZip));
        }


        private static async Task StartDownload()
        {
            Console.WriteLine(@"Checking Stella resources...");

            if (File.Exists(_resourcesPath))
            {
                File.Delete(_resourcesPath);
                Default._status_Label.Text += $"[✓] {Resources.NormalRelease_DeletedOldSetupFileFromTempDir}\n";
                Log.Output(string.Format(Resources.NormalRelease_DeletedOldSetupFireFrom_, _resourcesPath));
            }

            Log.Output(Resources.NormalRelease_DownloadingInProgress);
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);

            using (WebClient client = new WebClient())
            {
                client.Headers.Add("user-agent", Program.UserAgent);
                client.DownloadProgressChanged += Client_DownloadProgressChanged;
                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                await client.DownloadFileTaskAsync(new Uri("https://github.com/sefinek24/Genshin-Stella-Resources/releases/latest/download/resources.zip"), StellaResZip);
            }
        }

        private static void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            int progress = (int)Math.Floor(e.BytesReceived * 100.0 / e.TotalBytesToReceive);
            Default._progressBar1.Value = progress;
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

            Default._preparingPleaseWait.Text = $@"{string.Format(Resources.StellaResources_DownloadingResources, $"{bytesReceivedMb:00.00}", $"{bytesReceiveMb:00.00}")} [{downloadSpeedInMb:00.00} MB/s]";

            Log.Output(string.Format(Resources.NormalRelease_DownloadingNewUpdate_, $"{bytesReceivedMb:00.00}", $"{bytesReceiveMb:000.00}", $"{downloadSpeedInMb:00.00}"));
        }

        private static async void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Default._preparingPleaseWait.Text = Resources.StellaResources_UnpackingFiles;

            await UnzipWithProgress(StellaResZip, _resourcesPath);

            Default._progressBar1.Hide();
            Default._preparingPleaseWait.Hide();

            Default._discordServerIco_Picturebox.Show();
            Default._discordServer_LinkLabel.Show();
            Default._supportMeIco_PictureBox.Show();
            Default._supportMe_LinkLabel.Show();
            Default._youtubeIco_Picturebox.Show();
            Default._youTube_LinkLabel.Show();

            Default._toolsIco_PictureBox.Show();
            Default._tools_LinkLabel.Show();
            Default._shortcutIco_PictureBox.Show();
            Default._links_LinkLabel.Show();
            Default._padIco_PictureBox.Show();
            Default._gameplay_LinkLabel.Show();
            Default._websiteIco_PictureBox.Show();
            Default._website_LinkLabel.Show();

            Default._version_LinkLabel.Text = $@"v{Program.AppVersion}";

            Default._status_Label.Text += $"[✓] {Resources.StellaResources_SuccessfullyUpdatedResources}\n";
            Log.Output(string.Format(Resources.StellaResources_SuccessfullyUnpacked, StellaResZip));

            await Default.CheckUpdates();
        }

        private static async Task UnzipWithProgress(string zipFilePath, string extractPath)
        {
            using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
            {
                int totalEntries = archive.Entries.Count;
                int currentEntry = 0;
                long totalBytes = 0;
                long totalBytesToExtract = archive.Entries.Sum(entry => entry.Length);

                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string entryPath = Path.Combine(extractPath, entry.FullName);

                    if (entry.FullName.EndsWith("/"))
                    {
                        Directory.CreateDirectory(entryPath);
                        continue;
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(entryPath));

                    using (Stream source = entry.Open())
                    using (Stream destination = File.Create(entryPath))
                    {
                        byte[] buffer = new byte[8192];
                        int bytesRead;

                        while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await destination.WriteAsync(buffer, 0, bytesRead);
                            totalBytes += bytesRead;

                            int progressPercentage = (int)((double)totalBytes / totalBytesToExtract * 100);
                            Default._progressBar1.Value = progressPercentage;
                            TaskbarManager.Instance.SetProgressValue(progressPercentage, 100);
                        }
                    }

                    currentEntry++;

                    Default._preparingPleaseWait.Text = string.Format(Resources.StellaResources_UnpackingFiles_From_, currentEntry, totalEntries);
                    Log.Output(string.Format(Resources.StellaResources_UnpackingFiles_Log, currentEntry, totalEntries, totalBytes, totalBytesToExtract));
                }
            }
        }
    }
}
