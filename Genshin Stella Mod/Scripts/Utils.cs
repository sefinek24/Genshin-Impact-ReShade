using System;
using System.Security.Principal;

namespace GenshinStellaMod.Scripts
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

        public static void Pause()
        {
            const string text = "Press any key to close this application...";
            Console.Write($"\n{text}");
            Log.Output(text);

            Console.ReadKey();

            Log.Output("Exiting...");
            Environment.Exit(0);
        }
    }
}
