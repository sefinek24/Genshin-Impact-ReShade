using Microsoft.Web.WebView2.Core;
using StellaModLauncher.Properties;
using StellaModLauncher.Scripts;

namespace StellaModLauncher.Forms.Other;

public partial class WebView2Window : Form
{
	internal string Title;
	internal string Url;

	public WebView2Window()
	{
		InitializeComponent();
	}

	private void Gallery_Load(object sender, EventArgs e)
	{
		Text = Title ?? "sefinek.net";
		Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

		InitWebView2();

		Discord.SetStatus(Resources.Gallery_BrowsingTheGallery);
		Program.Logger.Info(string.Format(Resources.Main_LoadedForm_, Text));
	}

	private async void InitWebView2()
	{
		await webView21.EnsureCoreWebView2Async(await CoreWebView2Environment.CreateAsync(null, Program.AppData, new CoreWebView2EnvironmentOptions()));
		webView21.CoreWebView2.Settings.UserAgent += $" StellaLauncher/{Program.ProductVersion}";
		webView21.CoreWebView2.Navigate(Url);
	}

	private void Gallery_FormClosed(object sender, FormClosedEventArgs e)
	{
		Discord.SetStatus(Resources.Gallery_ExitedTheGallery);

		Program.Logger.Info(string.Format(Resources.Main_ClosedForm_, Text));
	}
}
