using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Storage;
using IWshRuntimeLibrary;
using StellaLauncher.Properties;
using File = System.IO.File;

namespace StellaLauncher.Scripts
{
    internal static class Utils
    {
        private static readonly string FileWithGamePath = Path.Combine(Program.AppData, "game-path.sfn");
        public static readonly string FirstAppLaunch = Path.Combine(Program.AppPath, "First app launch.exe");

        public static async Task<string> GetGame(string type)
        {
            if (!File.Exists(FileWithGamePath))
            {
                DialogResult result = MessageBox.Show(string.Format(Resources.Utils_FileWithGamePathWasNotFoundIn_DoYouWantToResetAllSMSettings, FileWithGamePath), Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                Log.Output(string.Format(Resources.Utils_FileWithGamePathWasNotFoundIn, FileWithGamePath));

                if (result != DialogResult.Yes) return string.Empty;
                Directory.Delete(Program.AppData, true);
                await Cmd.CliWrap(FirstAppLaunch, null, null, true, false);
                return string.Empty;
            }

            string gameFilePath = File.ReadAllLines(FileWithGamePath).First();
            string gamePath = Path.Combine(gameFilePath);
            if (!Directory.Exists(gamePath))
            {
                DialogResult result = MessageBox.Show(string.Format(Resources.Utils_FolderWithGamePathDoesNotExists_DoYouWantToResetAllSMSettings, gamePath), Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result != DialogResult.Yes) return string.Empty;
                Directory.Delete(Program.AppData, true);
                await Cmd.CliWrap(FirstAppLaunch, null, null, true, false);
                return string.Empty;
            }


            switch (type)
            {
                case "giDir":
                {
                    Log.Output($"Found main Genshin Impact dir: {gamePath} [giDir]");
                    return gamePath;
                }

                case "giGameDir":
                {
                    string genshinImpactGame = Path.Combine(gamePath, "Genshin Impact game");
                    if (!Directory.Exists(genshinImpactGame))
                    {
                        Log.Output($"Genshin Impact game was not found in: {genshinImpactGame} [giGameDir]");
                        return string.Empty;
                    }

                    Log.Output($"Found Genshin Impact Game dir: {genshinImpactGame} [giGameDir]");
                    return genshinImpactGame;
                }

                case "giExe":
                {
                    string genshinImpactExeMain = Path.Combine(gamePath, "Genshin Impact game", "GenshinImpact.exe");
                    if (File.Exists(genshinImpactExeMain))
                    {
                        Log.Output($"Found GenshinImpact.exe in: {genshinImpactExeMain} [giExe]");
                        return genshinImpactExeMain;
                    }

                    MessageBox.Show($"File does not exists.\n{genshinImpactExeMain}", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Log.Output($"File does not exists in: {genshinImpactExeMain} [giExe]");

                    string genshinImpactExeYuanShen = Path.Combine(gamePath, "Genshin Impact game", "YuanShen.exe");
                    if (File.Exists(genshinImpactExeYuanShen))
                    {
                        Log.Output($"Found GenshinImpact.exe in: {genshinImpactExeMain} [giExe]");
                        return genshinImpactExeYuanShen;
                    }

                    MessageBox.Show($"File {genshinImpactExeYuanShen} does not exists.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.Output($"File {genshinImpactExeYuanShen} does not exists. [giExe]");

                    return string.Empty;
                }

                case "giLauncher":
                {
                    string genshinImpactExe = Path.Combine(gamePath, "launcher.exe");
                    if (!File.Exists(genshinImpactExe))
                    {
                        MessageBox.Show($"Launcher file does not exists.\n{genshinImpactExe}", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Log.Output($"Launcher file does not exists in: {genshinImpactExe} [giLauncher]");
                        return string.Empty;
                    }

                    Log.Output($"Found Genshin Impact Launcher in: {genshinImpactExe} [giLauncher]");
                    return genshinImpactExe;
                }

                default:
                {
                    Log.ThrowError(new Exception("Wrong parameter."));
                    return "";
                }
            }
        }

        public static string GetAppData()
        {
            try
            {
                return Path.Combine(ApplicationData.Current?.LocalFolder?.Path);
            }
            catch (InvalidOperationException)
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stella Mod Launcher");
            }
        }

        public static async Task<string> GetGameVersion()
        {
            string gvSfn = Path.Combine(Program.AppData, "game-version.sfn");
            string exePath = await GetGame("giExe");
            string exe = Path.GetFileName(exePath);

            string number = exe == "GenshinImpact.exe" ? "1" : "2";
            if (!File.Exists(gvSfn)) File.WriteAllText(gvSfn, number);

            return number;
        }

        public static void OpenUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                Log.ThrowError(new Exception(Resources.Utils_URLIsNullOrEmpty));
                return;
            }

            try
            {
                Process.Start(url);
                Log.Output(Resources.Utils_Opened_InDefaultBrowser);
            }
            catch (Exception ex)
            {
                Log.ThrowError(new Exception(string.Format(Resources.Utils_FailedToOpen_InDefaultBrowser, url, ex)));
            }
        }

        public static void RemoveClickEvent(Label button)
        {
            FieldInfo eventClickField = typeof(Control).GetField("EventClick", BindingFlags.Static | BindingFlags.NonPublic);
            object eventHandler = eventClickField?.GetValue(button);
            if (eventHandler == null) return;

            PropertyInfo eventsProperty = button.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
            EventHandlerList eventHandlerList = (EventHandlerList)eventsProperty?.GetValue(button, null);

            eventHandlerList?.RemoveHandler(eventHandler, eventHandlerList[eventHandler]);
        }

        public static bool CheckFileExists(string fileName)
        {
            string filePath = Path.Combine(fileName);
            bool fileExists = File.Exists(filePath);

            Log.Output(fileExists
                ? string.Format(Resources.Utils_File_WasFoundAt_, fileName, filePath)
                : string.Format(Resources.Utils_File_WasNotFoundAt_, fileName, filePath));

            return fileExists;
        }

        public static bool CreateShortcut()
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                string shortcutPath = Path.Combine(desktopPath, "Stella Mod Launcher.lnk");

                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                shortcut.Description = Resources.Utils_RunOfficialLauncherForStellaModMadeBySefinek;
                shortcut.IconLocation = Path.Combine(Environment.CurrentDirectory, "icons", "52x52.ico");
                shortcut.WorkingDirectory = Environment.CurrentDirectory;
                shortcut.TargetPath = Path.Combine(Environment.CurrentDirectory, $"{Program.AppName}.exe");
                shortcut.Save();

                Log.Output(Resources.Utils_DesktopShortcutHasBeenCreated);
                return true;
            }
            catch (Exception ex)
            {
                Log.ThrowError(new Exception(string.Format(Resources.Utils_AnErrorOccurredWhileCreatingTheShortcut, ex)));
                return false;
            }
        }
    }
}
