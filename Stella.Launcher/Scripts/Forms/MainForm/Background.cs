using System.Runtime.Caching;

namespace StellaModLauncher.Scripts.Forms.MainForm;

internal static class Background
{
	private static readonly ObjectCache Cache = MemoryCache.Default;
	private static readonly string BackgroundImagesPath = Path.Combine(Program.AppPath, "data", "images", "backgrounds", "main");
	private static readonly string[] Extensions = ["*.png", "*.jpg", "*.jpeg", "*.bmp"];

	private static string[] LoadBackgroundFiles()
	{
		try
		{
			if (!Directory.Exists(BackgroundImagesPath))
			{
				MessageBox.Show($@"Background folder does not exist: {BackgroundImagesPath}", Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return [];
			}

			List<string> fileList = [];
			foreach (string ext in Extensions) fileList.AddRange(Directory.GetFiles(BackgroundImagesPath, ext, SearchOption.AllDirectories));

			return [.. fileList];
		}
		catch (Exception ex)
		{
			MessageBox.Show($@"Error occurred while loading background files: {ex.Message}", Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Error);
			return [];
		}
	}

	public static Image? OnStart(ToolTip toolTip, LinkLabel changeBg)
	{
		string[] backgroundFiles = LoadBackgroundFiles();
		if (backgroundFiles.Length == 0) return null;

		int bgInt = Program.Settings.ReadInt("Launcher", "Background", 0);
		bgInt = bgInt >= backgroundFiles.Length || bgInt < 0 ? 0 : bgInt;

		return GetCachedOrLoadImage(bgInt, backgroundFiles, toolTip, changeBg);
	}

	private static Bitmap? GetCachedOrLoadImage(int bgInt, IReadOnlyList<string> backgroundFiles, ToolTip toolTip, Control changeBg)
	{
		if (backgroundFiles.Count == 0) return null;

		string cacheKey = $"background_{bgInt}";
		if (Cache.Contains(cacheKey) && Cache.Get(cacheKey) is Bitmap cachedImage)
		{
			toolTip.SetToolTip(changeBg, $"Current background: {Path.GetFileName(backgroundFiles[bgInt])}");
			return cachedImage;
		}

		if (!File.Exists(backgroundFiles[bgInt]))
		{
			MessageBox.Show($@"Sorry, background {Path.GetFileName(backgroundFiles[bgInt])} was not found.", Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			return null;
		}

		Bitmap backgroundImage = new(backgroundFiles[bgInt]);
		Cache.Add(cacheKey, backgroundImage, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(20) });
		SetToolTipForBackground(changeBg, toolTip, backgroundFiles[bgInt]);

		return backgroundImage;
	}

	private static void SetToolTipForBackground(Control changeBg, ToolTip toolTip, string filePath)
	{
		string[] pathParts = filePath.Split(Path.DirectorySeparatorChar);
		if (pathParts.Length > 3)
		{
			string folderNames = $@"{pathParts[^3]}\{pathParts[^2]}";
			string fileName = Path.GetFileName(filePath);
			toolTip.SetToolTip(changeBg, $@"Current background: {folderNames}\{fileName}");
		}
		else
		{
			toolTip.SetToolTip(changeBg, $"Current background file: {Path.GetFileName(filePath)}");
		}
	}

	public static Image? Change(ToolTip toolTip, LinkLabel changeBg)
	{
		string[] backgroundFiles = LoadBackgroundFiles();
		if (backgroundFiles.Length == 0)
		{
			MessageBox.Show(@"No background images found.", Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Error);
			return null;
		}

		int bgInt = Program.Settings.ReadInt("Launcher", "Background", 0);
		bgInt = (bgInt + 1) % backgroundFiles.Length;
		Program.Settings.WriteInt("Launcher", "Background", bgInt);
		Program.Settings.Save();

		return GetCachedOrLoadImage(bgInt, backgroundFiles, toolTip, changeBg);
	}
}
