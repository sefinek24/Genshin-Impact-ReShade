using System;
using System.IO;
using IWshRuntimeLibrary;

namespace Prepare_mod.Scripts.Preparing
{
    internal static class DesktopIcon
    {
        public static void Run()
        {
            Console.WriteLine(@"Creating Desktop shortcut...");

            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                string shortcutPath = Path.Combine(desktopPath, "Stella Mod Launcher.lnk");

                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                shortcut.Description = "Run official launcher for Genshin Impact Mod made by Sefinek.";
                shortcut.IconLocation = Path.Combine(Program.AppPath, "icons", "52x52.ico");
                shortcut.WorkingDirectory = Program.AppPath;
                shortcut.TargetPath = Path.Combine(Program.AppPath, "Genshin Stella Mod.exe");

                shortcut.Save();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
