using ByteSizeLib;
using CliWrap.Builders;
using StellaModLauncher.Forms;
using StellaModLauncher.Properties;
using StellaModLauncher.Scripts.Forms.MainForm;
using StellaPLFNet;

namespace StellaModLauncher.Scripts.Remote;

internal static class NormalRelease
{
	public static readonly string SetupPathExe = Path.Combine(Path.GetTempPath(), "Stella_Mod_Update.exe");
	private static readonly string DownloadUrl = "https://github.com/sefinek24/Genshin-Impact-ReShade/releases/latest/download/Stella-Mod-Setup.exe";

	public static async void Run(string? remoteVersion, DateTime remoteVerDate, bool beta)
	{
		// 1
		Default._version_LinkLabel!.Text = $@"v{Program.AppFileVersion} → v{remoteVersion}";

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

		// Log
		Default._status_Label!.Text += $"[i] {string.Format(Resources.NormalRelease_NewVersionFrom_IsAvailable, remoteVerDate)}\n";
		Program.Logger.Info($"New release from {remoteVerDate} is available: v{Program.AppFileVersion} → v{remoteVersion} ({(beta ? "Beta" : "Stable")})");


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

				Default._status_Label.Text += $"[i] {string.Format(Resources.NormalRelease_UpdateSize, $"{updateSize} MB")}\n";
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

	private static async void Update_Event(object? sender, LinkLabelLinkClickedEventArgs e)
	{
		Program.Logger.Info(Resources.NormalRelease_PreparingToDownloadNewUpdate);
		TaskbarProgress.SetProgressState(TaskbarProgress.Flags.Indeterminate);

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
			Default._preparingPleaseWait!.Text = $@"😥 {Resources.NormalRelease_SomethingWentWrong}";
			Log.ThrowError(ex);
		}

		Program.Logger.Info($"Output: {SetupPathExe}");
	}

	private static async Task StartDownload()
	{
		Utils.RemoveLinkClickedEventHandler(Default._updates_LinkLabel!);

		if (File.Exists(SetupPathExe))
		{
			File.Delete(SetupPathExe);
			Default._status_Label!.Text += $"[✓] {Resources.NormalRelease_DeletedOldSetupFileFromTempDir}\n";
			Program.Logger.Info($"Deleted old setup file: {SetupPathExe}");
		}

		Program.Logger.Info(Resources.NormalRelease_DownloadingInProgress);
		TaskbarProgress.SetProgressState(TaskbarProgress.Flags.Normal);


		try
		{
			HttpResponseMessage response = await Program.WbClient.Value.GetAsync(DownloadUrl, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(true);
			response.EnsureSuccessStatusCode();

			long totalBytes = response.Content.Headers.ContentLength ?? 0;
			DateTime startTime = DateTime.Now;

			using Stream streamToReadFrom = await response.Content.ReadAsStreamAsync().ConfigureAwait(true);
			using FileStream streamToWriteTo = File.Open(SetupPathExe, FileMode.Create, FileAccess.Write, FileShare.None);
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
			DownloadFileCompleted();
		}
	}

	private static async void DownloadFileCompleted()
	{
		string logDir = Path.Combine(Log.Folder!, "updates");
		if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);

		// Wait 5 seconds
		Default._progressBar1!.Style = ProgressBarStyle.Continuous;
		for (int i = 5; i >= 0; i--)
		{
			Default._preparingPleaseWait!.Text = string.Format(Resources.NormalRelease_JustAMoment_PleaseWait, i);
			Program.Logger.Info($"Waiting {i}s...");

			double progressPercentage = (5 - i) / 5.0 * 100;
			Default._progressBar1.Value = (int)progressPercentage;

			await Task.Delay(1000).ConfigureAwait(true);
		}

		Default._preparingPleaseWait!.Text = Resources.NormalRelease_EverythingIsOkay_StartingSetup;
		TaskbarProgress.SetProgressState(TaskbarProgress.Flags.Indeterminate);

		await Task.Delay(500).ConfigureAwait(true);


		// Run setup
		string logFile = Path.Combine(logDir, $"{DateTime.Now:yyyy-dd-M...HH-mm-ss}.log");
		Cmd.CliWrap command = new()
		{
			App = SetupPathExe,
			Arguments = new ArgumentsBuilder()
				.Add("/UPDATE")
				.Add($"/LOG={logFile}")
				.Add("/NORESTART"),
			DownloadingSetup = true
		};
		_ = Cmd.Execute(command);


		// Save settings
		Program.Settings.WriteInt("Updates", "UpdateAvailable", 1);
		Program.Settings.WriteString("Updates", "OldVersion", Program.AppVersion);
		Program.Settings.Save();


		// Wait 16 seconds and close launcher
		Default._progressBar1.Style = ProgressBarStyle.Marquee;
		for (int i = 16; i >= 0; i--)
		{
			Default._preparingPleaseWait.Text = string.Format(Resources.NormalRelease_InstallANewVersionInTheWizard_ClosingLauncherIn_, i);
			Program.Logger.Info($"Closing launcher in {i}s...");
			await Task.Delay(1000).ConfigureAwait(true);
		}

		Program.Logger.Info("Closing...");
		Application.Exit();
	}
}
