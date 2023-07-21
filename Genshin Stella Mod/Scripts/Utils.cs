using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Storage;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Taskbar;
using StellaLauncher.Forms;
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
                using (RegistryKey newKey = Registry.CurrentUser.CreateSubKey(Program.RegistryPath))
                {
                    newKey?.SetValue("AppIsConfigured", 0);
                }

                _ = Cmd.CliWrap(FirstAppLaunch, null, null, true, false);
                Environment.Exit(99587986);
                return string.Empty;
            }

            string gameFilePath = File.ReadAllText(FileWithGamePath);
            if (!File.Exists(gameFilePath))
            {
                DialogResult result = MessageBox.Show(string.Format(Resources.Utils_FolderWithGamePathDoesNotExists_DoYouWantToResetAllSMSettings, gameFilePath), Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result != DialogResult.Yes) return string.Empty;
                using (RegistryKey newKey = Registry.CurrentUser.CreateSubKey(Program.RegistryPath))
                {
                    newKey?.SetValue("AppIsConfigured", 0);
                }

                _ = Cmd.CliWrap(FirstAppLaunch, null, null, true, false);
                Environment.Exit(99587987);
                return string.Empty;
            }

            switch (type)
            {
                case "giDir":
                {
                    string path = Path.GetDirectoryName(Path.GetDirectoryName(gameFilePath));
                    Log.Output(string.Format(Resources.Utils_FoundMainGIDir, path, "giDir"));

                    return path;
                }

                case "giGameDir":
                {
                    string path = Path.GetDirectoryName(gameFilePath);
                    string giGameDir = Path.Combine(path);
                    if (Directory.Exists(giGameDir)) return giGameDir;

                    Log.Output(string.Format(Resources.Utils_FoundGIgameDir__, giGameDir, "giGameDir"));
                    return string.Empty;
                }

                case "giExe":
                {
                    return gameFilePath;
                }

                case "giLauncher":
                {
                    string giDir = await GetGame("giDir");

                    string genshinImpactExe = Path.Combine(giDir, "launcher.exe");
                    if (!File.Exists(genshinImpactExe))
                    {
                        MessageBox.Show(string.Format(Resources.Utils_LauncherFileDoesNotExists, genshinImpactExe), Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Log.Output(string.Format(Resources.Utils_LauncherFileDoesNotExistsIn_, genshinImpactExe, "giLauncher"));
                        return string.Empty;
                    }

                    Log.Output(string.Format(Resources.Utils_FoundGILauncherIn_, genshinImpactExe, "giLauncher"));
                    return genshinImpactExe;
                }

                default:
                {
                    Log.ThrowError(new Exception(Resources.Utils_WrongParameter));
                    return string.Empty;
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
                Log.Output(string.Format(Resources.Utils_Opened_InDefaultBrowser, url));
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
                shortcut.WorkingDirectory = Program.AppPath;
                shortcut.TargetPath = Path.Combine(Program.AppPath, $"{Program.AppName}.exe");
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

        public static void HideProgressBar(bool error)
        {
            if (error)
            {
                Default.UpdateIsAvailable = false;

                Default._updates_LinkLabel.LinkColor = Color.Red;
                Default._updates_LinkLabel.Text = Resources.Utils_OopsAnErrorOccurred;

                TaskbarManager.Instance.SetProgressValue(100, 100);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
            }

            Default._progressBar1.Hide();
            Default._preparingPleaseWait.Hide();

            Default._progressBar1.Value = 0;
        }
    }
}
