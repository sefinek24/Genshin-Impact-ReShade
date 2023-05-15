using System;
using System.Media;
using System.Windows.Forms;
using Genshin_Stella_Mod.Scripts;

namespace Genshin_Stella_Mod.Forms.Errors
{
    public partial class WrongDirectory : Form
    {
        public WrongDirectory()
        {
            InitializeComponent();
        }

        private void WrongDir_Shown(object sender, EventArgs e)
        {
            SystemSounds.Beep.Play();

            Log.Output($"Loaded form '{Text}'.");
            Log.SaveErrorLog(new Exception($"Invalid application path.\n\nYour: {Environment.CurrentDirectory}\nRequired: ???"));
        }

        private void WrongDir_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Output($"Closed form '{Text}'.");
        }

        private void Youtube_Button(object sender, EventArgs e)
        {
            Utils.OpenUrl("https://www.youtube.com/watch?v=rDeO26RapAk");
        }

        private void Discord_Button(object sender, EventArgs e)
        {
            Utils.OpenUrl("https://discord.gg/SVcbaRc7gH");
        }
    }
}
