using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Taskbar;
using PrepareStella.Forms;
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

        // Links
        public static readonly string AppWebsite = "https://genshin.sefinek.net";
        public static readonly string DiscordUrl = "https://discord.gg/SVcbaRc7gH";

        // Files and folders
        private static readonly IniFile PrepareIni = new IniFile(Path.Combine(AppData, "prepare-stella.ini"));
        public static readonly string ProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        private static readonly string GameGenshinImpact = Path.Combine(ProgramFiles, "Genshin Impact", "Genshin Impact game", "GenshinImpact.exe");
        private static readonly string GameYuanShen = Path.Combine(ProgramFiles, "Genshin Impact", "Genshin Impact game", "YuanShen.exe");
        public static readonly string WindowsApps = Path.Combine(ProgramFiles, "WindowsApps");
        private static readonly string ConfiguredSfn = Path.Combine(AppData, "configured.sfn");

        // Dependencies
        public static readonly string VcLibsAppx = Path.Combine("dependencies", "Microsoft.VCLibs.x64.14.00.Desktop.appx");
        public static readonly string WtMsixBundle = Path.Combine("dependencies", "Microsoft.WindowsTerminal_1.17.11461.0_8wekyb3d8bbwe.msixbundle");

        // Other
        public static readonly string UserAgent = $"Mozilla/5.0 (compatible; PrepareStella/{AppVersion}; +{AppWebsite})";
        public static readonly string Line = "===============================================================================================";
        private static readonly Icon Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

        // Global variables
        public static string SavedGamePath;
        public static string ResourcesGlobal;

        // Registry
        public static readonly string RegistryPath = @"SOFTWARE\Stella Mod Launcher";

        [STAThread]
        public static async Task Start()
        {
            TaskbarManager.Instance.SetProgressValue(12, 100);

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n-- Select the correct localizations --");


            // Check game path
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(@"» Game path: ");
            Console.ResetColor();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            // Load the game path from the registry
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath))
            {
                if (key != null)
                    SavedGamePath = (string)key.GetValue("GamePath");
            }

            // Try to find the game in the default localizations
            if (string.IsNullOrEmpty(SavedGamePath))
            {
                if (File.Exists(GameGenshinImpact))
                    SavedGamePath = GameGenshinImpact;
                else if (File.Exists(GameYuanShen))
                    SavedGamePath = GameYuanShen;
                else
                    new GamePath($"{GameGenshinImpact}\n{GameYuanShen}") { Icon = Icon }.ShowDialog();
            }

            // Check if the path is valid
            if (!File.Exists(SavedGamePath))
                new GamePath(SavedGamePath) { Icon = Icon }.ShowDialog();

            // Check if the variable is empty or if the path is still not valid
            if (string.IsNullOrEmpty(SavedGamePath) || !File.Exists(SavedGamePath))
            {
                string errorMessage = SavedGamePath != null ? $"File was not found: {SavedGamePath}" : "Full game path was not found";
                Log.ErrorAndExit(new Exception($"{errorMessage}\n\nYou must provide the specific location where the game is installed.\nThis program will not modify ANY game files."), false, false);
            }

            // If the variable is not empty, save the data
            if (!string.IsNullOrEmpty(SavedGamePath))
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryPath))
                {
                    key?.SetValue("GamePath", SavedGamePath);
                }

                Console.WriteLine(SavedGamePath);
            }


            // Check resources
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(@"» Resources: ");
            Console.ResetColor();

            // Prepare WinForm
            Resources selectResPath = new Resources { Icon = Icon };


            // Get ResourcesPath from the registry
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath))
            {
                if (key != null) ResourcesGlobal = (string)key.GetValue("ResourcesPath");
            }

            // Path is not valid?
            if (string.IsNullOrEmpty(SavedGamePath) || !Directory.Exists(ResourcesGlobal)) selectResPath.ShowDialog();

            // Path is now valid?
            if (!string.IsNullOrEmpty(SavedGamePath) || Directory.Exists(ResourcesGlobal))
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryPath))
                {
                    key?.SetValue("ResourcesPath", ResourcesGlobal);
                }

                Console.WriteLine(ResourcesGlobal);
            }
            else
            {
                Log.ErrorAndExit(new Exception("Unknown\n\nSorry. Directory with the resources was not found.\nIn the resources directory, files such as your shaders, presets, screenshots, and custom mods are stored."), false, false);
            }

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


            // Download shaders, presets and addons (Stella resources)
            if (downloadOrUpdateShaders == 1)
            {
                Console.WriteLine(@"Checking Stella resources...");
                await DownloadUpdateResources.Run();
                TaskbarManager.Instance.SetProgressValue(39, 100);
            }

            // Download and prepare ReShade config
            if (updateReShadeCfg == 1)
            {
                Console.WriteLine(@"Downloading ReShade files...");
                await UpdateReShadeCfg.Run();
                TaskbarManager.Instance.SetProgressValue(46, 100);
            }

            // Delete ReShade cache
            if (delReShadeCache == 1)
            {
                Console.WriteLine(@"Deleting ReShade cache...");
                await DeleteReShadeCache.Run();
                TaskbarManager.Instance.SetProgressValue(57, 100);
            }

            // Download FPS Unlocker config
            if (updateFpsUnlockerCfg == 1)
            {
                Console.WriteLine(@"Downloading FPS Unlocker configuration...");
                await DownloadFpsUnlockerCfg.Run();
                TaskbarManager.Instance.SetProgressValue(68, 100);
            }

            // Windows Terminal installation
            if (installWtUpdate == 1)
            {
                Console.Write(@"Backing up the Windows Terminal configuration file in app data... ");
                await TerminalInstallation.Run();
                TaskbarManager.Instance.SetProgressValue(77, 100);
            }

            // Create or update Desktop icon
            if (newShortcuts == 1)
            {
                Console.WriteLine(@"Creating Desktop shortcut...");
                await DesktopIcon.Run();
                TaskbarManager.Instance.SetProgressValue(89, 100);
            }

            // Create new Internet shortcuts in menu start
            if (newIntShortcuts == 1)
            {
                Console.WriteLine(@"Creating new Internet shortcut...");
                await InternetShortcuts.Run();
                TaskbarManager.Instance.SetProgressValue(96, 100);
            }


            // Create files
            if (!Directory.Exists(AppData)) Directory.CreateDirectory(AppData);
            if (!File.Exists(ConfiguredSfn)) File.Create(ConfiguredSfn);


            // Registry
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryPath))
            {
                key?.SetValue("AppIsConfigured", 1);
                key?.SetValue("ConfiguredDate", DateTime.Now);
            }


            // Final
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
                        $"/r /t 30 /c \"{AppName} - scheduled reboot.\n\nThank you for installing. If you need help, add me on Discord: sefinek\n\nGood luck and have fun!\"", null);

                    Console.WriteLine(@"Your computer will restart in 30 seconds. Save your work!");
                    Log.Output("PC reboot was scheduled.");
                }
            }
            else
            {
                // Run Genshin Stella Mod
                string stellaLauncher = Path.Combine(AppPath, "Stella Mod Launcher.exe");

                Console.WriteLine($@"Launching {Path.GetFileName(stellaLauncher)}...");
                _ = Cmd.CliWrap(stellaLauncher, null, null);
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
