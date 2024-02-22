using System.Diagnostics;
using System.Text.RegularExpressions;
using CliWrap;
using CliWrap.Buffered;
using StellaPLFNet;

namespace PrepareStella.Scripts;

internal abstract class Cmd
{
	public static bool RebootNeeded;
	private static int _vcLibsAttemptNumber;

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


				// VcLibs
				if (_vcLibsAttemptNumber >= 3)
				{
					Start.Logger.Error(
						$"Command execution failed because the underlying process (PowerShell) returned a non-zero exit code - {result.ExitCode}.\nI can't install this required package. Reboot your PC or close all opened apps and try again.{info}");
					Environment.Exit(4242141);
					return;
				}

				Process[] wtProcess1 = Process.GetProcessesByName("WindowsTerminal");
				if (wtProcess1.Length != 0) await CliWrap("taskkill", "/F /IM WindowsTerminal.exe", null).ConfigureAwait(false);

				if (Regex.Match(stderr, "(?:80073D02)", RegexOptions.IgnoreCase | RegexOptions.Multiline).Success)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(
						$"     » We cannot install this package because some process is currently in use.\n       Reboot your PC or close all opened apps from Microsoft Store.\n\n{stderr}");

					Start.Logger.Error($"I cannot install this package because some process is currently open.\n\n» Attempt: {_vcLibsAttemptNumber}\n» Exit code: 80073D02\n\n{stderr}");

					TaskbarProgress.SetProgressState(TaskbarProgress.Flags.Paused);

					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine(@"     » Click ENTER to try again...");
					Console.ReadLine();

					Start.Logger.Info("Restarting session...");
					Console.ResetColor();
					await Program.Run().ConfigureAwait(false);
					;
					return;
				}

				if (Regex.Match(stderr, "(?:80073CF3|Microsoft.VCLibs.)", RegexOptions.IgnoreCase | RegexOptions.Multiline).Success)
				{
					_vcLibsAttemptNumber++;

					Start.Logger.Error($"Found missing dependency Microsoft.VCLibs.\n\nAttempt {_vcLibsAttemptNumber}\nExit code: 80073CF3\n\n{stderr}");

					BalloonTip.Show("Ughh, sorry. We need more time 😥", "Found missing dependency with name VCLibs.\nClose all Microsoft Store apps and go back to the setup!");

					// Preparing...
					Console.WriteLine($@"Preparing to install Microsoft Visual C++ 2015 UWP Desktop Package (attempt {_vcLibsAttemptNumber}/3)...");

					TaskbarProgress.SetProgressState(TaskbarProgress.Flags.Paused);
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine(@"    » ATTENTION: Close all Microsoft Store apps and press ENTER to continue...");
					Console.ResetColor();

					Console.ReadLine();

					// Close apps
					Process[] dllHostProcess = Process.GetProcessesByName("dllhost");
					if (dllHostProcess.Length != 0) await CliWrap("taskkill", "/F /IM dllhost.exe", null).ConfigureAwait(false);
					;
					Process[] wtProcess2 = Process.GetProcessesByName("WindowsTerminal");
					if (wtProcess2.Length != 0) await CliWrap("taskkill", "/F /IM WindowsTerminal.exe", null).ConfigureAwait(false);
					;

					// Installing...
					Console.WriteLine(@"Installing Microsoft Visual C++ 2015 UWP Desktop Package...");

					if (!File.Exists(Start.VcLibsAppx))
						Log.ErrorAndExit(new Exception($"I can't find a required file. Please unpack downloaded zip archive.\nNot found: {Start.VcLibsAppx}"), false, false);

					Start.Logger.Info("Installing missing dependency VCLibs...");
					await CliWrap("powershell", $"Add-AppxPackage -Path {Start.VcLibsAppx}", null).ConfigureAwait(false);
					;

					// Throw info
					BalloonTip.Show("First part was finished 🎉", "VCLibs has been successfully installed, but now we need to restart your computer.");

					// Completed!
					Start.Logger.Info("Installed Microsoft Visual C++ 2015 UWP Desktop Package.");
					Console.WriteLine("      » Successfully! Please reboot your PC and open the installer again!\n");

					// Reboot PC
					Console.ForegroundColor = ConsoleColor.Green;
					Console.Write(@"» Restart your computer now? This is required. [Yes/no]: ");
					Console.ResetColor();

					string? rebootPc = Console.ReadLine();
					if (Regex.Match(rebootPc ?? string.Empty, "(?:y)", RegexOptions.IgnoreCase | RegexOptions.Singleline).Success)
					{
						await CliWrap("shutdown",
							$"/r /t 25 /c \"{Start.AppName} - scheduled reboot, version {Start.AppVersion}.\n\nAfter restarting, run the installer again. If you need help, add me on Discord Sefinek#0001.\n\nGood luck!\"",
							null).ConfigureAwait(false);
						;

						Console.WriteLine("Your computer will restart in 25 seconds. Save your work!\nAfter restarting, run the installer again.");
						Start.Logger.Info("PC reboot was scheduled. Installed VCLibs.");
					}
					else
					{
						Console.WriteLine(@"Woaah, okay!");
					}

					while (true) Console.ReadLine();
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
								$"Software was denied access to a location for the purposes of saving, copying, opening, or loading files.\nRestart your computer or suspend antivirus program and try again.{info}"),
							false, false);
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
}
