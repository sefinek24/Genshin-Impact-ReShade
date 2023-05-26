using System;
using System.Drawing;
using System.Windows.Forms;
using Genshin_Stella_Mod.Forms.Errors;
using Genshin_Stella_Mod.Properties;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace Genshin_Stella_Mod.Scripts.Updates
{
    internal class MajorRelease
    {
        public static void Run(string remoteVersion, DateTime remoteVerDate, LinkLabel version_Label, LinkLabel updates_Label, PictureBox update_Icon)
        {
            version_Label.Text = $@"v{Program.AppVersion} → v{remoteVersion}";
            updates_Label.LinkColor = Color.Cyan;
            updates_Label.Text = @"Major version is available";
            update_Icon.Image = Resources.icons8_download_from_the_cloud;
            Log.Output($"New major version from {remoteVerDate} is available: v{Program.AppVersion} → v{remoteVersion}");

            TaskbarManager.Instance.SetProgressValue(100, 100);
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
            new NotCompatible { Icon = Resources.icon_52x52 }.ShowDialog();

            Environment.Exit(0);
        }
    }
}
