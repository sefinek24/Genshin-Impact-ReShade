using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace PrepareStella.Scripts.Preparing
{
    /// <summary>
    ///     Downloads and updates the FPS Unlocker configuration file.
    /// </summary>
    internal static class DownloadFpsUnlockerCfg
    {
        public static async Task Run()
        {
            Console.WriteLine(@"Updating FPS Unlocker configuration...");

            try
            {
                string unlockerFolderPath = Path.Combine(Program.AppPath, "data", "unlocker");
                if (!Directory.Exists(unlockerFolderPath))
                    Directory.CreateDirectory(unlockerFolderPath);

                string fpsUnlockerConfig;
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("user-agent", Program.UserAgent);
                    fpsUnlockerConfig = await client.DownloadStringTaskAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/unlocker.config.json");
                }

                string fpsUnlockerConfigPath = Path.Combine(unlockerFolderPath, "unlocker.config.json");

                string gameExePath = Program.GameExeGlobal?.Replace("\\", "\\\\");
                string fpsUnlockerConfigContent = fpsUnlockerConfig.Replace("{GamePath}", gameExePath ?? string.Empty);

                File.WriteAllText(fpsUnlockerConfigPath, fpsUnlockerConfigContent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
