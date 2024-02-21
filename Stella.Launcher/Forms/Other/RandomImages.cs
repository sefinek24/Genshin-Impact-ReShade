using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using StellaModLauncher.Models;
using StellaModLauncher.Properties;
using StellaModLauncher.Scripts;
using StellaModLauncher.Scripts.Forms;

namespace StellaModLauncher.Forms.Other;

public sealed partial class RandomImages : Form
{
	private static Label _poweredBy;
	private string? _sourceUrl;

	public RandomImages()
	{
		InitializeComponent();

		DoubleBuffered = true;

		if (RegionInfo.CurrentRegion.Name == "PL") linkLabel46.Visible = true;
	}

	private void RandomThings_Load(object sender, EventArgs e)
	{
		_poweredBy = poweredBy_Label;
		WindowState = FormWindowState.Maximized;
	}

	private async void RandomImg_Shown(object sender, EventArgs e)
	{
		await WebViewHelper.Initialize(webView21, null);

		Discord.SetStatus(Resources.RandomImages_GeneratingBeautifulPictures);

		Program.Logger.Info(string.Format(Resources.Main_LoadedForm_, Text));
	}

	private void RandomImg_FormClosed(object sender, FormClosedEventArgs e)
	{
		Program.Logger.Info(string.Format(Resources.Main_ClosedForm_, Text));
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

		Music.PlaySound("winxp", "pop-up_blocked");

		try
		{
			string jsonResponse = await Program.SefinWebClient.GetStringAsync(url);
			return jsonResponse;
		}
		catch (WebException e)
		{
			if (e.Status == WebExceptionStatus.ProtocolError && e.Response is HttpWebResponse response)
				MessageBox.Show(e.Message, Program.AppNameVer, MessageBoxButtons.OK, (int)response.StatusCode >= 500 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
			else
				MessageBox.Show(e.Message, Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Error);

			Program.Logger.Error(string.Format(Resources.RandomImages_ErrorWithTheAPI, url, e));
			return null;
		}
	}

	private async void SefinekApi(string url)
	{
		string json = await GetData(url);
		if (json == null) return;

		SefinekApi res = JsonConvert.DeserializeObject<SefinekApi>(json);

		webView21.CoreWebView2.Navigate(res.Message);
		text_Label.Visible = false;

		Program.Logger.Info($"Received data from: {new Uri(url).Host}; Success {res.Success}; Status {res.Status}; Category {res.Info.Category}; Endpoint {res.Info.Endpoint}; Message {res.Message};");
	}

	private async void NekosBest(string url, bool gif) // The best api uwu
	{
		string json = await GetData(url);
		if (json == null) return;

		NekosBest res = JsonConvert.DeserializeObject<NekosBest>(json);

		string picUrl = res.Results[0].Url;
		webView21.CoreWebView2.Navigate(picUrl);

		string animeName = res.Results[0].Anime_name ?? "N/A";
		string? sourceUrl = res.Results[0].Source_url;

		if (gif)
		{
			text_Label.Text = $"{Resources.RandomImages_AnimeName}\n{animeName}";
			text_Label.LinkBehavior = LinkBehavior.NeverUnderline;
		}
		else
		{
			text_Label.ActiveLinkColor = Color.DodgerBlue;
			text_Label.Text = $"{Resources.RandomImages_Source}\n{Regex.Replace(sourceUrl, @"(?:https?://|www\.)", "")}";
			text_Label.LinkBehavior = LinkBehavior.HoverUnderline;
		}

		text_Label.Visible = true;
		_sourceUrl = sourceUrl;

		Program.Logger.Info($"Received data from {new Uri(url).Host}; Anime_name {animeName}; Source_url {sourceUrl}; Url {picUrl};");
	}

	private async void PurrBot(string url)
	{
		string json = await GetData(url);
		if (json == null) return;

		PurrBot res = JsonConvert.DeserializeObject<PurrBot>(json);

		webView21.CoreWebView2.Navigate(res.Link);
		_sourceUrl = res.Link;
		text_Label.Visible = false;

		Program.Logger.Info($"Received data from {new Uri(url).Host}; Link {res.Link};");
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

		Program.Logger.Info($"Received data from {new Uri(url).Host}; Color {res.Color} rgbColor {rgbColor}; Message {res.Message};");
	}

	/* Random animals */
	private void RandomCat_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		SefinekApi("https://api.sefinek.net/api/v2/random/animal/cat");
	}

	private void RandomDog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		SefinekApi("https://api.sefinek.net/api/v2/random/animal/dog");
	}

	private void RandomFox_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		SefinekApi("https://api.sefinek.net/api/v2/random/animal/fox");
	}

	/* Random anime bitches */
	private void CatGirl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/neko", false);
	}

	private void FoxGirl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/kitsune", false);
	}

	private void Waifu_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/waifu", false);
	}

	private void Coffee_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekoBot("https://nekobot.xyz/api/image?type=coffee");
	}

	private void Shiro_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		PurrBot("https://purrbot.site/api/img/sfw/shiro/img");
	}

	private void Holo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		PurrBot("https://purrbot.site/api/img/sfw/holo/img");
	}

	private void Senko_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		PurrBot("https://purrbot.site/api/img/sfw/senko/img");
	}

	/* Random anime gifs */
	private void Hug_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/hug", true);
	}

	private void Cuddle_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/cuddle", true);
	}

	private void Kiss_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/kiss", true);
	}

	private void Happy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/happy", true);
	}

	private void Cry_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/cry", true);
	}

	private void Pat_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/pat", true);
	}

	private void Wink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/wink", true);
	}

	private void Wave_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/wave", true);
	}

	private void Thumbsup_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/thumbsup", true);
	}

	private void Blush_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/blush", true);
	}

	private void Smile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/smile", true);
	}

	private void Laugh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/laugh", true);
	}

	private void Shoot_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/shoot", true);
	}

	private void Sleep_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/sleep", true);
	}

	/* Random anime gifs */
	private void Baka_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/baka", true);
	}

	private void Bite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/bite", true);
	}

	private void Bored_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/bored", true);
	}

	private void Dance_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/dance", true);
	}

	private void FacePalm_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/facepalm", true);
	}

	private void Feed_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/feed", true);
	}

	private void Handhold_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/handhold", true);
	}

	private void HighFive_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/highfive", true);
	}

	private void Kick_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/kick", true);
	}

	private void Poke_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/poke", true);
	}

	private void Pout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/pout", true);
	}

	private void Punch_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/punch", true);
	}

	private void Shrug_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/shrug", true);
	}

	private void Slap_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/slap", true);
	}

	private void Smug_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/smug", true);
	}

	private void Stare_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/stare", true);
	}

	private void Think_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/think", true);
	}

	private void Tickle_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/tickle", true);
	}

	private void Yeet_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		NekosBest("https://nekos.best/api/v2/yeet", true);
	}

	private void Tail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		PurrBot("https://purrbot.site/api/img/sfw/tail/gif");
	}

	/* Random YouTube videos */
	private void HlCat_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		SefinekApi("https://api.sefinek.net/api/v2/random/yt-video/hl-cats");
	}

	/* Footer */
	private void Source_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		if (!string.IsNullOrEmpty(_sourceUrl)) Utils.OpenUrl(_sourceUrl);
	}

	private void RandomHentai_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		webView21.CoreWebView2.Navigate(Path.Combine(Program.AppPath, "data", "videos", "gengbeng.mp4"));
	}
}
