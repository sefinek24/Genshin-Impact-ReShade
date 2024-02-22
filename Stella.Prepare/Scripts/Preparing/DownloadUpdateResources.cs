using System.IO.Compression;
using Newtonsoft.Json;
using PrepareStella.Models;

namespace PrepareStella.Scripts.Preparing;

internal static class DownloadUpdateResources
{
	public static async Task RunAsync()
	{
		string resourcesGlobalPath = Program.ResourcesGlobal;

		if (!Directory.Exists(resourcesGlobalPath))
		{
			Directory.CreateDirectory(resourcesGlobalPath);
			Console.WriteLine($@"Created folder: {resourcesGlobalPath}");
		}
		else
		{
			Console.WriteLine($@"Found: {resourcesGlobalPath}");
		}

		// Checking current version of resources
		Console.WriteLine(@"Checking current version of resources...");
		using (HttpClient httpClient = new())
		{
			httpClient.DefaultRequestHeaders.Add("User-Agent", Start.UserAgent);
			string responseContent = await httpClient.GetStringAsync($"{Start.WebApi}/genshin-stella-mod/version/app/launcher/resources").ConfigureAwait(false);
			StellaResources? json = JsonConvert.DeserializeObject<StellaResources>(responseContent);

			string zipPath = Path.Combine(resourcesGlobalPath, "Temp files", $"Stella Mod Resources - v{json.Message}.zip");
			if (!Directory.Exists(Path.GetDirectoryName(zipPath))) Directory.CreateDirectory(Path.GetDirectoryName(zipPath) ?? string.Empty);
			if (File.Exists(zipPath))
			{
				Console.WriteLine($@"Deleting {zipPath}...");
				File.Delete(zipPath);
				Start.Logger.Info($"Deleted {zipPath}");
			}

			Console.Write(@"Preparing to download resources...");

			long receivedBytes = 0;
			int lastPercentage = 0;
			DateTime startTime = DateTime.Now;

			HttpResponseMessage response = await httpClient.GetAsync("https://github.com/sefinek24/Genshin-Stella-Resources/releases/latest/download/resources.zip", HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
			long? totalBytes = response.Content.Headers.ContentLength;

			using (Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
			using (FileStream fs = new(zipPath, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				byte[] buffer = new byte[8192];
				int bytesRead;
				while ((bytesRead = await stream.ReadAsync(buffer).ConfigureAwait(false)) > 0)
				{
					await fs.WriteAsync(buffer.AsMemory(0, bytesRead)).ConfigureAwait(false);
					receivedBytes += bytesRead;
					int percentage = (int)(receivedBytes * 100 / totalBytes.GetValueOrDefault(1));
					TimeSpan elapsed = DateTime.Now - startTime;
					double speed = receivedBytes / elapsed.TotalSeconds;

					if (percentage == lastPercentage) continue;
					Console.SetCursorPosition(0, Console.CursorTop);
					Console.Write($"\rDownloading Stella Mod resources... {receivedBytes}/{totalBytes} bytes ({percentage}%) at {speed / 1024:0.00} KB/s ");
					lastPercentage = percentage;
				}
			}

			Console.WriteLine("\nUnpacking resources...");
			using (ZipArchive archive = ZipFile.OpenRead(zipPath))
			{
				foreach (ZipArchiveEntry entry in archive.Entries)
				{
					string fullPath = Path.Combine(resourcesGlobalPath, entry.FullName);
					if (entry.Name == "")
					{
						Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
						continue;
					}

					await Task.Run(() => entry.ExtractToFile(fullPath, true)).ConfigureAwait(false);
				}
			}
		}
	}
}
