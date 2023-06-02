using System;
using System.Diagnostics;
using System.Windows.Forms;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts
{
    internal static class WebView2
    {
        public static void HandleError(Exception ex)
        {
            if (ex.HResult == -2146233088)
            {
                DialogResult res = MessageBox.Show(string.Format(Resources.WebView2Handler_DoYouWantToDownloadThisDependencyFromMStore, ex.Message), Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    Process.Start("https://developer.microsoft.com/en-us/microsoft-edge/webview2/#download-section");
                    MessageBox.Show(Resources.WebView2Handler_ChooseEvergreenStandaloneInstaller, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show(Resources.WebView2Handler_OhhSorrySomethingWentWrongWithWV2, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Log.SaveErrorLog(ex);
        }
    }
}
