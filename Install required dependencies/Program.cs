using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Storage;
using IWshRuntimeLibrary;
using Prepare_mod.Forms;
using Prepare_mod.Properties;
using Prepare_mod.Scripts;
using PrepareMod.Scripts;
using File = System.IO.File;

namespace Prepare_mod
{
    internal static class Program
    {
        // App
        private static readonly string AppPath = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
        private static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static string AppData = GetAppData();

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
        private static readonly string WtWin10Setup = Path.Combine("dependencies", "WindowsTerminal_Win10.msixbundle");
        private static readonly string WtWin11Setup = Path.Combine("dependencies", "WindowsTerminal_Win11.msixbundle");

        // Other
        public static readonly string UserAgent = $"Mozilla/5.0 (compatible; GenshinStellaSetup/{AppVersion}; +{AppWebsite})";
        private static readonly string Line = "===============================================================================================";

        // Global variables
        public static string GameExeGlobal;
        public static string GameDirGlobal;
        public static string ResourcesGlobal;
        public static string ReShadeConfig;
        public static string ReShadeLogFile;


        private static string GetAppData()
        {
            try
            {
                return Path.Combine(ApplicationData.Current?.LocalFolder?.Path);
            }
            catch (InvalidOperationException)
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Genshin Stella Mod");
            }
        }

        private static async Task Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"                           Genshin Impact Stella Mod 2023 - Early access");
            Console.WriteLine($"                                        Version: v{AppVersion}\n");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(@"» Author  : Sefinek [Country: Poland]");
            Console.WriteLine(@"» Website : " + AppWebsite);
            Console.WriteLine(@"» Discord : " + DiscordUrl);
            Console.ResetColor();
            Console.WriteLine(Line);

            Console.Title = $@"{AppName} • v{AppVersion}";


            // Game path
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(@"» Game path: ");
            Console.ResetColor();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (File.Exists(SelectGamePath.SfnFilePath))
            {
                string fileWithGamePath = File.ReadAllText(SelectGamePath.SfnFilePath).Trim();
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
                File.WriteAllText(SelectGamePath.SfnFilePath, GameDirGlobal);
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
            int newShortcuts = PrepareIni.ReadInt("PrepareStella", "NewShortcutsOnDesktop", 0);
            int newIntShortcuts = PrepareIni.ReadInt("PrepareStella", "InternetShortcutsInStartMenu", 1);
            int downloadOrUpdateShaders = PrepareIni.ReadInt("PrepareStella", "DownloadOrUpdateShaders", 1);
            int updateReShadeCfg = PrepareIni.ReadInt("PrepareStella", "UpdateReShadeConfig", 1);
            int updateFpsUnlockerCfg = PrepareIni.ReadInt("PrepareStella", "UpdateFpsUnlockerConfig", 1);
            int delReShadeCache = PrepareIni.ReadInt("PrepareStella", "DeleteReShadeCache", 1);
            int installWtUpdate = PrepareIni.ReadInt("PrepareStella", "InstOrUpdWT", 1);


            // Create or update Desktop icon
            if (newShortcuts == 1)
            {
                Console.WriteLine(@"Creating Desktop shortcut...");

                try
                {
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                    string shortcutPath = Path.Combine(desktopPath, "Stella Mod Launcher.lnk");

                    WshShell shell = new WshShell();
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                    shortcut.Description = "Run official launcher for Genshin Impact Mod made by Sefinek.";
                    shortcut.IconLocation = Path.Combine(AppPath, "icons", "52x52.ico");
                    shortcut.WorkingDirectory = AppPath;
                    shortcut.TargetPath = Path.Combine(AppPath, "Genshin Stella Mod.exe");

                    shortcut.Save();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }


            // Create new Internet shortcuts in menu start
            if (newIntShortcuts == 1)
            {
                Console.WriteLine(@"Creating new Internet shortcut...");

                string appStartMenuPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs", "Genshin Stella Mod");
                Directory.CreateDirectory(appStartMenuPath);

                try
                {
                    // Create shortcut in Start Menu
                    WshShell shell = new WshShell();
                    string shortcutLocation = Path.Combine(appStartMenuPath, "Genshin Stella Mod.lnk");
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);
                    shortcut.Description = "Run official Genshin Stella Mod Launcher made by Sefinek.";
                    shortcut.IconLocation = Path.Combine(AppPath, "icons", "52x52.ico");
                    shortcut.WorkingDirectory = AppPath;
                    shortcut.TargetPath = Path.Combine(AppPath, "Genshin Stella Mod.exe");
                    shortcut.Save();

                    // Create Internet shortcuts
                    Dictionary<string, string> urls = new Dictionary<string, string>
                    {
                        { "Official website", "https://genshin.sefinek.net" },
                        { "Donate", "https://sefinek.net/support-me" },
                        { "Gallery", "https://sefinek.net/genshin-impact-reshade/gallery?page=1" },
                        { "Support", "https://sefinek.net/genshin-impact-reshade/support" },
                        { "Leave feedback", "https://sefinek.net/genshin-impact-reshade/feedback" }
                    };
                    foreach (KeyValuePair<string, string> kvp in urls)
                    {
                        string url = Path.Combine(appStartMenuPath, $"{kvp.Key} - Genshin Stella Mod.url");
                        using (StreamWriter writer = new StreamWriter(url))
                        {
                            await writer.WriteLineAsync($"[InternetShortcut]\nURL={kvp.Value}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }


            // Update ReShade config
            if (updateReShadeCfg == 1)
            {
                Console.WriteLine(@"Updating ReShade config... ");

                if (Directory.Exists(GameDirGlobal))
                {
                    if (File.Exists(ReShadeConfig)) File.Delete(ReShadeConfig);

                    if (File.Exists(ReShadeLogFile)) File.Delete(ReShadeLogFile);

                    await ReShade.DownloadFiles();
                }
                else
                {
                    Console.WriteLine(@"You must configure some settings manually");
                }
            }


            // Download or update shaders, presets
            if (downloadOrUpdateShaders == 1)
            {
                Console.WriteLine(@"Checking Stella resources...");

                string resourcesPath = Path.Combine(AppData, "resources-path.sfn");
                if (File.Exists(resourcesPath))
                {
                    string fileWithGamePath = File.ReadAllText(resourcesPath).Trim();
                    if (Directory.Exists(fileWithGamePath))
                    {
                        ResourcesGlobal = fileWithGamePath;
                    }
                    else
                    {
                        Console.WriteLine($@"Not found in: {fileWithGamePath}");
                        Application.Run(new SelectShadersPath { Icon = Resources.icon });
                    }
                }
                else
                {
                    Application.Run(new SelectShadersPath { Icon = Resources.icon });
                }

                if (!Directory.Exists(ResourcesGlobal) && ResourcesGlobal != null)
                {
                    Directory.CreateDirectory(ResourcesGlobal);

                    Console.WriteLine($@"Created folder: {ResourcesGlobal}");
                }
                else
                {
                    Console.WriteLine($@"Found: {ResourcesGlobal}");
                }


                Console.WriteLine(@"Downloading presets and shaders...");

                string zipPath = $@"{ResourcesGlobal}\Backup.zip";
                WebClient webClient = new WebClient();
                webClient.Headers.Add("user-agent", UserAgent);
                await webClient.DownloadFileTaskAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/reshade/zip/resources.zip", zipPath);


                Console.WriteLine(@"Unpacking resources...");
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string fullPath = Path.Combine(ResourcesGlobal, entry.FullName);

                        if (entry.Name == "")
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                            continue;
                        }

                        await Task.Run(() => entry.ExtractToFile(fullPath, true));
                    }
                }


                Console.WriteLine(@"Configuring ReShade...");

                string cache = Path.Combine(ResourcesGlobal, "Cache");

                string reShadeIniFile = File.ReadAllText(ReShadeConfig);
                string reShadeData = reShadeIniFile?
                    .Replace("{addon.path}", Path.Combine(ResourcesGlobal, "Addons"))
                    .Replace("{general.effects}", Path.Combine(ResourcesGlobal, "Shaders", "Effects"))
                    .Replace("{general.cache}", cache)
                    .Replace("{general.preset}", Path.Combine(ResourcesGlobal, "Presets", "3. Preset by Sefinek - Medium settings [Default].ini"))
                    .Replace("{general.textures}", Path.Combine(ResourcesGlobal, "Shaders", "Textures"))
                    .Replace("{screenshot.path}", Path.Combine(ResourcesGlobal, "Screenshots"))
                    .Replace("{screenshot.sound}", Path.Combine(ResourcesGlobal, "data", "sounds", "screenshot.wav"));

                File.WriteAllText(ReShadeConfig, reShadeData);

                if (!Directory.Exists(cache))
                {
                    Console.WriteLine(@"Creating cache folder...");
                    Directory.CreateDirectory(cache);
                }
            }


            // Update FPS Unlocker config
            if (updateFpsUnlockerCfg == 1)
            {
                Console.WriteLine(@"Updating FPS Unlocker config...");

                try
                {
                    string unlockerFolderPath = Path.Combine(AppPath, "data", "unlocker");
                    if (!Directory.Exists(unlockerFolderPath))
                        Directory.CreateDirectory(unlockerFolderPath);

                    string fpsUnlockerConfig;
                    using (WebClient client = new WebClient())
                    {
                        client.Headers.Add("user-agent", UserAgent);
                        fpsUnlockerConfig = await client.DownloadStringTaskAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/unlocker.config.json");
                    }

                    string fpsUnlockerConfigPath = Path.Combine(unlockerFolderPath, "unlocker.config.json");

                    string gameExePath = GameExeGlobal?.Replace("\\", "\\\\");
                    string fpsUnlockerConfigContent = fpsUnlockerConfig.Replace("{GamePath}", gameExePath ?? string.Empty);

                    File.WriteAllText(fpsUnlockerConfigPath, fpsUnlockerConfigContent);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }


            // Delete ReShade cache
            if (delReShadeCache == 1)
            {
                Console.WriteLine(@"Deleting ReShade cache...");

                int numFilesDeleted = 0;
                long spaceSaved = 0;
                DirectoryInfo cacheDir = new DirectoryInfo(Path.Combine(ResourcesGlobal, "Cache"));
                if (cacheDir.Exists)
                    foreach (FileInfo file in cacheDir.EnumerateFiles())
                    {
                        spaceSaved += file.Length;
                        file.Delete();
                        numFilesDeleted++;
                    }

                Console.WriteLine(spaceSaved > 1000 ? $@"Deleted {numFilesDeleted} cache files and saved {spaceSaved / 1000000} MB." : $@"Deleted {numFilesDeleted} cache files and saved {spaceSaved} KB.");
            }


            // Windows Terminal installation
            if (installWtUpdate == 1)
            {
                // Make backup
                Console.Write(@"Backing up the Windows Terminal configuration file in app data... ");

                string wtAppDataLocal = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Windows Terminal");
                if (!Directory.Exists(wtAppDataLocal))
                {
                    Console.WriteLine(@"Config folder not found");
                }
                else
                {
                    string wtSettingsJson = Path.Combine(wtAppDataLocal, "settings.json");
                    string wtStateJson = Path.Combine(wtAppDataLocal, "state.json");
                    string readMeTxt = Path.Combine(wtAppDataLocal, "README.txt");
                    Log.Output($"Files and directories of backup.\n» wtAppDataLocal: {wtAppDataLocal}\n» wtSettingsJson: {wtSettingsJson}\n» wtStateJson: {wtStateJson}\n» readMeTxt: {readMeTxt}");

                    string localShortcut = Path.Combine(wtAppDataLocal, "WT Backup Folder.lnk");
                    if (File.Exists(localShortcut)) File.Delete(localShortcut);

                    string stellaWtBkp = Path.Combine(AppData, "Windows Terminal");
                    try
                    {
                        using (StreamWriter sw = File.CreateText(readMeTxt))
                        {
                            await sw.WriteAsync(
                                $"⠀   ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀\n⠀  ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿\n⠀   ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                           Genshin Impact Stella Mod Pack 2023\n   ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿                                     Made by Sefinek\n⠀   ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿\n    ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟\n⠀   ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀\n ⠀   ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀\n  ⠀ ⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆\n    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆\n   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇\n  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟\n   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟\n=========================================================================================\n» Windows Terminal application configuration backup files from {DateTime.Now}.");
                        }

                        string zipFile = Path.Combine(AppData, "Windows Terminal", $"wt-config.backup-{DateTime.Now:HHmm_dd.MM.yyyy}.zip");
                        if (!Directory.Exists(stellaWtBkp)) Directory.CreateDirectory(stellaWtBkp);
                        Zip.Create(wtAppDataLocal, zipFile);

                        Log.Output($"The Windows Terminal application configuration files has been backed up.\n» Source: {wtAppDataLocal}\n» Backup: {zipFile}");
                    }
                    catch (Exception e)
                    {
                        Log.ThrowError(e, false);
                    }

                    if (File.Exists(readMeTxt)) File.Delete(readMeTxt);

                    WshShell wshShell = new WshShell();
                    IWshShortcut shBkpWt = (IWshShortcut)wshShell.CreateShortcut(localShortcut);
                    shBkpWt.Description = "View the backup copy of Windows Terminal created by Genshin Stella Mod.";
                    shBkpWt.TargetPath = stellaWtBkp;
                    shBkpWt.Save();

                    Log.Output($@"Created: {wtAppDataLocal}\Stella WT Backup.lnk");
                    Console.WriteLine(@"Saved in Stella AppData.");
                }

                // Installing
                Console.WriteLine(@"Installing latest Windows Terminal...");

                if (!File.Exists(WtWin10Setup) || !File.Exists(WtWin11Setup))
                    Log.ErrorAndExit(new Exception($"I can't find a required file.\n\n{WtWin10Setup} or {WtWin11Setup}\n\nPlease unpack all files from zip archive and try again."), false, false);

                Process[] dllHostName = Process.GetProcessesByName("dllhost");
                if (dllHostName.Length != 0) await Cmd.CliWrap("taskkill", "/F /IM dllhost.exe", null);
                Process[] wtName = Process.GetProcessesByName("WindowsTerminal");
                if (wtName.Length != 0) await Cmd.CliWrap("taskkill", "/F /IM WindowsTerminal.exe", null);

                if (Environment.OSVersion.Version.Build >= 22000)
                {
                    await Cmd.CliWrap("powershell", $"Add-AppxPackage -Path \"{WtWin11Setup}\"", AppPath);
                    Log.Output($"Installed WT for Win 11: {WtWin11Setup}");
                }
                else
                {
                    await Cmd.CliWrap("powershell", $"Add-AppxPackage -Path \"{WtWin10Setup}\"", AppPath);
                    Log.Output($"Installed WT for Win 10: {WtWin10Setup}");
                }

                // Check installed WT
                Console.WriteLine("Checking installed software...");
                string wtProgramFiles = Utils.GetProgramFiles();
                if (string.IsNullOrEmpty(wtProgramFiles))
                {
                    Log.ErrorAndExit(new Exception($"Windows Terminal directory was not found in: {WindowsApps}"), false, false);
                }
                else
                {
                    Log.Output($"Windows Terminal has been successfully installed in {wtProgramFiles}");

                    string wtAppData2 = Utils.GetAppData();
                    if (string.IsNullOrEmpty(wtAppData2))
                        Log.ErrorAndExit(new Exception("Fatal error. Code: 3781780149"), false, true);
                }
            }

            // Create file
            if (!Directory.Exists(AppData)) Directory.CreateDirectory(AppData);
            if (!File.Exists(InstalledViaSetup)) File.Create(InstalledViaSetup);

            // Reboot is required?
            if (Cmd.RebootNeeded)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("» Restart your computer now? This is required! [Yes/no]: ");
                Console.ResetColor();

                string rebootPc = Console.ReadLine();
                if (Regex.Match(rebootPc ?? string.Empty, "(?:y)", RegexOptions.IgnoreCase | RegexOptions.Singleline).Success)
                {
                    await Cmd.CliWrap("shutdown",
                        $"/r /t 30 /c \"{AppName} - scheduled reboot, version {AppVersion}.\n\nThank you for installing. If you need help, add me on Discord Sefinek#0001.\n\nGood luck and have fun!\"",
                        null);

                    Console.WriteLine(@"Your computer will restart in 30 seconds. Save your work!");
                    Log.Output("PC reboot was scheduled.");
                }
            }

            // Run Genshin Stella Mod
            Console.WriteLine(@"Launching Genshin Stella Mod...");
            Process.Start(Path.Combine(AppPath, "Genshin Stella Mod.exe"));

            // Close app
            Console.WriteLine($"\n{Line}\n");
            Console.ForegroundColor = ConsoleColor.Green;
            const int seconds = 20;
            Console.WriteLine($@"» The program will close in {seconds} seconds.");
            for (int i = seconds; i >= 1; i--) Thread.Sleep(1000);
        }
    }
}
