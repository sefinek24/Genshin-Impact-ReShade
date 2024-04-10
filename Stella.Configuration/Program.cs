using ConfigurationNC.Forms;
using ConfigurationNC.Properties;
using NLog;
using StellaUtils;

namespace ConfigurationNC;

internal static class Program
{
	public static Logger Logger = null!;

	/// <summary>
	///    The main entry point for the application.
	/// </summary>
	[STAThread]
	private static void Main()
	{
		// Prepare NLog
		LogManagerHelper.Initialize(Path.Combine(Window.AppPath, "NLog_CW.config"), "Configuration window", Window.AppVersion);
		Logger = LogManagerHelper.GetLogger();

		try
		{
			ApplicationConfiguration.Initialize();
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
