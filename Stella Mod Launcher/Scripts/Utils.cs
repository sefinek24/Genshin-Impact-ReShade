using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Taskbar;
using StellaLauncher.Forms;
using StellaLauncher.Properties;
using StellaLauncher.Scripts.Forms;
using File = System.IO.File;

namespace StellaLauncher.Scripts
{
    internal static class Utils
    {
        public static async Task<string> GetGame(string type)
        {
            string fullGamePath = null;
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Program.RegistryPath))
            {
                if (key != null) fullGamePath = (string)key.GetValue("GamePath");
            }

            if (!File.Exists(fullGamePath))
            {
                DialogResult result = MessageBox.Show(string.Format(Resources.Utils_FileWithGamePathWasNotFoundIn_DoYouWantToResetAllSMSettings, fullGamePath), Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                Log.Output($"File with game path was not found in: {fullGamePath}");

                if (result != DialogResult.Yes) return string.Empty;
                using (RegistryKey newKey = Registry.CurrentUser.CreateSubKey(Program.RegistryPath))
                {
                    newKey?.SetValue("AppIsConfigured", 0);
                }

                _ = Cmd.Execute(new Cmd.CliWrap { App = Program.PrepareLauncher });

                Environment.Exit(99587986);
                return string.Empty;
            }


            switch (type)
            {
                case "giDir":
                {
                    string path = Path.GetDirectoryName(Path.GetDirectoryName(fullGamePath));
                    Log.Output($"giDir: {path}");

                    return path;
                }

                case "giGameDir":
                {
                    string path = Path.GetDirectoryName(fullGamePath);
                    string giGameDir = Path.Combine(path);
                    if (Directory.Exists(giGameDir)) return giGameDir;

                    Log.Output($"giGameDir: {giGameDir}");
                    return string.Empty;
                }

                case "giExe":
                {
                    return fullGamePath;
                }

                case "giLauncher":
                {
                    string giDir = await GetGame("giDir");

                    string genshinImpactExe = Path.Combine(giDir, "launcher.exe");
                    if (!File.Exists(genshinImpactExe))
                    {
                        MessageBox.Show(string.Format(Resources.Utils_LauncherFileDoesNotExists, genshinImpactExe), Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Log.Output($"Launcher file does not exists in: {genshinImpactExe} [giLauncher]");
                        return string.Empty;
                    }

                    Log.Output($"giLauncher: {genshinImpactExe}");
                    return genshinImpactExe;
                }

                default:
                {
                    Log.ThrowError(new Exception(Resources.Utils_WrongParameter));
                    return string.Empty;
                }
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

            Music.PlaySound("winxp", "minimize");

            try
            {
                Process.Start(url);
                Log.Output($"Opened '{url}' in default browser.");
            }
            catch (Exception ex)
            {
                Log.ThrowError(new Exception($"Failed to open '{url}' in default browser.\n\n{ex}"));
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

            Log.Output(fileExists ? $"File '{fileName}' was found at '{filePath}'." : $"File '{fileName}' was not found at '{filePath}'.");

            return fileExists;
        }

        public static bool CreateShortcut()
        {
            try
            {
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(Shortcut.ScPath);
                shortcut.Description = Resources.Utils_RunOfficialLauncherForStellaModMadeBySefinek;
                shortcut.WorkingDirectory = Program.AppPath;
                shortcut.TargetPath = Shortcut.ProgramExe;
                shortcut.Save();

                Log.Output($"Desktop shortcut has been created in: {Shortcut.ScPath}");
                return true;
            }
            catch (Exception ex)
            {
                Log.ThrowError(new Exception($"An error occurred while creating the shortcut.\n\n{ex}"));
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

        public static void ShowToast(string title, string desc)
        {
            Log.Output($"ShowToast: {title}");

            try
            {
                new ToastContentBuilder()
                    .AddText(title)
                    .AddText(desc)
                    .Show();
            }
            catch (Exception ex)
            {
                Log.SaveError(ex.ToString());
            }
        }
    }
}
