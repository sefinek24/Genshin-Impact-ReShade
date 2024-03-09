using Newtonsoft.Json;
using StellaModLauncher.Properties;

namespace StellaModLauncher.Scripts.Remote;

/// <summary>
///    Class responsible for downloading and updating the FPS unlocker config file.
/// </summary>
internal static class FpsUnlockerCfg
{
	public static async Task RunAsync()
	{
		Program.Logger.Info("Downloading config file for FPS Unlocker...");

		await StartDownload().ConfigureAwait(true);
	}

	private static async Task StartDownload()
	{
		try
		{
			HttpResponseMessage response = await Program.SefinWebClient.GetAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/unlocker.config.json").ConfigureAwait(true);
			if (response.IsSuccessStatusCode)
			{
				Stream contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(true);
				StreamReader reader = new(contentStream);
				string json = await reader.ReadToEndAsync().ConfigureAwait(true);
				contentStream.Close();

				// Parse the JSON
				dynamic config = JsonConvert.DeserializeObject(json)!;

				// Replace the placeholder with the actual game path
				string? gamePath = await Utils.GetGame("giExe").ConfigureAwait(true);
				config.GamePath = gamePath!;

				// Serialize the updated JSON back to a string
				string updatedJson = JsonConvert.SerializeObject(config, Formatting.Indented);

				// Write the updated JSON to the config file
				await File.WriteAllTextAsync(Program.FpsUnlockerCfgPath, updatedJson).ConfigureAwait(true);

				// Update the status label to indicate successful completion
				Utils.UpdateStatusLabel(Resources.Default_DownloadedConfigFileForFPSUnlocker, Utils.StatusType.Success);
				Program.Logger.Info("Done.");
			}
			else
			{
				Utils.UpdateStatusLabel($"Download failed: {response.ReasonPhrase}", Utils.StatusType.Error);
				Program.Logger.Error($"Failed to download {Path.GetFileName(Program.FpsUnlockerCfgPath)} in {Path.GetDirectoryName(Program.FpsUnlockerCfgPath)}.\n\n{response.ReasonPhrase}");
			}
		}
		catch (Exception ex)
		{
			Utils.UpdateStatusLabel(ex.Message, Utils.StatusType.Error);
			Program.Logger.Error($"Failed to download {Path.GetFileName(Program.FpsUnlockerCfgPath)} in {Path.GetDirectoryName(Program.FpsUnlockerCfgPath)}.\n\n{ex}");
		}
	}
}
