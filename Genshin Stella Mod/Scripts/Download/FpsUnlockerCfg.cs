using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts.Download
{
    /// <summary>
    ///     Class responsible for downloading and updating the FPS unlocker config file.
    /// </summary>
    internal static class FpsUnlockerCfg
    {
        private static Label _statusLabel;

        /// <summary>
        ///     Starts the process of downloading and updating the FPS unlocker config file.
        /// </summary>
        /// <param name="statusLabel">Label control to display the download status.</param>
        public static async void Run(Label statusLabel)
        {
            Log.Output(Resources.Default_DownloadingConfigFileForFPSUnlocker);

            statusLabel.Text += $"[i] {Resources.Default_DownloadingConfigFileForFPSUnlocker}\n";
            _statusLabel = statusLabel;

            await StartDownload();
        }

        private static async Task StartDownload()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("user-agent", Program.UserAgent);
                    client.DownloadProgressChanged += Client_DownloadProgressChanged;
                    client.DownloadFileCompleted += Client_DownloadFileCompleted;

                    await client.DownloadFileTaskAsync(new Uri("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/unlocker.config.json"), Program.FpsUnlockerCfgPath);
                }
            }
            catch (Exception ex)
            {
                _statusLabel.Text += $"[x] {ex.Message}\n";
                Log.SaveError(Path.Combine(Resources.Default_FailedToDownloadUnlockerConfigJson, ex.ToString()));
            }
        }

        private static void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // _statusLabel.Text += $"[i] {Resources.Main_PleaseWait}\n";
        }

        private static void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // Read the downloaded FPS unlocker config file
            string fpsUnlockerCfg = File.ReadAllText(Program.FpsUnlockerCfgPath);

            // Replace the placeholder "{GamePath}" with the actual game path
            fpsUnlockerCfg = fpsUnlockerCfg.Replace("{GamePath}", @"C:\\Program Files\\Genshin Impact\\Genshin Impact game\\GenshinImpact.exe");

            // Write the updated FPS unlocker config file back to disk
            File.WriteAllText(Program.FpsUnlockerCfgPath, fpsUnlockerCfg);

            // Update the status label to indicate successful completion
            _statusLabel.Text += $"[✓] {Resources.Default_Success}\n";

            Log.Output(Resources.Default_Done);
        }
    }
}
