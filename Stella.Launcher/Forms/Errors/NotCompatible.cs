using System.Media;
using Microsoft.Win32;
using StellaModLauncher.Properties;
using StellaModLauncher.Scripts;

namespace StellaModLauncher.Forms.Errors;

public sealed partial class NotCompatible : Form
{
	public NotCompatible()
	{
		InitializeComponent();

		DoubleBuffered = true;
	}

	private void NotCompatible_Shown(object sender, EventArgs e)
	{
		string? resourcesPath = null;
		using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(Program.RegistryPath, true))
		{
			if (key != null)
			{
				resourcesPath = key.GetValue("ResourcesPath") as string;
				if (!string.IsNullOrEmpty(resourcesPath))
				{
					key.DeleteValue("ResourcesPath", false);
					Program.Logger.Info($"Deleted 'ResourcesPath' from {Program.RegistryPath}");
				}
			}
		}

		if (!string.IsNullOrEmpty(resourcesPath))
		{
			string bkpSuffix = $"_bkp-{DateTime.Now:yyyyMMddHHmmss}-{Program.AppFileVersion}";
			Program.Logger.Info($"Suffix: {bkpSuffix}");

			string newFolderPath = resourcesPath + bkpSuffix;
			if (Directory.Exists(resourcesPath))
			{
				Directory.Move(resourcesPath, newFolderPath);
				label1.Text += string.Format($@"{Environment.NewLine}{Resources.NotCompatible_WeveRenamedYourResourceFolderByAdding__ToTheEnd}", bkpSuffix);
				Program.Logger.Info($"Renamed {resourcesPath} to {newFolderPath}");
			}
			else
			{
				Program.Logger.Info($"Resource path {resourcesPath} not found. Unable to rename folder to {newFolderPath}");
			}
		}

		using (RegistryKey key = Registry.CurrentUser.CreateSubKey(Program.RegistryPath))
		{
			key.SetValue("AppIsConfigured", 0);
			Program.Logger.Info("Set 'AppIsConfigured' to 0");
		}

		SystemSounds.Beep.Play();
		Program.Logger.Info(string.Format(Resources.Main_LoadedForm_, Text));
	}

	private void NotCompatible_Closed(object sender, FormClosedEventArgs e)
	{
		Program.Logger.Info(string.Format(Resources.Main_ClosedForm_, Text));
	}

	private void DownloadInstaller_Click(object sender, EventArgs e)
	{
		Utils.OpenUrl(Program.AppWebsiteFull);
	}
}
