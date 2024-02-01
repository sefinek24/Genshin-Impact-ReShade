using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;
using ByteSizeLib;
using Microsoft.WindowsAPICodePack.Taskbar;
using Newtonsoft.Json;
using StellaLauncher.Forms;
using StellaLauncher.Models;
using StellaLauncher.Properties;
using StellaLauncher.Scripts.Download;
using StellaLauncher.Scripts.Forms.MainForm;

namespace StellaLauncher.Scripts.Patrons
{
	internal static class UpdateBenefits
	{
		// Download
		private static double _downloadSpeed;
		private static long _lastBytesReceived;
		private static DateTime _lastUpdateTime = DateTime.Now;

		private static string _zipFile;
		private static string _outputDir;
		private static string _successfullyUpdated;
		private static readonly string BenefitsTempFile = Path.Combine(Default.ResourcesPath, "Temp files");

		// Paths
		public static async void Download(string benefitName, string zipFilename, string dirPathToUnpack)
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


			// Get update URL (mirror)
			string jsonResponse = await Program.SefinWebClient.GetStringAsync($"{Program.WebApi}/genshin-stella-mod/patrons/benefits/update?benefitType={benefitName}");
			GetUpdateUrl data = JsonConvert.DeserializeObject<GetUpdateUrl>(jsonResponse);
			Program.Logger.Info($"The download url is ready; {data.PreparedUrl}");


			// Switch info
			switch (benefitName)
			{
				case "3dmigoto":
					_successfullyUpdated = Resources.UpdateBenefits_SuccessfullyUpdated3DMigoto;
					break;
				case "3dmigoto-mods":
					_successfullyUpdated = Resources.UpdateBenefits_SuccessfullyUpdated3DMigotoMods;
					break;
				case "addons":
					_successfullyUpdated = Resources.UpdateBenefits_SuccessfullyUpdatedAddons;
					break;
				case "presets":
					_successfullyUpdated = Resources.UpdateBenefits_SuccessfullyUpdatedPresets;
					break;
				case "shaders":
					_successfullyUpdated = Resources.UpdateBenefits_SuccessfullyUpdatedShaders;
					break;
				case "cmd":
					_successfullyUpdated = Resources.UpdateBenefits_SuccessfullyUpdatedCmdFiles;
					break;
				default:
					throw new InvalidEnumArgumentException();
			}

			Default._progressBar1.Show();
			Default._preparingPleaseWait.Show();


			// Run download
			await DownloadFileAsync($"{data.PreparedUrl}?benefitType={benefitName}", _zipFile);

			// Prepare presets
			if (benefitName != "presets") return;

			string currentPreset = await RsConfig.Prepare();
			if (currentPreset == null) return;

			BalloonTip.Show("ReShade configuration", $"The ReShade configuration file has also been updated, including setting the default preset to {Path.GetFileNameWithoutExtension(currentPreset)}.");
		}

		private static async Task DownloadFileAsync(string requestUri, string filename)
		{
			HttpClient client = Program.WbClient.Value;

			try
			{
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Secret.BearerToken);

				HttpResponseMessage response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead);
				response.EnsureSuccessStatusCode();

				long totalBytes = response.Content.Headers.ContentLength.GetValueOrDefault(-1L);
				long totalBytesRead = 0L;
				long readCount = 0L;
				byte[] buffer = new byte[8192];
				bool isMoreToRead = true;

				using (FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
				{
					using (Stream contentStream = await response.Content.ReadAsStreamAsync())
					{
						do
						{
							int bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
							if (bytesRead == 0)
							{
								isMoreToRead = false;
								continue;
							}

							await fileStream.WriteAsync(buffer, 0, bytesRead);

							totalBytesRead += bytesRead;
							readCount += 1;

							if (readCount % 100 == 0) UpdateUi(totalBytesRead, totalBytes);
						} while (isMoreToRead);
					}
				}

				if (totalBytesRead != totalBytes) throw new IOException($"Expected {totalBytes} bytes but got {totalBytesRead} bytes.");
			}
			catch (HttpRequestException ex)
			{
				Program.Logger.Error($"HttpRequestException: {ex.Message}");
				MessageBox.Show($"{ex.Message}\n\nAuthorization server returned a different status code than expected. Please contact the developer.",
					Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Error);

				Environment.Exit(563211);
			}
			catch (Exception ex)
			{
				Program.Logger.Error($"Exception: {ex.Message}");
				MessageBox.Show($"An unexpected error occurred {ex.Message}\n\nPlease contact the application developer for assistance.",
					Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Error);
				Environment.Exit(563212);
			}
			finally
			{
				client.DefaultRequestHeaders.Authorization = null;
			}


			Client_DownloadFileCompleted(null, null);
		}

		private static void UpdateUi(long bytesRead, long totalBytes)
		{
			int progress = (int)(bytesRead * 100 / totalBytes);
			Default._progressBar1.Value = progress;
			TaskbarManager.Instance.SetProgressValue(progress, 100);

			DateTime currentTime = DateTime.Now;
			TimeSpan elapsedTime = currentTime - _lastUpdateTime;
			long bytesReceivedSinceLastUpdate = bytesRead - _lastBytesReceived;

			if (elapsedTime.TotalMilliseconds <= 1000) return;

			_lastUpdateTime = currentTime;
			_lastBytesReceived = bytesRead;

			double bytesReceivedMb = ByteSize.FromBytes(bytesRead).MegaBytes;
			double totalBytesMb = ByteSize.FromBytes(totalBytes).MegaBytes;

			_downloadSpeed = bytesReceivedSinceLastUpdate / elapsedTime.TotalSeconds;
			double downloadSpeedInMb = _downloadSpeed / (1024 * 1024);

			Default._preparingPleaseWait.Text = $@"{string.Format(Resources.NormalRelease_DownloadingUpdate_, $"{bytesReceivedMb:00.00}", $"{totalBytesMb:000.00}")} [{downloadSpeedInMb:00.00} MB/s]";
			Program.Logger.Info($"Downloading new update... {bytesReceivedMb:00.00} MB of {totalBytesMb:000.00} MB / {downloadSpeedInMb:00.00} MB/s");
		}

		private static async void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			// Unpack files
			Program.Logger.Info($"Unpacking {_zipFile} to {_outputDir}");
			Default._preparingPleaseWait.Text = Resources.StellaResources_UnpackingFiles;
			await DownloadResources.UnzipWithProgress(_zipFile, _outputDir);
			Program.Logger.Info($"Unpacked: {_zipFile}");

			// Delete file
			File.Delete(_zipFile);

			// Success
			Default._progressBar1.Hide();
			Default._preparingPleaseWait.Hide();
			Default._status_Label.Text += $"[✓] {_successfullyUpdated}\n";
			Program.Logger.Info(_successfullyUpdated);

			// Check for updates again
			int foundUpdated = await CheckForUpdates.Analyze();
			if (foundUpdated == 0)
				Labels.ShowStartGameBtns();
			else
				Labels.HideStartGameBtns();
		}
	}
}
