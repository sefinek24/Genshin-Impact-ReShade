using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Toolkit.Uwp.Notifications;
using StellaLauncher.Forms;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts
{
    internal static class Cmd
    {
        public static async Task<bool> CliWrap(string app, string args, string workingDir, bool bypassUpdates, bool downloadSetup)
        {
            try
            {
                Log.Output($"CliWrap: {app} {args} {workingDir}");

                if (Default.UpdateIsAvailable && !bypassUpdates)
                {
                    MessageBox.Show(Resources.Cmd_UpdateIsRequired, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Log.Output(Resources.Cmd_CommandExecutionFailed);
                    return false;
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
                Log.Output(string.Format(Resources.Cmd_SuccessfullyExecutedCommand, app, result.ExitCode, result.StartTime, result.ExitTime, stdoutLine, stderrLine));

                // StandardError
                if (result.ExitCode == 0) return true;

                string showCommand = !string.IsNullOrEmpty(app) ? $"\n\n» {Resources.Cmd_ExecutedCommand}:\n{app} {args}" : "";
                string showWorkingDir = !string.IsNullOrEmpty(workingDir)
                    ? $"\n\n» {Resources.Cmd_WorkingDirectory}: {workingDir}"
                    : "";
                string showExitCode = !double.IsNaN(result.ExitCode) ? $"\n\n» ${Resources.Cmd_ExitCode}: {result.ExitCode}" : "";
                string showError = !string.IsNullOrEmpty(stderr) ? $"\n\n» {Resources.Cmd_Error}:\n{stderr}" : "";
                string info = $"{showCommand}{showWorkingDir}{showExitCode}{showError}";

                switch (result.ExitCode)
                {
                    case 3010:
                    {
                        try
                        {
                            new ToastContentBuilder()
                                .AddText($"{Resources.Cmd_RebootIsRequired} 📄")
                                .AddText(Resources.Cmd_TheRequiredDependencyHasBeenSuccessfullyInstalledButyourComputerNeedsToBeRestart)
                                .Show();
                        }
                        catch (Exception ex)
                        {
                            Log.SaveErrorLog(ex);
                        }

                        MessageBox.Show(Resources.Cmd_TheRequestOperationWasSuccessfulButYourPCNeedsToBeRebooted, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Log.Output(string.Format(Resources.Cmd__AppInstalled, app, result.ExitCode));
                        return false;
                    }

                    case 5:
                        string mainInfo = Resources.Cmd_FailedToUpdate;
                        MessageBox.Show(mainInfo, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        Log.SaveErrorLog(new Exception($"{mainInfo}\n{Resources.Cmd_RestartYourComputerOrSuspendAntivirusProgramAndTryAgain}{info}"));
                        return false;

                    default:
                    {
                        if (!downloadSetup)
                            Log.ErrorAndExit(new Exception(string.Format(Resources.Cmd_CommandExecutionFailedBeacuseTheUnderlyingProcessReturnedANonZeroExitCode, app, result.ExitCode, info)));
                        else
                            Log.SaveErrorLog(new Exception(info));
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
    }
}
