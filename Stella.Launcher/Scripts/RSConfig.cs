using StellaModLauncher.Forms;

namespace StellaModLauncher.Scripts;

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

		// Dirs
		string screenshots = Path.Combine(Default.ResourcesPath, "Screenshots");

		IniFile ini = new(reShadePath);
		ini.WriteString("ADDON", "AddonPath", $"{Path.Combine(Default.ResourcesPath, "ReShade", "Addons")}");
		ini.WriteString("GENERAL", "EffectSearchPaths", Path.Combine(Default.ResourcesPath, "ReShade", "Shaders", "Effects"));
		ini.WriteString("GENERAL", "IntermediateCachePath", Path.Combine(Default.ResourcesPath, "ReShade", "Cache"));

		ini.WriteString("GENERAL", "PresetPath", defaultPreset);

		ini.WriteString("GENERAL", "TextureSearchPaths", Path.Combine(Default.ResourcesPath, "ReShade", "Shaders", "Textures"));
		ini.WriteString("SCREENSHOT", "SavePath", screenshots);
		ini.WriteString("SCREENSHOT", "SoundPath", Path.Combine(Program.AppPath, "data", "sounds", "screenshot.wav"));
		ini.Save();

		if (!Directory.Exists(screenshots)) Directory.CreateDirectory(screenshots);

		return defaultPreset;
	}
}
