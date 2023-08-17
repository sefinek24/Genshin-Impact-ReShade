using System;
using System.IO;
using System.Reflection;

namespace GenshinStellaMod.Scripts
{
    internal static class Log
    {
        private static readonly string Folder = Path.Combine(Program.AppData, "logs");
        private static readonly string OutputFile = Path.Combine(Folder, "gsmod.output.log");

        public static void InitDirs()
        {
            if (!Directory.Exists(Program.AppData)) Directory.CreateDirectory(Program.AppData);
            if (!Directory.Exists(Folder)) Directory.CreateDirectory(Folder);
        }

        public static async void Output(string log)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(OutputFile))
                {
                    await sw.WriteLineAsync($"[{Program.AppVersion}: {DateTime.Now}]: {log}");
                }
            }
            catch
            {
                // . . .
            }
        }

        public static async void WriteLine(string log)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(OutputFile))
                {
                    await sw.WriteLineAsync($"[{Program.AppVersion}: {DateTime.Now}]: {log}");
                }
            }
            catch
            {
                // . . .
            }
        }

        public static async void SaveError(string log)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(OutputFile))
                {
                    await sw.WriteLineAsync($"[{Program.AppVersion}: {DateTime.Now}]: SaveError() • {Assembly.GetExecutingAssembly().GetName().Name}\n{log}\n");
                }
            }
            catch
            {
                // . . .
            }
        }

        public static void ThrowError(Exception ex)
        {
            Console.WriteLine($"{ex.Message}\n");

            SaveError(ex.ToString());
        }

        public static void ThrowErrorString(string log)
        {
            Console.WriteLine(log);

            Console.WriteLine("\n=========================================================================================");
            Console.WriteLine("[x] Meeow (=〃ﻌ〃=)! We're sorry. It seems like we've encountered an issue.");
            Console.WriteLine("[i] If you require assistance, kindly visit: https://genshin.sefinek.net/support");

            SaveError(log);
            Utils.Pause();
        }

        public static void ErrorAndExit(Exception ex)
        {
            ThrowError(ex);

            Environment.Exit(999991000);
        }
    }
}
