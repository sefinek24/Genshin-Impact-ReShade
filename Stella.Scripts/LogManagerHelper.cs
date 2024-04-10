using NLog;
using NLog.Config;

namespace StellaUtils;

public static class LogManagerHelper
{
	private static Logger? _logger;

	public static void Initialize(string cfgPath, string appName, string? appVersion)
	{
		_logger = LogManager.GetCurrentClassLogger()
			.WithProperty("AppName", appName)
			.WithProperty("AppVersion", appVersion);

		LogManager.Configuration = new XmlLoggingConfiguration(cfgPath);
	}

	public static Logger GetLogger()
	{
		if (_logger == null) throw new InvalidOperationException("Logger has not been initialized. Call LogManagerHelper.Initialize(appName, appVersion) first.");

		return _logger;
	}
}
