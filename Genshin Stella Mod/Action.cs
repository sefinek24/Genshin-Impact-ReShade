using System;
using System.IO;
using System.Threading.Tasks;
using GenshinStellaMod.Scripts;

namespace GenshinStellaMod
{
    internal static class Action
    {
        public static async Task Run(string launchMode, string resources)
        {
            Console.WriteLine("1/4 - Preparing...");


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
