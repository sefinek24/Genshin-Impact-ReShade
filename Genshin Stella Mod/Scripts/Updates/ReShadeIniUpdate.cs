using System;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using ByteSizeLib;
using Genshin_Stella_Mod.Forms;
using Genshin_Stella_Mod.Properties;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace Genshin_Stella_Mod.Scripts.Updates
{
    internal class ReShadeIniUpdate
    {
        // LinkLabel, Label, ProgressBar etc.
        private static LinkLabel _updatesLabel;
        private static Label _statusLabel;
        private static PictureBox _updateIcon;
        private static LinkLabel _versionLabel;

        public static async Task<int> Run(LinkLabel updatesLabel, Label statusLabel, PictureBox updateIcon, LinkLabel versionLabel)
        {
            _updatesLabel = updatesLabel;
            _statusLabel = statusLabel;
            _updateIcon = updateIcon;
            _versionLabel = versionLabel;


            string gamePath = await Utils.GetGame("giGameDir");
            if (!Directory.Exists(gamePath))
            {
                Default.UpdateIsAvailable = false;

                _updatesLabel.LinkColor = Color.Red;
                _updatesLabel.Text = @"Oh no~~! Failed.";
                _statusLabel.Text += "[x] Game path was not found on your PC.\n";

                Log.SaveErrorLog(new Exception("Game path was not found on your PC."));
                TaskbarManager.Instance.SetProgressValue(100, 100);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                return -1;
            }

            string reShadePath = Path.Combine(gamePath, "ReShade.ini");
            if (!File.Exists(reShadePath))
            {
                Default.UpdateIsAvailable = false;

                _updatesLabel.LinkColor = Color.OrangeRed;
                _updatesLabel.Text = @"Download the required file";
                _statusLabel.Text += "[x] File ReShade.ini was not found in your game directory.\n";
                _updateIcon.Image = Resources.icons8_download_from_the_cloud;

                Log.Output($"ReShade.ini was not found in: {reShadePath}");
                return -2;
            }

            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", Program.UserAgent);
            string content = await webClient.DownloadStringTaskAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/reshade/ReShade.ini");
            NameValueCollection iniData = new NameValueCollection();
            using (StringReader reader = new StringReader(content))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                    if (line.Contains("="))
                    {
                        int separatorIndex = line.IndexOf("=", StringComparison.Ordinal);
                        string key = line.Substring(0, separatorIndex).Trim();
                        string value = line.Substring(separatorIndex + 1).Trim();
                        iniData.Add(key, value);
                    }
            }

            string localIniVersion = Default.ReShadeIni.ReadString("STELLA", "ConfigVersion", null);
            if (string.IsNullOrEmpty(localIniVersion))
            {
                Default.UpdateIsAvailable = false;

                _updatesLabel.LinkColor = Color.Cyan;
                _updatesLabel.Text = @"Download the required file";
                _statusLabel.Text += "[x] The version of ReShade config was not found.\n";
                _updateIcon.Image = Resources.icons8_download_from_the_cloud;

                Log.Output("STELLA.ConfigVersion is null in ReShade.ini.");
                TaskbarManager.Instance.SetProgressValue(100, 100);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                return -2;
            }

            string remoteIniVersion = iniData["ConfigVersion"];
            if (localIniVersion == remoteIniVersion) return 0;

            Default.UpdateIsAvailable = true;

            _updatesLabel.LinkColor = Color.DodgerBlue;
            _updatesLabel.Text = @"Update ReShade config";
            _updateIcon.Image = Resources.icons8_download_from_the_cloud;
            _versionLabel.Text = $@"v{localIniVersion} → v{remoteIniVersion}";
            _statusLabel.Text += "[i] New ReShade config version is available. Update is required.\n";
            Log.Output($"New ReShade config version is available: v{localIniVersion} → v{remoteIniVersion}");

            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("user-agent", Program.UserAgent);
                await wc.OpenReadTaskAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/reshade/ReShade.ini");
                string updateSize = ByteSize.FromBytes(Convert.ToInt64(wc.ResponseHeaders["Content-Length"])).KiloBytes.ToString("0.00");
                _statusLabel.Text += $"[i] Update size: {updateSize} KB\n";

                Log.Output($"File size: {updateSize} KB");
                TaskbarManager.Instance.SetProgressValue(100, 100);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
                return 1;
            }
        }
    }
}
