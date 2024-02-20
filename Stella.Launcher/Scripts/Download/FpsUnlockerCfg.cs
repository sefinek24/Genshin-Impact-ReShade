using Newtonsoft.Json;
using StellaModLauncher.Forms;
using StellaModLauncher.Properties;

namespace StellaModLauncher.Scripts.Download;

/// <summary>
///    Class responsible for downloading and updating the FPS unlocker config file.
/// </summary>
internal static class FpsUnlockerCfg
{
	public static async Task RunAsync()
	{
		Program.Logger.Info("Downloading config file for FPS Unlocker...");

		await StartDownload();
	}

	private static async Task StartDownload()
	{
		try
		{
			HttpResponseMessage response = await Program.SefinWebClient.GetAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/unlocker.config.json");
			if (response.IsSuccessStatusCode)
			{
				Stream contentStream = await response.Content.ReadAsStreamAsync();
				StreamReader reader = new(contentStream);
				string json = await reader.ReadToEndAsync();
				contentStream.Close();

				// Parse the JSON
				dynamic config = JsonConvert.DeserializeObject(json);

				// Replace the placeholder with the actual game path
				string gamePath = await Utils.GetGame("giExe");
				config.GamePath = gamePath;

				// Serialize the updated JSON back to a string
				string updatedJson = JsonConvert.SerializeObject(config, Formatting.Indented);

				// Write the updated JSON to the config file
				File.WriteAllText(Program.FpsUnlockerCfgPath, updatedJson);

				// Update the status label to indicate successful completion
				Default._status_Label.Text += $"[✓] {Resources.Default_DownloadedConfigFileForFPSUnlocker}\n";
				Program.Logger.Info("Done.");
			}
			else
			{
				Default._status_Label.Text += $"[x] Download failed: {response.ReasonPhrase}\n";
				Program.Logger.Error($"Failed to download {Path.GetFileName(Program.FpsUnlockerCfgPath)} in {Path.GetDirectoryName(Program.FpsUnlockerCfgPath)}.\n\n{response.ReasonPhrase}");
			}
		}
		catch (Exception ex)
		{
			Default._status_Label.Text += $"[x] {ex.Message}\n";
			Program.Logger.Error($"Failed to download {Path.GetFileName(Program.FpsUnlockerCfgPath)} in {Path.GetDirectoryName(Program.FpsUnlockerCfgPath)}.\n\n{ex}");
		}
	}
}
