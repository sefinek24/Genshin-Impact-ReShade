using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Taskbar;
using NLog;
using NLog.Config;
using PrepareStella.Scripts;

namespace PrepareStella
{
    internal static class Start
    {
        // App
        public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
        public static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static readonly string AppPath = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string AppData = Utils.GetAppData();

        // Links
        private static readonly string AppWebsite = "https://genshin.sefinek.net";
        public static readonly string DiscordUrl = "https://discord.gg/SVcbaRc7gH";

        // Web
        public static readonly string UserAgent = $"Mozilla/5.0 (compatible; PrepareStella/{AppVersion}; +{AppWebsite})";

        public static readonly string WebApi = Debugger.IsAttached ? "http://127.0.0.1:4010/api/v5" : "https://api.sefinek.net/api/v5";
        //  public static readonly string WebApi = "https://api.sefinek.net/api/v5";

        // Dependencies
        public static readonly string VcLibsAppx = Path.Combine("dependencies", "Microsoft.VCLibs.x64.14.00.Desktop.appx");
        public static readonly string WtMsixBundle = Path.Combine("dependencies", "Microsoft.WindowsTerminal_1.18.3181.0_8wekyb3d8bbwe.msixbundle");

        // Other
        public static readonly string Line = "===============================================================================================";
        public static readonly Icon Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

        // Logger
        public static Logger Logger = LogManager.GetCurrentClassLogger();

        private static async Task Main()
        {
            Logger = Logger.WithProperty("AppName", "Prepare Stella");
            Logger = Logger.WithProperty("AppVersion", AppVersion);
            LogManager.Configuration = new XmlLoggingConfiguration(Path.Combine(AppPath, "NLog_PS.config"));

            Console.OutputEncoding = Encoding.UTF8;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"                               Genshin Impact Stella Mod - Prepare");
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
                TaskbarManager.Instance.SetProgressValue(100, 100);
                Log.ErrorAndExit(new Exception("» This application requires administrator privileges to run."), false, false);
                return;
            }


            Log.InitDirs();


            try
            {
                await Program.Run();
            }
            catch (Exception ex)
            {
                Log.ErrorAndExit(ex, false, false);
            }
        }
    }
}
