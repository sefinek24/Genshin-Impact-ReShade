using IWshRuntimeLibrary;
using PrepareStella.Properties;

namespace PrepareStella.Scripts.Preparing;

internal static class DesktopIcon
{
	public static async Task RunAsync()
	{
		try
		{
			string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
			string shortcutPath = Path.Combine(desktopPath, "Stella Mod Launcher.lnk");
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
				shortcut.TargetPath = Path.Combine(Start.AppPath!, "Stella Mod Launcher.exe");

				shortcut.Save();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
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
				string shortcutLocation = Path.Combine(appStartMenuPath, "Stella Mod Launcher.lnk");
				IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);
				shortcut.Description = Resources.Utils_RunOfficialLauncherForStellaModMadeBySefinek;
				shortcut.WorkingDirectory = Start.AppPath;
				shortcut.TargetPath = Path.Combine(Start.AppPath, "Stella Mod Launcher.exe");
				shortcut.Save();
			}
			catch (Exception e)
			{
				Log.ThrowError(e, false);
			}
		}).ConfigureAwait(false);
	}
}
