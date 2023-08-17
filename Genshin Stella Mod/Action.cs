using System;
using System.IO;
using System.Threading.Tasks;
using GenshinStellaMod.Scripts;
using Microsoft.Win32;

namespace GenshinStellaMod
{
    internal static class Action
    {
        private static string _fullGamePath;
        public static string GameExeFile;

        public static async Task Run(string launchMode, string resources)
        {
            // Find game path
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Program.RegistryPath))
            {
                if (key != null) _fullGamePath = (string)key.GetValue("GamePath");
            }

            if (!File.Exists(_fullGamePath)) Log.ThrowErrorString($"[X] Game instance was not found . I cannot inject ReShade.\n[i] File: {_fullGamePath ?? "Unknown"}");

            GameExeFile = Path.GetFileName(_fullGamePath);
            if (GameExeFile == null) Log.ThrowErrorString("[X] Fatal error. Game exe file is null. But why?");

            Console.WriteLine($"[✓] Found game path: {_fullGamePath}");


            // Check if the required files exist
            if (!File.Exists(Logic.FpsUnlockerPath)) Log.ThrowErrorString($"[x] Failed to start. File {Path.GetFileName(Logic.FpsUnlockerPath)} was not found.");
            if (!File.Exists(Logic.ReShadeDllPath)) Log.ThrowErrorString($"[x] Failed to start. File {Path.GetFileName(Logic.ReShadeDllPath)} was not found.");
            if (!File.Exists(Logic.InjectorPath)) Log.ThrowErrorString($"[x] Failed to start. File {Path.GetFileName(Logic.InjectorPath)} was not found.");

            Console.WriteLine("[✓] Found required files\n");


            /***** 3 *****/
            Console.WriteLine("2/3 - Checking processes...");
            Utils.CheckProcess(GameExeFile);
            Utils.CheckProcess("");
            Utils.CheckProcess(Path.GetFileName(Logic.FpsUnlockerPath));
            Utils.CheckProcess(Path.GetFileName(Logic.InjectorPath));


            Console.WriteLine("[✓] Completed\n");


            /***** 4 *****/
            // Start
            Console.WriteLine($"3/3 - Starting (launch mode {launchMode})...");
            switch (launchMode)
            {
                case "1":
                {
                    Logic.UnlockFps();

                    await Logic.InjectReShade();
                    await Logic.Migoto(resources);
                    break;
                }
                case "3":
                {
                    await Logic.InjectReShade();
                    break;
                }
                case "4":
                {
                    Logic.UnlockFps();

                    Console.WriteLine("~~ Please start the game now. ~~");
                    Console.WriteLine();
                    break;
                }
                case "5":
                {
                    await Logic.Migoto(resources);
                    break;
                }
                case "6":
                {
                    Logic.UnlockFps();
                    await Logic.InjectReShade();
                    break;
                }
                default:
                    Console.WriteLine("[x] Failed to start. Invalid launch mode.");
                    return;
            }

            Logic.Completed();

            Utils.Pause();
        }
    }
}
