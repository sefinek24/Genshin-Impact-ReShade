using System.Globalization;
using Newtonsoft.Json;
using StellaModLauncher.Forms;
using StellaModLauncher.Forms.Errors;
using StellaModLauncher.Models;
using StellaModLauncher.Properties;
using StellaModLauncher.Scripts.Forms;
using StellaModLauncher.Scripts.Forms.MainForm;
using StellaModLauncher.Scripts.StellaPlus;
using StellaPLFNet;

namespace StellaModLauncher.Scripts.Remote;

internal static class CheckForUpdates
{
	public static async Task<int> Analyze()
	{
		Default._checkForUpdates_LinkLabel!.LinkColor = Color.White;
		Default._checkForUpdates_LinkLabel.Text = Resources.Default_CheckingForUpdates;

		if (Stages._currentStage == Stages.AllStages) TaskbarProgress.SetProgressState(TaskbarProgress.Flags.Indeterminate);

		Program.Logger.Info("Checking for new updates...");

		try
		{
			string json = await Program.SefinWebClient.GetStringAsync($"{Program.WebApi}/genshin-stella-mod/version").ConfigureAwait(true);
			Program.Logger.Info(json);

			StellaApiVersion? res = JsonConvert.DeserializeObject<StellaApiVersion>(json);

			string? remoteDbLauncherVersion = res!.Launcher?.Release;
			DateTime remoteLauncherDate = DateTime.Parse(res.Launcher?.Date!, null, DateTimeStyles.RoundtripKind).ToUniversalTime().ToLocalTime();

			// == Major release ==
			if (Program.AppVersion![0] != remoteDbLauncherVersion![0])
			{
				Default.UpdateIsAvailable = true;

				MajorRelease.Run(remoteDbLauncherVersion, remoteLauncherDate);
				return 2;
			}


			// == Normal release ==
			if (Program.AppVersion != remoteDbLauncherVersion)
			{
				Default.UpdateIsAvailable = true;

				await NormalRelease.Run(remoteDbLauncherVersion, remoteLauncherDate, res.Launcher!.Beta, res.UpdateUrl).ConfigureAwait(true);
				return 2;
			}


			// == Check new updates of resources ==
			if (!Secret.IsStellaPlusSubscriber)
			{
				string jsonFile = Path.Combine(Default.ResourcesPath!, "data.json");
				if (!File.Exists(jsonFile))
				{
					Utils.UpdateStatusLabel(string.Format(Resources.Default_File_WasNotFound, jsonFile), Utils.StatusType.Error);
					Program.Logger.Error($"File {jsonFile} was not found.");

					Labels.HideProgressbar(null, true);
					return -1;
				}


				string jsonContent = await File.ReadAllTextAsync(jsonFile).ConfigureAwait(true);
				Program.Logger.Info($"{jsonFile}: {jsonContent}");
				LocalResources? data = JsonConvert.DeserializeObject<LocalResources>(jsonContent);

				string? remoteDbResourcesVersion = res.Resources!.Release;
				if (data?.Version != remoteDbResourcesVersion)
				{
					Default.UpdateIsAvailable = true;

					DateTime remoteResourcesDate = DateTime.Parse(res.Resources.Date!, null, DateTimeStyles.RoundtripKind).ToUniversalTime().ToLocalTime();
					await DownloadResources.Run(data?.Version!, remoteDbResourcesVersion, remoteResourcesDate).ConfigureAwait(true);
					return 1;
				}

				Default.UpdateIsAvailable = false;
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

			//            Labels.HideProgressbar(true);
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
				switch (found)
				{
					case 0:
						Default.UpdateIsAvailable = false;
						break;
					case 1:
						Default.UpdateIsAvailable = true;

						Default._checkForUpdates_LinkLabel.LinkColor = Color.Cyan;
						Default._checkForUpdates_LinkLabel.Text = Resources.Default_UpdatingBenefits;
						Default._updateIco_PictureBox!.Image = Resources.icons8_download_from_the_cloud;
						return found;
					case 666:
						Labels.FailedToLoad();
						return 666;
				}
			}


			// == Banned? ==
			if (res!.IsBanned)
			{
				Labels.FailedToLoad();

				new Banned().Show();

				return 666;
			}


			// == Not found any new updates ==
			Default._checkForUpdates_LinkLabel.Text = Resources.Default_CheckForUpdates;
			Default._updateIco_PictureBox!.Image = Resources.icons8_available_updates;

			Default._version_LinkLabel!.Text = $@"v{(Program.AppVersion == Program.AppFileVersion ? Program.AppVersion : $"{Program.AppFileVersion}-alpha")}";
			// if (actionOnClick) Default._webView21!.Visible = true;
			Default.UpdateIsAvailable = false;
			Program.Logger.Info($"Not found any new updates. AppVersion v{Program.AppVersion}; ProductVersion v{Program.AppFileVersion};");

			if (Stages._currentStage == Stages.AllStages) TaskbarProgress.SetProgressState(TaskbarProgress.Flags.NoProgress);

			Labels.ShowStartGameBtns();
			return 0;
		}
		catch (Exception ex)
		{
			Default.UpdateIsAvailable = false;

			Default._checkForUpdates_LinkLabel.LinkColor = Color.Red;
			Default._checkForUpdates_LinkLabel.Text = Resources.Default_OhhSomethingWentWrong;
			Utils.UpdateStatusLabel(ex.Message, Utils.StatusType.Error);

			Program.Logger.Error(string.Format(Resources.Default_SomethingWentWrongWhileCheckingForNewUpdates, ex));
			Labels.HideProgressbar(null, true);
			return -1;
		}
	}

	public static async void CheckUpdates_Click(object? sender, LinkLabelLinkClickedEventArgs e)
	{
		if (Secret.IsStellaPlusSubscriber) Music.PlaySound("winxp", "hardware_insert");
		Default._webView21!.Visible = false;

		int update = await Analyze().ConfigureAwait(true);

		switch (update)
		{
			case -1:
				Default._webView21!.Visible = false;
				Music.PlaySound("winxp", "hardware_fail");
				return;
			case 0:
				Default._webView21!.Visible = true;
				break;
		}

		if (update != 0) return;
		if (Secret.IsStellaPlusSubscriber) Music.PlaySound("winxp", "hardware_remove");

		Default._checkForUpdates_LinkLabel!.LinkColor = Color.LawnGreen;
		Default._checkForUpdates_LinkLabel.Text = Resources.Default_YouHaveTheLatestVersion;
		Default._updateIco_PictureBox!.Image = Resources.icons8_available_updates;
	}
}
