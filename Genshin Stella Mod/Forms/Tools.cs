using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
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
                    Log.SaveErrorLog(new Exception(Resources.Tools_SpecialBackgroundWasNotFoundIn_ForToolsWindow));
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
        private void OpenConfWindow_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DialogResult res = MessageBox.Show(@"Are you sure to run Stella Configuration Window again?", Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.No) return;

            string path = Path.Combine(Program.AppPath, "First app launch.exe");
            if (!File.Exists(path))
            {
                string fileName = Path.GetFileName(path);
                MessageBox.Show($@"Required file '{fileName}' was not found in the main mod directory.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.SaveErrorLog(new Exception($"File {fileName} was not found in {path}."));
                return;
            }

            string confSfn = Path.Combine(Program.AppData, "configured.sfn");
            if (File.Exists(confSfn)) File.Delete(confSfn);

            _ = Cmd.CliWrap(path, null, null, true, false);
            Environment.Exit(0);
        }

        private void CreateShortcut_Button(object sender, EventArgs e)
        {
            bool success = Utils.CreateShortcut();
            if (success) MessageBox.Show(@"The shortcut has been successfully created.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MuteMusic_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MusicLabel_Set();

            int data = Program.Settings.ReadInt("Launcher", "EnableMusic", 1);
            switch (data)
            {
                case 0:
                    Program.Settings.WriteInt("Launcher", "EnableMusic", 1);
                    break;
                case 1:
                    Program.Settings.WriteInt("Launcher", "EnableMusic", 0);
                    break;
            }
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
                    Log.SaveErrorLog(new Exception(Resources.Tools_WrongEnableMusicValueInTheIniFile));
                    MessageBox.Show(Resources.Tools_WrongEnableMusicValueInTheIniFile, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            }
        }

        private void DisableRPC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RPCLabel_Set();

            int iniData = Program.Settings.ReadInt("Launcher", "DiscordRPC", 1);
            switch (iniData)
            {
                case 0:
                    Program.Settings.WriteInt("Launcher", "DiscordRPC", 1);
                    Discord.InitRpc();
                    if (Discord.Username.Length > 0) // Fix
                        MessageBox.Show(string.Format(Resources.Tools_YoureConnectedAs__HiAndNiceToMeetYou_CheckYourDiscordActivity, Discord.Username), Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case 1:
                    Program.Settings.WriteInt("Launcher", "DiscordRPC", 0);
                    Discord.Client.Dispose();
                    break;
            }
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
                default:
                    Log.SaveErrorLog(new Exception(Resources.Tools_WrongEnableMusicValueInTheIniFile));
                    MessageBox.Show(Resources.Tools_WrongEnableMusicValueInTheIniFile, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            }
        }

        private void ChangeLang_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new Language { Icon = Resources.icon_52x52 }.ShowDialog();
        }


        // ---------------------------------- Misc ----------------------------------
        private async void ScanSysFiles_Click(object sender, EventArgs e)
        {
            await Cmd.CliWrap("wt.exe", Path.Combine(Program.AppPath, "data", "cmd", "scan_sys_files.cmd"), Program.AppPath, true, false);
        }


        // ------------------------------ Config files ------------------------------
        private async void ReShadeConfig_Click(object sender, EventArgs e)
        {
            string gamePath = await Utils.GetGame("giGameDir");
            string reShadeIni = Path.Combine(gamePath, "ReShade.ini");

            if (!File.Exists(reShadeIni))
                MessageBox.Show(string.Format(Resources.Tools_ReShadeConfigFileWasNotFoundIn_, reShadeIni), Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                await Cmd.CliWrap("notepad.exe", reShadeIni, null, true, false);
        }

        private async void UnlockerConfig_Click(object sender, EventArgs e)
        {
            await Cmd.CliWrap("notepad.exe", Path.Combine(Program.AppPath, "data", "unlocker", "unlocker.config.json"), null, true, false);
        }


        // --------------------------------- Cache ---------------------------------
        private async void DeleteCache_Button(object sender, EventArgs e)
        {
            string resources = File.ReadAllText(Path.Combine(Program.AppData, "resources-path.sfn"));
            string cache = Path.Combine(resources, "Cache");
            string webViewCache = Path.Combine(Program.AppData, "EBWebView");
            string gameDir = await Utils.GetGame("giGameDir");
            string reShadeLog = Path.Combine(gameDir, "ReShade.log");
            string logs = Path.Combine(Log.Folder);

            await Cmd.CliWrap(
                "wt.exe",
                $"{Path.Combine(Program.AppPath, "data", "cmd", "delete_cache.cmd")} \"{Path.Combine(Program.AppData, "game-path.sfn")}\" \"{cache}\" \"{webViewCache}\" \"{reShadeLog}\" \"{logs}\"",
                Program.AppPath, true, false);
        }

        private async void DeleteWebViewCache_Click(object sender, EventArgs e)
        {
            await Cmd.CliWrap("wt.exe", Path.Combine(Program.AppPath, "data", "cmd", "delete_webview_cache.cmd"), Program.AppPath, true, false);
        }


        // ---------------------------------- Logs ---------------------------------
        private async void LauncherLogs_Click(object sender, EventArgs e)
        {
            await Cmd.CliWrap("notepad.exe", Path.Combine(Log.Folder, "launcher.output.log"), null, true, false);
        }

        private async void ReShadeLogs_Button(object sender, EventArgs e)
        {
            string gameDir = await Utils.GetGame("giGameDir");
            string logFile = Path.Combine(gameDir, "ReShade.log");

            if (!Directory.Exists(gameDir) || !File.Exists(logFile))
                MessageBox.Show(string.Format(Resources.Tools_ReShadeLogFileWasNotFoundIn_, logFile), Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                await Cmd.CliWrap("notepad.exe", logFile, null, true, false);
        }

        private async void PreparationLogs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            await Cmd.CliWrap("notepad.exe", Path.Combine(Log.Folder, "prepare.output.log"), null, true, false);
        }


        private async void InnoSetup_Button(object sender, EventArgs e)
        {
            await Cmd.CliWrap("notepad.exe", Path.Combine(Log.Folder, "innosetup-logs.install.log"), null, true, false);
        }

        // -------------------------- Nothing special ((: ---------------------------
        private void Notepad_MouseClick(object sender, MouseEventArgs e)
        {
            string path = Path.Combine(Program.AppPath, "data", "videos", "poland-strong.mp4");
            if (!Utils.CheckFileExists(path)) return;

            WebViewWindow viewer = new WebViewWindow { Location = Location, Icon = Resources.icon_52x52 };
            viewer.Navigate(path);
            viewer.Show();
        }
    }
}
