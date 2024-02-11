using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using StellaLauncher.Forms.Errors;

namespace StellaLauncher.Scripts
{
	/// <summary>
	///    Provides logging functionality for the launcher.
	/// </summary>
	internal static class Log
	{
		public static readonly string Folder = Path.Combine(Program.AppData, "logs");
		public static readonly string CmdLogs = Path.Combine(Folder, "cmd.output.log");

		/// <summary>
		///    Logs the provided exception and shows an error dialog.
		/// </summary>
		/// <param name="ex"> The exception to be logged.</param>
		public static void ThrowError(Exception ex)
		{
			Program.Logger.Error(ex);

			// Show an error dialog with associated icon
			new ErrorOccurred { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) }.ShowDialog();
		}

		/// <summary>
		///    Logs the provided exception, sends telemetry data, shows an error dialog, and exits the application.
		/// </summary>
		/// <param name="ex">The exception to be logged and reported.</param>
		public static void ErrorAndExit(Exception ex)
		{
			Telemetry.Error(ex);

			// Log the exception and show the error dialog
			ThrowError(ex);

			// Exit the application with a specific exit code
			Environment.Exit(999991000);
		}
	}
}
