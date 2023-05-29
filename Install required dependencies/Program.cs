using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Taskbar;
using PrepareStella.Forms;
using PrepareStella.Properties;
using PrepareStella.Scripts;
using PrepareStella.Scripts.Preparing;

namespace PrepareStella
{
    internal static class Program
    {
        // App
        public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
        public static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static readonly string AppPath = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string AppData = Utils.GetAppData();
        public static string GamePathSfn;

        // Links
        public static readonly string AppWebsite = "https://genshin.sefinek.net";
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
        public static readonly string UserAgent = $"Mozilla/5.0 (compatible; PrepareStella/{AppVersion}; +{AppWebsite})";
        public static readonly string Line = "===============================================================================================";

        // Global variables
        public static string GameExeGlobal;
        public static string GameDirGlobal;
        public static string ResourcesGlobal;
        public static string ReShadeConfig;
        public static string ReShadeLogFile;


        [STAThread]
        public static async Task Start()
        {
            TaskbarManager.Instance.SetProgressValue(12, 100);

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

            TaskbarManager.Instance.SetProgressValue(26, 100);

            // Read ini file
            Console.WriteLine("\nStarting...");

            // Save AppData path
            File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "stella-appdata.sfn"), AppData);

            int downloadOrUpdateShaders = PrepareIni.ReadInt("PrepareStella", "DownloadOrUpdateShaders", 1);
            int updateReShadeCfg = PrepareIni.ReadInt("PrepareStella", "UpdateReShadeConfig", 1);
            int updateFpsUnlockerCfg = PrepareIni.ReadInt("PrepareStella", "UpdateFpsUnlockerConfig", 1);
            int delReShadeCache = PrepareIni.ReadInt("PrepareStella", "DeleteReShadeCache", 1);
            int installWtUpdate = PrepareIni.ReadInt("PrepareStella", "InstOrUpdWT", 1);
            int newShortcuts = PrepareIni.ReadInt("PrepareStella", "NewShortcutsOnDesktop", 1);
            int newIntShortcuts = PrepareIni.ReadInt("PrepareStella", "InternetShortcutsInStartMenu", 1);

            // Download and prepare ReShade config
            if (updateReShadeCfg == 1) await UpdateReShadeCfg.Run();
            TaskbarManager.Instance.SetProgressValue(51, 100);

            // Download shaders, presets and addons (Stella resources)
            if (downloadOrUpdateShaders == 1) await DownloadUpdateResources.Run();
            TaskbarManager.Instance.SetProgressValue(68, 100);

            // Download FPS Unlocker config
            if (updateFpsUnlockerCfg == 1) await DownloadFpsUnlockerCfg.Run();
            TaskbarManager.Instance.SetProgressValue(76, 100);

            // Delete ReShade cache
            if (delReShadeCache == 1) await DeleteReShadeCache.Run();
            TaskbarManager.Instance.SetProgressValue(82, 100);

            // Windows Terminal installation
            if (installWtUpdate == 1) await TerminalInstallation.Run();
            TaskbarManager.Instance.SetProgressValue(87, 100);

            // Create or update Desktop icon
            if (newShortcuts == 1) await DesktopIcon.Run();
            TaskbarManager.Instance.SetProgressValue(39, 100);

            // Create new Internet shortcuts in menu start
            if (newIntShortcuts == 1) await InternetShortcuts.Run();
            TaskbarManager.Instance.SetProgressValue(45, 100);


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

            TaskbarManager.Instance.SetProgressValue(100, 100);

            // Run Genshin Stella Mod
            Console.WriteLine(@"Launching Genshin Stella Mod...");
            Process.Start(Path.Combine(AppPath, "Genshin Stella Mod.exe"));

            // Close app
            Console.WriteLine($"\n{Line}\n");

            const int seconds = 20;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($@"» The program will close in {seconds} seconds.");
            for (int i = seconds; i >= 1; i--) Thread.Sleep(1000);

            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Indeterminate);
        }
    }
}
