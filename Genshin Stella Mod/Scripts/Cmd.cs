using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using CliWrap;
using CliWrap.Buffered;
using CliWrap.Builders;
using Microsoft.Toolkit.Uwp.Notifications;
using StellaLauncher.Forms;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts
{
    internal static class Cmd
    {
        public static async Task<bool> Execute(CliWrap cliWrapCommand)
        {
            string commandArguments = string.Empty;
            if (!string.IsNullOrEmpty(cliWrapCommand.Arguments?.ToString())) commandArguments = cliWrapCommand.Arguments.Build();

            try
            {
                Log.Output($"CliWrap: {cliWrapCommand.App} {commandArguments} {cliWrapCommand.WorkingDir}");

                if (Default.UpdateIsAvailable && !cliWrapCommand.BypassUpdates)
                {
                    MessageBox.Show(Resources.Cmd_UpdateIsRequired, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Log.Output(Resources.Cmd_CommandExecutionFailed);
                    return false;
                }

                // CliWrap
                Command action = Cli.Wrap(cliWrapCommand.App)
                    .WithWorkingDirectory(cliWrapCommand.WorkingDir)
                    .WithArguments(commandArguments)
                    .WithValidation(cliWrapCommand.Validation);

                BufferedCommandResult result = await action.ExecuteBufferedAsync();

                // Variables
                string stdout = result.StandardOutput;
                string stderr = result.StandardError;

                // StandardOutput
                string stdoutLine = !string.IsNullOrEmpty(stdout) ? $"\n✅ STDOUT: {stdout}" : "";
                string stderrLine = !string.IsNullOrEmpty(stderr) ? $"\n❌ STDERR: {stderr}" : "";
                Log.Output(string.Format(Resources.Cmd_SuccessfullyExecutedCommand, cliWrapCommand.App, result.ExitCode, result.StartTime, result.ExitTime, stdoutLine, stderrLine));

                // StandardError
                if (result.ExitCode == 0) return true;

                string showCommand = !string.IsNullOrEmpty(cliWrapCommand.App) ? $"\n\n» {Resources.Cmd_ExecutedCommand}:\n{cliWrapCommand.App} {cliWrapCommand.Arguments}" : "";
                string showWorkingDir = !string.IsNullOrEmpty(cliWrapCommand.WorkingDir)
                    ? $"\n\n» {Resources.Cmd_WorkingDirectory}: {cliWrapCommand.WorkingDir}"
                    : "";
                string showExitCode = !double.IsNaN(result.ExitCode) ? $"\n\n» {Resources.Cmd_ExitCode}: {result.ExitCode}" : "";
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
                            Log.SaveError(ex.ToString());
                        }

                        MessageBox.Show(Resources.Cmd_TheRequestOperationWasSuccessfulButYourPCNeedsToBeRebooted, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Log.Output(string.Format(Resources.Cmd__AppInstalled, cliWrapCommand.App, result.ExitCode));
                        return false;
                    }

                    case 5:
                        string mainInfo = Resources.Cmd_FailedToUpdate;
                        MessageBox.Show(mainInfo, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        Log.SaveError($"{mainInfo}\n{Resources.Cmd_RestartYourComputerOrSuspendAntivirusProgramAndTryAgain}{info}");
                        return false;

                    default:
                    {
                        if (!cliWrapCommand.DownloadingSetup)
                            Log.ErrorAndExit(new Exception(string.Format(Resources.Cmd_CommandExecutionFailedBeacuseTheUnderlyingProcessReturnedANonZeroExitCode, cliWrapCommand.App, result.ExitCode, info)));
                        else
                            Log.SaveError(info);
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
}
