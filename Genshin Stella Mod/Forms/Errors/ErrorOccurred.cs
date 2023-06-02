using System;
using System.IO;
using System.Media;
using System.Windows.Forms;
using StellaLauncher.Properties;
using StellaLauncher.Scripts;

namespace StellaLauncher.Forms.Errors
{
    public partial class ErrorOccurred : Form
    {
        public ErrorOccurred()
        {
            InitializeComponent();
        }

        private void ErrorOccurred_Shown(object sender, EventArgs e)
        {
            SystemSounds.Hand.Play();

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
            Utils.OpenUrl($"{Program.AppWebsiteFull}/download");
        }

        private void Discord_Button(object sender, EventArgs e)
        {
            Utils.OpenUrl(Discord.Invitation);
        }

        private async void SfcScan_Click(object sender, EventArgs e)
        {
            await Cmd.CliWrap("wt", Path.Combine(Program.AppPath, "data", "cmd", "scan_sys_files.cmd"), null, true, false);
        }
    }
}
