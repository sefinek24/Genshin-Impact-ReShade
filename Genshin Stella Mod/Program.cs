using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GenshinStellaMod.Scripts;

namespace GenshinStellaMod
{
    internal static class Program
    {
        // App
        public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
        public static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        // Files and folders
        public static readonly string AppPath = AppContext.BaseDirectory;
        public static readonly string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stella Mod Launcher");

        // Registry
        public static readonly string RegistryPath = @"SOFTWARE\Stella Mod Launcher";

        private static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            string isMyPatron = args[4];
            Console.WriteLine("⠀   ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀");
            Console.WriteLine("⠀  ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿");
            Console.WriteLine("⠀   ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                              Genshin Impact Stella Mod 2023");
            Console.WriteLine($"⠀   ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿                                {(isMyPatron == "0" ? "     Start the game" : "~~ Release for Patrons ~~")}");
            Console.WriteLine("⠀   ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿");
            Console.WriteLine("⠀    ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟");
            Console.WriteLine("⠀   ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀");
            Console.WriteLine("⠀   ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀");
            Console.WriteLine("⠀    ⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆      » Mod version          : v" + (args[0].EndsWith(".0") ? args[0].Substring(0, args[0].Length - 2) : args[0]));
            Console.WriteLine("⠀    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    » ReShade version      : v" + args[1]);
            Console.WriteLine("⠀   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇     » FPS Unlocker version : v" + args[2]);
            Console.WriteLine("⠀  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟");
            Console.WriteLine("⠀   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟                                          ~ Made by Sefinek");
            Console.WriteLine(" =========================================================================================\n");

            /***** 1 *****/
            Console.WriteLine("1/3 - Starting program...");

            string launchMode = args[3];
            Console.Title = $"Genshin Stella Mod - {launchMode}";

            // Check if the application is running with administrative permissions
            if (!Utils.IsRunningWithAdminPrivileges())
            {
                Log.ThrowErrorString("[X] This file needs to be executed with administrative privileges.");
                Utils.Pause();
                return;
            }

            Console.WriteLine("[✓] Permissions");

            // Init dirs
            Log.InitDirs();


            // Start the application
            try
            {
                string resources = args[5];

                Log.Output("Starting...");
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
