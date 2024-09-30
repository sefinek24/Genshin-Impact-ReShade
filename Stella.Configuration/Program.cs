using ConfigurationNC.Forms;
using ConfigurationNC.Properties;
using NLog;
using StellaUtils;

namespace ConfigurationNC;

internal static class Program
{
	internal static Logger Logger = null!;

	[STAThread]
	private static void Main()
	{
		// Prepare NLog
		LogManagerHelper.Initialize(Path.Combine(Window.AppPath, "NLog.config"), "Configuration window", Window.AppVersion);
		Logger = LogManagerHelper.GetLogger();

		Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
		ApplicationConfiguration.Initialize();
		Application.ThreadException += (_, e) => Logger.Error($"ThreadException: {e.Exception.Message}");
		AppDomain.CurrentDomain.UnhandledException += (_, e) => Logger.Error($"UnhandledException: {((Exception)e.ExceptionObject).Message}");

		try
		{
			Application.Run(new Window { Icon = Resources.cat_white_52x52 });

			Logger.Info("Application.Run(): new Window");
		}
		catch (Exception ex)
		{
			Logger.Error(ex);
			MessageBox.Show(ex.Message, Window.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
