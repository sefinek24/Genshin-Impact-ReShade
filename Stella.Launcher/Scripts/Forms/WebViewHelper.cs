using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;

namespace StellaLauncher.Scripts.Forms
{
	internal static class WebViewHelper
	{
		public static async Task Initialize(Microsoft.Web.WebView2.WinForms.WebView2 webView21, string url)
		{
			await webView21.EnsureCoreWebView2Async(await CoreWebView2Environment.CreateAsync(null, Program.AppData, new CoreWebView2EnvironmentOptions()));
			webView21.CoreWebView2.Settings.UserAgent += $" StellaLauncher/{Program.ProductVersion}";

			webView21.CoreWebView2.Navigate(url);
		}
	}
}
