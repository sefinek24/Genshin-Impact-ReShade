using System;
using System.Media;
using System.Windows.Forms;
using StellaLauncher.Scripts;

namespace StellaLauncher.Forms.Errors
{
    public partial class NotInstalledViaSetup : Form
    {
        public NotInstalledViaSetup()
        {
            InitializeComponent();
        }

        private void NotConfigured_Load(object sender, EventArgs e)
        {
            SystemSounds.Beep.Play();

            Log.Output($"Loaded form '{Text}'.");
            Log.SaveErrorLog(new Exception($"Launcher is not installed using our installation wizard.\n\nApplication data: {Program.AppData}"));
        }

        private void NotInstalledViaSetup_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Output($"Closed form '{Text}'.");
        }

        private void Installer_Button(object sender, EventArgs e)
        {
            Utils.OpenUrl(Program.AppWebsiteFull);
        }

        private void Discord_Button(object sender, EventArgs e)
        {
            Utils.OpenUrl(Discord.Invitation);
        }
    }
}
