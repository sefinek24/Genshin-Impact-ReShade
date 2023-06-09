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
        private static readonly string ConfiguredSfn = Path.Combine(AppData, "configured.sfn");

        // Dependencies
        public static readonly string VcLibsAppx = Path.Combine("dependencies", "Microsoft.VCLibs.x64.14.00.Desktop.appx");
        public static readonly string WtMsixBundle = Path.Combine("dependencies", "Microsoft.WindowsTerminal_1.17.11461.0_8wekyb3d8bbwe.msixbundle");

        // Other
        public static readonly string UserAgent = $"Mozilla/5.0 (compatible; PrepareStella/{AppVersion}; +{AppWebsite})";
        public static readonly string Line = "===============================================================================================";

        // Global variables
        public static string GameExeGlobal;
        public static string GameDirGlobal;
        public static string ResourcesGlobal;


        [STAThread]
        public static async Task Start()
        {
            TaskbarManager.Instance.SetProgressValue(12, 100);

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n-- Select the correct paths --");


            // Check game path
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(@"» Game path: ");
            Console.ResetColor();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string gamePathSfn = Path.Combine(AppData, "game-path.sfn");
            GamePathSfn = gamePathSfn;

            if (File.Exists(gamePathSfn))
            {
                string gamePathContent = File.ReadAllText(gamePathSfn);

                if (File.Exists(gamePathContent))
                {
                    GameExeGlobal = gamePathContent;
                    GameDirGlobal = Path.GetDirectoryName(gamePathContent);
                    Console.WriteLine(gamePathContent);
                }
            }

            if (GameExeGlobal == null)
            {
                string selectedGameExe = null;

                if (File.Exists(GameGenshinImpact))
                {
                    selectedGameExe = GameGenshinImpact;
                }
                else if (File.Exists(GameYuanShen))
                {
                    selectedGameExe = GameYuanShen;
                }
                else
                {
                    SelectGamePath form = new SelectGamePath(GameExeGlobal ?? $"{GameGenshinImpact}\n{GameYuanShen}") { Icon = Resources.icon };
                    Application.Run(form);
                }

                if (selectedGameExe != null)
                {
                    GameExeGlobal = selectedGameExe;
                    GameDirGlobal = Path.GetDirectoryName(selectedGameExe);
                    File.WriteAllText(GamePathSfn, selectedGameExe);
                    Console.WriteLine(selectedGameExe);
                }
            }

            if (GameExeGlobal == null || !File.Exists(GameExeGlobal))
            {
                string errorMessage = GameExeGlobal != null
                    ? $"File was not found: {GameExeGlobal}"
                    : "Sorry. Full game path was not found.";

                Log.ErrorAndExit(new Exception($"Unknown\n\n{errorMessage}\nPlease delete all Stella Mod files from AppData (%appdata%) folder."), false, false);
            }


            // Check resources
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(@"» Resources: ");
            Console.ResetColor();

            string resourcesPath = Path.Combine(AppData, "resources-path.sfn");
            if (File.Exists(resourcesPath))
            {
                string sfnFileContent = File.ReadAllText(resourcesPath).Trim();
                if (Directory.Exists(sfnFileContent))
                    ResourcesGlobal = sfnFileContent;
                else
                    Application.Run(new SelectShadersPath { Icon = Resources.icon });
            }
            else
            {
                Application.Run(new SelectShadersPath { Icon = Resources.icon });
            }

            if (ResourcesGlobal != null)
                Console.WriteLine(ResourcesGlobal);
            else
                Log.ErrorAndExit(new Exception("Unknown\n\nSorry. Directory with the resources was not found.\nPlease delete all Stella Mod files from AppData (%appdata%) folder."), false, false);


            TaskbarManager.Instance.SetProgressValue(26, 100);


            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n-- Run the final configuration --");
            Console.ResetColor();

            // Read ini file
            Console.WriteLine(@"Starting...");

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
            if (updateReShadeCfg == 1)
            {
                Console.WriteLine(@"Downloading ReShade files...");
                await UpdateReShadeCfg.Run();
                TaskbarManager.Instance.SetProgressValue(51, 100);
            }

            // Download shaders, presets and addons (Stella resources)
            if (downloadOrUpdateShaders == 1)
            {
                Console.WriteLine(@"Checking Stella resources...");
                await DownloadUpdateResources.Run();
                TaskbarManager.Instance.SetProgressValue(68, 100);
            }

            // Download FPS Unlocker config
            if (updateFpsUnlockerCfg == 1)
            {
                Console.WriteLine(@"Downloading FPS Unlocker configuration...");
                await DownloadFpsUnlockerCfg.Run();
                TaskbarManager.Instance.SetProgressValue(76, 100);
            }

            // Delete ReShade cache
            if (delReShadeCache == 1)
            {
                Console.WriteLine(@"Deleting ReShade cache...");
                await DeleteReShadeCache.Run();
                TaskbarManager.Instance.SetProgressValue(82, 100);
            }

            // Windows Terminal installation
            if (installWtUpdate == 1)
            {
                Console.Write(@"Backing up the Windows Terminal configuration file in app data... ");
                await TerminalInstallation.Run();
                TaskbarManager.Instance.SetProgressValue(87, 100);
            }

            // Create or update Desktop icon
            if (newShortcuts == 1)
            {
                Console.WriteLine(@"Creating Desktop shortcut...");
                await DesktopIcon.Run();
                TaskbarManager.Instance.SetProgressValue(39, 100);
            }

            // Create new Internet shortcuts in menu start
            if (newIntShortcuts == 1)
            {
                Console.WriteLine(@"Creating new Internet shortcut...");
                await InternetShortcuts.Run();
                TaskbarManager.Instance.SetProgressValue(45, 100);
            }


            // Create files
            if (!Directory.Exists(AppData)) Directory.CreateDirectory(AppData);
            if (!File.Exists(ConfiguredSfn)) File.Create(ConfiguredSfn);

            TaskbarManager.Instance.SetProgressValue(100, 100);


            // Reboot is required?
            if (Cmd.RebootNeeded)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(@"» Restart your computer now? This is required. [Yes/no]: ");
                Console.ResetColor();

                string rebootPc = Console.ReadLine();
                if (Regex.Match(rebootPc ?? string.Empty, "(?:y)", RegexOptions.IgnoreCase | RegexOptions.Singleline).Success)
                {
                    await Cmd.CliWrap("shutdown",
                        $"/r /t 30 /c \"{AppName} - scheduled reboot.\n\nThank you for installing. If you need help, add me on Discord Sefinek#2714.\n\nGood luck and have fun!\"", null);

                    Console.WriteLine(@"Your computer will restart in 30 seconds. Save your work!");
                    Log.Output("PC reboot was scheduled.");
                }
            }
            else
            {
                // Run Genshin Stella Mod
                Console.WriteLine(@"Launching Genshin Stella Mod...");
                Process.Start(Path.Combine(AppPath, "Genshin Stella Mod.exe"));
            }


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
