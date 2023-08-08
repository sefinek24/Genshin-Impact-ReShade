using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Taskbar;
using Newtonsoft.Json;
using StellaLauncher.Forms;
using StellaLauncher.Models;
using StellaLauncher.Properties;
using StellaLauncher.Scripts.Download;

namespace StellaLauncher.Scripts.Forms.MainForm
{
    internal static class CheckForUpdatesMain
    {
        public static async Task<int> Analyze()
        {
            Default._updates_LinkLabel.LinkColor = Color.White;
            Default._updates_LinkLabel.Text = Resources.Default_CheckingForUpdates;

            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
            Log.Output(Resources.Default_CheckingForUpdates);

            try
            {
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", Program.UserAgent);
                string json = await client.DownloadStringTaskAsync("https://api.sefinek.net/api/v4/genshin-stella-mod/version/app/launcher");
                StellaApiVersion res = JsonConvert.DeserializeObject<StellaApiVersion>(json);

                string remoteVersion = res.Launcher.Version;
                DateTime remoteVerDate = DateTime.Parse(res.Launcher.ReleaseDate, null, DateTimeStyles.RoundtripKind).ToUniversalTime().ToLocalTime();

                Default._progressBar1.Value = 60;
                // == Major release ==
                if (Program.AppVersion[0] != remoteVersion[0])
                {
                    Default.UpdateIsAvailable = true;

                    MajorRelease.Run(remoteVersion, remoteVerDate);
                    return 1;
                }

                Default._progressBar1.Value = 70;
                // == Normal release ==
                if (Program.AppVersion != remoteVersion)
                {
                    Default.UpdateIsAvailable = true;

                    NormalRelease.Run(remoteVersion, remoteVerDate);
                    return 1;
                }

                Default._progressBar1.Value = 80;
                // == Check new updates of resources ==
                string resourcesPath = null;
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Program.RegistryPath))
                {
                    if (key != null) resourcesPath = (string)key.GetValue("ResourcesPath");
                }

                if (string.IsNullOrEmpty(resourcesPath) || !Directory.Exists(resourcesPath))
                {
                    Default._status_Label.Text += $"{string.Format(Resources.Default_Directory_WasNotFound, resourcesPath)}\n";
                    Log.SaveError(string.Format(Resources.Default_Directory_WasNotFound, resourcesPath));

                    Utils.HideProgressBar(true);
                    return -1;
                }

                Default._resourcesPath = resourcesPath;

                string jsonFile = Path.Combine(resourcesPath, "data.json");
                if (!File.Exists(jsonFile))
                {
                    Default._status_Label.Text += $"{string.Format(Resources.Default_File_WasNotFound, jsonFile)}\n";
                    Log.SaveError(string.Format(Resources.Default_File_WasNotFound, jsonFile));

                    Utils.HideProgressBar(true);
                    return -1;
                }


                string jsonContent = File.ReadAllText(jsonFile);
                LocalResources data = JsonConvert.DeserializeObject<LocalResources>(jsonContent);

                WebClient resClient = new WebClient();
                resClient.Headers.Add("user-agent", Program.UserAgent);
                string resJson = await resClient.DownloadStringTaskAsync("https://api.sefinek.net/api/v4/genshin-stella-mod/version/app/launcher/resources");
                StellaResources resourcesRes = JsonConvert.DeserializeObject<StellaResources>(resJson);

                if (data.Version != resourcesRes.Message)
                {
                    Default.UpdateIsAvailable = true;

                    DownloadResources.Run(resourcesPath, data.Version, resourcesRes.Message, resourcesRes.Date);
                    return 1;
                }


                Default._progressBar1.Value = 90;
                // == Check new updates for ReShade.ini file ==
                int resultInt = await ReShadeIni.CheckForUpdates();
                switch (resultInt)
                {
                    case -2:
                    {
                        DialogResult msgBoxResult = MessageBox.Show(Resources.Default_TheReShadeIniFileCouldNotBeLocatedInYourGameFiles, Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        int number = await ReShadeIni.Download(resultInt, resourcesPath, msgBoxResult);
                        return number;
                    }

                    case 1:
                    {
                        DialogResult msgReply = MessageBox.Show(Resources.Default_AreYouSureWantToUpdateReShadeConfiguration, Program.AppName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                        if (msgReply == DialogResult.No || msgReply == DialogResult.Cancel)
                        {
                            Log.Output(Resources.Default_TheUpdateOfReShadIniHasBeenCanceledByTheUser);
                            MessageBox.Show(Resources.Default_ForSomeReasonYouDidNotGiveConsentForTheAutomaticUpdateOfTheReShadeFile, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                            Utils.HideProgressBar(true);
                            return 1;
                        }

                        int number = await ReShadeIni.Download(resultInt, resourcesPath, DialogResult.Yes);
                        return number;
                    }
                }


                // == Not found any new updates ==
                Default._updates_LinkLabel.Text = Resources.Default_CheckForUpdates;
                Default._updateIco_PictureBox.Image = Resources.icons8_available_updates;

                Utils.RemoveClickEvent(Default._updates_LinkLabel);
                Default._updates_LinkLabel.Click += CheckUpdates_Click;

                Default.UpdateIsAvailable = false;
                Log.Output(string.Format(Resources.Default_NotFoundAnyNewUpdates_YourInstalledVersion_, Program.AppVersion));

                Default._startGame_LinkLabel.Visible = true;
                Default._injectReShade_LinkLabel.Visible = true;
                Default._runFpsUnlocker_LinkLabel.Visible = true;
                Default._only3DMigoto_LinkLabel.Visible = true;
                Default._runGiLauncher_LinkLabel.Visible = true;
                if (!Secret.IsMyPatron) Default._becomeMyPatron_LinkLabel.Visible = true;

                Default._progressBar1.Value = 100;
                Utils.HideProgressBar(false);
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
                return 0;
            }
            catch (Exception e)
            {
                Default.UpdateIsAvailable = false;

                Default._updates_LinkLabel.LinkColor = Color.Red;
                Default._updates_LinkLabel.Text = Resources.Default_OhhSomethingWentWrong;
                Default._status_Label.Text += $"[x] {e.Message}\n";

                Log.SaveError(string.Format(Resources.Default_SomethingWentWrongWhileCheckingForNewUpdates, e));
                Utils.HideProgressBar(true);
                return -1;
            }
        }

        public static async void CheckUpdates_Click(object sender, EventArgs e)
        {
            Music.PlaySound("winxp", "hardware_insert");
            int update = await Analyze();

            if (update == -1)
            {
                Music.PlaySound("winxp", "hardware_fail");
                return;
            }

            if (update != 0) return;

            Music.PlaySound("winxp", "hardware_remove");

            Default._updates_LinkLabel.LinkColor = Color.LawnGreen;
            Default._updates_LinkLabel.Text = Resources.Default_YouHaveTheLatestVersion;
            Default._updateIco_PictureBox.Image = Resources.icons8_available_updates;
        }
    }
}
