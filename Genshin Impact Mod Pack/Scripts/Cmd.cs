using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using CliWrap;
using CliWrap.Buffered;
using Genshin_Stella_Mod.Forms;
using Microsoft.Toolkit.Uwp.Notifications;

namespace Genshin_Stella_Mod.Scripts
{
    internal static class Cmd
    {
        public static async Task CliWrap(string app, string args, string workingDir, bool bypassUpdates, bool downloadSetup)
        {
            try
            {
                Log.Output($"CliWrap: {app} {args} {workingDir}");

                if (Default.UpdateIsAvailable && !bypassUpdates)
                {
                    MessageBox.Show(@"Sorry. Update is required.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Log.Output("CliWrap: Command execution failed. An update is required.");
                    return;
                }

                // CliWrap
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
                Log.Output($"CliWrap: Successfully executed {app}; Exit code: {result.ExitCode}; Start time: {result.StartTime}; Exit time: {result.ExitTime}{stdoutLine}{stderrLine};");

                // StandardError
                if (result.ExitCode != 0)
                {
                    string showCommand = !string.IsNullOrEmpty(app) ? $"\n\n» Executed command:\n{app} {args}" : "";
                    string showWorkingDir = !string.IsNullOrEmpty(workingDir)
                        ? $"\n\n» Working directory: {workingDir}"
                        : "";
                    string showExitCode = !double.IsNaN(result.ExitCode) ? $"\n\n» Exit code: {result.ExitCode}" : "";
                    string showError = !string.IsNullOrEmpty(stderr) ? $"\n\n» Error:\n{stderr}" : "";
                    string info = $"{showCommand}{showWorkingDir}{showExitCode}{showError}";

                    switch (result.ExitCode)
                    {
                        case 3010:
                        {
                            try
                            {
                                new ToastContentBuilder()
                                    .AddText("Update alert 📄")
                                    .AddText("Required dependency has been successfully installed, but your computer needs a restart.")
                                    .Show();
                            }
                            catch (Exception ex)
                            {
                                Log.SaveErrorLog(ex);
                            }

                            MessageBox.Show(@"The requested operation is successful, but your PC needs reboot.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Log.Output(
                                $"CliWrap: {app} installed. Exit code: {result.ExitCode}\nThe requested operation is successful. Changes will not be effective until the system is rebooted.");
                            return;
                        }

                        case 5:
                            const string mainInfo = "Failed to update. The software was denied access to a location, preventing it from saving, copying, opening or loading files.";
                            MessageBox.Show(mainInfo, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                            Log.SaveErrorLog(new Exception($"{mainInfo}\nRestart your computer or suspend antivirus program and try again.{info}"));
                            return;

                        default:
                        {
                            if (!downloadSetup)
                                Log.ErrorAndExit(new Exception($"Command execution failed because the underlying process ({app}) returned a non-zero exit code - {result.ExitCode}.\n{info}"));
                            else
                                Log.SaveErrorLog(new Exception(info));
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ThrowError(ex);
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

                Log.Output($"Process.Start: Successfully executed {app}; Args: {args}; Working dir: {workingDir}; Run as admin: {runAsAdmin}; Exit: {exit};");
            }
            catch (Exception ex)
            {
                Log.ThrowError(ex);
            }

            // Exit
            if (exit) Environment.Exit(0);
        }
    }
}
