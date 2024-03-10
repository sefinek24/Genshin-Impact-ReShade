using IWshRuntimeLibrary;
using Microsoft.Win32;
using StellaModLauncher.Properties;
using StellaModLauncher.Scripts.Helpers;
using File = System.IO.File;

namespace StellaModLauncher.Scripts;

internal static class Shortcut
{
	private static readonly string? ProgramExe = Environment.ProcessPath;
	private static readonly string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
	private static readonly string ScPath = Path.Combine(DesktopPath, "Stella Mod Launcher.lnk");

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
			CreateOrUpdate();

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

	public static bool CreateOrUpdate()
	{
		try
		{
			// Desktop shortcut
			WshShell shell1 = new();
			IWshShortcut desktop = (IWshShortcut)shell1.CreateShortcut(ScPath);
			desktop.Description = Resources.Utils_RunOfficialLauncherForStellaModMadeBySefinek;
			desktop.WorkingDirectory = Program.AppPath;
			desktop.TargetPath = ProgramExe;
			desktop.Save();

			Program.Logger.Info("Desktop shortcut has been created");

			// Start menu shortcut
			string shortcutLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs", "Stella Mod Launcher", Path.GetFileName(ScPath));

			IWshShortcut menuStart = (IWshShortcut)shell1.CreateShortcut(shortcutLocation);
			menuStart.Description = Resources.Utils_RunOfficialLauncherForStellaModMadeBySefinek;
			menuStart.WorkingDirectory = Program.AppPath;
			menuStart.TargetPath = ProgramExe;
			menuStart.Save();

			Program.Logger.Info("Start menu shortcut shortcut has been created");

			// Done
			Program.Logger.Info($"Desktop shortcut: {ScPath}");
			Program.Logger.Info($"Start menu shortcut: {shortcutLocation}");
			Program.Logger.Info($"Working directory: {ScPath}");
			Program.Logger.Info($"Target path: {ProgramExe}");

			return true;
		}
		catch (Exception ex)
		{
			Log.ThrowError(new Exception($"An error occurred while creating the shortcut.\n\n{ex}"));
			return false;
		}
	}
}
