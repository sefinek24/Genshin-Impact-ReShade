using IWshRuntimeLibrary;
using Microsoft.Win32;
using File = System.IO.File;

namespace StellaModLauncher.Scripts;

internal static class Shortcut
{
	public static readonly string? ProgramExe = Environment.ProcessPath;
	private static readonly string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
	public static readonly string ScPath = Path.Combine(DesktopPath, "Stella Mod Launcher.lnk");

	public static void Check()
	{
		try
		{
			// Checking if the shortcut exists or needs updating
			bool updateIsRequired = false;

			if (File.Exists(ScPath))
			{
				WshShell shell = new();
				IWshShortcut existingShortcut = (IWshShortcut)shell.CreateShortcut(ScPath);
				if (existingShortcut.TargetPath == ProgramExe && existingShortcut.WorkingDirectory == Program.AppPath)
				{
					Program.Logger.Info("The desktop shortcut is already up to date");
				}
				else
				{
					Program.Logger.Info("A desktop shortcut was found, but it has a different path. It will be overwritten");
					updateIsRequired = true;
				}
			}

			// Update
			if (!updateIsRequired) return;

			// Shortcut
			Utils.CreateShortcut();

			// Registry
			using (RegistryKey key = Registry.CurrentUser.CreateSubKey(Program.RegistryPath))
			{
				key.SetValue("StellaPath", Program.AppPath);
			}

			Program.Logger.Info("The `StellaPath` registry key has been successfully updated");

			BalloonTip.Show("Change of the launcher path", "We have detected a likely change in the location of the Stella Mod Launcher. The necessary paths have been updated, as well as the shortcut on your desktop.");
		}
		catch (Exception ex)
		{
			Program.Logger.Error("An error occurred while checking desktop shortcut: " + ex.Message);
		}
	}
}
