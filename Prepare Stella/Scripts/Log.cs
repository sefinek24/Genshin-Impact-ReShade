using System;
using System.IO;
using System.Threading;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace PrepareStella.Scripts
{
    internal abstract class Log
    {
        private static readonly string Folder = Path.Combine(Program.AppData, "logs");
        private static readonly string OutputFile = Path.Combine(Folder, "prepare.output.log");

        private static void TryAgain(bool tryAgain)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            const string prompt = "\n» Something went wrong. Press ENTER to";
            Console.WriteLine(tryAgain ? $"{prompt} try again..." : $"{prompt} continue...");
            Program.Logger.Warn(prompt);

            Console.ReadLine();
            Console.WriteLine(@">> Waiting 5 seconds. Please wait... <<");
            Thread.Sleep(5000);

            Console.ResetColor();
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
        }

        public static void ThrowError(Exception msg, bool tryAgain)
        {
            Program.Logger.Error(msg);

            try
            {
                new ToastContentBuilder()
                    .AddText("Oh no! Error occurred 😿")
                    .AddText("Go back to the configuration window.")
                    .Show();

                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex);
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(string.IsNullOrEmpty(msg.InnerException?.ToString()) ? msg : msg.InnerException);

            TryAgain(tryAgain);
        }

        public static void ErrorAndExit(Exception log, bool hideError, bool reportIssue)
        {
            Program.Logger.Error(log);

            if (!hideError)
            {
                try
                {
                    new ToastContentBuilder()
                        .AddText("Failed 😿")
                        .AddText("🎶 Sad song... Could you please try again?")
                        .Show();

                    TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
                }
                catch (Exception ex)
                {
                    Program.Logger.Error(ex);
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{log.Message}\n");

                if (reportIssue)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(
                        $"Oh nooo!! I'm sorry, but something went wrong. If you need help, please do one of the following:\n• Join my Discord server: {Program.DiscordUrl} [My username: sefinek]\n• Send an email: contact@sefinek.net\n• Use the chat available on my website.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(
                        $"Visit our Discord server for help or try again. Good luck!\n• Discord: {Program.DiscordUrl} [My username: sefinek]\n• E-mail: contact@sefinek.net\n• Use the chat available on my website.");

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write("\n» Would you like to join our Discord server? [Yes/no]: ");
                    Console.ResetColor();

                    string joinDiscord = Console.ReadLine()?.ToLower();
                    switch (joinDiscord)
                    {
                        case "y":
                        case "yes":
                            Utils.OpenUrl(Program.DiscordUrl);

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(@"An invitation to the server has been opened in your default web browser.");
                            Console.WriteLine();
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine(@"You can close the setup window.");
                            break;

                        case "n":
                        case "no":
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine(@"Okay... You can close this window.");
                            break;

                        default:
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine(@"Wrong answer. Close this window.");
                            break;
                    }

                    while (true) Console.ReadLine();
                }
            }

            while (true) Console.ReadLine();
        }
    }
}
