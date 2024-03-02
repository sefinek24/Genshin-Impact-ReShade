using StellaModLauncher.Forms;
using StellaModLauncher.Forms.Errors;
using StellaModLauncher.Properties;
using StellaPLFNet;

namespace StellaModLauncher.Scripts.Remote;

internal static class MajorRelease
{
	public static void Run(string? remoteVersion, DateTime remoteVerDate)
	{
		Default._version_LinkLabel!.Text = $@"v{Program.AppVersion} → v{remoteVersion}";
		Default._checkForUpdates_LinkLabel!.LinkColor = Color.Cyan;
		Default._checkForUpdates_LinkLabel.Text = Resources.MajorRelease_MajorVersionIsAvailable;
		Default._updateIco_PictureBox!.Image = Resources.icons8_download_from_the_cloud;
		Program.Logger.Info($"New major version from {remoteVerDate} is available: v{Program.AppVersion} → v{remoteVersion}");

		TaskbarProgress.SetProgressValue(100);
		TaskbarProgress.SetProgressState(TaskbarProgress.Flags.Paused);
		new NotCompatible { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) }.ShowDialog();

		Application.Exit();
	}
}
