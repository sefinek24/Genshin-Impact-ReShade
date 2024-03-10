using IWshRuntimeLibrary;
using PrepareStella.Properties;

namespace PrepareStella.Scripts.Preparing;

internal static class DesktopIcon
{
	private static readonly string LnkFileName = "Stella Mod Launcher.lnk";
	private static readonly string ExeFileName = "Stella Mod Launcher.exe";

	public static async Task RunAsync()
	{
		try
		{
			string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
			string shortcutPath = Path.Combine(desktopPath, LnkFileName);
			await CreateShortcutAsync(shortcutPath).ConfigureAwait(false);

			string appStartMenuPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs", "Stella Mod Launcher");
			Directory.CreateDirectory(appStartMenuPath);
			await CreateStartMenuShortcutAsync(appStartMenuPath).ConfigureAwait(false);
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
		}
	}

	private static async Task CreateShortcutAsync(string shortcutPath)
	{
		await Task.Run(() =>
		{
			try
			{
				WshShell shell = new();
				IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
				shortcut.Description = Resources.Utils_RunOfficialLauncherForStellaModMadeBySefinek;
				shortcut.WorkingDirectory = Start.AppPath;
				shortcut.TargetPath = Path.Combine(Start.AppPath!, ExeFileName);

				shortcut.Save();
			}
			catch (Exception err)
			{
				Console.WriteLine(err.Message);
			}
		}).ConfigureAwait(false);
	}

	private static async Task CreateStartMenuShortcutAsync(string appStartMenuPath)
	{
		await Task.Run(() =>
		{
			try
			{
				WshShell shell = new();
				string shortcutLocation = Path.Combine(appStartMenuPath, LnkFileName);
				IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);
				shortcut.Description = Resources.Utils_RunOfficialLauncherForStellaModMadeBySefinek;
				shortcut.WorkingDirectory = Start.AppPath;
				shortcut.TargetPath = Path.Combine(Start.AppPath!, ExeFileName);
				shortcut.Save();
			}
			catch (Exception err)
			{
				Log.ThrowError(err, false);
			}
		}).ConfigureAwait(false);
	}
}
