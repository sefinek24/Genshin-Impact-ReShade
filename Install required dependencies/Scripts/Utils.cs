using System;
using System.Diagnostics;
using System.IO;
using Prepare_mod;
using Prepare_mod.Scripts;

namespace PrepareMod.Scripts
{
    internal static class Utils
    {
        public static void OpenUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                Log.ThrowError(new Exception("URL is null or empty."), false);
                return;
            }

            try
            {
                Process.Start(url);
                Log.Output($"Opened '{url}' in default browser.");
            }
            catch (Exception ex)
            {
                Log.ThrowError(new Exception($"Failed to open '{url}' in default browser.\n{ex}"), false);
            }
        }

        public static string GetAppData()
        {
            if (!Directory.Exists(Program.Packages))
            {
                Log.Output($"{Program.Packages} was not found.");
                return null;
            }

            string[] dirs = Directory.GetDirectories(Program.Packages, "Microsoft.WindowsTerminal_*", SearchOption.AllDirectories);

            string path = "";
            foreach (string dir in dirs) path = dir;

            return path;
        }

        public static string GetProgramFiles()
        {
            if (!Directory.Exists(Program.WindowsApps))
            {
                Log.Output($"{Program.WindowsApps} was not found.");
                return null;
            }

            string[] dirs = Directory.GetDirectories(Program.WindowsApps, "Microsoft.WindowsTerminal_*", SearchOption.AllDirectories);

            string path = "";
            foreach (string dir in dirs) path = dir;

            return path;
        }
    }
}
