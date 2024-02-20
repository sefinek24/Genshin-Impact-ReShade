using System.ComponentModel;
using System.Net;
using _8_2._Class_Library;
using ByteSizeLib;
using CliWrap.Builders;
using StellaModLauncherNET.Forms;
using StellaModLauncherNET.Properties;

namespace StellaModLauncherNET.Scripts.Download;

internal static class NormalRelease
{
	// Files
	public static readonly string SetupPathExe = Path.Combine(Path.GetTempPath(), "Stella_Mod_Update.exe");
	private static readonly string DownloadUrl = "https://github.com/sefinek24/Genshin-Impact-ReShade/releases/latest/download/Stella-Mod-Setup.exe";

	// Download
	private static double _downloadSpeed;
	private static long _lastBytesReceived;
	private static DateTime _lastUpdateTime = DateTime.Now;

	public static async void Run(string remoteVersion, DateTime remoteVerDate, bool beta)
	{
		// 1
		Default._version_LinkLabel.Text = $@"v{Program.ProductVersion} → v{remoteVersion}";

		// 2
		Default._updates_LinkLabel.LinkColor = Color.Cyan;
		Default._updates_LinkLabel.Text = Resources.NormalRelease_ClickHereToUpdate;
		Default._updateIco_PictureBox.Image = Resources.icons8_download_from_the_cloud;
		Utils.RemoveClickEvent(Default._updates_LinkLabel);
		Default._updates_LinkLabel.Click += Update_Event;

		// Hide and show elements
		Default._progressBar1.Hide();
		Default._preparingPleaseWait.Hide();
		Default._preparingPleaseWait.Text = Resources.NormalRelease_Preparing_IfProcessIsStuckReopenLauncher;

		Default._discordServerIco_Picturebox.Show();
		Default._discordServer_LinkLabel.Show();
		Default._supportMeIco_PictureBox.Show();
		Default._supportMe_LinkLabel.Show();
		Default._youtubeIco_Picturebox.Show();

		Default._progressBar1.Value = 0;

		// BalloonTip
		BalloonTip.Show($"📥 {Resources.NormalRelease_WeFoundNewUpdates}", Resources.NormalRelease_NewReleaseIsAvailableDownloadNow);

		// Log
		Default._status_Label.Text += $"[i] {string.Format(Resources.NormalRelease_NewVersionFrom_IsAvailable, remoteVerDate)}\n";
		Program.Logger.Info($"New release from {remoteVerDate} is available: v{Program.ProductVersion} → v{remoteVersion} ({(beta ? "Beta" : "Stable")})");

		// Taskbar
		TaskbarProgress.SetProgressValue(100);

		// Check update size
		using (WebClient wc = new())
		{
			wc.Headers.Add("User-Agent", Program.UserAgent);
			await wc.OpenReadTaskAsync(DownloadUrl);
			string updateSize = ByteSize.FromBytes(Convert.ToInt64(wc.ResponseHeaders["Content-Length"])).MegaBytes.ToString("00.00");
			Default._status_Label.Text += $"[i] {string.Format(Resources.StellaResources_UpdateSize, $"{updateSize} MB")}\n";

			// Final
			TaskbarProgress.SetProgressState(TaskbarProgress.Flags.Paused);
			Program.Logger.Info($"Update size: {updateSize} MB");
		}
	}

	private static async void Update_Event(object sender, EventArgs e)
	{
		Program.Logger.Info(Resources.NormalRelease_PreparingToDownloadNewUpdate);
		TaskbarProgress.SetProgressState(TaskbarProgress.Flags.Indeterminate);

		Default._updates_LinkLabel.LinkColor = Color.DodgerBlue;
		Default._updates_LinkLabel.Text = Resources.NormalRelease_UpdatingPleaseWait;
		Utils.RemoveClickEvent(Default._updates_LinkLabel);

		Default._progressBar1.Show();
		Default._preparingPleaseWait.Show();

		Default._progressBar1.Show();
		Default._preparingPleaseWait.Show();

		Default._discordServerIco_Picturebox.Hide();
		Default._discordServer_LinkLabel.Hide();
		Default._supportMeIco_PictureBox.Hide();
		Default._supportMe_LinkLabel.Hide();
		Default._youtubeIco_Picturebox.Hide();
		Default._youTube_LinkLabel.Hide();

		try
		{
			Program.Logger.Info("Starting...");
			await StartDownload();
		}
		catch (Exception ex)
		{
			Default._preparingPleaseWait.Text = $@"😥 {Resources.NormalRelease_SomethingWentWrong}";
			Log.ThrowError(ex);
		}

		Program.Logger.Info($"Output: {SetupPathExe}");
	}


	private static async Task StartDownload()
	{
		if (File.Exists(SetupPathExe))
		{
			File.Delete(SetupPathExe);
			Default._status_Label.Text += $"[✓] {Resources.NormalRelease_DeletedOldSetupFileFromTempDir}\n";
			Program.Logger.Info($"Deleted old setup file: {SetupPathExe}");
		}

		Program.Logger.Info(Resources.NormalRelease_DownloadingInProgress);
		TaskbarProgress.SetProgressState(TaskbarProgress.Flags.Normal);

		using (WebClient client = new())
		{
			client.Headers.Add("User-Agent", Program.UserAgent);
			client.DownloadProgressChanged += Client_DownloadProgressChanged;
			client.DownloadFileCompleted += Client_DownloadFileCompleted;
			await client.DownloadFileTaskAsync(new Uri(DownloadUrl), SetupPathExe);
		}
	}

	private static void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
	{
		int progress = (int)Math.Floor(e.BytesReceived * 100.0 / e.TotalBytesToReceive);
		Default._progressBar1.Value = progress;

		TaskbarProgress.SetProgressValue((ulong)progress);

		DateTime currentTime = DateTime.Now;
		TimeSpan elapsedTime = currentTime - _lastUpdateTime;
		long bytesReceived = e.BytesReceived - _lastBytesReceived;

		if (!(elapsedTime.TotalMilliseconds > 1000)) return;

		_lastUpdateTime = currentTime;
		_lastBytesReceived = e.BytesReceived;

		double bytesReceivedMb = ByteSize.FromBytes(e.BytesReceived).MegaBytes;
		double bytesReceiveMb = ByteSize.FromBytes(e.TotalBytesToReceive).MegaBytes;

		_downloadSpeed = bytesReceived / elapsedTime.TotalSeconds;
		double downloadSpeedInMb = _downloadSpeed / (1024 * 1024);

		Default._preparingPleaseWait.Text = $@"{string.Format(Resources.NormalRelease_DownloadingUpdate_, $"{bytesReceivedMb:00.00}", $"{bytesReceiveMb:000.00}")} [{downloadSpeedInMb:00.00} MB/s]";

		Program.Logger.Info($"Downloading new update... {bytesReceivedMb:00.00} MB of {bytesReceiveMb:000.00} MB / {downloadSpeedInMb:00.00} MB/s");
	}

	private static async void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
	{
		string logDir = Path.Combine(Log.Folder, "updates");
		if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);

		// Wait 5 seconds
		Default._progressBar1.Style = ProgressBarStyle.Continuous;
		for (int i = 5; i >= 0; i--)
		{
			Default._preparingPleaseWait.Text = string.Format(Resources.NormalRelease_JustAMoment_PleaseWait, i);
			Program.Logger.Info($"Waiting {i}s...");

			double progressPercentage = (5 - i) / 5.0 * 100;
			Default._progressBar1.Value = (int)progressPercentage;

			await Task.Delay(1000);
		}

		Default._preparingPleaseWait.Text = Resources.NormalRelease_EverythingIsOkay_StartingSetup;
		TaskbarProgress.SetProgressState(TaskbarProgress.Flags.Indeterminate);

		await Task.Delay(500);


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
			await Task.Delay(1000);
		}

		Program.Logger.Info("Closing...");
		Application.Exit();
	}
}
