using System;
using System.IO;
using System.Threading.Tasks;
using IWshRuntimeLibrary;
using PrepareStella.Properties;

namespace PrepareStella.Scripts.Preparing
{
    /// <summary>
    ///     Provides functionality to create a desktop icon for the Stella Launcher.
    /// </summary>
    internal static class DesktopIcon
    {
        public static async Task Run()
        {
            Console.WriteLine(@"Creating Desktop shortcut...");

            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                string shortcutPath = Path.Combine(desktopPath, "Stella Mod Launcher.lnk");

                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                shortcut.Description = Resources.Utils_RunOfficialLauncherForStellaModMadeBySefinek;
                shortcut.IconLocation = Path.Combine(Program.AppPath, "icons", "52x52.ico");
                shortcut.WorkingDirectory = Program.AppPath;
                shortcut.TargetPath = Path.Combine(Program.AppPath, "Genshin Stella Mod.exe");

                await Task.Run(() => shortcut.Save());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
