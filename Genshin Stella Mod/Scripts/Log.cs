using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Toolkit.Uwp.Notifications;
using StellaLauncher.Forms.Errors;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts
{
    /// <summary>
    ///     Provides logging functionality for the launcher.
    /// </summary>
    internal static class Log
    {
        public static readonly string Folder = Path.Combine(Program.AppData, "logs");
        private static readonly string OutputFile = Path.Combine(Folder, "launcher.output.log");

        /// <summary>
        ///     Initializes the necessary directories for logging.
        /// </summary>
        private static void InitDirs()
        {
            if (!Directory.Exists(Program.AppData)) Directory.CreateDirectory(Program.AppData);
            if (!Directory.Exists(Folder)) Directory.CreateDirectory(Folder);
        }

        /// <summary>
        ///     Logs the provided information to the output log file asynchronously.
        /// </summary>
        /// <param name="log">The log message to be written to the log file.</param>
        public static async void Output(string log)
        {
            InitDirs();

            try
            {
                using (StreamWriter sw = File.AppendText(OutputFile))
                {
                    await sw.WriteLineAsync($"[{DateTime.Now}]: Output() • {Program.AppName} • v{Program.AppVersion}\n{log}");
                }
            }
            catch
            {
                ShowToastNotification(Resources.Log_SomethingWentWrong, Resources.Log_ForSomeReasonICannotSaveTheActionInfoInTheLogFile);
            }
        }

        /// <summary>
        ///     Logs the provided error information to the output log file asynchronously.
        /// </summary>
        /// <param name="log">The error log message to be written to the log file.</param>
        public static async void SaveError(string log)
        {
            InitDirs();

            try
            {
                using (StreamWriter sw = File.AppendText(OutputFile))
                {
                    await sw.WriteLineAsync($"[{DateTime.Now}]: SaveError() • {Program.AppName} • v{Program.AppVersion}\n{log}\n");
                }
            }
            catch
            {
                ShowToastNotification(Resources.Log_SomethingWentWrong, Resources.Log_ForSomeReasonICantSaveTheErrorInfoInTheLogFile);
            }
        }

        /// <summary>
        ///     Logs the provided exception and shows an error dialog.
        /// </summary>
        /// <param name="ex">The exception to be logged.</param>
        public static void ThrowError(Exception ex)
        {
            SaveError(ex.ToString());

            // Show an error dialog with associated icon
            new ErrorOccurred { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) }.ShowDialog();
        }

        /// <summary>
        ///     Logs the provided exception, sends telemetry data, shows an error dialog, and exits the application.
        /// </summary>
        /// <param name="ex">The exception to be logged and reported.</param>
        public static void ErrorAndExit(Exception ex)
        {
            Telemetry.Error(ex);

            // Log the exception and show the error dialog
            ThrowError(ex);

            // Exit the application with a specific exit code
            Environment.Exit(999991000);
        }

        /// <summary>
        ///     Helper method to display a toast notification.
        /// </summary>
        /// <param name="title">The title of the toast notification.</param>
        /// <param name="message">The message of the toast notification.</param>
        private static void ShowToastNotification(string title, string message)
        {
            try
            {
                new ToastContentBuilder()
                    .AddText(title)
                    .AddText(message)
                    .Show();
            }
            catch (Exception ex)
            {
                ErrorAndExit(ex);
            }
        }
    }
}
