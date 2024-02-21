using StellaModLauncher.Forms;
using StellaModLauncher.Properties;
using StellaModLauncher.Scripts.Forms.MainForm;

namespace StellaModLauncher.Scripts;

internal static class ReShadeIni
{
	private const string UrlCdn = "https://cdn.sefinek.net/resources/v3/genshin-stella-mod/reshade/ReShade.ini";

	public static async Task CheckIfExists()
	{
		string? gameDir = await Utils.GetGame("giGameDir").ConfigureAwait(true);
		string reShadePath = Path.Combine(gameDir!, "ReShade.ini");
		if (File.Exists(reShadePath)) return;

		try
		{
			HttpResponseMessage response = await Program.SefinWebClient.GetAsync(UrlCdn).ConfigureAwait(true);

			await using (Stream contentStream = await response.Content.ReadAsStreamAsync(), fileStream = new FileStream(reShadePath, FileMode.Create, FileAccess.Write))
			{
				await contentStream.CopyToAsync(fileStream).ConfigureAwait(true);
			}

			await Prepare().ConfigureAwait(true);

			Default._status_Label.Text += $"[✓] {Resources.Default_SuccessfullyDownloadedReShadeCfg}\n";
			Program.Logger.Info($"Successfully downloaded ReShade.ini and saved in: {reShadePath}");

			await CheckForUpdates.Analyze().ConfigureAwait(true);
		}
		catch (Exception ex)
		{
			Default._status_Label.Text += $"[x] {Resources.Default_Meeow_FailedToDownloadReShadeIni_TryAgain}\n";
			Program.Logger.Error(ex.ToString());
			if (!File.Exists(reShadePath)) Program.Logger.Info(Resources.Default_TheReShadeIniFileStillDoesNotExist);
		}
	}

	public static async Task<string?> Prepare()
	{
		if (!Directory.Exists(Default.ResourcesPath))
		{
			MessageBox.Show(@"Stella Resources was not found.", Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return null;
		}

		string? gameDir = await Utils.GetGame("giGameDir").ConfigureAwait(false);
		string reShadePath = Path.Combine(gameDir!, "ReShade.ini");
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
