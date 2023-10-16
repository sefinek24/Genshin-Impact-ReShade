using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using StellaLauncher.Forms.Other;
using StellaLauncher.Properties;
using StellaLauncher.Scripts;

namespace StellaLauncher.Forms
{
    public partial class Settings : Form
    {
        private bool _mouseDown;
        private Point _offset;

        public Settings()
        {
            InitializeComponent();
        }

        private void Tools_Load(object sender, EventArgs e)
        {
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
            new Language { Icon = Program.Ico }.ShowDialog();
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


        // ---------------------------------- Injection ----------------------------------
        private void ChangeInjectionMethod_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new InjectionMethod { Icon = Program.Ico }.ShowDialog();
        }
    }
}
