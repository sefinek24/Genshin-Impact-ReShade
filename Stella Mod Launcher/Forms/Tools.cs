using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CliWrap.Builders;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using StellaLauncher.Forms.Other;
using StellaLauncher.Properties;
using StellaLauncher.Scripts;

namespace StellaLauncher.Forms
{
    public partial class Tools : Form
    {
        private bool _mouseDown;
        private Point _offset;

        public Tools()
        {
            InitializeComponent();
        }

        private void Tools_Load(object sender, EventArgs e)
        {
            Random random = new Random();
            int randomInt = random.Next(1, 50 + 1);
            if (randomInt == 25)
            {
                string pathStr = Path.Combine(Program.AppPath, "data", "images", "launcher", "backgrounds", "forms", "tools", "2.png");
                if (!Utils.CheckFileExists(pathStr))
                {
                    Log.SaveError(Resources.Tools_SpecialBackgroundWasNotFoundIn_ForToolsWindow);
                    return;
                }

                string pathCombine = Path.Combine(pathStr);
                BackgroundImage = new Bitmap(pathCombine);
                panel3.Visible = false;
            }

            MusicLabel_Set();
            RPCLabel_Set();
        }

        private void Utils_Shown(object sender, EventArgs e)
        {
            Discord.SetStatus(Resources.Tools_BrowsingUtils);

            Log.Output(string.Format(Resources.Main_LoadedForm_, Text));
        }

        private void MouseDown_Event(object sender, MouseEventArgs e)
        {
            _offset.X = e.X;
            _offset.Y = e.Y;
            _mouseDown = true;
        }

        private void MouseMove_Event(object sender, MouseEventArgs e)
        {
            if (!_mouseDown) return;
            Point currentScreenPos = PointToScreen(e.Location);
            Location = new Point(currentScreenPos.X - _offset.X, currentScreenPos.Y - _offset.Y);
        }

        private void MouseUp_Event(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Log.Output(string.Format(Resources.Main_ClosedForm_, Text));
            Close();

            Discord.Home();
        }


        // -------------------------------- Launcher --------------------------------
        private async void OpenConfWindow_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DialogResult res = MessageBox.Show(Resources.Tools_AreYouSureToRunStellaConfigurationWindowAgain, Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.No) return;

            if (!File.Exists(Program.PrepareLauncher))
            {
                string fileName = Path.GetFileName(Program.PrepareLauncher);
                MessageBox.Show(Resources.Program_RequiredFileFisrtAppLaunchExeWasNotFound_, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.SaveError($"Required file was not found in: {fileName}");
                return;
            }

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Program.RegistryPath, true))
            {
                key?.SetValue("AppIsConfigured", 0);
            }

            Cmd.CliWrap cliWrapCommand2 = new Cmd.CliWrap
            {
                App = Program.PrepareLauncher,
                WorkingDir = Program.AppPath,
                BypassUpdates = true
            };
            await Cmd.Execute(cliWrapCommand2);

            Environment.Exit(0);
        }

        private void CreateShortcut_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            bool success = Utils.CreateShortcut();
            if (success) MessageBox.Show(Resources.Tools_TheShortcutHasBeenSuccessfullyCreated, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MuteMusic_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int data = Program.Settings.ReadInt("Launcher", "EnableMusic", 1);
            switch (data)
            {
                case 0:
                    Program.Settings.WriteInt("Launcher", "EnableMusic", 1);
                    break;
                case 1:
                    Program.Settings.WriteInt("Launcher", "EnableMusic", 0);
                    break;
                default:
                    Log.SaveError(Resources.Tools_WrongEnableMusicValueInTheIniFile);
                    MessageBox.Show(Resources.Tools_WrongEnableMusicValueInTheIniFile, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            }

            MusicLabel_Set();
        }

        private void MusicLabel_Set()
        {
            int data = Program.Settings.ReadInt("Launcher", "EnableMusic", 1);

            switch (data)
            {
                case 0:
                    MuteMusicOnStart.Text = Resources.Tools_UnmuteMusicOnStart;
                    break;
                case 1:
                    MuteMusicOnStart.Text = Resources.Tools_MuteMusicOnStart;
                    break;
                default:
                    Log.SaveError(Resources.Tools_WrongEnableMusicValueInTheIniFile);
                    MessageBox.Show(Resources.Tools_WrongEnableMusicValueInTheIniFile, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            }
        }

        private void DisableRPC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int iniData = Program.Settings.ReadInt("Launcher", "DiscordRPC", 1);

            switch (iniData)
            {
                case 0:
                    Program.Settings.WriteInt("Launcher", "DiscordRPC", 1);
                    Discord.InitRpc();

                    if (!string.IsNullOrEmpty(Discord.Username))
                        MessageBox.Show(string.Format(Resources.Tools_YoureConnectedAs__HiAndNiceToMeetYou_CheckYourDiscordActivity, Discord.Username), Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    break;

                case 1:
                    Program.Settings.WriteInt("Launcher", "DiscordRPC", 0);
                    if (!Discord.IsReady)
                    {
                        Discord.Client.Dispose();
                        Discord.Username = null;
                    }

                    break;
            }

            RPCLabel_Set();
        }

        private void RPCLabel_Set()
        {
            int iniData = Program.Settings.ReadInt("Launcher", "DiscordRPC", 1);
            switch (iniData)
            {
                case 0:
                    DisableDiscordRPC.Text = Resources.Tools_EnableDiscordRPC;
                    break;

                case 1:
                    DisableDiscordRPC.Text = Resources.Tools_DisableDiscordRPC;
                    break;
            }
        }


        private void ChangeLang_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new Language { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) }.ShowDialog();
        }


        // ---------------------------------- ReShade ----------------------------------
        private async void ConfReShade_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string currentPreset = await RsConfig.Prepare();
            if (currentPreset == null) return;

            MessageBox.Show($"Successfully updated the ReShade.ini file. Paths to resource locations have been changed to current, and similar changes have been made.\n\nCurrent preset:\n{Path.GetFileNameWithoutExtension(currentPreset).Trim()}",
                Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);

            Log.Output($"Updated ReShade config.\nCurrent preset: {currentPreset}");
        }


        // ---------------------------------- Misc ----------------------------------
        private async void ScanSysFiles_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Cmd.CliWrap command = new Cmd.CliWrap
            {
                App = "wt.exe",
                WorkingDir = Program.AppPath,
                Arguments = new ArgumentsBuilder()
                    .Add(Path.Combine(Program.AppPath, "data", "cmd", "scan_sys_files.cmd"))
                    .Add(Program.AppVersion)
                    .Add(Data.ReShadeVer)
                    .Add(Data.UnlockerVer)
            };
            await Cmd.Execute(command);
        }

        private void RemoveStellaNotifications_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ToastNotificationManagerCompat.History.Clear();
        }


        // ------------------------------ Config files ------------------------------
        private async void ReShadeConfig_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string gamePath = await Utils.GetGame("giGameDir");
            string reShadeIni = Path.Combine(gamePath, "ReShade.ini");

            if (!File.Exists(reShadeIni))
            {
                MessageBox.Show(string.Format(Resources.Tools_ReShadeConfigFileWasNotFoundIn_, reShadeIni), Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                Cmd.CliWrap command = new Cmd.CliWrap
                {
                    App = "notepad.exe",
                    WorkingDir = Program.AppPath,
                    Arguments = new ArgumentsBuilder()
                        .Add(reShadeIni)
                };
                await Cmd.Execute(command);
            }
        }

        private async void UnlockerConfig_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Cmd.CliWrap command = new Cmd.CliWrap
            {
                App = "notepad.exe",
                WorkingDir = Program.AppPath,
                Arguments = new ArgumentsBuilder()
                    .Add(Path.Combine(Program.AppPath, "data", "unlocker", "unlocker.config.json"))
            };
            await Cmd.Execute(command);
        }


        // --------------------------------- Cache ---------------------------------
        private async void DeleteCache_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string gameDir = await Utils.GetGame("giGameDir");

            Cmd.CliWrap command = new Cmd.CliWrap
            {
                App = "wt.exe",
                WorkingDir = Program.AppPath,
                Arguments = new ArgumentsBuilder()
                    .Add(Path.Combine(Program.AppPath, "data", "cmd", "delete_cache.cmd")) // 0
                    .Add(Program.AppVersion) // 1
                    .Add(Data.ReShadeVer) // 2
                    .Add(Data.UnlockerVer) // 3
                    .Add(Program.AppData) // 4
                    .Add(Default.ResourcesPath) // 5
                    .Add(gameDir) // 6
                    .Add(Program.AppData) // 7
            };
            await Cmd.Execute(command);
        }

        private async void DeleteWebViewCache_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Cmd.CliWrap command = new Cmd.CliWrap
            {
                App = "wt.exe",
                WorkingDir = Program.AppPath,
                Arguments = new ArgumentsBuilder()
                    .Add(Path.Combine(Program.AppPath, "data", "cmd", "delete_webview_cache.cmd")) // 0
                    .Add(Program.AppVersion) // 1
                    .Add(Data.ReShadeVer) // 2
                    .Add(Data.UnlockerVer) // 3
                    .Add(Program.AppData) // 4
            };
            await Cmd.Execute(command);
        }


        // ---------------------------------- Logs ---------------------------------
        private static void LogSharingAlert()
        {
            int logSharingAlert = Program.Settings.ReadInt("Launcher", "LogSharingAlert", 0);
            if (logSharingAlert >= 2) return;

            Utils.ShowToast("Important", "Remember not to share log files with unfamiliar individuals, as they might contain sensitive data. Share them only with the Stella Mod developer.");
            Program.Settings.WriteInt("Launcher", "LogSharingAlert", logSharingAlert + 1);
        }

        private void LogDir_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!Directory.Exists(Log.Folder))
            {
                MessageBox.Show($"Directory with the Stella Logs was not found in:\n\n{Log.Folder}", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Log.Output($"Directory with the Stella Logs was not found in: {Log.Folder}");
                return;
            }

            Cmd.Start(Log.Folder);
            LogSharingAlert();
        }

        private async void LauncherLogs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Cmd.CliWrap command = new Cmd.CliWrap
            {
                App = "notepad.exe",
                WorkingDir = Program.AppPath,
                Arguments = new ArgumentsBuilder()
                    .Add(Path.Combine(Log.Folder, "launcher.output.log"))
            };
            await Cmd.Execute(command);

            LogSharingAlert();
        }

        private async void GSModLogs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            await Cmd.Execute(new Cmd.CliWrap
            {
                App = "notepad.exe",
                WorkingDir = Program.AppPath,
                Arguments = new ArgumentsBuilder()
                    .Add(Path.Combine(Log.Folder, "gsmod.output.log"))
            });

            LogSharingAlert();
        }

        private async void ReShadeLogs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string gameDir = await Utils.GetGame("giGameDir");
            string logFile = Path.Combine(gameDir, "ReShade.log");

            if (!Directory.Exists(gameDir) || !File.Exists(logFile))
                MessageBox.Show(string.Format(Resources.Tools_ReShadeLogFileWasNotFoundIn_, logFile), Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                await Cmd.Execute(new Cmd.CliWrap
                {
                    App = "notepad.exe",
                    WorkingDir = Program.AppPath,
                    Arguments = new ArgumentsBuilder()
                        .Add(logFile)
                });

            LogSharingAlert();
        }


        // -------------------------- Nothing special ((: ---------------------------
        private void Notepad_MouseClick(object sender, MouseEventArgs e)
        {
            string path = Path.Combine(Program.AppPath, "data", "videos", "poland-strong.mp4");
            if (!Utils.CheckFileExists(path)) return;

            WebViewWindow viewer = new WebViewWindow { DesktopLocation = DesktopLocation, Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) };
            viewer.Navigate(path);
            viewer.Show();
        }
    }
}
