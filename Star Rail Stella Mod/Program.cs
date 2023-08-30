using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StarRailStellaMod.Scripts;
using Microsoft.Win32;

/*
 *
 * IMPORTANT!
 * Some files of this project are not publicly accessible!
 *
 */

namespace StarRailStellaMod
{
    using StarRailStellaMod.Scripts;

    internal static class Program
    {
        // App
        public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
        public static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        // Files and folders
        public static readonly string AppPath = AppContext.BaseDirectory;
        public static readonly string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stella Mod Launcher");

        // Registry
        public static readonly string RegistryPath = @"Software\Stella Mod Launcher\Star Rail";

        private static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            if (Utils.IsArrayEmpty(args))
            {
                Console.Title = @" /ᐠ –ꞈ –ᐟ\";
                Console.SetWindowSize(110, 45);

                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.WriteLine(
                    "⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣷⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⣀⠀⠀⠀⠀⠀⠀⠀⢿⣇⠀⢸⡆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⠀⠀⠀⠀⠀⠀⠀⠈⠛⠿⣽⣛⠳⠶⣤⣄⠀⠘⣿⣧⡀⣷⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢹⡿⣷⠶⠶⣦⣤⣄⣀⣤⠴⠶⠿⠿⠷⠦⣍⣙⠶⣿⣎⠻⣼⣷⡀⠀⠀⠀⠀⠀⠀⠀⣀⣠⣤⣶⢿⡇\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⣿⠈⢶⠀⠀⠐⠻⠟⠒⣺⡾⢷⣒⠛⢀⠀⠀⠀⠀⠙⠷⣌⠻⣿⣄⣀⣤⡶⠶⠶⠛⠋⠉⡽⠃⢸⡇\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⠈⢷⠀⠀⢀⣴⣿⣷⣾⡽⣶⠟⠋⠀⠀⠀⠀⠀⠀⠀⠉⠈⠻⣿⣄⠀⠀⠀⠀⢀⡾⠀⠀⣼⡇\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⠀⠈⣇⣠⡞⣩⡿⠋⢀⡞⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠛⢧⡀⠀⢀⣟⣀⠀⢀⣿⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⠀⢰⣾⠏⠰⠋⠀⠀⠏⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢡⡀⢺⣏⠥⠀⢸⡏⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⡆⠀⣸⡏⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢳⢸⠓⠀⠀⣼⠇⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⣿⠀⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⣤⣤⣤⣤⡀⠀⠀⠀⠀⠀⢿⡇⠀⣸⡏⠀⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠹⣿⣿⢸⠟⣿⣶⣄⠀⠀⠀⠀⠀⠀⠀⠀⢠⣿⡟⠛⠛⢿⣿⣿⣷⡄⠀⠀⠀⠘⣧⢠⡿⠁⠀⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢹⡿⠘⢿⡇⠀⢻⣷⠀⠀⠀⠀⠀⠀⠀⣿⣿⡄⠀⠀⢀⣿⣿⣿⣿⡆⠀⠀⠀⢿⡟⠁⠀⠀⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⠘⣿⣶⣾⣿⡇⠀⠀⠀⠀⠀⠀⣯⣿⣿⣶⣶⣿⣿⣿⡿⣿⡇⠀⠀⠀⠘⣧⠀⠀⠀⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣼⠇⠀⠀⢹⣟⠟⢿⡇⠀⠀⠀⠀⠀⠀⠀⢹⣿⠘⢿⣿⣿⡋⠃⢸⡇⠀⠀⠀⠀⢿⡄⠀⠀⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣰⡿⠀⠀⠀⠀⢻⣄⣿⠁⠀⠀⠀⠀⠀⠀⠀⠀⠻⣧⣀⠈⠀⠀⣠⡿⠃⠀⠀⠀⠀⢾⣷⡀⠀⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣿⡶⠀⠀⠀⠀⠘⠿⣃⠀⠀⢠⣶⠆⠀⠀⠀⠀⠀⠈⠉⠓⠶⠞⠋⠁⠀⠀⠀⠀⠘⣞⣿⢷⡀⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣻⠃⠀⠀⠀⠀⠀⠈⢁⠀⠀⠰⠷⠆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣄⣀⠀⠀⢹⣾⡆⠙⠂\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⣿⣤⣶⣿⣿⣿⣥⣄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⣤⣶⣶⣶⣶⣬⣿⣿⢷⣬⣿⡇⠀⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠃⠘⠃⣠⣾⣿⠈⠉⠛⠓⠒⠀⠀⠀⠀⠀⠀⠒⠚⠉⠉⠀⠀⠀⢛⣿⣿⣦⡙⠓⠙⠇⠀⠀⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⢻⣿⣧⠆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⡝⢮⣷⣅⠀⠀⠀⠀⠀⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣿⣿⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⡙⢮⣻⡿⣷⠀⠀⠀⠀⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⣴⠟⠋⠃⡿⣷⡳⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⡜⣷⣿⠙⠀⠀⠀⠀⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⣴⠿⠋⠀⠀⠀⠀⠙⠙⠷⣽⣄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣸⣿⣽⡼⠃⠀⠀⠀⠀⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣴⡾⠿⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠉⠓⠦⣄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣿⣿⣿⠁⠀⠀⠀⠀⠀⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣿⠏⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡄⠀⠀⠀⠈⠻⢦⡄⠀⠀⠀⠀⠀⠀⠀⠀⣼⠟⠈⣿⠀⠀⠀⠀⠀⠀⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⡀⠀⢀⠀⢀⣿⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡾⠁⠀⠀⠀⠀⠀⠀⠙⣆⠀⠀⠀⠀⢀⣷⡼⠃⠀⢸⣿⠀⠀⠀⠀⠀⠀⠀\n⠀⠀⠀⠀⠀⢀⣀⡈⢿⣦⣼⡄⣾⡏⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⣶⣶⡋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠀⠀⣀⡴⠟⠉⡷⠀⠀⣸⡏⠀⠀⠀⠀⠀⠀⠀\n⠀⠀⠀⠀⠀⣉⣻⣿⣽⠿⠿⢿⡿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⢿⡗⠒⠀⠀⠀⠀⠀⠀⠀⢠⠀⠐⠊⠉⠀⠀⣶⠃⠀⢰⡿⠀⠀⠀⠀⠀⠀⠀⠀\n⠀⠀⠀⠀⣻⣿⡟⠋⠁⠀⠀⣿⠇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⣿⣆⠀⠀⠀⠀⠀⠀⠀⣸⠀⠀⠀⠀⣠⣾⠛⠀⢰⡿⠃⠀⠀⠀⠀⠀⠀⠀⠀\n⠀⠀⣠⣾⠟⠋⠀⠀⠀⠀⠀⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣾⣿⡄⠀⠀⠀⠀⠀⠀⣿⠀⠀⣠⡼⠟⠁⠀⢠⣿⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀\n⠀⢀⣿⣡⡶⠋⠀⠀⠀⠀⠒⠻⣧⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⡿⠘⣿⡀⠀⠀⠀⠀⠀⢸⣷⣿⡁⠀⠀⠀⠀⣾⠇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀\n⢀⣾⣿⠏⠀⠀⠀⠀⠐⠀⠀⠀⠈⠻⠶⢤⣄⣀⣤⣴⠶⠂⠀⠀⢀⣾⠃⠀⠙⣷⡀⠀⠀⠀⠀⠈⣯⢹⡇⠀⠀⠀⢠⣿⠛⠻⢷⡄⠀⠀⠀⠀⠀⠀⠀\n⠀⣼⠏⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠿⢿⣶⣶⣄⣤⢾⣧⣀⣠⣤⣼⣷⡄⠀⠀⠀⠀⢹⣧⣿⡀⠀⠀⡴⠿⣶⡜⣿⣷⠀⠀⠀⠀⠀⠀⠀\n⢸⣯⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠻⢷⣦⣄⠉⠁⢤⠀⣽⣿⣦⣀⣠⠔⢏⠙⢮⣻⣄⠀⣀⣧⣿⡿⠛⠁⠀⠀⠀⠀⠀⠀⠀\n⢸⣿⡇⣦⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠹⠿⢶⣶⣧⣼⠾⠋⠛⠷⠶⠾⠷⠟⠋⠉⠉⠉⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀\n⠘⠋⢿⡿⣿⣄⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠿⣷⣦⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀\n⠀⠀⠀⠀⠉⠙⠻⠷⢦⣄⣤⣄⡀⠀⠀⣄⠀⢀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠨⣝⢧⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠉⠛⠓⠺⠷⣬⣿⣦⣿⣦⣤⣰⣄⡀⠀⠠⣤⣀⠀⠈⠻⢿⣦⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀\n⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⠀⢈⠉⠁⠈⡑⠉⣉⠛⠋⠛⠲⠿⡿⡟⠓⢲⣿⠂⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀\n\n");

                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Red;
                const int violationCode = 54632789;
                Log.ThrowError(new Exception($"Potential violation has occurred.\n       Exit code: {violationCode}       "));

                MessageBox.Show(
                    "The `string[]` is empty. Don't tinker with anything in this file!\nKindly run the file `Stella Mod Launcher.exe` instead.\n\nThere might have been a certain violation. Please remember to respect the authors of this software as well as the game Genshin Impact.",
                    AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                Environment.Exit(violationCode);
            }

            string isMyPatron = args[4];
            Console.WriteLine("⠀   ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀");
            Console.WriteLine("⠀  ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿");
            Console.WriteLine("⠀   ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                              Honkai Star Rail Stella Mod 2023");
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
            Log.Output($"Launch mode: {launchMode}");
            Console.Title = $"Star Rail Stella Mod v{AppVersion}";

            // Check if the application is running with administrative permissions
            if (!Utils.IsRunningWithAdminPrivileges())
            {
                Log.ThrowErrorString("[X] This file needs to be executed with administrative privileges.");
                Utils.Pause();
            }

            Console.WriteLine("[✓] Administrative permissions");


            // Is launcher configured?
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath, true))
            {
                int value = (int)(key?.GetValue("AppIsConfigured") ?? 0);
                if (value == 0)
                {
                    Log.ThrowErrorString("[X] The software is not configured yet. Please run the launcher first");

                    Utils.Pause();
                }
                else
                {
                    Console.WriteLine("[✓] The launcher is configured");
                }
            }


            // Init dirs
            Log.InitDirs();


            // Start the application
            try
            {
                string resources = args[5];
                Log.Output($"Resources: {resources}");

                await Action.Run(launchMode, resources);
            }
            catch (Exception ex)
            {
                Log.ThrowError(ex);

                Console.WriteLine("=========================================================================================");
                Console.WriteLine("[x] We apologize, but unfortunately something didn't go according to our plan.");
                Console.WriteLine("[i] If you believe this error is not your fault, please report it: https://genshin.sefinek.net/support");

                Music.PlaySound("winxp", "critical_stop");
            }
        }
    }
}
