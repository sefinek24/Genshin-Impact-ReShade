using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using StellaLauncher.Models;
using StellaLauncher.Properties;
using StellaLauncher.Scripts;
using StellaLauncher.Scripts.Forms;

namespace StellaLauncher.Forms.Other
{
    public partial class RandomImages : Form
    {
        private static Label _poweredBy;
        private string _sourceUrl;

        public RandomImages()
        {
            InitializeComponent();

            webView21.DefaultBackgroundColor = Color.Transparent;

            if (RegionInfo.CurrentRegion.Name == "PL") linkLabel46.Visible = true;
        }

        private void RandomThings_Load(object sender, EventArgs e)
        {
            _poweredBy = poweredBy_Label;
            WindowState = FormWindowState.Maximized;
        }

        private async void RandomImg_Shown(object sender, EventArgs e)
        {
            try
            {
                CoreWebView2Environment coreWeb = await CoreWebView2Environment.CreateAsync(null, Program.AppData, new CoreWebView2EnvironmentOptions());
                await webView21.EnsureCoreWebView2Async(coreWeb);
            }
            catch (Exception ex)
            {
                WebView2.HandleError(ex);
                Close();
            }

            Discord.SetStatus(Resources.RandomImages_GeneratingBeautifulPictures);

            Log.Output(string.Format(Resources.Main_LoadedForm_, Text));
        }

        private void RandomImg_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Output(string.Format(Resources.Main_ClosedForm_, Text));
            Discord.Home();
        }

        private static async Task<string> GetData(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                string host = uri.Host;

                int colonIndex = _poweredBy.Text.IndexOf(':');
                if (colonIndex >= 0)
                {
                    string prefix = _poweredBy.Text.Substring(0, colonIndex + 1);
                    _poweredBy.Text = $@"{prefix} {host}";
                }
                else
                {
                    _poweredBy.Text = host;
                }

                _poweredBy.Visible = true;
            }


            try
            {
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", Program.UserAgent);
                return await client.DownloadStringTaskAsync(url);
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError && e.Response is HttpWebResponse response)
                    MessageBox.Show(e.Message, Program.AppName, MessageBoxButtons.OK, (int)response.StatusCode >= 500 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
                else
                    MessageBox.Show(e.Message, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                Log.SaveErrorLog(new Exception(string.Format(Resources.RandomImages_ErrorWithTheAPI, url, e)));
                return null;
            }
        }

        private async void SkiffyApi(string url)
        {
            string json = await GetData(url);
            if (json == null) return;

            SkiffyBotApi res = JsonConvert.DeserializeObject<SkiffyBotApi>(json);

            webView21.CoreWebView2.Navigate(res.Message);
            text_Label.Visible = false;

            Log.Output($"{string.Format(Resources.RandomImages_ReceivedDataFrom, new Uri(url).Host)}: {res.Success} {res.Status} {res.Category} {res.Endpoint} {res.Message}");
        }

        private async void NekosBest(string url, bool gif) // The best api uwu
        {
            string json = await GetData(url);
            if (json == null) return;

            NekosBest res = JsonConvert.DeserializeObject<NekosBest>(json);

            webView21.CoreWebView2.Navigate(res.Results[0].Url);

            if (gif)
            {
                text_Label.ActiveLinkColor = Color.Transparent;
                text_Label.Text = $"{Resources.RandomImages_AnimeName}\n{res.Results[0].Anime_name}";
                text_Label.LinkBehavior = LinkBehavior.NeverUnderline;
            }
            else
            {
                text_Label.ActiveLinkColor = Color.DodgerBlue;
                text_Label.Text = $"{Resources.RandomImages_Source}\n{Regex.Replace(res.Results[0].Source_url, @"(?:https?://|www\.)", "")}";
                text_Label.LinkBehavior = LinkBehavior.HoverUnderline;
            }

            text_Label.Visible = true;
            _sourceUrl = res.Results[0].Source_url;

            Log.Output($"{string.Format(Resources.RandomImages_ReceivedDataFrom, new Uri(url).Host)}: {res.Results[0].Anime_name} {res.Results[0].Source_url} {res.Results[0].Url}");
        }

        private async void PurrBot(string url)
        {
            string json = await GetData(url);
            if (json == null) return;

            PurrBot res = JsonConvert.DeserializeObject<PurrBot>(json);

            webView21.CoreWebView2.Navigate(res.Link);
            _sourceUrl = res.Link;
            text_Label.Visible = false;

            Log.Output($"{string.Format(Resources.RandomImages_ReceivedDataFrom, new Uri(url).Host)}: {res.Link}");
        }

        private async void NekoBot(string url)
        {
            string json = await GetData(url);
            NekoBot res = JsonConvert.DeserializeObject<NekoBot>(json);

            webView21.CoreWebView2.Navigate(res.Message);

            Color color = ColorTranslator.FromHtml(res.Color);
            Color rgbColor = Color.FromArgb(Convert.ToInt16(color.R), Convert.ToInt16(color.G), Convert.ToInt16(color.B));

            text_Label.LinkColor = rgbColor;
            poweredBy_Label.ForeColor = rgbColor;
            text_Label.Text = $@">> {Resources.RandomImages_OpenImageInDefaultBrowser} <<";
            text_Label.LinkBehavior = LinkBehavior.HoverUnderline;

            text_Label.Visible = true;
            _sourceUrl = res.Message;

            Log.Output($"{string.Format(Resources.RandomImages_ReceivedDataFrom, new Uri(url).Host)}: {res.Color} {rgbColor} {res.Message}");
        }

        /* Random animals */
        private void RandomCat_Click(object sender, EventArgs e)
        {
            SkiffyApi("https://api.sefinek.net/api/v2/random/animal/cat");
        }

        private void RandomDog_Click(object sender, EventArgs e)
        {
            SkiffyApi("https://api.sefinek.net/api/v2/random/animal/dog");
        }

        private void RandomFox_Click(object sender, EventArgs e)
        {
            SkiffyApi("https://api.sefinek.net/api/v2/random/animal/fox");
        }

        /* Random anime bitches */
        private void CatGirl_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/neko", false);
        }

        private void FoxGirl_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/kitsune", false);
        }

        private void Waifu_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/waifu", false);
        }

        private void Coffee_Click(object sender, EventArgs e)
        {
            NekoBot("https://nekobot.xyz/api/image?type=coffee");
        }

        private void Shiro_Click(object sender, EventArgs e)
        {
            PurrBot("https://purrbot.site/api/img/sfw/shiro/img");
        }

        private void Holo_Click(object sender, EventArgs e)
        {
            PurrBot("https://purrbot.site/api/img/sfw/holo/img");
        }

        private void Senko_Click(object sender, EventArgs e)
        {
            PurrBot("https://purrbot.site/api/img/sfw/senko/img");
        }

        /* Random anime gifs */
        private void Hug_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/hug", true);
        }

        private void Cuddle_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/cuddle", true);
        }

        private void Kiss_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/kiss", true);
        }

        private void Happy_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/happy", true);
        }

        private void Cry_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/cry", true);
        }

        private void Pat_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/pat", true);
        }

        private void Wink_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/wink", true);
        }

        private void Wave_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/wave", true);
        }

        private void Thumbsup_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/thumbsup", true);
        }

        private void Blush_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/blush", true);
        }

        private void Smile_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/smile", true);
        }

        private void Laugh_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/laugh", true);
        }

        private void Shoot_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/shoot", true);
        }

        private void Sleep_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/sleep", true);
        }

        /* Random anime gifs */
        private void Baka_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/baka", true);
        }

        private void Bite_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/bite", true);
        }

        private void Bored_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/bored", true);
        }

        private void Dance_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/dance", true);
        }

        private void FacePalm_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/facepalm", true);
        }

        private void Feed_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/feed", true);
        }

        private void Handhold_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/handhold", true);
        }

        private void Highfive_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/highfive", true);
        }

        private void Kick_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/kick", true);
        }

        private void Poke_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/poke", true);
        }

        private void Pout_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/pout", true);
        }

        private void Punch_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/punch", true);
        }

        private void Shrug_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/shrug", true);
        }

        private void Slap_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/slap", true);
        }

        private void Smug_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/smug", true);
        }

        private void Stare_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/stare", true);
        }

        private void Think_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/think", true);
        }

        private void Tickle_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/tickle", true);
        }

        private void Yeet_Click(object sender, EventArgs e)
        {
            NekosBest("https://nekos.best/api/v2/yeet", true);
        }

        private void Tail_Click(object sender, EventArgs e)
        {
            PurrBot("https://purrbot.site/api/img/sfw/tail/gif");
        }

        /* Random YouTube videos */
        private void HlCat_Click(object sender, EventArgs e)
        {
            SkiffyApi("https://api.sefinek.net/api/v2/random/yt-video/hl-cats");
        }

        /* Footer */
        private void Source_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_sourceUrl)) Utils.OpenUrl(_sourceUrl);
        }

        private void RandomHentai_Click(object sender, EventArgs e)
        {
            webView21.CoreWebView2.Navigate(Path.Combine(Program.AppPath, "data", "videos", "gengbeng.mp4"));
        }
    }
}
