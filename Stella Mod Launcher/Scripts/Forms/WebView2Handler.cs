using System;
using System.Diagnostics;
using System.Windows.Forms;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts.Forms
{
	internal static class WebView2
	{
		public static void HandleError(Exception ex)
		{
			if (ex.HResult == -2146233088)
			{
				DialogResult res = MessageBox.Show(
					string.Format(Resources.WebView2Handler_DoYouWantToDownloadThisDependencyFromMStore, ex.Message),
					Program.AppNameVer, MessageBoxButtons.YesNo, MessageBoxIcon.Question
				);

				if (res == DialogResult.Yes)
				{
					Process.Start("https://developer.microsoft.com/en-us/microsoft-edge/webview2/#download-section");
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
