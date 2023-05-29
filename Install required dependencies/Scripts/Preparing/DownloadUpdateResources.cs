using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using PrepareStella.Forms;
using PrepareStella.Properties;

namespace PrepareStella.Scripts.Preparing
{
    /// <summary>
    ///     Runs the resource download and preparation process.
    /// </summary>
    internal static class DownloadUpdateResources
    {
        public static async Task Run()
        {
            Console.WriteLine(@"Checking Stella resources...");

            string resourcesPath = Path.Combine(Program.AppData, "resources-path.sfn");
            if (File.Exists(resourcesPath))
            {
                string fileWithGamePath = File.ReadAllText(resourcesPath).Trim();
                if (Directory.Exists(fileWithGamePath))
                {
                    Program.ResourcesGlobal = fileWithGamePath;
                }
                else
                {
                    Console.WriteLine($@"Not found in: {fileWithGamePath}");
                    Application.Run(new SelectShadersPath { Icon = Resources.icon });
                }
            }
            else
            {
                Application.Run(new SelectShadersPath { Icon = Resources.icon });
            }

            if (!Directory.Exists(Program.ResourcesGlobal) && Program.ResourcesGlobal != null)
            {
                Directory.CreateDirectory(Program.ResourcesGlobal);
                Console.WriteLine($@"Created folder: {Program.ResourcesGlobal}");
            }
            else
            {
                Console.WriteLine($@"Found: {Program.ResourcesGlobal}");
            }

            Console.WriteLine(@"Downloading presets and shaders...");

            string zipPath = Path.Combine(Program.ResourcesGlobal, "Resources - Backup.zip");
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
            string reShadeIniFile = File.ReadAllText(Program.ReShadeConfig);
            string reShadeData = reShadeIniFile?
                .Replace("{addon.path}", Path.Combine(resourcesGlobal, "Addons"))
                .Replace("{general.effects}", Path.Combine(resourcesGlobal, "Shaders", "Effects"))
                .Replace("{general.cache}", Path.Combine(resourcesGlobal, "Cache"))
                .Replace("{general.preset}", Path.Combine(resourcesGlobal, "Presets", "3. Preset by Sefinek - Medium settings [Default].ini"))
                .Replace("{general.textures}", Path.Combine(resourcesGlobal, "Shaders", "Textures"))
                .Replace("{screenshot.path}", Path.Combine(resourcesGlobal, "Screenshots"))
                .Replace("{screenshot.sound}", Path.Combine(Program.AppPath, "data", "sounds", "screenshot.wav"));

            File.WriteAllText(Program.ReShadeConfig, reShadeData);
        }
    }
}
