using System;
using System.IO;
using System.Windows.Forms;
using CliWrap.Builders;
using StellaLauncher.Properties;
using StellaLauncher.Scripts;
using StellaLauncher.Scripts.Forms.MainForm;

namespace StellaLauncher.Forms.Errors
{
    public partial class ErrorOccurred : Form
    {
        public ErrorOccurred()
        {
            InitializeComponent();
        }

        private void ErrorOccurred_Load(object sender, EventArgs e)
        {
            label1.Text = string.Format(label1.Text, Data.Discord, Data.Email);

            Music.PlaySound("winxp", "error");
        }

        private void ErrorOccurred_Shown(object sender, EventArgs e)
        {
            Log.Output(string.Format(Resources.Main_LoadedForm_, Text));
        }

        private void ErrorOccurred_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Output(string.Format(Resources.Main_ClosedForm_, Text));
        }

        private void SeeLogs_Button(object sender, EventArgs e)
        {
            Utils.OpenUrl(Log.Folder);
        }

        private void Reinstall_Button(object sender, EventArgs e)
        {
            Utils.OpenUrl($"{Program.AppWebsiteFull}/download?referrer=error_occurred&hash=null");
        }

        private void Discord_Button(object sender, EventArgs e)
        {
            Utils.OpenUrl(Discord.Invitation);
        }

        private async void SfcScan_Click(object sender, EventArgs e)
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
    }
}
