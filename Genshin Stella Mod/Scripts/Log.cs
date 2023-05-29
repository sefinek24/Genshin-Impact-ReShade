using System;
using System.IO;
using StellaLauncher;
using StellaLauncher.Forms.Errors;
using StellaLauncher.Properties;
using StellaLauncher.Scripts;

namespace Genshin_Stella_Mod.Scripts
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

            using (StreamWriter sw = File.AppendText(OutputFile))
            {
                await sw.WriteLineAsync($"[{DateTime.Now}]: {log}");
            }
        }

        public static async void SaveErrorLog(Exception log)
        {
            InitDirs();

            using (StreamWriter sw = File.AppendText(OutputFile))
            {
                await sw.WriteLineAsync($"[{DateTime.Now}]: SaveErrorLog() • {Program.AppName} • v{Program.AppVersion}\n{log}\n");
            }
        }

        public static void ThrowError(Exception log)
        {
            new ErrorOccurred { Icon = Resources.icon_52x52 }.ShowDialog();

            SaveErrorLog(log);
        }

        public static void ErrorAndExit(Exception ex)
        {
            ThrowError(ex);

            Telemetry.Error(ex);
            Environment.Exit(999991000);
        }
    }
}
