using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Taskbar;
using StellaLauncher.Forms;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts.Download
{
    internal class ReShadeCfg
    {
        public static async Task<int> Download(int resultInt, string resourcesPath)
        {
            DialogResult msgBoxResult = MessageBox.Show(Resources.Default_TheReShadeIniFileCouldNotBeLocatedInYourGameFiles, Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            string gameDir = await Utils.GetGame("giGameDir");
            string reShadePath = Path.Combine(gameDir, "ReShade.ini");

            switch (msgBoxResult)
            {
                case DialogResult.Yes:
                    try
                    {
                        Default._updates_LinkLabel.LinkColor = Color.DodgerBlue;
                        Default._updates_LinkLabel.Text = Resources.Default_Downloading;
                        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);

                        WebClient client2 = new WebClient();
                        client2.Headers.Add("user-agent", Program.UserAgent);
                        await client2.DownloadFileTaskAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/reshade/ReShade.ini", reShadePath);

                        if (File.Exists(reShadePath))
                        {
                            string reShadeIniContent = File.ReadAllText(reShadePath);
                            string newData = reShadeIniContent?
                                .Replace("{addon.path}", Path.Combine(resourcesPath, "ReShade", "Addons"))
                                .Replace("{general.effects}", Path.Combine(resourcesPath, "ReShade", "Shaders", "Effects"))
                                .Replace("{general.cache}", Path.Combine(resourcesPath, "ReShade", "Cache"))
                                .Replace("{general.preset}", Path.Combine(resourcesPath, "ReShade", "Presets", "3. Preset by Sefinek - Medium settings [Default].ini"))
                                .Replace("{general.textures}", Path.Combine(resourcesPath, "ReShade", "Shaders", "Textures"))
                                .Replace("{screenshot.path}", Path.Combine(resourcesPath, "ReShade", "Screenshots"))
                                .Replace("{screenshot.sound}", Path.Combine(Program.AppPath, "data", "sounds", "screenshot.wav"));

                            File.WriteAllText(reShadePath, newData);

                            Default._status_Label.Text += $"[✓] {Resources.Default_SuccessfullyDownloadedReShadeIni}\n";
                            Log.Output(string.Format(Resources.Default_SuccessfullyDownloadedReShadeIniAndSavedIn, reShadePath));

                            await Default.CheckForUpdates();
                            return 0;
                        }

                        Default._status_Label.Text += $"[x] {Resources.Default_FileWasNotFound}\n";
                        Log.SaveErrorLog(new Exception(string.Format(Resources.Default_DownloadedReShadeIniWasNotFoundIn_, reShadePath)));

                        TaskbarManager.Instance.SetProgressValue(100, 100);
                        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                    }
                    catch (Exception ex)
                    {
                        Default._status_Label.Text += $"[x] {Resources.Default_Meeow_FailedToDownloadReShadeIni_TryAgain}\n";
                        Default._updates_LinkLabel.LinkColor = Color.Red;
                        Default._updates_LinkLabel.Text = Resources.Default_FailedToDownload;

                        Log.SaveErrorLog(ex);
                        if (!File.Exists(reShadePath)) Log.Output(Resources.Default_TheReShadeIniFileStillDoesNotExist);

                        TaskbarManager.Instance.SetProgressValue(100, 100);
                        TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                    }

                    break;
                case DialogResult.No:
                {
                    Default._status_Label.Text += $"[i] {Resources.Default_CanceledByTheUser_AreYouSureOfWhatYoureDoing}\n";
                    Log.Output(Resources.Default_FileDownloadHasBeenCanceledByTheUser);

                    if (!File.Exists(reShadePath)) Log.Output(Resources.Default_TheReShadeIniFileStillDoesNotExist);

                    TaskbarManager.Instance.SetProgressValue(100, 100);
                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
                    break;
                }
            }

            return resultInt;
        }
    }
}
