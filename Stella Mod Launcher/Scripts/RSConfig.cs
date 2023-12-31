using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using StellaLauncher.Forms;

namespace StellaLauncher.Scripts
{
    internal static class RsConfig
    {
        public static async Task<string> Prepare()
        {
            if (!Directory.Exists(Default.ResourcesPath))
            {
                MessageBox.Show(@"Stella Resources was not found.", Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            string gameDir = await Utils.GetGame("giGameDir");
            string reShadePath = Path.Combine(gameDir, "ReShade.ini");
            if (!File.Exists(reShadePath)) return null;

            // Presets
            string defaultPreset = Path.Combine(Default.ResourcesPath, "ReShade", "Presets", "1. Default preset - Medium settings.ini");
            string rtPreset = Path.Combine(Default.ResourcesPath, "ReShade", "Presets", "3. Only for patrons", "Ray Tracing", "1. RT by Sefinek - Medium settings #1 [Default]                     .ini");
            string currentPreset;

            // Dirs
            string screenshots = Path.Combine(Default.ResourcesPath, "Screenshots");

            IniFile ini = new IniFile(reShadePath);
            ini.WriteString("ADDON", "AddonPath", $"{Path.Combine(Default.ResourcesPath, "ReShade", "Addons")}");
            ini.WriteString("GENERAL", "EffectSearchPaths", Path.Combine(Default.ResourcesPath, "ReShade", "Shaders", "Effects"));
            ini.WriteString("GENERAL", "IntermediateCachePath", Path.Combine(Default.ResourcesPath, "ReShade", "Cache"));

            if (Secret.IsMyPatron)
            {
                if (File.Exists(rtPreset))
                {
                    ini.WriteString("GENERAL", "PresetPath", rtPreset);
                    currentPreset = rtPreset;
                }
                else
                {
                    MessageBox.Show($@"Preset for Patrons '{Path.GetFileNameWithoutExtension(rtPreset).Trim()}' was not found. We will set the default preset for ReShade.", Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ini.WriteString("GENERAL", "PresetPath", defaultPreset);
                    currentPreset = defaultPreset;
                }
            }
            else
            {
                ini.WriteString("GENERAL", "PresetPath", defaultPreset);
                currentPreset = defaultPreset;
            }

            ini.WriteString("GENERAL", "TextureSearchPaths", Path.Combine(Default.ResourcesPath, "ReShade", "Shaders", "Textures"));
            ini.WriteString("SCREENSHOT", "SavePath", screenshots);
            ini.WriteString("SCREENSHOT", "SoundPath", Path.Combine(Program.AppPath, "data", "sounds", "screenshot.wav"));
            ini.Save();

            if (!Directory.Exists(screenshots)) Directory.CreateDirectory(screenshots);

            return currentPreset;
        }
    }
}
