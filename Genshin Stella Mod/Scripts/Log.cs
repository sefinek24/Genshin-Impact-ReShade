using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Toolkit.Uwp.Notifications;
using StellaLauncher.Forms.Errors;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts
{
    internal static class Log
    {
        public static readonly string Folder = Path.Combine(Program.AppData, "logs");
        private static readonly string OutputFile = Path.Combine(Folder, "launcher.output.log");

        private static void InitDirs()
        {
            if (!Directory.Exists(Program.AppData)) Directory.CreateDirectory(Program.AppData);
            if (!Directory.Exists(Folder)) Directory.CreateDirectory(Folder);
        }

        public static async void Output(string log)
        {
            InitDirs();

            try
            {
                using (StreamWriter sw = File.AppendText(OutputFile))
                {
                    await sw.WriteLineAsync($"[{DateTime.Now}]: {log}");
                }
            }
            catch
            {
                new ToastContentBuilder()
                    .AddText(Resources.Log_SomethingWentWrong)
                    .AddText(Resources.Log_ForSomeReasonICannotSaveTheActionInfoInTheLogFile)
                    .Show();
            }
        }

        public static async void SaveErrorLog(Exception log)
        {
            InitDirs();

            try
            {
                using (StreamWriter sw = File.AppendText(OutputFile))
                {
                    await sw.WriteLineAsync($"[{DateTime.Now}]: SaveErrorLog() • {Program.AppName} • v{Program.AppVersion}\n{log}\n");
                }
            }
            catch
            {
                new ToastContentBuilder()
                    .AddText(Resources.Log_SomethingWentWrong)
                    .AddText(Resources.Log_ForSomeReasonICantSaveTheErrorInfoInTheLogFile)
                    .Show();
            }
        }

        public static void ThrowError(Exception log)
        {
            SaveErrorLog(log);

            new ErrorOccurred { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) }.ShowDialog();
        }

        public static void ErrorAndExit(Exception ex)
        {
            Telemetry.Error(ex);

            ThrowError(ex);
            Environment.Exit(999991000);
        }
    }
}
