using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GenshinStellaMod.Models;
using GenshinStellaMod.Scripts;
using Microsoft.Win32;
using Newtonsoft.Json;
using NLog;
using NLog.Config;

/*
 *
 * IMPORTANT!
 * Some files of this project are not publicly accessible!
 *
 */

namespace GenshinStellaMod
{
    internal static class Program
    {
        // App
        public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
        public static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        // Files and folders
        public static readonly string AppPath = AppDomain.CurrentDomain.BaseDirectory;

        // API
        // public static readonly string WebApi = Debugger.IsAttached ? "http://127.0.0.1:4010/api/v5" : "https://api.sefinek.net/api/v5";
        public static readonly string WebApi = "https://api.sefinek.net/api/v5";

        // Logger
        public static Logger Logger = LogManager.GetCurrentClassLogger();

        private static async Task Main(string[] args)
        {
            Logger = Logger.WithProperty("AppName", "Genshin Stella Mod");
            Logger = Logger.WithProperty("AppVersion", AppVersion);
            LogManager.Configuration = new XmlLoggingConfiguration(Path.Combine(AppPath, "NLog_GSM.config"));

            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = $"Genshin Stella Mod - v{AppVersion}";

            Console.ForegroundColor = ConsoleColor.White;

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


            Console.ForegroundColor = ConsoleColor.White;
            bool isSubscriber = Data.IsUserMyPatron();
            Console.WriteLine("⠀  ⠀⠀⠀⠀⠀⠀⠀ ⢀⣤⡶⢶⣦⡀");
            Console.WriteLine("⠀  ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿");
            Console.WriteLine($"⠀  ⠀⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                            {(!isSubscriber ? "Genshin Impact Stella Mod 2023" : "Genshin Impact Stella Mod Plus+ 2023")}");
            Console.WriteLine($"⠀  ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿                             {(!isSubscriber ? "        Start the game" : "    ~ Release for subscribers ~")}");
            Console.WriteLine("⠀  ⠀⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿");
            Console.WriteLine("⠀⠀  ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟");
            Console.WriteLine("⠀⠀⠀  ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀");
            Console.WriteLine("⠀⠀⠀  ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀");
            Console.WriteLine("⠀⠀⠀  ⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆      » Mod version          : v" + (args[0].EndsWith(".0") ? args[0].Substring(0, args[0].Length - 2) : args[0]));
            Console.WriteLine("⠀⠀  ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆    » ReShade version      : v" + args[1]);
            Console.WriteLine("  ⠀⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇     » FPS Unlocker version : v" + args[2]);
            Console.WriteLine("  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟");
            Console.WriteLine("  ⠀⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⡴⠟⠁                                         ~ Made by Sefinek");
            Console.WriteLine("=========================================================================================\n");

            /***** 1 *****/
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("1/3 - Starting program...");
            Console.ResetColor();


            // Set app title etc.
            string launchMode = args[3];
            Logger.Info($"Launch mode: {launchMode}");


            // Verify that the application is running with administrative privileges.
            if (!Utils.IsRunningWithAdminPrivileges())
            {
                Log.ThrowErrorString("[x] This file needs to be executed with administrative privileges");
                Utils.Pause();
            }

            Console.WriteLine("[✓] Administrative permissions");


            // Is launcher configured?
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Secret.RegistryPath, true))
            {
                int value = (int)(key?.GetValue("AppIsConfigured") ?? 0);
                if (value == 0)
                {
                    Log.ThrowErrorString("[x] The software is not configured yet. Please run the launcher first");

                    Utils.Pause();
                }
                else
                {
                    Console.WriteLine("[✓] The launcher is configured");
                }
            }


            // Is the user a patron?
            if (launchMode == "1" || launchMode == "5")
            {
                string mainPcKey = Secret.GetTokenFromRegistry();
                if (mainPcKey != null)
                {
                    Secret.Attempt = true;

                    string data = await Secret.VerifyToken(mainPcKey);
                    if (data == null)
                    {
                        Secret.IsMyPatron = false;
                        Logger.Info($"Received null from the server. Closing {AppName}...");

                        Environment.Exit(6660666);
                    }

                    VerifyToken remote = JsonConvert.DeserializeObject<VerifyToken>(data);
                    Logger.Info($"Status: {remote.Status}; Tier {remote.TierId}; Message: {remote.Message ?? "Unknown"};");

                    switch (remote.Status)
                    {
                        case 200:
                            Secret.IsMyPatron = true;
                            Logger.Info($"The user is a subscriber to Stella Mod Plus ({Secret.IsMyPatron}); Allowed;");
                            Console.WriteLine("[✓] Verified Stella Mod Plus subscriber");
                            break;

                        case 500:
                            Secret.IsMyPatron = false;

                            MessageBox.Show(
                                $"Unfortunately, something went wrong, and 3DMigoto will not be injected into your game. There was a server-side error while verifying the registered license on your computer. Please try again in a few minutes. If the issue persists, please contact Sefinek.{remote.Message}",
                                AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;

                        default:
                            Secret.IsMyPatron = false;

                            MessageBox.Show(
                                $"An error occurred while verifying the benefits of your subscription. The server informed the client that it sent an invalid request. If you launch the game after closing this message, you will be playing the free version. Please contact Sefinek for more information. Error details can be found below.\n\n{remote.Message}",
                                AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                    }
                }
                else
                {
                    Secret.IsMyPatron = false;
                }
            }


            // Good?
            if (!Secret.IsMyPatron && (launchMode == "1" || launchMode == "5"))
            {
                Console.WriteLine("[x] Not this time bro");

                Logger.Error($"An attempt was made to use launchMode {launchMode} without being a Stella Mod Plus subscriber; Secret.IsMyPatron: {Secret.IsMyPatron}; Secret.Attempt: {Secret.Attempt}");
                MessageBox.Show("The security system has detected a breach.", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                Environment.Exit(1432166809);
            }


            // Start
            try
            {
                const string id = "48793142";
                string path = Path.Combine(AppPath, "net8.0-windows10.0.18362.0", $"{id}.exe");

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Secret.RegistryPath, true))
                {
                    int value = (int)(key?.GetValue(id) ?? 0);
                    if (value == 0)
                    {
                        if (!File.Exists(path)) return;
                        _ = Cmd.Execute(new Cmd.CliWrap { App = path });
                        key?.SetValue(id, 1);

                        bool processFound = false;

                        while (!processFound)
                        {
                            Process[] processes = Process.GetProcessesByName(id);
                            if (processes.Length == 0) processFound = true;

                            Thread.Sleep(1000);
                        }
                    }
                }

                await Action.Run(launchMode);
            }
            catch (Exception ex)
            {
                Log.ThrowError(ex);

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("=========================================================================================");
                Console.WriteLine("[x] We apologize, but unfortunately something didn't go according to our plan ):");
                Console.WriteLine("[i] If you believe this error is not your fault, please report it: https://genshin.sefinek.net/support");

                Music.PlaySound("winxp", "critical_stop");
            }
        }
    }
}
