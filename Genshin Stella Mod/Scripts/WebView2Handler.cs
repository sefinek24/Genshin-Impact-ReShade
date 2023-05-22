using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Genshin_Stella_Mod.Scripts
{
    internal static class WebView2
    {
        public static void HandleError(Exception ex)
        {
            if (ex.HResult == -2146233088)
            {
                DialogResult res = MessageBox.Show($"{ex.Message}\n\nDo you want to download this dependency form Microsoft website?", Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    Process.Start("https://developer.microsoft.com/en-us/microsoft-edge/webview2/#download-section");
                    MessageBox.Show(@"Choose Evergreen Standalone Installer.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show(@"Ohh, sorry. Something went wrong with WebView2.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Log.SaveErrorLog(ex);
        }
    }
}
