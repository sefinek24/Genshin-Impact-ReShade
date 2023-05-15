using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Buffered;

namespace Injector.Scripts;

internal abstract class Cmd
{
    public static async Task CliWrap(string app, string args, string workingDir, bool waitForExit)
    {
        Log.Output($"Execute command: {app} {args} {workingDir}");

        try
        {
            using Stream stdOut = Console.OpenStandardOutput();
            using Stream stdErr = Console.OpenStandardError();

            Command action = Cli.Wrap(app)
                .WithArguments(args)
                .WithWorkingDirectory(workingDir)
                .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine))
                .WithValidation(CommandResultValidation.None) | (stdOut, stdErr);

            if (waitForExit)
            {
                BufferedCommandResult result = await action.ExecuteBufferedAsync();

                // Variables
                string stdout = result.StandardOutput;
                string stderr = result.StandardError;

                // StandardOutput
                string stdoutLine = !string.IsNullOrEmpty(stdout) ? $"\n✅ STDOUT: {stdout}" : "";
                string stderrLine = !string.IsNullOrEmpty(stderr) ? $"\n❌ STDERR: {stderr}" : "";
                Log.Output($"Successfully executed {app} command. Exit code: {result.ExitCode}, start time: {result.StartTime}, exit time: {result.ExitTime}{stdoutLine}{stderrLine}");

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
                    if (wtProcess1.Length != 0) await CliWrap("taskkill", "/F /IM WindowsTerminal.exe", null, true);


                    Log.ErrorAndExit(
                        new Exception(
                            $"Command execution failed because the underlying process ({app.Replace(@"Dependencies\", "").Replace(@"C:\Program Files\Git\cmd\", "")}) returned a non-zero exit code - {result.ExitCode}.\nCheck your Internet connection, antivirus program or restart PC and try again.{info}"),
                        false, result.ExitCode != 128);
                }
                else
                {
                    BufferedCommandResult xd = await action.ExecuteBufferedAsync();
                }
            }
        }
        catch (Exception e)
        {
            Log.ErrorAndExit(e, false, true);
        }
    }


    public static void Execute(string app, string args, string workingDir, bool runAsAdmin, bool exit)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = app,
                Arguments = args,
                WorkingDirectory = workingDir,
                Verb = runAsAdmin ? "runas" : "",
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Log.ThrowError(ex, false);
        }

        // Exit
        if (exit) Environment.Exit(0);
    }
}
