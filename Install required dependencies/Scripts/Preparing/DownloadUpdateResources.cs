using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PrepareStella.Models;

namespace PrepareStella.Scripts.Preparing
{
    /// <summary>
    ///     Runs the resource download and preparation process.
    /// </summary>
    internal static class DownloadUpdateResources
    {
        public static async Task Run()
        {
            if (!Directory.Exists(Program.ResourcesGlobal))
            {
                Directory.CreateDirectory(Program.ResourcesGlobal);
                Console.WriteLine($@"Created folder: {Program.ResourcesGlobal}");
            }
            else
            {
                Console.WriteLine($@"Found: {Program.ResourcesGlobal}");
            }

            Console.WriteLine(@"Downloading presets and shaders...");

            WebClient wb = new WebClient();
            wb.Headers.Add("user-agent", Program.UserAgent);
            string res = await wb.DownloadStringTaskAsync("https://api.sefinek.net/api/v4/genshin-stella-mod/version/app/launcher/resources");
            StellaResources json = JsonConvert.DeserializeObject<StellaResources>(res);

            string zipPath = Path.Combine(Program.ResourcesGlobal, $"Stella resources - v{json.Message}.zip");
            using (WebClient webClient = new WebClient())
            {
                webClient.Headers.Add("user-agent", Program.UserAgent);
                await webClient.DownloadFileTaskAsync("https://github.com/sefinek24/Genshin-Stella-Resources/releases/latest/download/resources.zip", zipPath);
            }

            Console.WriteLine(@"Unpacking resources...");
            await ExtractZipFile(zipPath, Program.ResourcesGlobal);

            Console.WriteLine(@"Configuring ReShade...");
            ConfigureReShade(Program.ResourcesGlobal);

            string cache = Path.Combine(Program.ResourcesGlobal, "Cache");
            if (!Directory.Exists(cache))
            {
                Console.WriteLine(@"Creating cache folder...");
                Directory.CreateDirectory(cache);
            }
        }

        private static async Task ExtractZipFile(string zipPath, string destinationPath)
        {
            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string fullPath = Path.Combine(destinationPath, entry.FullName);

                    if (entry.Name == "")
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                        continue;
                    }

                    await Task.Run(() => entry.ExtractToFile(fullPath, true));
                }
            }
        }

        private static void ConfigureReShade(string resourcesGlobal)
        {
            string reshadeIniPath = Path.Combine(Program.GameDirGlobal, "ReShade.ini");

            string reShadeIniContent = File.ReadAllText(reshadeIniPath);
            string newData = reShadeIniContent?
                .Replace("{addon.path}", Path.Combine(resourcesGlobal, "Addons"))
                .Replace("{general.effects}", Path.Combine(resourcesGlobal, "Shaders", "Effects"))
                .Replace("{general.cache}", Path.Combine(resourcesGlobal, "Cache"))
                .Replace("{general.preset}", Path.Combine(resourcesGlobal, "Presets", "3. Preset by Sefinek - Medium settings [Default].ini"))
                .Replace("{general.textures}", Path.Combine(resourcesGlobal, "Shaders", "Textures"))
                .Replace("{screenshot.path}", Path.Combine(resourcesGlobal, "Screenshots"))
                .Replace("{screenshot.sound}", Path.Combine(Program.AppPath, "data", "sounds", "screenshot.wav"));

            File.WriteAllText(reshadeIniPath, newData);
        }
    }
}
