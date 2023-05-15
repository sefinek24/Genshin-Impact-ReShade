using System;
using System.Media;
using System.Windows.Forms;
using Genshin_Stella_Mod.Scripts;

namespace Genshin_Stella_Mod.Forms.Errors
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

            Log.Output($"Loaded form '{Text}'.");
        }

        private void NotCompatible_Closed(object sender, FormClosedEventArgs e)
        {
            Log.Output($"Closed form '{Text}'.");
        }

        private void DownloadInstaller_Click(object sender, EventArgs e)
        {
            Utils.OpenUrl(Program.AppWebsiteFull);
        }
    }
}
