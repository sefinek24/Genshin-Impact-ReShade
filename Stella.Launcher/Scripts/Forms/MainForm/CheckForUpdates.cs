using System.Globalization;
using Newtonsoft.Json;
using StellaModLauncher.Forms;
using StellaModLauncher.Models;
using StellaModLauncher.Properties;
using StellaModLauncher.Scripts.Download;
using StellaModLauncher.Scripts.Patrons;
using StellaPLFNet;

namespace StellaModLauncher.Scripts.Forms.MainForm;

internal static class CheckForUpdates
{
	public static async Task<int> Analyze()
	{
		Default._updates_LinkLabel.LinkColor = Color.White;
		Default._updates_LinkLabel.Text = Resources.Default_CheckingForUpdates;

		TaskbarProgress.SetProgressState(TaskbarProgress.Flags.Indeterminate);

		Program.Logger.Info("Checking for new updates...");

		try
		{
			string json = await Program.SefinWebClient.GetStringAsync($"{Program.WebApi}/genshin-stella-mod/versions").ConfigureAwait(true);
			Program.Logger.Info(json);

			StellaApiVersion res = JsonConvert.DeserializeObject<StellaApiVersion>(json);

			string remoteDbLauncherVersion = res.Launcher.Release;
			DateTime remoteLauncherDate = DateTime.Parse(res.Launcher.Date, null, DateTimeStyles.RoundtripKind).ToUniversalTime().ToLocalTime();

			// == Major release ==
			if (Program.AppVersion[0] != remoteDbLauncherVersion[0])
			{
				Default.UpdateIsAvailable = true;

				MajorRelease.Run(remoteDbLauncherVersion, remoteLauncherDate);
				return 1;
			}


			// == Normal release ==
			if (Program.AppVersion != remoteDbLauncherVersion)
			{
				Default.UpdateIsAvailable = true;

				NormalRelease.Run(remoteDbLauncherVersion, remoteLauncherDate, res.Launcher.Beta);
				return 1;
			}


			// == Check new updates of resources ==
			if (!Secret.IsStellaPlusSubscriber)
			{
				string jsonFile = Path.Combine(Default.ResourcesPath, "data.json");
				if (!File.Exists(jsonFile))
				{
					Default._status_Label.Text += $"{string.Format(Resources.Default_File_WasNotFound, jsonFile)}\n";
					Program.Logger.Error($"File {jsonFile} was not found.");

					Utils.HideProgressBar(true);
					return -1;
				}


				string jsonContent = File.ReadAllText(jsonFile);
				Program.Logger.Info($"{jsonFile}: {jsonContent}");
				LocalResources data = JsonConvert.DeserializeObject<LocalResources>(jsonContent);

				string remoteDbResourcesVersion = res.Resources.Release;
				if (data.Version != remoteDbResourcesVersion)
				{
					Default.UpdateIsAvailable = true;

					DateTime remoteResourcesDate = DateTime.Parse(res.Resources.Date, null, DateTimeStyles.RoundtripKind).ToUniversalTime().ToLocalTime();
					DownloadResources.Run(data.Version, remoteDbResourcesVersion, remoteResourcesDate);
					return 1;
				}
			}


			// == Check new updates for ReShade.ini file ==
			// int resultInt = await ReShadeIni.CheckForUpdates();
			//switch (resultInt)
			//{
			//    case -2:
			//   {
			//        DialogResult msgBoxResult = MessageBox.Show(Resources.Default_TheReShadeIniFileCouldNotBeLocatedInYourGameFiles, Program.AppNameVer, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			//        int number = await ReShadeIni.Download(resultInt, Default.ResourcesPath, msgBoxResult);
			//        return number;
			//    }

			//    case 1:
			//    {
			//        DialogResult msgReply = MessageBox.Show(Resources.Default_AreYouSureWantToUpdateReShadeConfiguration, Program.AppNameVer, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

			//        if (msgReply == DialogResult.No || msgReply == DialogResult.Cancel)
			//       {
			//            Program.Logger.Info("The update of ReShade.ini has been cancelled by the user");
			//            MessageBox.Show(Resources.Default_ForSomeReasonYouDidNotGiveConsentForTheAutomaticUpdateOfTheReShadeFile, Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Stop);

			//            Utils.HideProgressBar(true);
			//            return 1;
			//        }

			//        int number = await ReShadeIni.Download(resultInt, Default.ResourcesPath, DialogResult.Yes);
			//        return number;
			//    }
			// }


			// == For Stella Mod Plus subscribers ==
			if (Secret.IsStellaPlusSubscriber)
			{
				int found = await CheckForUpdatesOfBenefits.Analyze().ConfigureAwait(true);
				if (found == 1)
				{
					Labels.HideStartGameBtns();

					Default._updates_LinkLabel.LinkColor = Color.Cyan;
					Default._updates_LinkLabel.Text = Resources.Default_UpdatingBenefits;
					Default._updateIco_PictureBox.Image = Resources.icons8_download_from_the_cloud;
					Utils.RemoveClickEvent(Default._updates_LinkLabel);
					return found;
				}
			}


			// == Not found any new updates ==
			Default._updates_LinkLabel.Text = Resources.Default_CheckForUpdates;
			Default._updateIco_PictureBox.Image = Resources.icons8_available_updates;

			Default._version_LinkLabel.Text = $@"v{(Program.AppVersion == Program.ProductVersion ? Program.AppVersion : $"{Program.ProductVersion}-alpha")}";

			// Utils.RemoveClickEvent(Default._updates_LinkLabel);
			// Default._updates_LinkLabel.Click += CheckUpdates_Click;

			Default.UpdateIsAvailable = false;
			Program.Logger.Info($"Not found any new updates. AppVersion v{Program.AppVersion}; ProductVersion v{Program.ProductVersion};");

			TaskbarProgress.SetProgressState(TaskbarProgress.Flags.NoProgress);

			Utils.ShowStartGameBts();
			return 0;
		}
		catch (Exception e)
		{
			Default.UpdateIsAvailable = false;

			Default._updates_LinkLabel.LinkColor = Color.Red;
			Default._updates_LinkLabel.Text = Resources.Default_OhhSomethingWentWrong;
			Default._status_Label.Text += $"[x] {e.Message}\n";

			Program.Logger.Error(string.Format(Resources.Default_SomethingWentWrongWhileCheckingForNewUpdates, e));
			Utils.HideProgressBar(true);
			return -1;
		}
	}

	public static async void CheckUpdates_Click()
	{
		Music.PlaySound("winxp", "hardware_insert");
		int update = await Analyze().ConfigureAwait(true);

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
