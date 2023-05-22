using System;
using System.Media;
using System.Windows.Forms;
using Genshin_Stella_Mod.Scripts;
using Microsoft.Toolkit.Uwp.Notifications;

namespace Genshin_Stella_Mod.Forms.Errors
{
    public partial class WrongCountry : Form
    {
        private readonly Random _random = new Random();

        public WrongCountry()
        {
            InitializeComponent();
        }

        private void WrongCountry_Shown(object sender, EventArgs e)
        {
            try
            {
                new SoundPlayer { SoundLocation = @"data\kurwa.wav" }.Play();
            }
            catch (Exception ex)
            {
                Log.SaveErrorLog(ex);
            }

            Timer randomSizeTimer = new Timer { Interval = 500 };
            randomSizeTimer.Tick += ChangeWindowSize;
            randomSizeTimer.Start();

            Timer closeAppTimer = new Timer { Interval = 35300 };
            closeAppTimer.Tick += (sender1, e1) => Application.Exit();
            closeAppTimer.Start();

            try
            {
                new ToastContentBuilder()
                    .AddText("slava poland")
                    .AddText("axaxaxaxaxaxaxa language for monkeys lol")
                    .Show();
            }
            catch (Exception ex)
            {
                Log.SaveErrorLog(ex);
            }

            Log.Output($"Loaded form '{Text}'.");
        }

        private void WrongCountry_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Output($"Closed form '{Text}'.");
        }

        private void ChangeWindowSize(object sender, EventArgs e)
        {
            Width = _random.Next(200, 800);
            Height = _random.Next(200, 800);
        }
    }
}
