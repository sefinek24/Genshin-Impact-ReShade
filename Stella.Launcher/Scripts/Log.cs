using StellaModLauncher.Forms.Errors;

namespace StellaModLauncher.Scripts;

/// <summary>
///    Provides logging functionality for the launcher.
/// </summary>
internal static class Log
{
	public static readonly string? Folder = Path.Combine(Program.AppData, "logs");
	public static readonly string CmdLogs = Path.Combine(Folder, "cmd.output.log");

	/// <summary>
	///    Logs the provided exception and shows an error dialog.
	/// </summary>
	public static void ThrowError(Exception ex)
	{
		Program.Logger.Error(ex);

		// Show an error dialog with associated icon
		new ErrorOccurred { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) }.ShowDialog();
	}

	/// <summary>
	///    Logs the provided exception, sends telemetry data, shows an error dialog, and exits the application.
	/// </summary>
	public static void ErrorAndExit(Exception ex, bool showForm)
	{
		Telemetry.Error(ex);

		if (showForm)
			ThrowError(ex);
		else
			Program.Logger.Error(ex);

		Environment.Exit(999991000);
	}
}
