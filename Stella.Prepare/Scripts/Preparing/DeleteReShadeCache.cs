namespace PrepareStella.Scripts.Preparing;

/// <summary>
///    Deletes the ReShade cache files and reports the amount of disk space freed.
/// </summary>
internal static class DeleteReShadeCache
{
	private const long KilobyteInBytes = 1024;
	private const long MegabyteInBytes = 1024 * KilobyteInBytes;

	/// <summary>
	///    Asynchronously runs the operation to delete cache files.
	/// </summary>
	public static async Task RunAsync()
	{
		int deletedFilesCount = 0;
		long savedSpace = 0;
		string cacheDirectoryPath = Path.Combine(Program.ResourcesGlobal!, "ReShade", "Cache");

		if (Directory.Exists(cacheDirectoryPath))
		{
			string[] files = Directory.GetFiles(cacheDirectoryPath);
			foreach (string filePath in files)
			{
				FileInfo file = new(filePath);
				savedSpace += file.Length;
				await DeleteFileAsync(filePath).ConfigureAwait(false);
				deletedFilesCount++;
			}
		}
		else
		{
			Console.WriteLine(@"Creating cache folder...");
			Directory.CreateDirectory(cacheDirectoryPath);
		}

		if (deletedFilesCount > 0)
		{
			string spaceUnit = savedSpace > MegabyteInBytes ? "MB" : "KB";
			double spaceSaved = savedSpace / (double)(savedSpace > MegabyteInBytes ? MegabyteInBytes : KilobyteInBytes);
			Console.WriteLine($@"Deleted {deletedFilesCount} cache files and saved {spaceSaved:F2} {spaceUnit}.");
		}
		else
		{
			Console.WriteLine(@"No cache files found to delete.");
		}
	}

	/// <summary>
	///    Deletes a file asynchronously.
	/// </summary>
	/// <param name="filePath">The path to the file to be deleted.</param>
	private static async Task DeleteFileAsync(string filePath)
	{
		try
		{
			await Task.Run(() => File.Delete(filePath)).ConfigureAwait(false);
			Start.Logger.Info($"Deleted cache file: {filePath}");
		}
		catch (Exception ex)
		{
			Start.Logger.Info($"Error deleting file {filePath}: {ex.Message}");
		}
	}
}
