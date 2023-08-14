using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using ByteSizeLib;
using Microsoft.WindowsAPICodePack.Taskbar;
using StellaLauncher.Forms;
using StellaLauncher.Properties;
using StellaLauncher.Scripts.Download;
using StellaLauncher.Scripts.Forms.MainForm;

namespace StellaLauncher.Scripts.Patrons
{
    internal static class UpdateBenefits
    {
        // Download
        private static double _downloadSpeed;
        private static long _lastBytesReceived;
        private static DateTime _lastUpdateTime = DateTime.Now;

        private static readonly string DownloadUrl = $"{Program.WebApi}/genshin-stella-mod/patrons/benefits/download";
        private static string _zipFile;
        private static string _outputDir;
        private static string successfullyUpdated;

        // Paths
        public static async void Download(string benefitName, string zipFilename, string dirPathToUnpack)
        {
            string tempPath = Path.Combine(Default.ResourcesPath, "Temp files");
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
                Log.Output($"Created dir: {tempPath}");
            }

            _zipFile = Path.Combine(tempPath, zipFilename);
            if (File.Exists(_zipFile))
            {
                File.Delete(_zipFile);
                Log.Output($"Deleted file: {_zipFile}");
            }

            _outputDir = dirPathToUnpack;

            switch (benefitName)
            {
                case "3dmigoto":
                    successfullyUpdated = Resources.UpdateBenefits_SuccessfullyUpdated3DMigotoMods;
                    break;
                case "addons":
                    successfullyUpdated = Resources.UpdateBenefits_SuccessfullyUpdatedAddons;
                    break;
                case "presets":
                    successfullyUpdated = Resources.UpdateBenefits_SuccessfullyUpdatedPresets;
                    break;
                case "shaders":
                    successfullyUpdated = Resources.UpdateBenefits_SuccessfullyUpdatedShaders;
                    break;
                case "cmd":
                    successfullyUpdated = Resources.UpdateBenefits_SuccessfullyUpdatedCmdFiles;
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }

            Default._progressBar1.Show();
            Default._preparingPleaseWait.Show();

            using (WebClient client = new WebClient())
            {
                client.Headers.Add("user-agent", Program.UserAgent);
                client.Headers.Add("Authorization", $"Bearer {Secret.BearerToken}");

                client.DownloadProgressChanged += Client_DownloadProgressChanged;
                client.DownloadFileCompleted += Client_DownloadFileCompleted;

                await client.DownloadFileTaskAsync(new Uri($"{DownloadUrl}?benefitType={benefitName}"), _zipFile);
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

            Default._preparingPleaseWait.Text = $@"{string.Format(Resources.NormalRelease_DownloadingUpdate_, $"{bytesReceivedMb:00.00}", $"{bytesReceiveMb:000.00}")} [{downloadSpeedInMb:00.00} MB/s]";

            Log.Output($"Downloading new update... {bytesReceivedMb:00.00} MB of {bytesReceiveMb:000.00} MB / {downloadSpeedInMb:00.00} MB/s");
        }

        private static async void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // Unpack files
            Log.Output($"Unpacking {_zipFile} to {_outputDir}");
            Default._preparingPleaseWait.Text = Resources.StellaResources_UnpackingFiles;
            await DownloadResources.UnzipWithProgress(_zipFile, _outputDir);
            Log.Output($"Unpacked: {_zipFile}");

            // Success
            Default._progressBar1.Hide();
            Default._preparingPleaseWait.Hide();
            Default._status_Label.Text += $"[\u2713] {successfullyUpdated}\n";
            Log.Output(successfullyUpdated);


            // Check for updates again
            int foundUpdated = await CheckForUpdates.Analyze();
            if (foundUpdated == 0) Labels.ShowStartGameBtns();
            else Labels.HideStartGameBtns();
        }
    }
}
