using System;
using System.IO;
using System.Threading.Tasks;
using GenshinStellaMod.Scripts;
using Microsoft.Win32;

namespace GenshinStellaMod
{
    internal static class Action
    {
        private static string FullGamePath;
        public static string GameExeFile;

        public static async Task Run(string launchMode, string resources)
        {
            Console.WriteLine("1/4 - Checking required files...");

            // Check if the required files exist
            if (!File.Exists(Logic.FpsUnlockerPath))
            {
                Console.WriteLine($"[x] Failed to start. File {Path.GetFileName(Logic.FpsUnlockerPath)} was not found.");
                Utils.Pause();
                return;
            }

            if (!File.Exists(Program.ReShadeDllPath))
            {
                Console.WriteLine($"[x] Failed to start. File {Path.GetFileName(Program.ReShadeDllPath)} was not found.");
                Utils.Pause();
                return;
            }

            if (!File.Exists(Logic.InjectorPath))
            {
                Console.WriteLine($"[x] Failed to start. File {Path.GetFileName(Logic.InjectorPath)} was not found.");
                Utils.Pause();
                return;
            }

            Console.WriteLine("[✓] Status: OK\n");


            // Find game path
            Console.WriteLine("2/4 - Finding game path...");
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Program.RegistryPath))
            {
                if (key != null) FullGamePath = (string)key.GetValue("GamePath");
            }

            if (!File.Exists(FullGamePath))
            {
                Console.WriteLine($"[X] Game instance was not found . I cannot inject ReShade.\n[i] File: {FullGamePath ?? "Unknown"}");
                Utils.Pause();
            }

            GameExeFile = Path.GetFileName(FullGamePath);
            if (GameExeFile == null)
            {
                Console.WriteLine("[X] Fatal error. Game exe file is null. But why?");
                Utils.Pause();
            }

            Console.WriteLine($"[✓] {FullGamePath}\n");


            // Start
            Console.WriteLine($"3/4 - Starting (launch mode {launchMode})...");
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
