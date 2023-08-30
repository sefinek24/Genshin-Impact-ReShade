using System;
using System.Diagnostics;
using System.Security.Principal;

namespace StarRailStellaMod.Scripts
{
    internal static class Utils
    {
        public static bool IsRunningWithAdminPrivileges()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public static bool IsArrayEmpty(string[] obj)
        {
            return obj.Length == 0;
        }

        public static void Pause()
        {
            const string text = "Press any key to close this application...";
            Console.Write($"\n{text}");
            Log.Output(text);

            Console.ReadKey();

            Log.Output("Exiting...");
            Environment.Exit(0);
        }

        public static void CheckProcess(string processName)
        {
            if (string.IsNullOrEmpty(processName))
            {
                Log.ThrowErrorString("[X] Process name cannot be null or empty.");
                return;
            }

            if (processName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)) processName = processName.Substring(0, processName.Length - 4);

            Process[] processes = Process.GetProcessesByName(processName);

            foreach (Process process in processes)
            {
                Console.WriteLine($"[i] Process {process.ProcessName} is running. Killing...");
                Log.Output($"Killing process {process.ProcessName}...");

                process.Kill();
                process.WaitForExit();
            }
        }
    }
}
