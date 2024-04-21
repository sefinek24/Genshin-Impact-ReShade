using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using NLog;
using PrepareStella.Scripts;
using StellaUtils;

namespace PrepareStella;

internal static class Start
{
	// App
	public static readonly string? AppName = Assembly.GetExecutingAssembly().GetName().Name;
	public static readonly string? AppVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
	public static readonly string? AppPath = AppDomain.CurrentDomain.BaseDirectory;
	public static readonly string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stella Mod Launcher");

	// Links
	private static readonly string AppWebsite = "https://stella.sefinek.net";
	public static readonly string DiscordUrl = "https://discord.gg/k2wfGRq4dT";

	// Web
	public static readonly string UserAgent = $"Mozilla/5.0 (compatible; PrepareStella/{AppVersion}; +{AppWebsite})";

	// public static readonly string WebApi = Debugger.IsAttached ? "http://127.0.0.1:4010/api/v7" : "https://api.sefinek.net/api/v7";
	public static readonly string WebApi = "https://api.sefinek.net/api/v7";

	// Other
	public static readonly string Line = "===============================================================================================";
	public static NotifyIcon? NotifyIconInstance;
	public static readonly Icon? Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

	// Logger
	public static Logger Logger = null!;

	[DllImport("kernel32.dll")]
	private static extern IntPtr GetConsoleWindow();

	private static async Task Main()
	{
		// Prepare NLog
		LogManagerHelper.Initialize(Path.Combine(AppPath!, "NLog_PS.config"), "Prepare Stella", AppVersion);
		Logger = LogManagerHelper.GetLogger();

		// Start
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
			Log.ErrorAndExit(new Exception("» This application requires administrator privileges to run."));
			return;
		}

		// WinForms
		ApplicationConfiguration.Initialize();

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
			await Program.Run().ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			Log.ErrorAndExit(ex);
		}
	}

	private static void OnProcessExit(object? sender, EventArgs e)
	{
		NotifyIconInstance?.Dispose();
	}

	private static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
	{
		NotifyIconInstance?.Dispose();
		NotifyIconInstance = null;
	}
}
