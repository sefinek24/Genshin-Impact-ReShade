using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using StellaLauncher.Forms;
using StellaLauncher.Forms.Errors;
using StellaLauncher.Forms.Other;
using StellaLauncher.Properties;
using StellaLauncher.Scripts;

namespace StellaLauncher
{
    internal static class Program
    {
        // App
        public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
        public static readonly string AppVersion = Application.ProductVersion;
        public static readonly string AppWebsiteSub = "https://genshin.sefinek.net";
        public static readonly string AppWebsiteFull = "https://sefinek.net/genshin-impact-reshade";

        // Files and folders
        public static string AppData = Utils.GetAppData();
        private static string _appIsConfigured;
        public static readonly string AppPath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string PrepareLauncher = Path.Combine(AppPath, "First app launch.exe");
        public static readonly string ReShadePath = Path.Combine(AppPath, "data", "reshade", "ReShade64.dll");
        public static readonly string InjectorPath = Path.Combine(AppPath, "data", "reshade", "inject64.exe");
        public static readonly string FpsUnlockerExePath = Path.Combine(AppPath, "data", "unlocker", "unlockfps_clr.exe");
        public static readonly string FpsUnlockerCfgPath = Path.Combine(AppPath, "data", "unlocker", "unlocker.config.json");
        private static readonly string PatronsDir = Path.Combine(AppPath, "data", "presets", "3. Only for patrons");
        private static readonly string TierActivated = Path.Combine(AppPath, "tier-activated.sfn");

        // Web
        public static readonly string UserAgent = $"Mozilla/5.0 (compatible; StellaLauncher/{AppVersion}; +{AppWebsiteSub})";

        // Config
        public static IniFile Settings;

        // Registry
        public static string RegistryPath = @"SOFTWARE\Stella Mod Launcher";

        [STAThread]
        private static void Main()
        {
            _appIsConfigured = Path.Combine(AppData, "configured.sfn");
            Settings = new IniFile(Path.Combine(AppData, "settings.ini"));

            int selected = Settings.ReadInt("Launcher", "LanguageID", 0);
            switch (selected)
            {
                case 0:
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                    break;
                case 1:
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("pl");
                    break;
                default:
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
                    break;
            }

            Log.Output(string.Format(
                Resources.Program_ARequestToStartTheProgramHasBeenReceived,
                Debugger.IsAttached,
                Os.CpuId,
                AppPath,
                AppData,
                _appIsConfigured,
                FpsUnlockerCfgPath,
                PatronsDir
            ));


            if (Process.GetProcessesByName(AppName).Length > 1)
            {
                MessageBox.Show(string.Format(Resources.Program_SorryOneInstanceIsCurrentlyOpen_, Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly()?.Location)), AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                Log.Output(Resources.Program_OneInstanceIsCurrentlyOpen);
                Environment.Exit(998765341);
            }

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath))
            {
                int value = (int)(key?.GetValue("AppIsConfigured") ?? 0);
                if (value == 0)
                {
                    _ = Cmd.CliWrap(PrepareLauncher, null, AppPath, true, false);
                    Environment.Exit(997890421);
                }
            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            if (RegionInfo.CurrentRegion.Name == "RU")
            {
                new WrongCountry { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) }.ShowDialog();
                Environment.Exit(999222999);
            }

            try
            {
                if (!File.Exists(TierActivated) && Directory.Exists(PatronsDir)) Directory.Delete(PatronsDir, true);

                RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryPath);
                int launchCount = (int)key?.GetValue("LaunchCount", 0);
                launchCount++;
                key.SetValue("LaunchCount", launchCount);

                switch (launchCount)
                {
                    case 5:
                    case 20:
                    case 30:
                        DialogResult discordResult = MessageBox.Show(Resources.Program_DoYouWantToJoinOurDiscord, AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        Log.Output(string.Format(Resources.Program_QuestionMessageBox_DoYouWantToJoinOurDiscord_, discordResult));
                        if (discordResult == DialogResult.Yes) Utils.OpenUrl(Discord.Invitation);
                        break;

                    case 2:
                    case 12:
                    case 40:
                        DialogResult feedbackResult = MessageBox.Show(Resources.Program_WouldYouShareOpinionAboutStellaMod, AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        Log.Output(string.Format(Resources.Program_QuestionMessageBox_WouldYouShareOpinionAboutStellaMod, feedbackResult));
                        if (feedbackResult == DialogResult.Yes) Utils.OpenUrl("https://www.trustpilot.com/review/genshin.sefinek.net");
                        break;

                    case 3:
                    case 10:
                    case 25:
                    case 35:
                    case 45:
                        if (!File.Exists(TierActivated)) Application.Run(new SupportMe { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) });
                        return;

                    case 28:
                    case 70:
                    case 100:
                    case 200:
                    case 300:
                        DialogResult logFilesResult = MessageBox.Show(Resources.Program_DoYouWantToSendUsanonymousLogFiles, AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        Log.Output(string.Format(Resources.Program_QuestionMessageBox_DoYouWantToSendUsanonymousLogFiles_, logFilesResult));
                        if (logFilesResult == DialogResult.Yes)
                        {
                            Telemetry.SendLogFiles();

                            DialogResult showFilesResult = MessageBox.Show(Resources.Program_IfYouWishToSendLogsToTheDeveloperPleaseSendThemToMeOnDiscord, AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (showFilesResult == DialogResult.Yes)
                            {
                                Process.Start(Log.Folder);
                                Log.Output($"Opened: {Log.Folder}");
                            }
                        }

                        break;
                }


                Application.Run(new Default { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) });
            }
            catch (Exception e)
            {
                Log.ErrorAndExit(e);
            }
        }
    }
}
