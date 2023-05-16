using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Prepare_mod.Forms;
using Prepare_mod.Properties;

namespace Prepare_mod.Scripts.Preparing
{
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

            string zipPath = $@"{Program.ResourcesGlobal}\Backup.zip";
            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", Program.UserAgent);
            await webClient.DownloadFileTaskAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/reshade/zip/resources.zip", zipPath);


            Console.WriteLine(@"Unpacking resources...");
            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string fullPath = Path.Combine(Program.ResourcesGlobal, entry.FullName);

                    if (entry.Name == "")
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                        continue;
                    }

                    await Task.Run(() => entry.ExtractToFile(fullPath, true));
                }
            }


            Console.WriteLine(@"Configuring ReShade...");

            string cache = Path.Combine(Program.ResourcesGlobal, "Cache");

            string reShadeIniFile = File.ReadAllText(Program.ReShadeConfig);
            string reShadeData = reShadeIniFile?
                .Replace("{addon.path}", Path.Combine(Program.ResourcesGlobal, "Addons"))
                .Replace("{general.effects}", Path.Combine(Program.ResourcesGlobal, "Shaders", "Effects"))
                .Replace("{general.cache}", cache)
                .Replace("{general.preset}", Path.Combine(Program.ResourcesGlobal, "Presets", "3. Preset by Sefinek - Medium settings [Default].ini"))
                .Replace("{general.textures}", Path.Combine(Program.ResourcesGlobal, "Shaders", "Textures"))
                .Replace("{screenshot.path}", Path.Combine(Program.ResourcesGlobal, "Screenshots"))
                .Replace("{screenshot.sound}", Path.Combine(Program.ResourcesGlobal, "data", "sounds", "screenshot.wav"));

            File.WriteAllText(Program.ReShadeConfig, reShadeData);

            if (!Directory.Exists(cache))
            {
                Console.WriteLine(@"Creating cache folder...");
                Directory.CreateDirectory(cache);
            }
        }
    }
}
