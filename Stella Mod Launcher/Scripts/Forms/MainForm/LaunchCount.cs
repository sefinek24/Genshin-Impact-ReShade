using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using StellaLauncher.Forms;
using StellaLauncher.Forms.Other;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts.Forms.MainForm
{
    internal static class LaunchCountHelper
    {
        public static async Task CheckLaunchCountAndShowMessages()
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
                    DialogResult discordResult = MessageBox.Show(Resources.Program_DoYouWantToJoinOurDiscord, Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    Log.Output($"Question (MessageBox): {Resources.Program_DoYouWantToJoinOurDiscord}");
                    if (discordResult == DialogResult.Yes) Utils.OpenUrl(Discord.Invitation);
                    Log.Output($"Selected: {discordResult}");
                    break;

                case 4:
                case 17:
                    DialogResult feedbackResult = MessageBox.Show(Resources.Program_WouldYouShareOpinionAboutStellaMod, Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    Log.Output($"Question (MessageBox): {Resources.Program_WouldYouShareOpinionAboutStellaMod}");
                    if (feedbackResult == DialogResult.Yes) Utils.OpenUrl("https://www.trustpilot.com/review/genshin.sefinek.net");
                    Log.Output($"Selected: {feedbackResult}");
                    break;

                case 2:
                case 10:
                case 25:
                case 35:
                case 45:
                case 55:
                case 60:
                    if (!Secret.IsMyPatron) new SupportMe { Icon = Program.Ico }.ShowDialog();
                    break;

                // case 15:
                // case 29:
                // case 40:
                // case 80:
                // case 110:
                //     DialogResult logFilesResult = MessageBox.Show(Resources.Program_DoYouWantToSendUsanonymousLogFiles, Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                //     Log.Output($"Question (MessageBox): Do you want to send log files? Selected: {logFilesResult}");
                //     if (logFilesResult == DialogResult.Yes)
                //     {
                //         Telemetry.SendLogFiles();
                // 
                //       DialogResult showFilesResult = MessageBox.Show(Resources.Program_IfYouWishToSendLogsToTheDeveloperPleaseSendThemToMeOnDiscord, Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                //       if (showFilesResult == DialogResult.Yes)
                //       {
                //           Process.Start(Log.Folder);
                //           Log.Output($"Opened: {Log.Folder}");
                //       }
                //    }

                //    break;
            }

            switch (launchCount)
            {
                case 1:
                    await ShowMessage(MessageType.MessageBox);
                    await ShowMessage(MessageType.StatusLabel);
                    break;
                case 2:
                case 3:
                    await ShowMessage(MessageType.StatusLabel);
                    break;
            }
        }

        private static Task ShowMessage(MessageType type)
        {
            switch (type)
            {
                case MessageType.MessageBox:
                    MessageBox.Show(Resources.Default_ItAppersThatIsYourFirstTimeLaunchingTheLauncher, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Log.Output(Resources.Default_ItAppersThatIsYourFirstTimeLaunchingTheLauncher);
                    break;
                case MessageType.StatusLabel:
                {
                    if (Secret.IsMyPatron)
                        Default._status_Label.Text += $"[i] {Resources.Default_ClickStartGameButtonToInjectReShadeFPSUnlockAnd3DMigoto}\n[i] {Resources.LaunchCount_ABigThankYouTouYouForYourWillingnessToSupport}\n";
                    else
                        Default._status_Label.Text += $"[i] {Resources.Default_ClickStartGameButtonToInjectReShadeAndUseFPSUnlock}\n[i] {Resources.LaunchCount_TheSimultaneousOfUseRSFU3DM}\n";

                    Log.Output(Default._status_Label.Text);
                    break;
                }
                default:
                    throw new ArgumentException("Invalid message type. Supported types are 'MessageBox' and 'StatusLabel'.");
            }

            return Task.CompletedTask;
        }

        private enum MessageType
        {
            MessageBox,
            StatusLabel
        }
    }
}
