using System.ComponentModel;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using StellaModLauncher.Forms;
using StellaModLauncher.Models;
using StellaModLauncher.Properties;
using StellaModLauncher.Scripts.Forms.MainForm;
using StellaModLauncher.Scripts.Remote;
using StellaPLFNet;

namespace StellaModLauncher.Scripts.Patrons;

internal static class UpdateBenefits
{
	private static string? _zipFile;
	private static string? _outputDir;
	private static string? _successfullyUpdated;
	private static readonly string BenefitsTempFile = Path.Combine(Default.ResourcesPath!, "Temp files");

	// Paths
	public static async void Start(string benefitName, string zipFilename, string? dirPathToUnpack)
	{
		if (!Directory.Exists(BenefitsTempFile))
		{
			Directory.CreateDirectory(BenefitsTempFile);
			Program.Logger.Info($"Created dir: {BenefitsTempFile}");
		}

		_zipFile = Path.Combine(BenefitsTempFile, zipFilename);
		if (File.Exists(_zipFile))
		{
			File.Delete(_zipFile);
			Program.Logger.Info($"Deleted file: {_zipFile}");
		}

		_outputDir = dirPathToUnpack;

		Program.Logger.Info($"Found the new version of: {benefitName}");

		Utils.RemoveLinkClickedEventHandler(Default._checkForUpdates_LinkLabel!);

		// Get update URL (mirror)
		string jsonResponse = await Program.SefinWebClient.GetStringAsync($"{Program.WebApi}/stella-mod-plus/benefits/update?benefitType={benefitName}").ConfigureAwait(true);
		GetUpdateUrl? data = JsonConvert.DeserializeObject<GetUpdateUrl>(jsonResponse);
		Program.Logger.Info($"The download url is ready; {data!.PreparedUrl}");


		// Switch info
		_successfullyUpdated = benefitName switch
		{
			"3dmigoto" => Resources.UpdateBenefits_SuccessfullyUpdated3DMigoto,
			"3dmigoto-mods" => Resources.UpdateBenefits_SuccessfullyUpdated3DMigotoMods,
			"addons" => Resources.UpdateBenefits_SuccessfullyUpdatedAddons,
			"presets" => Resources.UpdateBenefits_SuccessfullyUpdatedPresets,
			"shaders" => Resources.UpdateBenefits_SuccessfullyUpdatedShaders,
			"cmd" => Resources.UpdateBenefits_SuccessfullyUpdatedCmdFiles,
			_ => throw new InvalidEnumArgumentException()
		};

		Default._progressBar1!.Show();
		Default._preparingPleaseWait!.Show();


		// Run download
		await StartDownload($"{data.PreparedUrl}?benefitType={benefitName}").ConfigureAwait(true);

		Utils.AddLinkClickedEventHandler(Default._checkForUpdates_LinkLabel!, CheckForUpdates.CheckUpdates_Click);


		// Prepare presets
		if (benefitName != "presets") return;

		string? currentPreset = await ReShadeIni.Prepare().ConfigureAwait(false);
		if (currentPreset == null) return;

		BalloonTip.Show("ReShade configuration", $"The ReShade configuration file has also been updated, including setting the default preset to {Path.GetFileNameWithoutExtension(currentPreset)}.");
	}

	private static async Task StartDownload(string requestUri)
	{
		HttpClient client = Program.SefinWebClient;

		try
		{
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Secret.BearerToken);

			HttpResponseMessage response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(true);
			response.EnsureSuccessStatusCode();

			long totalBytes = response.Content.Headers.ContentLength ?? 0;
			DateTime startTime = DateTime.Now;

			using Stream streamToReadFrom = await response.Content.ReadAsStreamAsync().ConfigureAwait(true);
			using FileStream streamToWriteTo = File.Open(_zipFile!, FileMode.Create, FileAccess.Write, FileShare.None);
			byte[] buffer = new byte[8192];
			int bytesRead;
			long totalRead = 0;

			TaskbarProgress.SetProgressState(TaskbarProgress.Flags.Normal);

			while ((bytesRead = await streamToReadFrom.ReadAsync(buffer).ConfigureAwait(true)) > 0)
			{
				await streamToWriteTo.WriteAsync(buffer.AsMemory(0, bytesRead)).ConfigureAwait(true);
				totalRead += bytesRead;

				int progressPercentage = (int)((double)totalRead / totalBytes * 100);
				double downloadSpeedInMb = totalRead / (1024 * 1024) / (DateTime.Now - startTime).TotalSeconds;

				Download.UpdateProgressBar(Resources.NormalRelease_DownloadingUpdate_, progressPercentage, totalRead, totalBytes, downloadSpeedInMb);
			}
		}
		catch (HttpRequestException ex)
		{
			Program.Logger.Error($"HttpRequestException: {ex.Message}");
			MessageBox.Show($"{ex.Message}\n\nAuthorization server returned a different status code than expected. Please contact the developer.", Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Error);

			Environment.Exit(563211);
		}
		catch (Exception ex)
		{
			Program.Logger.Error($"Exception: {ex.Message}");
			MessageBox.Show($"An unexpected error occurred {ex.Message}\n\nPlease contact the application developer for assistance.", Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Error);

			Environment.Exit(563212);
		}
		finally
		{
			await Download.UnzipWithProgress(_zipFile, _outputDir).ConfigureAwait(true);

			Labels.HideProgressbar(_successfullyUpdated, false);

			await CheckForUpdates.Analyze().ConfigureAwait(true);
		}
	}
}
