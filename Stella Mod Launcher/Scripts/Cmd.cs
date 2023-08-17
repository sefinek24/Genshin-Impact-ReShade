using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using CliWrap;
using CliWrap.Buffered;
using CliWrap.Builders;
using Microsoft.Toolkit.Uwp.Notifications;
using StellaLauncher.Forms;
using StellaLauncher.Properties;
using StellaLauncher.Scripts.Forms.MainForm;

namespace StellaLauncher.Scripts
{
    internal static class Cmd
    {
        public static async Task<bool> Execute(CliWrap cliWrapCommand)
        {
            Music.PlaySound("winxp", "information_bar");

            string commandArguments = string.Empty;
            if (cliWrapCommand.Arguments != null) commandArguments = cliWrapCommand.Arguments.Build();

            Log.Output($"CliWrap: {cliWrapCommand.App} {commandArguments} {cliWrapCommand.WorkingDir}");

            if (Default.UpdateIsAvailable && !cliWrapCommand.DownloadingSetup)
            {
                MessageBox.Show(Resources.Cmd_UpdateIsRequired, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Log.Output("CliWrap: Command execution failed. An update is required.");
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
                string stdout = result.StandardOutput;
                string stderr = result.StandardError;

                // StandardOutput
                string stdoutLine = !string.IsNullOrEmpty(stdout) ? $"\n✅ STDOUT: {stdout}" : "";
                string stderrLine = !string.IsNullOrEmpty(stderr) ? $"\n❌ STDERR: {stderr}" : "";
                Log.Output($"CliWrap: Successfully executed {cliWrapCommand.App}; Exit code: {result.ExitCode}; Start time: {result.StartTime}; Exit time: {result.ExitTime}{stdoutLine}{stderrLine};");

                // StandardError
                if (result.ExitCode == 0) return true;

                string showCommand = !string.IsNullOrEmpty(cliWrapCommand.App) ? $"\n\n» {Resources.Cmd_ExecutedCommand}:\n{cliWrapCommand.App} {cliWrapCommand.Arguments?.Build()}" : "";
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
                        Log.Output($"CliWrap: {cliWrapCommand.App} installed. Exit code: {result.ExitCode}\nThe requested operation is successful. Changes will not be effective until the system is rebooted");
                        return false;
                    }

                    case 5:
                        string mainInfo = Resources.Cmd_FailedToUpdate;
                        MessageBox.Show(mainInfo, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        Log.SaveError($"{mainInfo}\nRestart your computer or suspend antivirus program and try again.{info}");
                        return false;

                    default:
                    {
                        if (!cliWrapCommand.DownloadingSetup)
                            Log.ErrorAndExit(new Exception($"Command execution failed because the underlying process ({cliWrapCommand.App}) returned a non-zero exit code - {result.ExitCode}.\n\n{info}"));
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

        public static void Start(string process)
        {
            Process.Start(process);
            Log.Output($"Process.Start = {process}");
            Music.PlaySound("winxp", "restore");
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
