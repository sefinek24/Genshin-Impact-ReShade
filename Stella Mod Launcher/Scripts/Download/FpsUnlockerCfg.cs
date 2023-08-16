using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using StellaLauncher.Forms;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts.Download
{
    /// <summary>
    ///     Class responsible for downloading and updating the FPS unlocker config file.
    /// </summary>
    internal static class FpsUnlockerCfg
    {
        /// <summary>
        ///     Starts the process of downloading and updating the FPS unlocker config file.
        /// </summary>
        public static async Task RunAsync()
        {
            Default._status_Label.Text += $"[i] {Resources.Default_DownloadingConfigFileForFPSUnlocker}\n";

            Log.Output("Downloading config file for FPS Unlocker...");

            await StartDownload();
        }

        private static async Task StartDownload()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("user-agent", Program.UserAgent);
                    client.DownloadFileCompleted += Client_DownloadFileCompleted;

                    await client.DownloadFileTaskAsync(new Uri("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/unlocker.config.json"), Program.FpsUnlockerCfgPath);
                }
            }
            catch (Exception ex)
            {
                Default._status_Label.Text += $"[x] {ex.Message}\n";
                Log.SaveError($"Failed to download {Path.GetFileName(Program.FpsUnlockerCfgPath)} in {Path.GetDirectoryName(Program.FpsUnlockerCfgPath)}.\n\n{ex}");
            }
        }

        private static async void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // Read the downloaded FPS unlocker config file
            string fpsUnlockerCfg = File.ReadAllText(Program.FpsUnlockerCfgPath);

            // Replace the placeholder "{GamePath}" with the actual game path
            string gamePath = await Utils.GetGame("giExe");
            fpsUnlockerCfg = fpsUnlockerCfg.Replace("{GamePath}", gamePath.Replace(@"\", @"\\"));

            // Write the updated FPS unlocker config file back to disk
            File.WriteAllText(Program.FpsUnlockerCfgPath, fpsUnlockerCfg);

            // Update the status label to indicate successful completion
            Default._status_Label.Text += $"[✓] {Resources.Default_Success}\n";
            Log.Output("Done.");
        }
    }
}
