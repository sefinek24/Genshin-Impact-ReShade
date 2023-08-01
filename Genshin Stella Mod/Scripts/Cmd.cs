using System;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Buffered;
using CliWrap.Builders;

namespace Inject_Mods.Scripts
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

                // CliWrap
                Command action = Cli.Wrap(cliWrapCommand.App)
                    .WithWorkingDirectory(cliWrapCommand.WorkingDir)
                    .WithArguments(commandArguments)
                    .WithValidation(cliWrapCommand.Validation);

                BufferedCommandResult result = await action.ExecuteBufferedAsync();


                // Variables
                string stdout = result.StandardOutput;
                Console.WriteLine(stdout);
                string stderr = result.StandardError;
                Console.WriteLine(stderr);

                // StandardOutput
                string stdoutLine = !string.IsNullOrEmpty(stdout) ? $"\n✅ STDOUT: {stdout}" : "";
                string stderrLine = !string.IsNullOrEmpty(stderr) ? $"\n❌ STDERR: {stderr}" : "";
                Log.Output($"{cliWrapCommand.App}, {result.ExitCode}, {result.StartTime}, {result.ExitTime}, {stdoutLine}, {stderrLine}");

                // StandardError
                if (result.ExitCode == 0) return true;

                string showCommand = !string.IsNullOrEmpty(cliWrapCommand.App) ? $"\n\n» Cmd_ExecutedCommand:\n{cliWrapCommand.App} {cliWrapCommand.Arguments}" : "";
                string showWorkingDir = !string.IsNullOrEmpty(cliWrapCommand.WorkingDir)
                    ? $"\n\n» Cmd_WorkingDirectory: {cliWrapCommand.WorkingDir}"
                    : "";
                string showExitCode = !double.IsNaN(result.ExitCode) ? $"\n\n» Cmd_ExitCode: {result.ExitCode}" : "";
                string showError = !string.IsNullOrEmpty(stderr) ? $"\n\n» Cmd_Error:\n{stderr}" : "";
                string info = $"{showCommand}{showWorkingDir}{showExitCode}{showError}";

                switch (result.ExitCode)
                {
                    case 3010:
                    {
                        Log.Output($"{cliWrapCommand.App} {result.ExitCode}");
                        return false;
                    }

                    case 5:
                        Log.SaveError($"Cmd_FailedToUpdate\nCmd_RestartYourComputerOrSuspendAntivirusProgramAndTryAgain{info}");
                        return false;

                    default:
                    {
                        if (!cliWrapCommand.DownloadingSetup)
                            Log.ErrorAndExit(new Exception($"Cmd_CommandExecutionFailedBeacuseTheUnderlyingProcessReturnedANonZeroExitCode {cliWrapCommand.App}, {result.ExitCode}, {info}"));
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
