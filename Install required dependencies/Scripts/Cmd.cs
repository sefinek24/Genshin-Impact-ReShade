using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Toolkit.Uwp.Notifications;

namespace PrepareStella.Scripts
{
    internal abstract class Cmd
    {
        public static bool RebootNeeded;

        public static async Task CliWrap(string app, string args, string workingDir)
        {
            Log.Output($"Execute command: {app} {args} {workingDir}");

            try
            {
                Command action = Cli.Wrap(app)
                    .WithArguments(args)
                    .WithWorkingDirectory(workingDir)
                    .WithValidation(CommandResultValidation.None);
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
                    if (wtProcess1.Length != 0) await CliWrap("taskkill", "/F /IM WindowsTerminal.exe", null);


                    switch (result.ExitCode)
                    {
                        case 3010:
                            try
                            {
                                new ToastContentBuilder()
                                    .AddText("Installation alert 📄")
                                    .AddText("Required dependency has been successfully installed, but your computer needs a restart. Please wait to complete installation.")
                                    .Show();
                            }
                            catch (Exception ex)
                            {
                                Log.SaveErrorLog(ex, true);
                            }

                            Log.Output($"{app} installed. Exit code: {result.ExitCode}\nThe requested operation is successful. Changes will not be effective until the system is rebooted.");

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
}
