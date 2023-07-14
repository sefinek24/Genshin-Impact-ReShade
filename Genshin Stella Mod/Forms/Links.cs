using System;
using System.Drawing;
using System.Windows.Forms;
using StellaLauncher.Properties;
using StellaLauncher.Scripts;

namespace StellaLauncher.Forms
{
    public partial class Links : Form
    {
        private bool _mouseDown;
        private Point _offset;

        public Links()
        {
            InitializeComponent();
        }

        private void URLs_Shown(object sender, EventArgs e)
        {
            Discord.SetStatus(Resources.Links_DRPC_OnTheWindowWithLinks);

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


        // --------------------------------- Game map ---------------------------------
        private void GenshinMap1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl("https://act.hoyolab.com/ys/app/interactive-map/index.html");
        }

        private void GenshinMap2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl("https://genshin-impact-map.appsample.com");
        }


        // -------------------------------- Characters --------------------------------
        private void TierList1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl("https://genshin.gg/tier-list");
        }

        private void TierList2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl("https://game8.co/games/Genshin-Impact/archives/297465");
        }


        // ------------------------------- Other links --------------------------------
        private void Uptimerobot_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl("https://stats.uptimerobot.com/kLXYEukEwW");
        }

        private void DataDog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl("https://p.datadoghq.eu/sb/4oq87a1ven2jwa4a-7ac3302923e782d617afbbc561d2f88d?theme=dark");
        }

        private void Api_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl("https://api.sefinek.net");
        }

        private void Cdn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utils.OpenUrl("https://cdn.sefinek.net");
        }


        // --------------------------- Nothing special ((: ----------------------------
        private void Flower_MouseClick(object sender, MouseEventArgs e)
        {
            if (ComputerInfo.GetSystemRegion() != "PL") return;

            Utils.OpenUrl("https://www.youtube.com/watch?v=zSeosXivzG4");
            MessageBox.Show(
                "{Zwrotka 1}\nHola!\nPierdole sobie konia (sobie konia)\nI ja\nRozpierdolę sobie chuja konia\nPierdole sobie gówno\nJesteś dla mnie kurwą\nRozpierdolę sobie gówno\nTwoja matka zwykłą kurwą\nJa rozjebałem sobie fiuta\nI ja, ściągam se mocarza bucha\nMuszę pałę sobie gnieść\nPotem odbyt Tobie zjeść\nPiwo sobie wypić tez\nKurwą mówię precz\n\n{Refren}\nChuj stoi dla mnie kurwo downie\nJak oglądam gówno downie gówno\nChuj stoi dla mnie kurwo downie\nJak oglądam gówno downie kurwo\n\n{Zwrotka 2}\nWale konia\nPije piwo\nTwoja stara to warzywo\nGówno z dupy twoja stara\nOna jest dziś jebana\nPale mocarz pale dopa\nA twój stary chuj i ciota\nZwykła pizda do jebania\nDziwka jest dziś ospermiana\nGram w minecraft'a pale peta\nWciągam dopy to uciecha\nSpotkam kurwę go rozjebie\nJego głowa, leży w zlewie\nRozjebana głowa twoja stara ospermiona\nŚciągam kreskę topa wale konia pale mocarz\n\n{Refren}\nChuj stoi dla mnie kurwo downie\nJak oglądam gówno downie gówno\nChuj stoi dla mnie kurwo downie\nJak oglądam gówno downie kurwo");
        }
    }
}
