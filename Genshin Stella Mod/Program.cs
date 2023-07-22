using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using StellaLauncher.Forms;
using StellaLauncher.Forms.Errors;
using StellaLauncher.Properties;
using StellaLauncher.Scripts;

namespace StellaLauncher
{
    internal static class Program
    {
        // App
        public static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static readonly string AppName = $"{Assembly.GetExecutingAssembly().GetName().Name} • v{AppVersion}";
        private static readonly string AppWebsiteSub = "https://genshin.sefinek.net";
        public static readonly string AppWebsiteFull = "https://sefinek.net/genshin-impact-reshade";

        // Files and folders
        public static readonly string AppPath = AppContext.BaseDirectory;
        public static readonly string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stella Mod Launcher");
        private static readonly string AppIsConfigured = Path.Combine(AppData, "configured.sfn");
        private static readonly string PrepareLauncher = Path.Combine(AppPath, "First app launch.exe");
        public static readonly string ReShadePath = Path.Combine(AppPath, "data", "reshade", "ReShade64.dll");
        public static readonly string InjectorPath = Path.Combine(AppPath, "data", "reshade", "inject64.exe");
        public static readonly string FpsUnlockerExePath = Path.Combine(AppPath, "data", "unlocker", "unlockfps_clr.exe");
        public static readonly string FpsUnlockerCfgPath = Path.Combine(AppPath, "data", "unlocker", "unlocker.config.json");
        public static readonly string PatronsDir = Path.Combine(AppPath, "data", "presets", "3. Only for patrons");
        public static readonly Icon Ico = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

        // Web
        public static readonly string UserAgent = $"Mozilla/5.0 (compatible; StellaLauncher/{AppVersion}; +{AppWebsiteSub})";

        // public static readonly string WebApi = "http://127.0.0.1:4010/api/v4";
        public static readonly string WebApi = "https://api.sefinek.net/api/v4";

        // Config
        public static readonly IniFile Settings = new IniFile(Path.Combine(AppData, "settings.ini"));

        // Lang
        private static readonly string[] SupportedLangs = { "en", "pl" };

        // Registry
        public static readonly string RegistryPath = @"SOFTWARE\Stella Mod Launcher";

        [DllImport("user32.dll")]
        private static extern bool SetProcessDpiAwarenessContext(IntPtr dpiContext);

        [STAThread]
        private static void Main()
        {
            // Set language
            string currentLang = Settings.ReadString("Language", "UI", null);
            bool isSupportedLanguage = SupportedLangs.Contains(currentLang);
            if (string.IsNullOrEmpty(currentLang) || !isSupportedLanguage)
            {
                string sysLang = CultureInfo.InstalledUICulture.Name.Substring(0, 2);
                currentLang = SupportedLangs.Contains(sysLang) ? sysLang : "en";

                Settings.WriteString("Language", "UI", currentLang);
            }

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(currentLang);


            // First log
            Log.Output(
                "==============================================================================================================\n" +
                string.Format(
                    Resources.Program_ARequestToStartTheProgramHasBeenReceived,
                    Debugger.IsAttached,
                    ComputerInfo.GetCpuSerialNumber(),
                    AppPath,
                    AppData,
                    AppIsConfigured,
                    FpsUnlockerCfgPath,
                    PatronsDir
                ) + "\n"
            );


            if (Process.GetProcessesByName(AppName).Length > 1)
            {
                MessageBox.Show(string.Format(Resources.Program_SorryOneInstanceIsCurrentlyOpen_, Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly()?.Location)), AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                Log.Output(Resources.Program_OneInstanceIsCurrentlyOpen);
                Environment.Exit(998765341);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SetProcessDpiAwarenessContext(new IntPtr(-4));


            // Found russian pig?
            if (RegionInfo.CurrentRegion.Name == "RU")
            {
                new WrongCountry { Icon = Ico }.ShowDialog();
                Environment.Exit(999222999);
            }

            // Is launcher configured?
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath, true))
            {
                int value = (int)(key?.GetValue("AppIsConfigured") ?? 0);
                if (value == 0)
                {
                    _ = Cmd.Execute(new Cmd.CliWrap { App = PrepareLauncher });
                    Environment.Exit(997890421);
                }
            }


            try
            {
                Application.Run(new Default { Icon = Ico });
            }
            catch (Exception e)
            {
                Log.ErrorAndExit(e);
            }
        }
    }
}
