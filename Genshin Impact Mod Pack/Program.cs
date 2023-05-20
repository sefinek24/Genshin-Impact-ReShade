using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Windows.Storage;
using Genshin_Stella_Mod.Forms;
using Genshin_Stella_Mod.Forms.Errors;
using Genshin_Stella_Mod.Forms.Other;
using Genshin_Stella_Mod.Properties;
using Genshin_Stella_Mod.Scripts;

namespace Genshin_Stella_Mod
{
    internal static class Program
    {
        // Files
        public static string AppData = GetAppData();
        private static string _appIsConfigured;
        public static readonly string AppPath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string PrepareLauncher = Path.Combine(AppPath, "First app launch.exe");
        public static readonly string ReShadePath = Path.Combine(AppPath, "data", "reshade", "ReShade64.dll");
        public static readonly string InjectorPath = Path.Combine(AppPath, "data", "reshade", "inject64.exe");
        public static readonly string FpsUnlockerExePath = Path.Combine(AppPath, "data", "unlocker", "unlockfps_clr.exe");
        public static readonly string FpsUnlockerCfgPath = Path.Combine(AppPath, "data", "unlocker", "unlocker.config.json");
        private static readonly string PatronsDir = Path.Combine(AppPath, "data", "presets", "3. Only for patrons");
        private static readonly string TierActivated = Path.Combine(AppPath, "tier-activated.sfn");

        // App
        public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
        public static readonly string AppVersion = Application.ProductVersion;
        private static readonly string AppWebsiteSub = "https://genshin.sefinek.net";
        public static readonly string AppWebsiteFull = "https://sefinek.net/genshin-impact-reshade";

        // Web
        public static readonly string UserAgent = $"Mozilla/5.0 (compatible; StellaLauncher/{AppVersion}; +{AppWebsiteSub})";

        // Config
        public static IniFile Settings;

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


        [STAThread]
        private static void Main()
        {
            _appIsConfigured = Path.Combine(AppData, "configured.sfn");
            Settings = new IniFile(Path.Combine(AppData, "settings.ini"));


            Log.Output(
                "A request to start the program has been received.\n" +
                $"Debugger is attached: {Debugger.IsAttached}\n" +
                $"CPU Serial Number: {Os.CpuId}\n" +
                $"App dir: {AppPath}\n" +
                $"App data: {AppData}\n" +
                $"App is configured: {_appIsConfigured}\n" +
                $"FPS Unlocker path: {FpsUnlockerCfgPath}\n" +
                $"Patrons dir: {PatronsDir}"
            );


            //if (!Debugger.IsAttached && Environment.CurrentDirectory != Folder)
            //{
            //    new WrongDirectory { Icon = Resources.icon_52x52 }.ShowDialog();
            //    Environment.Exit(994327186);
            //}

            if (Process.GetProcessesByName(AppName).Length > 1)
            {
                MessageBox.Show(
                    $"Sorry, one instance is currently open.\n\nQuit the process with name {Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly()?.Location)} and try again.", AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                Log.Output("One instance is currently open.");
                Environment.Exit(998765341);
            }

            if (!File.Exists(_appIsConfigured))
            {
                if (!File.Exists(PrepareLauncher))
                {
                    MessageBox.Show("Required file 'First app launch.exe' was not found. Please reinstall this app or join our Discord server for help.", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(997890421);
                }

                Process.Start(PrepareLauncher);
                Environment.Exit(997890421);
            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            if (RegionInfo.CurrentRegion.Name == "RU")
            {
                new WrongCountry { Icon = Resources.icon_52x52 }.ShowDialog();
                Environment.Exit(999222999);
            }

            try
            {
                if (!File.Exists(TierActivated) && Directory.Exists(PatronsDir)) Directory.Delete(PatronsDir, true);

                int launchCount = Settings.ReadInt("Launcher", "LaunchCount", 0);
                launchCount++;
                Settings.WriteInt("Launcher", "LaunchCount", launchCount);
                Settings.Save();

                switch (launchCount)
                {
                    case 3:
                    case 10:
                    case 18:
                        DialogResult discordResult = MessageBox.Show("Do you want to join our Discord server?", AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        Log.Output($"Question (MessageBox): Do you want to join our Discord server? Selected: {discordResult}");
                        if (discordResult == DialogResult.Yes) Utils.OpenUrl(Discord.Invitation);
                        break;

                    case 6:
                    case 20:
                    case 42:
                    case 63:
                        if (!File.Exists(TierActivated)) Application.Run(new SupportMe { Icon = Resources.icon_52x52 });
                        return;

                    case 26:
                    case 38:
                    case 60:
                    case 100:
                    case 200:
                        DialogResult logFilesResult = MessageBox.Show("Do you want to send us anonymous log files?", AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        Log.Output($"Question (MessageBox): Do you want to send log files? Selected: {logFilesResult}");
                        if (logFilesResult == DialogResult.Yes)
                        {
                            Telemetry.SendLogFiles();

                            DialogResult showFilesResult = MessageBox.Show(
                                "If you wish to send logs to the developer, please send them to me on Discord: Sefinek#2714. API communication is not yet available for the Stella Launcher.\n\nDo you want to see these files?", AppName,
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (showFilesResult == DialogResult.Yes)
                            {
                                Process.Start(Log.Folder);
                                Log.Output($"Opened: {Log.Folder}");
                            }
                        }

                        break;
                }


                Application.Run(new Default { Icon = Resources.icon_52x52 });
            }
            catch (Exception e)
            {
                Log.ErrorAndExit(e);
            }
        }
    }
}
