using System.Reflection;
using InformationWindow.Forms;
using InformationWindow.Properties;
using NLog;
using StellaUtils;

namespace InformationWindow;

internal static class Program
{
	// App
	private static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString();
	private static readonly string? AppPath = AppDomain.CurrentDomain.BaseDirectory;

	// Logger
	private static Logger _logger = null!;

	/// <summary>
	///    The main entry point for the application.
	/// </summary>
	[STAThread]
	private static void Main()
	{
		// Prepare NLog
		LogManagerHelper.Initialize(Path.Combine(AppPath!, "NLog.config"), "Info 4842", AppVersion);
		_logger = LogManagerHelper.GetLogger();

		try
		{
			ApplicationConfiguration.Initialize();
			Application.Run(new MainForm());

			_logger.Info("Application.Run(): new MainWindow");
		}
		catch (Exception ex)
		{
			_logger.Error(ex);

			MessageBox.Show(ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
