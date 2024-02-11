using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using NLog;
using StellaLauncher.Forms;
using StellaLauncher.Properties;
using StellaLauncher.Scripts;

namespace StellaLauncher
{
	internal static class Program
	{
		// App
		public static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
		public static readonly string ProductVersion = Application.ProductVersion;
		private static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
		public static readonly string AppNameVer = $"{AppName} • v{ProductVersion}";
		private static readonly string AppWebsiteSub = "https://genshin.sefinek.net";
		public static readonly string AppWebsiteFull = "https://sefinek.net/genshin-impact-reshade";

		// Files and folders
		public static readonly string AppPath = AppDomain.CurrentDomain.BaseDirectory;
		public static readonly string AppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stella Mod Launcher");
		public static readonly string ConfigurationWindow = Path.Combine(AppPath, "net8.0-windows10.0.18362.0", "Configuration Window.exe");
		public static readonly string ReShadePath = Path.Combine(AppPath, "data", "reshade", "ReShade64.dll");
		public static readonly string InjectorPath = Path.Combine(AppPath, "data", "reshade", "inject64.exe");
		public static readonly string FpsUnlockerExePath = Path.Combine(AppPath, "data", "unlocker", "gen-fps-unlocker.exe");
		public static readonly string FpsUnlockerCfgPath = Path.Combine(AppPath, "data", "unlocker", "unlocker.config.json");
		public static readonly Icon Ico = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

		// Config
		public static readonly IniFile Settings = new IniFile(Path.Combine(AppData, "settings.ini"));

		// Web
		public static readonly string UserAgent = $"Mozilla/5.0 (compatible; StellaLauncher/{ProductVersion}; +{AppWebsiteSub}) WebClient/0.1";

		public static readonly Lazy<HttpClient> WbClient = new Lazy<HttpClient>(() =>
		{
			HttpClient httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"Mozilla/5.0 (compatible; StellaLauncher/{ProductVersion}; +{AppWebsiteSub}) HttpClient/0.2");

			return httpClient;
		});

		// public static readonly string WebApi = Debugger.IsAttached ? "http://127.0.0.1:4010/api/v5" : "https://api.sefinek.net/api/v5";
		public static readonly string WebApi = "https://api.sefinek.net/api/v5";

		// Lang
		private static readonly string[] SupportedLangs = { "en", "pl" };

		// Registry
		public static readonly string RegistryPath = @"Software\Stella Mod Launcher";

		// Logger
		public static Logger Logger = LogManager.GetCurrentClassLogger();

		// public static void DisposeHttpClient()
		// {
		//     if (WbClient.IsValueCreated) WbClient.Value.Dispose();
		// }

		public static HttpClient SefinWebClient => WbClient.Value;

		[STAThread]
		private static void Main(string[] args)
		{
			// Prepare NLog
			Logger = Logger.WithProperty("AppName", "Launcher");
			Logger = Logger.WithProperty("AppVersion", AppVersion);

			// Set the correct language for WinForms
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
			Logger.Info(
				"==============================================================================================================\n" +
				"A request to start the program has been received.\n" +
				$"* Debugger.IsAttached: {Debugger.IsAttached}\n" +
				$"* AppPath: {AppPath}\n" +
				$"* AppData: {AppData}\n" +
				$"* Language: {currentLang}");

			if (Debugger.IsAttached) Logger.Debug($"* CPU Serial Number {ComputerInfo.GetCpuSerialNumber()}");

			// Check the number of running instances of Stella Mod Launcher. Only allow 1 process
			if (Process.GetProcessesByName(AppName).Length > 1)
			{
				Logger.Info("One instance is currently open");
				MessageBox.Show(string.Format(Resources.Program_SorryOneInstanceIsCurrentlyOpen_, Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly()?.Location)), AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Information);

				Environment.Exit(998765341);
			}

			// Execute the necessary functions for WinForms
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// Used exclusively for GitHub Actions. Currently, it is useless See https://github.com/sefinek24/Genshin-Impact-ReShade/blob/main/.github/workflows/build.yml
			foreach (string arg in args)
			{
				if (arg != "--gh-actions") continue;

				Logger.Info("Application closed due to detection of the `--gh-actions` parameter.");
				Environment.Exit(0);
			}

			// Block the software in Russia
			// if (RegionInfo.CurrentRegion.Name == "RU")
			// {
			//     Music.PlaySound("winxp", "battery-critical");
			//     new RussianCunt { Icon = Ico }.ShowDialog();
			//     Environment.Exit(999222999);
			// }

			// Has the software been configured on the user's PC?
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath, true))
			{
				int value = (int)(key?.GetValue("AppIsConfigured") ?? 0);
				if (value == 0)
				{
					_ = Cmd.Execute(new Cmd.CliWrap { App = ConfigurationWindow });
					Environment.Exit(997890421);
				}
			}

			// Has the launcher been successfully updated? If so, show the Configuration Window again.
			// IniFile prepareIni = new IniFile(Path.Combine(AppData, "prepare-stella.ini"));
			// int newUpdate = prepareIni.ReadInt("Launcher", "UserIsMyPatron", 0);
			// if (newUpdate == 1)
			// {
			//     _ = Cmd.Execute(new Cmd.CliWrap { App = PrepareLauncher });
			//     Environment.Exit(997890421);
			// }


			// Launch the default form (main window Forms\Default.cs)
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
