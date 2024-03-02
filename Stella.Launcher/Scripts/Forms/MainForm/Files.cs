using StellaModLauncher.Properties;
using StellaModLauncher.Scripts.Remote;

namespace StellaModLauncher.Scripts.Forms.MainForm;

internal static class Files
{
	public static async Task ScanAsync()
	{
		CheckIfExists(Program.FpsUnlockerExePath);
		CheckIfExists(Program.InjectorPath);
		CheckIfExists(Program.ReShadePath);

		if (!File.Exists(Program.FpsUnlockerCfgPath)) await FpsUnlockerCfg.RunAsync().ConfigureAwait(false);
	}

	private static void CheckIfExists(string filePath)
	{
		if (!File.Exists(filePath)) Utils.UpdateStatusLabel(string.Format(Resources.Default_File_WasNotFound, filePath), Utils.StatusType.Error);
	}

	public static void DeleteSetupAsync()
	{
		if (!File.Exists(NormalRelease.SetupPathExe)) return;

		try
		{
			File.Delete(NormalRelease.SetupPathExe);
			Utils.UpdateStatusLabel(Resources.Default_DeletedOldSetupFromTempDirectory, Utils.StatusType.Info);
			Program.Logger.Info($"Deleted old setup file from temp folder: {NormalRelease.SetupPathExe}");
		}
		catch (Exception ex)
		{
			Utils.UpdateStatusLabel(ex.Message, Utils.StatusType.Error);
			Program.Logger.Error(ex.ToString());
		}
	}
}
