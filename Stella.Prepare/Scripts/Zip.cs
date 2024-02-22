using Ionic.Zip;
using Ionic.Zlib;

namespace PrepareStella.Scripts;

internal abstract class Zip
{
	private static string GetCleanFolderName(string source, string filePath)
	{
		if (string.IsNullOrWhiteSpace(filePath)) return string.Empty;

		string result = filePath[source.Length..];
		if (result.StartsWith("\\")) result = result[1..];
		result = result[..^new FileInfo(filePath).Name.Length];

		return result;
	}

	public static async Task CreateAsync(string source, string destination)
	{
		using ZipFile zip = new() { CompressionLevel = CompressionLevel.BestCompression };
		string[] files = Directory
			.GetFiles(source, "*", SearchOption.AllDirectories)
			.Where(f => !Path.GetExtension(f).Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
			.ToArray();

		foreach (string f in files) zip.AddFile(f, GetCleanFolderName(source, f));

		string destinationFilename = destination;
		if (Directory.Exists(destination) && !destination.EndsWith(".zip"))
			destinationFilename += $"\\{new DirectoryInfo(source).Name}-{DateTime.Now:yyyy-MM-dd-HH-mm-ss-ffffff}.zip";

		await Task.Run(() => zip.Save(destinationFilename)).ConfigureAwait(false);
	}
}

// Source: https://stackoverflow.com/a/50407976
