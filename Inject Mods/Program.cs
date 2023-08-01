using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Inject_Mods.Scripts;

namespace Inject_Mods
{
    internal static class Program
    {
        // App
        public static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        // Files and folders
        public static readonly string AppPath = AppContext.BaseDirectory;
        public static readonly string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stella Mod Launcher");
        public static readonly string ReShadeDllPath = Path.Combine(AppPath, "data", "reshade", "ReShade64.dll");

        private static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            string isMyPatron = args[4];
            string launchMode = args[3];

            Console.Title = $"Start the game - {launchMode}";

            Console.WriteLine("⠀   ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀");
            Console.WriteLine("⠀  ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿");
            Console.WriteLine("⠀   ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                              Genshin Impact Stella Mod 2023");
            Console.WriteLine($"⠀   ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿                                {(isMyPatron == "0" ? "Start the game" : "~~ Release for Patrons ~~")}");
            Console.WriteLine("⠀   ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿");
            Console.WriteLine("⠀    ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟");
            Console.WriteLine("⠀   ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀");
            Console.WriteLine("⠀   ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀");
            Console.WriteLine("⠀    ⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆      » Mod version          : v" + args[0]);
            Console.WriteLine("⠀    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    » ReShade version      : v" + args[1]);
            Console.WriteLine("⠀   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇     » FPS Unlocker version : v" + args[2]);
            Console.WriteLine("⠀  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟");
            Console.WriteLine("⠀   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟                                          ~ Made by Sefinek");
            Console.WriteLine(" =========================================================================================\n");


            // Check if the application is running with administrative permissions
            if (!Utils.IsRunningWithAdminPrivileges())
            {
                Console.WriteLine("[x] This file needs to be executed with administrative privileges.");
                Log.Output("The file must be run as administrator.");
                Utils.Pause();
                return;
            }


            string resources = args[5];

            try
            {
                await Action.Run(launchMode, resources);
            }
            catch (Exception ex)
            {
                Log.ThrowError(ex);
                Console.WriteLine("=========================================================================================");
                Console.WriteLine("[x] Oops, we're sorry. The application failed to start for some reason.");
                Console.WriteLine("[i] If you need help, please visit: https://sefinek.net/genshin-impact-reshade/support");
            }
        }
    }
}
