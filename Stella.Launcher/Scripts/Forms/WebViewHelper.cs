using System.Diagnostics;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using StellaModLauncher.Properties;

namespace StellaModLauncher.Scripts.Forms;

internal static class WebViewHelper
{
	public static async Task Initialize(WebView2 webView21, string? url)
	{
		try
		{
			await webView21.EnsureCoreWebView2Async(await CoreWebView2Environment.CreateAsync(null, Program.AppData, new CoreWebView2EnvironmentOptions()).ConfigureAwait(false)).ConfigureAwait(true);
			webView21.CoreWebView2.Settings.UserAgent += $" StellaLauncher/{Program.AppFileVersion}";

			Program.Logger.Info("Loaded WebView2 via WebViewHelper.Initialize()");

			if (!string.IsNullOrEmpty(url))
			{
				webView21.CoreWebView2.Navigate(url);
				Program.Logger.Info($"Navigate {url}");
			}
		}
		catch (Exception ex)
		{
			if (ex.HResult == -2146233088)
			{
				DialogResult res = MessageBox.Show(
					string.Format(Resources.WebView2Handler_DoYouWantToDownloadThisDependencyFromMStore, ex.Message),
					Program.AppNameVer, MessageBoxButtons.YesNo, MessageBoxIcon.Question
				);

				if (res == DialogResult.Yes)
				{
					Utils.OpenUrl("https://developer.microsoft.com/en-us/microsoft-edge/webview2/#download-section");
					MessageBox.Show(Resources.WebView2Handler_ChooseEvergreenStandaloneInstaller, Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			else
			{
				MessageBox.Show(Resources.WebView2Handler_OhhSorrySomethingWentWrongWithWV2, Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			Program.Logger.Error(ex.ToString());
		}
	}
}
