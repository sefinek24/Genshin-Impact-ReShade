using ConfigurationNC.Forms;
using ConfigurationNC.Properties;
using NLog;
using NLog.Config;

namespace ConfigurationNC;

internal static class Program
{
	public static Logger Logger = LogManager.GetCurrentClassLogger();

	/// <summary>
	///    The main entry point for the application.
	/// </summary>
	[STAThread]
	private static void Main()
	{
		Logger = Logger.WithProperty("AppName", "Configuration window");
		Logger = Logger.WithProperty("AppVersion", Window.AppVersion);
		LogManager.Configuration = new XmlLoggingConfiguration(Path.Combine(Window.AppPath, "NLog_CW.config"));

		ApplicationConfiguration.Initialize();

		try
		{
			Application.Run(new Window { Icon = Resources.cat_white_52x52 });
			Logger.Info("Application Run: Window");
		}
		catch (Exception ex)
		{
			Logger.Error(ex);
			MessageBox.Show(ex.Message, Window.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
