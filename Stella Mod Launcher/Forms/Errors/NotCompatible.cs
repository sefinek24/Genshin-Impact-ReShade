using System;
using System.Media;
using System.Windows.Forms;
using StellaLauncher.Properties;
using StellaLauncher.Scripts;

namespace StellaLauncher.Forms.Errors
{
    public partial class NotCompatible : Form
    {
        public NotCompatible()
        {
            InitializeComponent();
        }

        private void NotCompatible_Shown(object sender, EventArgs e)
        {
            SystemSounds.Beep.Play();

            Log.Output(string.Format(Resources.Main_LoadedForm_, Text));
        }

        private void NotCompatible_Closed(object sender, FormClosedEventArgs e)
        {
            Log.Output(string.Format(Resources.Main_ClosedForm_, Text));
        }

        private void DownloadInstaller_Click(object sender, EventArgs e)
        {
            Utils.OpenUrl(Program.AppWebsiteFull);
        }
    }
}
