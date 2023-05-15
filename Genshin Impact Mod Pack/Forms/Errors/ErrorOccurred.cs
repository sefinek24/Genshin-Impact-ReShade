using System;
using System.Media;
using System.Windows.Forms;
using Genshin_Stella_Mod.Scripts;

namespace Genshin_Stella_Mod.Forms.Errors
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

            Log.Output($"Loaded form '{Text}'.");
        }

        private void ErrorOccurred_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Output($"Closed form '{Text}'.");
        }

        private void SeeLogs_Button(object sender, EventArgs e)
        {
            Utils.OpenUrl(Log.Folder);
        }

        private void Reinstall_Button(object sender, EventArgs e)
        {
            Utils.OpenUrl(Program.AppWebsiteFull);
        }

        private void Discord_Button(object sender, EventArgs e)
        {
            Utils.OpenUrl(Discord.Invitation);
        }

        private async void SfcScan_Click(object sender, EventArgs e)
        {
            await Cmd.CliWrap("wt", @"data\cmd\scan_sys_files.cmd", null, true, false);
        }
    }
}
