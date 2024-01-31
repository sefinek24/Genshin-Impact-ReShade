using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Taskbar;
using StellaLauncher.Forms;
using StellaLauncher.Forms.Errors;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts.Download
{
	internal static class MajorRelease
	{
		public static void Run(string remoteVersion, DateTime remoteVerDate)
		{
			Default._version_LinkLabel.Text = $@"v{Program.AppVersion} → v{remoteVersion}";
			Default._updates_LinkLabel.LinkColor = Color.Cyan;
			Default._updates_LinkLabel.Text = Resources.MajorRelease_MajorVersionIsAvailable;
			Default._updateIco_PictureBox.Image = Resources.icons8_download_from_the_cloud;
			Program.Logger.Info($"New major version from {remoteVerDate} is available: v{Program.AppVersion} → v{remoteVersion}");

			TaskbarManager.Instance.SetProgressValue(100, 100);
			TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Paused);
			new NotCompatible { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) }.ShowDialog();

			Environment.Exit(0);
		}
	}
}
