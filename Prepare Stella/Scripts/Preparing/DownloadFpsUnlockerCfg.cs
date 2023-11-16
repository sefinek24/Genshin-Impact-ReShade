using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PrepareStella.Scripts.Preparing
{
    internal static class DownloadFpsUnlockerCfg
    {
        public static async Task RunAsync()
        {
            try
            {
                string unlockerFolderPath = Path.Combine(Start.AppPath, "data", "unlocker");
                Directory.CreateDirectory(unlockerFolderPath);

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("user-agent", Start.UserAgent);
                    string fpsUnlockerConfig = await httpClient.GetStringAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/unlocker.config.json");

                    string fpsUnlockerConfigPath = Path.Combine(unlockerFolderPath, "unlocker.config.json");
                    string gameExePath = Program.SavedGamePath?.Replace("\\", "\\\\");
                    string fpsUnlockerConfigContent = fpsUnlockerConfig.Replace("{GamePath}", gameExePath ?? string.Empty);

                    await WriteToFileAsync(fpsUnlockerConfigPath, fpsUnlockerConfigContent);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static async Task WriteToFileAsync(string filePath, string content)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                await writer.WriteAsync(content);
            }
        }
    }
}
