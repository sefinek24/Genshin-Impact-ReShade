using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using Windows.Storage;

namespace PrepareStella.Scripts
{
    internal static class Utils
    {
        public static bool IsRunAsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static string GetAppData()
        {
            try
            {
                return Path.Combine(ApplicationData.Current?.LocalFolder?.Path);
            }
            catch (InvalidOperationException)
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Genshin Stella Mod");
            }
        }

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

        public static string GetWtAppData()
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

        public static string GetWtProgramFiles()
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
