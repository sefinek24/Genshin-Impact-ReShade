using System.IO.Compression;
using Newtonsoft.Json;
using PrepareStella.Models;

namespace PrepareStella.Scripts.Preparing;

internal static class DownloadUpdateResources
{
	public static async Task RunAsync()
	{
		string? resourcesGlobalPath = Program.ResourcesGlobal;

		if (!Directory.Exists(resourcesGlobalPath))
		{
			Directory.CreateDirectory(resourcesGlobalPath!);
			Console.WriteLine($@"Created folder: {resourcesGlobalPath}");
		}
		else
		{
			Console.WriteLine($@"Found: {resourcesGlobalPath}");
		}


		// Checking current version of resources
		Console.Write(@"Checking current version of resources... ");
		using HttpClient httpClient = new();
		httpClient.DefaultRequestHeaders.Add("User-Agent", Start.UserAgent);
		string responseContent = await httpClient.GetStringAsync($"{Start.WebApi}/genshin-stella-mod/versions").ConfigureAwait(false);
		StellaApiVersion? json = JsonConvert.DeserializeObject<StellaApiVersion>(responseContent);
		Console.WriteLine(@$"v{json!.Resources?.Release} ({(json.Resources!.Beta ? "beta" : "stable")}) from {json.Resources.Date}");

		string zipPath = Path.Combine(resourcesGlobalPath!, "Temp files", $"Stella Mod Resources - v{json.Resources?.Release}.zip");
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

		Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
		FileStream fs = new(zipPath, FileMode.Create, FileAccess.Write, FileShare.None);
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


		Console.WriteLine("\nWaiting for resources to be disposed...");
		await stream.DisposeAsync().ConfigureAwait(false);
		await fs.DisposeAsync().ConfigureAwait(false);


		Console.WriteLine(@"Unpacking resources...");
		using ZipArchive archive = ZipFile.OpenRead(zipPath);
		foreach (ZipArchiveEntry entry in archive.Entries)
		{
			string fullPath = Path.Combine(resourcesGlobalPath!, entry.FullName);
			if (entry.Name == "")
			{
				Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
				continue;
			}

			await Task.Run(() => entry.ExtractToFile(fullPath, true)).ConfigureAwait(false);
		}
	}
}
