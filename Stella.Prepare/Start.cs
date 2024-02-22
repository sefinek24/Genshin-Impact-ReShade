using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using NLog;
using NLog.Config;
using PrepareStella.Scripts;
using StellaPLFNet;

namespace PrepareStella;

internal static class Start
{
	// App
	public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
	public static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
	public static readonly string AppPath = AppDomain.CurrentDomain.BaseDirectory;
	public static readonly string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stella Mod Launcher");

	// Links
	private static readonly string AppWebsite = "https://genshin.sefinek.net";
	public static readonly string DiscordUrl = "https://discord.gg/k2wfGRq4dT";

	// Web
	public static readonly string UserAgent = $"Mozilla/5.0 (compatible; PrepareStella/{AppVersion}; +{AppWebsite})";

	// public static readonly string WebApi = Debugger.IsAttached ? "http://127.0.0.1:4010/api/v6" : "https://api.sefinek.net/api/v6";
	public static readonly string WebApi = "https://api.sefinek.net/api/v6";

	// Dependencies
	public static readonly string VcLibsAppx = Path.Combine("net8.0-windows", "dependencies", "Microsoft.VCLibs.x64.14.00.Desktop.appx");
	public static readonly string WtMsixBundle = Path.Combine("net8.0-windows", "dependencies", "Microsoft.WindowsTerminal_1.19.10302.0_8wekyb3d8bbwe.msixbundle");

	// Other
	public static readonly string Line = "===============================================================================================";
	public static NotifyIcon NotifyIconInstance;
	public static readonly Icon Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

	// Logger
	public static Logger Logger = LogManager.GetCurrentClassLogger();

	[DllImport("kernel32.dll")]
	private static extern IntPtr GetConsoleWindow();

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
			TaskbarProgress.SetProgressValue(100);
			Log.ErrorAndExit(new Exception("» This application requires administrator privileges to run."), false, false);
			return;
		}

		// WinForms
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);

		// Tray
		NotifyIconInstance = new NotifyIcon
		{
			Icon = Icon,
			Text = AppName,
			Visible = true
		};

		Log.InitDirs();

		IntPtr consoleHandle = GetConsoleWindow();
		TaskbarProgress.MainWinHandle = consoleHandle;

		AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
		Console.CancelKeyPress += OnCancelKeyPress;

		try
		{
			await Program.Run();
		}
		catch (Exception ex)
		{
			Log.ErrorAndExit(ex, false, false);
		}
	}

	private static void OnProcessExit(object sender, EventArgs e)
	{
		NotifyIconInstance?.Dispose();
	}

	private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
	{
		NotifyIconInstance?.Dispose();
		NotifyIconInstance = null;
	}
}
