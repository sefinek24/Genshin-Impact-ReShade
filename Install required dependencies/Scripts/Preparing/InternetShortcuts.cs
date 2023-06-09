using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IWshRuntimeLibrary;

namespace PrepareStella.Scripts.Preparing
{
    /// <summary>
    ///     Creates shortcuts for both internet links and executable files in the Start Menu.
    /// </summary>
    internal static class InternetShortcuts
    {
        public static async Task Run()
        {
            string appStartMenuPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs", "Stella Mod Launcher");
            Directory.CreateDirectory(appStartMenuPath);

            try
            {
                // Create shortcut in Start Menu
                await Task.Run(() =>
                {
                    WshShell shell = new WshShell();
                    string shortcutLocation = Path.Combine(appStartMenuPath, "Genshin Impact Stella Mod.lnk");
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);
                    shortcut.Description = "Run the official launcher for Genshin Impact Stella Mod made by Sefinek.";
                    shortcut.WorkingDirectory = Program.AppPath;
                    shortcut.TargetPath = Path.Combine(Program.AppPath, "Genshin Stella Mod.exe");
                    shortcut.Save();
                });

                // Create Internet shortcuts
                Dictionary<string, string> urls = new Dictionary<string, string>
                {
                    { "Official website", "https://genshin.sefinek.net" },
                    { "Donate", "https://sefinek.net/support-me" },
                    { "Gallery", "https://sefinek.net/genshin-impact-reshade/gallery?page=1" },
                    { "Support", "https://sefinek.net/genshin-impact-reshade/support" },
                    { "Leave feedback", "https://sefinek.net/genshin-impact-reshade/feedback" }
                };

                foreach (KeyValuePair<string, string> kvp in urls)
                {
                    string url = Path.Combine(appStartMenuPath, $"{kvp.Key} - Genshin Stella Mod.url");
                    using (StreamWriter writer = new StreamWriter(url))
                    {
                        await writer.WriteLineAsync($"[InternetShortcut]\nURL={kvp.Value}");
                    }
                }
            }
            catch (Exception e)
            {
                Log.ThrowError(e, false);
            }
        }
    }
}
