using System;
using System.IO;
using System.Reflection;
using IWshRuntimeLibrary;
using Microsoft.Toolkit.Uwp.Notifications;
using StellaLauncher.Properties;
using File = System.IO.File;

namespace StellaLauncher.Scripts
{
    internal static class Shortcut
    {
        public static readonly string ProgramExe = Assembly.GetExecutingAssembly().Location;
        private static readonly string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
        public static readonly string ScPath = Path.Combine(DesktopPath, "Stella Mod Launcher.lnk");

        public static void Check()
        {
            try
            {
                // Checking if the shortcut exists or needs updating
                bool createOrUpdateShortcut = false;

                WshShell shell = new WshShell();

                if (File.Exists(ScPath))
                {
                    IWshShortcut existingShortcut = (IWshShortcut)shell.CreateShortcut(ScPath);

                    if (existingShortcut.TargetPath == ProgramExe && existingShortcut.WorkingDirectory == Program.AppPath)
                    {
                        Program.Logger.Info("The desktop shortcut is already up to date.");
                    }
                    else
                    {
                        Program.Logger.Info("A desktop shortcut was found, but it has a different path. It will be overwritten.");
                        createOrUpdateShortcut = true;
                    }
                }

                // Creating or updating the shortcut
                if (!createOrUpdateShortcut) return;
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(ScPath);
                shortcut.Description = Resources.Utils_RunOfficialLauncherForStellaModMadeBySefinek;
                shortcut.WorkingDirectory = Program.AppPath;
                shortcut.TargetPath = ProgramExe;
                shortcut.Save();

                Program.Logger.Info("Created or updated the desktop shortcut.");

                try
                {
                    new ToastContentBuilder()
                        .AddText("Desktop shortcut 🖥️")
                        .AddText("The program icon on your desktop has been successfully updated. Other shortcuts may stop working.")
                        .Show();
                }
                catch (Exception ex)
                {
                    Program.Logger.Error(ex.ToString());
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Error("An error occurred while checking desktop shortcut: " + ex.Message);
            }
        }
    }
}
