using System.Diagnostics;
using System.Text.RegularExpressions;
using CliWrap;
using CliWrap.Buffered;
using StellaUtils;

namespace PrepareStella.Scripts;

internal abstract partial class Cmd
{
	public static bool RebootNeeded;

	public static async Task CliWrap(string app, string? args, string? workingDir)
	{
		Start.Logger.Info($"Execute command: {app} {args} {workingDir}");

		try
		{
			Command action = Cli.Wrap(app)
				.WithArguments(args!)
				.WithWorkingDirectory(workingDir!)
				.WithValidation(CommandResultValidation.None);
			BufferedCommandResult result = await action.ExecuteBufferedAsync();

			// Variables
			string stdout = result.StandardOutput;
			string stderr = result.StandardError;

			// StandardOutput
			string stdoutLine = !string.IsNullOrEmpty(stdout) ? $"\n✅ STDOUT: {stdout}" : "";
			string stderrLine = !string.IsNullOrEmpty(stderr) ? $"\n❌ STDERR: {stderr}" : "";
			Start.Logger.Info($"Successfully executed {app} command. Exit code: {result.ExitCode}, start time: {result.StartTime}, exit time: {result.ExitTime}{stdoutLine}{stderrLine}");

			// StandardError
			if (result.ExitCode != 0)
			{
				string showCommand = !string.IsNullOrEmpty(app) ? $"\n\n» Executed command:\n{app} {args}" : "";
				string showWorkingDir = !string.IsNullOrEmpty(workingDir)
					? $"\n\n» Working directory: {workingDir}"
					: "";
				string showExitCode = !double.IsNaN(result.ExitCode) ? $"\n\n» Exit code: {result.ExitCode}" : "";
				string showError = !string.IsNullOrEmpty(stderr) ? $"\n\n» Error [stderr]:\n{stderr}" : "";
				string info = $"{showCommand}{showWorkingDir}{showExitCode}{showError}";

				Process[] wtProcess1 = Process.GetProcessesByName("WindowsTerminal");
				if (wtProcess1.Length != 0) await CliWrap("taskkill", "/F /IM WindowsTerminal.exe", null).ConfigureAwait(false);

				if (MyRegex().Match(stderr).Success)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(
						$"     » We cannot install this package because some process is currently in use.\n       Reboot your PC or close all opened apps from Microsoft Store.\n\n{stderr}");

					Start.Logger.Error($"I cannot install this package because some process is currently open. Exit code: 80073D02\n\n{stderr}");

					TaskbarProgress.SetProgressState(TaskbarProgress.Flags.Paused);

					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine(@"     » Click ENTER to try again...");
					Console.ReadLine();

					Start.Logger.Info("Restarting session...");
					Console.ResetColor();
					await Program.Run().ConfigureAwait(false);
					return;
				}

				switch (result.ExitCode)
				{
					case 3010:
						BalloonTip.Show("Installation alert 📄", "Required dependency has been successfully installed, but your computer needs a restart. Please wait to complete installation.");

						Start.Logger.Info($"{app} installed. Exit code: {result.ExitCode}\nThe requested operation is successful. Changes will not be effective until the system is rebooted.");

						RebootNeeded = true;
						return;

					case 5:
						Log.ErrorAndExit(
							new Exception(
								$"Software was denied access to a location for the purposes of saving, copying, opening, or loading files.\nRestart your computer or suspend antivirus program and try again.{info}"));
						return;

					default:
						Log.ErrorAndExit(
							new Exception(
								$"Command execution failed because the underlying process ({app.Replace(@"Dependencies\", "").Replace(@"C:\Program Files\Git\cmd\", "")}) returned a non-zero exit code - {result.ExitCode}.\nCheck your Internet connection, antivirus program or restart PC and try again.{info}"),
							false, result.ExitCode != 128);
						return;
				}
			}
		}
		catch (Exception e)
		{
			Log.ErrorAndExit(e, false, true);
		}
	}

	[GeneratedRegex("(?:80073D02)", RegexOptions.IgnoreCase)]
	private static partial Regex MyRegex();
}
