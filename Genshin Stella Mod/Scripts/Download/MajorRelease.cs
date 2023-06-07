using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Taskbar;
using StellaLauncher.Forms.Errors;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts.Download
{
    internal class MajorRelease
    {
        public static void Run(string remoteVersion, DateTime remoteVerDate, LinkLabel versionLabel, LinkLabel updatesLabel, PictureBox updateIcon)
        {
            versionLabel.Text = $@"v{Program.AppVersion} → v{remoteVersion}";
            updatesLabel.LinkColor = Color.Cyan;
            updatesLabel.Text = Resources.MajorRelease_MajorVersionIsAvailable;
            updateIcon.Image = Resources.icons8_download_from_the_cloud;
            Log.Output(string.Format(Resources.MajorRelease_NewMajorVersionFrom_IsAvailable_v_, remoteVerDate, Program.AppVersion, remoteVersion));

            TaskbarManager.Instance.SetProgressValue(100, 100);
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
            new NotCompatible { Icon = Resources.icon_52x52 }.ShowDialog();

            Environment.Exit(0);
        }
    }
}
