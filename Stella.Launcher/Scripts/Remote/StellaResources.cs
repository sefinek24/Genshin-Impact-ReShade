using ByteSizeLib;
using StellaModLauncher.Forms;
using StellaModLauncher.Properties;
using StellaModLauncher.Scripts.Forms.MainForm;
using StellaPLFNet;

namespace StellaModLauncher.Scripts.Remote;

internal static class DownloadResources
{
	private const string DownloadUrl = "https://github.com/sefinek24/Genshin-Stella-Resources/releases/latest/download/resources.zip";
	private static string? _stellaResZip;

	public static async void Run(string? localResVersion, string? remoteResVersion, DateTime remoteResDate)
	{
		// 1
		Default._version_LinkLabel!.Text = $@"v{localResVersion} → v{remoteResVersion}";

		// 2
		Default._updates_LinkLabel!.LinkColor = Color.Cyan;
		Default._updates_LinkLabel.Text = Resources.NormalRelease_ClickHereToUpdate;
		Default._updateIco_PictureBox!.Image = Resources.icons8_download_from_the_cloud;

		Utils.RemoveLinkClickedEventHandler(Default._updates_LinkLabel);
		Utils.AddLinkClickedEventHandler(Default._updates_LinkLabel, Update_Event);

		// Hide and show elements
		Default._progressBar1!.Hide();
		Default._preparingPleaseWait!.Hide();
		Default._preparingPleaseWait.Text = Resources.NormalRelease_Preparing_IfProcessIsStuckReopenLauncher;

		Default._progressBar1.Value = 0;

		// BalloonTip
		BalloonTip.Show($"📥 {Resources.NormalRelease_WeFoundNewUpdates}", Resources.NormalRelease_NewReleaseIsAvailableDownloadNow);

		_stellaResZip = Path.Combine(Default.ResourcesPath!, $"Stella resources - v{remoteResVersion}.zip");

		// Log
		Default._status_Label!.Text += $"[i] {string.Format(Resources.StellaResources_NewResourcesUpdateIsAvailable, remoteResDate)}\n";
		Program.Logger.Info($"Found the new update of resources from {remoteResDate} - {remoteResDate}");

		// Check update size
		string? updateSize = null;
		try
		{
			HttpRequestMessage request = new(HttpMethod.Head, DownloadUrl);
			HttpResponseMessage response = await Program.WbClient.Value.SendAsync(request).ConfigureAwait(true);
			response.EnsureSuccessStatusCode();

			if (response.Content.Headers.ContentLength.HasValue)
			{
				updateSize = ByteSize.FromBytes(response.Content.Headers.ContentLength.Value).MegaBytes.ToString("00.00");

				Default._status_Label.Text += $"[i] {string.Format(Resources.StellaResources_UpdateSize, $"{updateSize} MB")}\n";
			}
			else
			{
				Default._status_Label.Text += "[i] Unknown file size.\n";
			}
		}
		catch (Exception ex)
		{
			Program.Logger.Error("Error while fetching update size", ex);
		}
		finally
		{
			TaskbarProgress.SetProgressState(TaskbarProgress.Flags.Paused);
			TaskbarProgress.SetProgressValue(100);
			Program.Logger.Info($"Update size: {updateSize} MB");
		}
	}

	private static async void Update_Event(object? sender, EventArgs e)
	{
		Program.Logger.Info(Resources.NormalRelease_PreparingToDownloadNewUpdate);
		TaskbarProgress.SetProgressState(TaskbarProgress.Flags.Normal);

		Default._updates_LinkLabel!.LinkColor = Color.DodgerBlue;
		Default._updates_LinkLabel.Text = Resources.NormalRelease_UpdatingPleaseWait;

		Labels.ShowProgressbar();

		try
		{
			Program.Logger.Info("Starting...");
			await StartDownload().ConfigureAwait(true);
		}
		catch (Exception ex)
		{
			Default._preparingPleaseWait!.Text = $@"): {Resources.NormalRelease_SomethingWentWrong}";
			Log.ThrowError(ex);
		}

		Program.Logger.Info($"Output: {_stellaResZip}");
	}

	private static async Task StartDownload()
	{
		if (File.Exists(_stellaResZip)) File.Delete(_stellaResZip);

		try
		{
			HttpResponseMessage response = await Program.WbClient.Value.GetAsync(DownloadUrl, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(true);
			response.EnsureSuccessStatusCode();

			long totalBytes = response.Content.Headers.ContentLength ?? 0;
			DateTime startTime = DateTime.Now;

			using Stream streamToReadFrom = await response.Content.ReadAsStreamAsync().ConfigureAwait(true);
			using FileStream streamToWriteTo = File.Open(_stellaResZip!, FileMode.Create, FileAccess.Write, FileShare.None);
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

				Download.UpdateProgressBar(Resources.StellaResources_DownloadingResources, progressPercentage, totalRead, totalBytes, downloadSpeedInMb);
			}
		}
		catch (Exception ex)
		{
			Log.ThrowError(ex);
		}
		finally
		{
			await Download.UnzipWithProgress(_stellaResZip, Default.ResourcesPath).ConfigureAwait(true);

			Labels.HideProgressbar(Resources.StellaResources_SuccessfullyUpdatedResources, false);

			await CheckForUpdates.Analyze().ConfigureAwait(true);
		}
	}
}
