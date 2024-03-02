using Microsoft.Win32;
using StellaModLauncher.Forms.Other;
using StellaModLauncher.Properties;

namespace StellaModLauncher.Scripts.Forms.MainForm;

internal static class LaunchCountHelper
{
	public static async Task<int> CheckLaunchCountAndShowMessages()
	{
		RegistryKey key = Registry.CurrentUser.CreateSubKey(Program.RegistryPath);
		int launchCount = (int)(key?.GetValue("LaunchCount") ?? 0);
		launchCount++;
		key?.SetValue("LaunchCount", launchCount);

		switch (launchCount)
		{
			case 3:
			case 20:
			case 30:
				DialogResult discordResult = MessageBox.Show(Resources.Program_DoYouWantToJoinOurDiscord, Program.AppNameVer, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				Program.Logger.Info($"Question (MessageBox): {Resources.Program_DoYouWantToJoinOurDiscord}");
				if (discordResult == DialogResult.Yes) Utils.OpenUrl(Discord.Invitation);
				Program.Logger.Info($"Selected: {discordResult}");
				break;

			case 4:
			case 17:
				DialogResult feedbackResult = MessageBox.Show(Resources.Program_WouldYouShareOpinionAboutStellaMod, Program.AppNameVer, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				Program.Logger.Info($"Question (MessageBox): {Resources.Program_WouldYouShareOpinionAboutStellaMod}");
				if (feedbackResult == DialogResult.Yes) Utils.OpenUrl("https://www.trustpilot.com/review/genshin.sefinek.net");
				Program.Logger.Info($"Selected: {feedbackResult}");
				break;

			case 2:
			case 8:
			case 6:
			case 10:
			case 15:
			case 25:
			case 35:
			case 45:
			case 60:
				if (!Secret.IsStellaPlusSubscriber) new SupportMe { Icon = Program.Ico }.ShowDialog();
				break;

			// case 15:
			// case 29:
			// case 40:
			// case 80:
			// case 110:
			//     DialogResult logFilesResult = MessageBox.Show(Resources.Program_DoYouWantToSendUsAnonymousLogFiles, Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			//     Program.Logger.Info($"Question (MessageBox): Do you want to send log files? Selected: {logFilesResult}");
			//     if (logFilesResult == DialogResult.Yes)
			//     {
			//         Telemetry.SendLogFiles();
			// 
			//       DialogResult showFilesResult = MessageBox.Show(Resources.Program_IfYouWishToSendLogsToTheDeveloperPleaseSendThemToMeOnDiscord, Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			//       if (showFilesResult == DialogResult.Yes)
			//       {
			//           Process.Start(Log.Folder);
			//           Program.Logger.Info($"Opened: {Log.Folder}");
			//       }
			//    }

			//    break;
		}

		switch (launchCount)
		{
			case 1:
				await ShowMessage(MessageType.MessageBox).ConfigureAwait(false);
				await ShowMessage(MessageType.StatusLabel).ConfigureAwait(false);
				break;

			case 2:
			case 3:
			case 4:
			case 5:
			case 6:
				await ShowMessage(MessageType.StatusLabel).ConfigureAwait(false);
				break;

			case 10:
			case 15:
				await ShowMessage(MessageType.TheSimultaneousOfUse).ConfigureAwait(false);
				break;

			case 20:
			case 30:
			case 40:
			case 50:
				await ShowMessage(MessageType.ThankYouForYourSupport).ConfigureAwait(false);

				break;
		}

		return launchCount;
	}

	private static Task ShowMessage(MessageType type)
	{
		switch (type)
		{
			case MessageType.MessageBox:
			{
				MessageBox.Show(Resources.Default_ItAppersThatIsYourFirstTimeLaunchingTheLauncher, Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Information);
				Program.Logger.Info(Resources.Default_ItAppersThatIsYourFirstTimeLaunchingTheLauncher);
				break;
			}
			case MessageType.StatusLabel:
			{
				if (Secret.IsStellaPlusSubscriber)
				{
					Utils.UpdateStatusLabel(Resources.Default_ClickStartGameButtonToInjectReShadeFPSUnlockAnd3DMigoto, Utils.StatusType.Info);
					Utils.UpdateStatusLabel(Resources.LaunchCount_ABigThankYouTouYouForYourWillingnessToSupport, Utils.StatusType.Info);
				}
				else
				{
					Utils.UpdateStatusLabel(Resources.Default_ClickStartGameButtonToInjectReShadeAndUseFPSUnlock, Utils.StatusType.Info);
					Utils.UpdateStatusLabel(Resources.LaunchCount_TheSimultaneousOfUseRSFU3DM, Utils.StatusType.Info);
				}

				break;
			}
			case MessageType.TheSimultaneousOfUse:
			{
				if (!Secret.IsStellaPlusSubscriber) Utils.UpdateStatusLabel(Resources.LaunchCount_TheSimultaneousOfUseRSFU3DM, Utils.StatusType.Info);

				break;
			}
			case MessageType.ThankYouForYourSupport:
			{
				if (Secret.IsStellaPlusSubscriber) Utils.UpdateStatusLabel(Resources.LaunchCount_ThankYouForYourSupport, Utils.StatusType.Info);
				break;
			}
			default:
				throw new ArgumentException("Invalid message type.");
		}

		return Task.CompletedTask;
	}

	private enum MessageType
	{
		MessageBox,
		StatusLabel,
		TheSimultaneousOfUse,
		ThankYouForYourSupport
	}
}
