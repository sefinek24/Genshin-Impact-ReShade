using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PrepareStella.Scripts.Preparing
{
    internal static class UpdateReShadeCfg
    {
        public static async Task RunAsync()
        {
            string giGame = Path.GetDirectoryName(Program.SavedGamePath);
            string reshadeIniPath = Path.Combine(giGame, "ReShade.ini");
            string reshadeLogPath = Path.Combine(giGame, "ReShade.log");

            if (!Directory.Exists(giGame))
            {
                Console.WriteLine(@"You must configure some settings manually.");
                return;
            }

            await DownloadFileAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/reshade/ReShade.ini", reshadeIniPath);
            await DownloadFileAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/reshade/ReShade.log", reshadeLogPath);

            if (!File.Exists(reshadeIniPath) || !File.Exists(reshadeLogPath)) Log.ErrorAndExit(new Exception($"Something went wrong. Config or log file for ReShade was not found in: {giGame}"), false, false);

            Log.Output($"{Path.GetFileName(reshadeIniPath)} and {Path.GetFileName(reshadeLogPath)} were successfully downloaded.");

            string cache = Path.Combine(Program.ResourcesGlobal, "ReShade", "Cache");
            if (!Directory.Exists(cache))
            {
                Console.WriteLine(@"Creating cache folder...");
                Directory.CreateDirectory(cache);
            }

            Console.WriteLine(@"Configuring ReShade...");
            ConfigureReShade(Program.ResourcesGlobal);
        }

        private static async Task DownloadFileAsync(string url, string filePath)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("user-agent", Program.UserAgent);
                using (HttpResponseMessage response = await httpClient.GetAsync(url))
                {
                    if (response.IsSuccessStatusCode)
                        using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                        using (FileStream fileStream = File.Create(filePath))
                        {
                            await contentStream.CopyToAsync(fileStream);
                        }
                }
            }
        }

        private static void ConfigureReShade(string resourcesGlobal)
        {
            bool isMyPatron = CheckData.IsUserMyPatron();
            string reshadeIniFilePath = Path.Combine(Path.GetDirectoryName(Program.SavedGamePath), "ReShade.ini");
            IniFile ini = new IniFile(reshadeIniFilePath);

            string addonsPath = Path.Combine(resourcesGlobal, "ReShade", "Addons");
            string effectsPath = Path.Combine(resourcesGlobal, "ReShade", "Shaders", "Effects");
            string cachePath = Path.Combine(resourcesGlobal, "ReShade", "Cache");
            string presetsPath = isMyPatron ? string.Empty : Path.Combine(resourcesGlobal, "ReShade", "Presets", "3. Preset by Sefinek - Medium settings [Default].ini");
            string texturesPath = Path.Combine(resourcesGlobal, "ReShade", "Shaders", "Textures");
            string screenshotsPath = Path.Combine(resourcesGlobal, "Screenshots");
            string soundPath = Path.Combine(Program.AppPath, "data", "sounds", "screenshot.wav");

            ini.WriteString("ADDON", "AddonPath", addonsPath);
            ini.WriteString("GENERAL", "EffectSearchPaths", effectsPath);
            ini.WriteString("GENERAL", "IntermediateCachePath", cachePath);
            if (!string.IsNullOrEmpty(presetsPath)) ini.WriteString("GENERAL", "PresetPath", presetsPath);
            ini.WriteString("GENERAL", "TextureSearchPaths", texturesPath);
            ini.WriteString("SCREENSHOT", "SavePath", screenshotsPath);
            ini.WriteString("SCREENSHOT", "SoundPath", soundPath);
            ini.Save();
        }
    }
}
