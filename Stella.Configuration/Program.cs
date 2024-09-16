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
