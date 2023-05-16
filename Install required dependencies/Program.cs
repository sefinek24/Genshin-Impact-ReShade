using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Prepare_mod.Forms;
using Prepare_mod.Properties;
using Prepare_mod.Scripts;
using Prepare_mod.Scripts.Preparing;
using PrepareMod.Scripts;

namespace Prepare_mod
{
    internal static class Program
    {
        // App
        public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
        private static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static readonly string AppPath = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string AppData = Utils.GetAppData();
        public static string GamePathSfn;

        // Links
        private static readonly string AppWebsite = "https://genshin.sefinek.net";
        public static readonly string DiscordUrl = "https://discord.gg/SVcbaRc7gH";

        // Files and folders
        private static readonly IniFile PrepareIni = new IniFile(Path.Combine(AppData, "prepare-stella.ini"));
        public static readonly string ProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        public static readonly string GameGenshinImpact = Path.Combine(ProgramFiles, "Genshin Impact", "Genshin Impact game", "GenshinImpact.exe");
        public static readonly string GameYuanShen = Path.Combine(ProgramFiles, "Genshin Impact", "Genshin Impact game", "YuanShen.exe");
        public static readonly string WindowsApps = Path.Combine(ProgramFiles, "WindowsApps");
        public static readonly string Packages = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData") ?? string.Empty, "Packages");
        private static readonly string InstalledViaSetup = Path.Combine(AppData, "configured.sfn");

        // Dependencies
        public static readonly string WtWin10Setup = Path.Combine("dependencies", "WindowsTerminal_Win10.msixbundle");
        public static readonly string WtWin11Setup = Path.Combine("dependencies", "WindowsTerminal_Win11.msixbundle");

        // Other
        public static readonly string UserAgent = $"Mozilla/5.0 (compatible; GenshinStellaSetup/{AppVersion}; +{AppWebsite})";
        private static readonly string Line = "===============================================================================================";

        // Global variables
        public static string GameExeGlobal;
        public static string GameDirGlobal;
        public static string ResourcesGlobal;
        public static string ReShadeConfig;
        public static string ReShadeLogFile;


        private static async Task Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"                             Genshin Impact Stella Mod - Beta release");
            Console.WriteLine($"                                        Version: v{AppVersion}\n");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(@"» Author  : Sefinek [Country: Poland]");
            Console.WriteLine(@"» Website : " + AppWebsite);
            Console.WriteLine(@"» Discord : " + DiscordUrl);
            Console.ResetColor();
            Console.WriteLine(Line);

            Console.Title = $@"{AppName} • v{AppVersion}";

            if (!Utils.IsRunAsAdmin())
            {
                Log.ErrorAndExit(new Exception("» This application requires administrator privileges to run."), false, false);
                return;
            }


            // Game path
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(@"» Game path: ");
            Console.ResetColor();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!Directory.Exists(AppData)) Directory.CreateDirectory(AppData);
            GamePathSfn = Path.Combine(AppData, "game-path.sfn");


            if (File.Exists(GamePathSfn))
            {
                string fileWithGamePath = File.ReadAllText(GamePathSfn).Trim();
                if (Directory.Exists(fileWithGamePath)) GameDirGlobal = fileWithGamePath;
            }
            else
            {
                if (File.Exists(GameGenshinImpact))
                    GameDirGlobal = Path.GetDirectoryName(Path.GetDirectoryName(GameGenshinImpact));

                if (File.Exists(GameYuanShen))
                    GameDirGlobal = Path.GetDirectoryName(Path.GetDirectoryName(GameYuanShen));
            }

            if (Directory.Exists(GameDirGlobal))
            {
                File.WriteAllText(GamePathSfn, GameDirGlobal);
                Console.WriteLine(GameDirGlobal);
            }
            else
            {
                Application.Run(new SelectGamePath { Icon = Resources.icon });
            }


            if (Directory.Exists(GameDirGlobal))
            {
                ReShadeConfig = Path.Combine(GameDirGlobal, "Genshin Impact game", "ReShade.ini");
                ReShadeLogFile = Path.Combine(GameDirGlobal, "Genshin Impact game", "ReShade.log");

                if (File.Exists(GameGenshinImpact)) GameExeGlobal = GameGenshinImpact;
                if (File.Exists(GameYuanShen)) GameExeGlobal = GameYuanShen;
            }
            else
            {
                Console.WriteLine();
            }


            // Read ini file
            Console.WriteLine("\nStarting... ");

            // Save AppData path
            File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "stella-appdata.sfn"), AppData);

            int newShortcuts = PrepareIni.ReadInt("PrepareStella", "NewShortcutsOnDesktop", 0);
            int newIntShortcuts = PrepareIni.ReadInt("PrepareStella", "InternetShortcutsInStartMenu", 1);
            int downloadOrUpdateShaders = PrepareIni.ReadInt("PrepareStella", "DownloadOrUpdateShaders", 1);
            int updateReShadeCfg = PrepareIni.ReadInt("PrepareStella", "UpdateReShadeConfig", 1);
            int updateFpsUnlockerCfg = PrepareIni.ReadInt("PrepareStella", "UpdateFpsUnlockerConfig", 1);
            int delReShadeCache = PrepareIni.ReadInt("PrepareStella", "DeleteReShadeCache", 1);
            int installWtUpdate = PrepareIni.ReadInt("PrepareStella", "InstOrUpdWT", 1);


            // Create or update Desktop icon
            if (newShortcuts == 1) DesktopIcon.Run();

            // Create new Internet shortcuts in menu start
            if (newIntShortcuts == 1) await InternetShortcuts.Run();

            // Download and prepare ReShade config
            if (updateReShadeCfg == 1) await UpdateReShadeCfg.Run();

            // Download shaders, presets and addons (Stella resources)
            if (downloadOrUpdateShaders == 1) await DownloadUpdateResources.Run();

            // Download FPS Unlocker config
            if (updateFpsUnlockerCfg == 1) await DownloadFpsUnlockerCfg.Run();

            // Delete ReShade cache
            if (delReShadeCache == 1) await DeleteReShadeCache.Run();

            // Windows Terminal installation
            if (installWtUpdate == 1) await TerminalInstallation.Run();


            // Create files
            if (!Directory.Exists(AppData)) Directory.CreateDirectory(AppData);
            if (!File.Exists(InstalledViaSetup)) File.Create(InstalledViaSetup);

            // Reboot is required?
            if (Cmd.RebootNeeded)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(@"» Restart your computer now? This is required! [Yes/no]: ");
                Console.ResetColor();

                string rebootPc = Console.ReadLine();
                if (Regex.Match(rebootPc ?? string.Empty, "(?:y)", RegexOptions.IgnoreCase | RegexOptions.Singleline).Success)
                {
                    await Cmd.CliWrap("shutdown",
                        $"/r /t 30 /c \"{AppName} - scheduled reboot, version {AppVersion}.\n\nThank you for installing. If you need help, add me on Discord Sefinek#2714.\n\nGood luck and have fun!\"", null);

                    Console.WriteLine(@"Your computer will restart in 30 seconds. Save your work!");
                    Log.Output("PC reboot was scheduled.");
                }
            }

            // Run Genshin Stella Mod
            Console.WriteLine(@"Launching Genshin Stella Mod...");
            Process.Start(Path.Combine(AppPath, "Genshin Stella Mod.exe"));

            // Close app
            Console.WriteLine($"\n{Line}\n");

            const int seconds = 20;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($@"» The program will close in {seconds} seconds.");
            for (int i = seconds; i >= 1; i--) Thread.Sleep(1000);
        }
    }
}
