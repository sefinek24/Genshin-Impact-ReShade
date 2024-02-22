using System.IO.Compression;
using StellaModLauncher.Forms;
using StellaModLauncher.Properties;
using StellaModLauncher.Scripts.Forms.MainForm;
using StellaPLFNet;

namespace StellaModLauncher.Scripts.Remote;

internal static class Download
{
	private static DateTime _lastLogTime = DateTime.MinValue;

	public static void UpdateProgressBar(string progressText, int value, double bytesReceived, double totalBytes, double downloadSpeedInMb)
	{
		string receivedText = bytesReceived < 1024 * 1024 ? $"{bytesReceived / 1024:00.00} KB" : $"{bytesReceived / (1024 * 1024):00.00} MB";
		string totalText = totalBytes < 1024 * 1024 ? $"{totalBytes / 1024:00.00} KB" : $"{totalBytes / (1024 * 1024):00.00} MB";

		Default._progressBar1.Invoke(UpdateUi);

		if ((DateTime.Now - _lastLogTime).TotalSeconds >= 2)
		{
			Program.Logger.Info($"Downloading file... {receivedText} of {totalText} / {downloadSpeedInMb:0.00} MB/s");
			_lastLogTime = DateTime.Now;
		}

		TaskbarProgress.SetProgressValue((ulong)value);
		return;

		void UpdateUi()
		{
			Default._progressBar1.Value = value;
			Default._preparingPleaseWait.Text = $@"{string.Format(progressText, receivedText, totalText)} [{downloadSpeedInMb:00.00} MB/s]";
		}
	}

	public static async Task UnzipWithProgress(string? zipFilePath, string? extractPath)
	{
		FileInfo fileInfo = new(zipFilePath);
		if (!fileInfo.Exists || fileInfo.Length == 0)
		{
			string msg = !fileInfo.Exists
				? $"The downloaded file cannot be unzipped because it does not exist. Path: {zipFilePath}"
				: "The downloaded ZIP file is empty and cannot be unzipped. Please verify your antivirus software or check your internet connection.";
			Program.Logger.Error(msg);
			MessageBox.Show(msg, Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Error);
			File.Delete(fileInfo.FullName);
			Environment.Exit(fileInfo.Exists ? 412411 : 412412);
		}

		// Logger
		Program.Logger.Info($"Unpacking: {zipFilePath}");
		Program.Logger.Info($"Destination: {extractPath}");

		// Zip
		using ZipArchive archive = ZipFile.OpenRead(zipFilePath);
		int totalEntries = archive.Entries.Count;
		int currentEntry = 0;
		long totalBytesToExtract = archive.Entries.Sum(entry => entry.Length);
		long extractedBytes = 0;

		foreach (ZipArchiveEntry entry in archive.Entries)
		{
			string entryPath = Path.Combine(extractPath, entry.FullName);
			if (entry.FullName.EndsWith("/"))
			{
				Directory.CreateDirectory(entryPath);
			}
			else
			{
				Directory.CreateDirectory(Path.GetDirectoryName(entryPath));
				using Stream source = entry.Open();
				using FileStream destination = File.Create(entryPath);
				byte[] buffer = new byte[8192];
				int bytesRead;
				while ((bytesRead = await source.ReadAsync(buffer).ConfigureAwait(true)) > 0)
				{
					await destination.WriteAsync(buffer.AsMemory(0, bytesRead)).ConfigureAwait(true);
					extractedBytes += bytesRead;
					int progressPercentage = (int)((double)extractedBytes / totalBytesToExtract * 100);
					UpdateProgressUi(progressPercentage);
				}
			}

			currentEntry++;
		}

		Program.Logger.Info($"Successfully unpacked; totalEntries {totalEntries}; totalBytesExtracted: {extractedBytes}; totalBytesToExtract: {totalBytesToExtract};");
		Utils.AddLinkClickedEventHandler(Default._updates_LinkLabel, CheckForUpdates.CheckUpdates_Click);
		return;

		// Update UI
		void UpdateProgressUi(int progress)
		{
			Default._progressBar1.Value = progress;
			Default._preparingPleaseWait.Text = string.Format(Resources.StellaResources_UnpackingFiles_From_, currentEntry, totalEntries);
		}
	}
}
