using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using StellaLauncher.Forms;
using StellaLauncher.Forms.Other;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts.Forms.MainForm
{
    internal static class LaunchCountHelper
    {
        public static void CheckLaunchCountAndShowMessages()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(Program.RegistryPath);
            int launchCount = (int)(key?.GetValue("LaunchCount") ?? 0);
            launchCount++;
            key?.SetValue("LaunchCount", launchCount);

            switch (launchCount)
            {
                case 5:
                case 20:
                case 30:
                    DialogResult discordResult = MessageBox.Show(Resources.Program_DoYouWantToJoinOurDiscord, Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    Log.Output(string.Format(Resources.Program_QuestionMessageBox_DoYouWantToJoinOurDiscord_, discordResult));
                    if (discordResult == DialogResult.Yes) Utils.OpenUrl(Discord.Invitation);
                    break;

                case 2:
                case 12:
                case 40:
                    DialogResult feedbackResult = MessageBox.Show(Resources.Program_WouldYouShareOpinionAboutStellaMod, Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    Log.Output(string.Format(Resources.Program_QuestionMessageBox_WouldYouShareOpinionAboutStellaMod, feedbackResult));
                    if (feedbackResult == DialogResult.Yes) Utils.OpenUrl("https://www.trustpilot.com/review/genshin.sefinek.net");
                    break;

                case 3:
                case 10:
                case 25:
                case 35:
                case 45:
                    if (!Secret.IsMyPatron) Application.Run(new SupportMe { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) });
                    return;

                case 28:
                case 70:
                case 100:
                case 200:
                case 300:
                    DialogResult logFilesResult = MessageBox.Show(Resources.Program_DoYouWantToSendUsanonymousLogFiles, Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    Log.Output(string.Format(Resources.Program_QuestionMessageBox_DoYouWantToSendUsanonymousLogFiles_, logFilesResult));
                    if (logFilesResult == DialogResult.Yes)
                    {
                        Telemetry.SendLogFiles();

                        DialogResult showFilesResult = MessageBox.Show(Resources.Program_IfYouWishToSendLogsToTheDeveloperPleaseSendThemToMeOnDiscord, Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (showFilesResult == DialogResult.Yes)
                        {
                            Process.Start(Log.Folder);
                            Log.Output($"Opened: {Log.Folder}");
                        }
                    }

                    break;
            }


            switch (launchCount)
            {
                case 1:
                    ShowMessage(MessageType.MessageBox);
                    ShowMessage(MessageType.StatusLabel);
                    break;
                case 2:
                case 3:
                    ShowMessage(MessageType.StatusLabel);
                    break;
            }
        }

        private static void ShowMessage(MessageType type)
        {
            switch (type)
            {
                case MessageType.MessageBox:
                    MessageBox.Show(Resources.Default_ItAppersThatIsYourFirstTimeLaunchingTheLauncher, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case MessageType.StatusLabel:
                    Default._status_Label.Text += $"[i] {(Secret.IsMyPatron ? Resources.Default_ClickStartGameButtonToInjectReShadeFPSUnlockAnd3DMigoto : Resources.Default_ClickStartGameButtonToInjectReShadeAndUseFPSUnlock)}\n";
                    break;
                default:
                    throw new ArgumentException("Invalid message type. Supported types are 'MessageBox' and 'StatusLabel'.");
            }
        }

        private enum MessageType
        {
            MessageBox,
            StatusLabel
        }
    }
}
