using System.Diagnostics;
using CliWrap;
using CliWrap.Buffered;
using CliWrap.Builders;
using StellaModLauncher.Forms;
using StellaModLauncher.Properties;
using StellaModLauncher.Scripts.Forms;

namespace StellaModLauncher.Scripts;

internal static class Cmd
{
	public static async Task<bool> Execute(CliWrap cliWrapCommand)
	{
		Music.PlaySound("winxp", "information_bar");

		string commandArguments = cliWrapCommand.Arguments != null ? cliWrapCommand.Arguments.Build() : string.Empty;

		Program.Logger.Info($"CliWrap: {cliWrapCommand.App} {commandArguments} {cliWrapCommand.WorkingDir}");

		if (Default.UpdateIsAvailable && !cliWrapCommand.DownloadingSetup)
		{
			MessageBox.Show(Resources.Cmd_UpdateIsRequired, Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Information);
			Program.Logger.Info("CliWrap: Command execution failed. An update is required.");
			return false;
		}

		try
		{
			// CliWrap
			Command action = Cli.Wrap(cliWrapCommand.App)
				.WithWorkingDirectory(cliWrapCommand.WorkingDir)
				.WithArguments(commandArguments)
				.WithValidation(cliWrapCommand.Validation);
			BufferedCommandResult result = await action.ExecuteBufferedAsync();

			// Variables
			string stdout = result.StandardOutput.Trim();
			string stderr = result.StandardError.Trim();

			// StandardOutput
			string stdoutLine = !string.IsNullOrEmpty(stdout) ? $"\n✅ STDOUT: {stdout}" : "";
			string stderrLine = !string.IsNullOrEmpty(stderr) ? $"\n❌ STDERR: {stderr}" : "";
			Program.Logger.Info($"CliWrap: Successfully executed {cliWrapCommand.App}; Exit code: {result.ExitCode}; Start time: {result.StartTime}; Exit time: {result.ExitTime}{stdoutLine}{stderrLine};");

			// StandardError
			if (result.ExitCode == 0) return true;

			string showCommand = !string.IsNullOrEmpty(cliWrapCommand.App)
				? $"» Executed command:\n{cliWrapCommand.App} {commandArguments}"
				: "";
			string showWorkingDir = !string.IsNullOrEmpty(cliWrapCommand.WorkingDir)
				? $"\n\n» Working directory: {cliWrapCommand.WorkingDir}"
				: "";
			string showExitCode = !double.IsNaN(result.ExitCode)
				? $"\n\n» Exit code: {result.ExitCode}"
				: "";
			string showError = !string.IsNullOrEmpty(stderr)
				? $"\n\n» Error:\n{stderr}"
				: "";
			string info = $"{showCommand}{showWorkingDir}{showExitCode}{showError}";

			switch (result.ExitCode)
			{
				case 3010:
				{
					BalloonTip.Show($"{Resources.Cmd_RebootIsRequired} 📄", Resources.Cmd_TheRequiredDependencyHasBeenSuccessfullyInstalledButyourComputerNeedsToBeRestart);

					MessageBox.Show(Resources.Cmd_TheRequestOperationWasSuccessfulButYourPCNeedsToBeRebooted, Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Information);
					Program.Logger.Info($"CliWrap: {cliWrapCommand.App} installed. Exit code: {result.ExitCode}\nThe requested operation is successful. Changes will not be effective until the system is rebooted");
					return false;
				}

				case 5:
					string mainInfo = Resources.Cmd_FailedToUpdate;
					MessageBox.Show(mainInfo, Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Error);

					Program.Logger.Error($"{mainInfo}\nRestart your computer or suspend antivirus program and try again.{info}");
					return false;

				default:
				{
					if (!cliWrapCommand.DownloadingSetup)
						Log.ErrorAndExit(new Exception($"Command execution failed because the underlying process ({cliWrapCommand.App}) returned a non-zero exit code - {result.ExitCode}.\n\n{info}"), true);
					else
						Program.Logger.Error(info);
					return false;
				}
			}
		}
		catch (Exception ex)
		{
			Log.ThrowError(ex);
			return false;
		}
	}

	public static void Start(string process)
	{
		ProcessStartInfo psi = new()
		{
			FileName = process,
			UseShellExecute = true
		};

		Process.Start(psi);
		Program.Logger.Info($"Process.Start = {process}");
	}

	public class CliWrap
	{
		public string App { get; set; }
		public string WorkingDir { get; set; }
		public ArgumentsBuilder Arguments { get; set; }
		public CommandResultValidation Validation { get; set; }
		public bool BypassUpdates { get; set; }
		public bool DownloadingSetup { get; set; }
	}
}
