using System;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Buffered;
using CliWrap.Builders;

namespace GenshinStellaMod.Scripts
{
    internal static class Cmd
    {
        public static async Task Execute(CliWrap cliWrapCommand)
        {
            try
            {
                string commandArguments = string.Empty;
                if (cliWrapCommand.Arguments != null) commandArguments = cliWrapCommand.Arguments.Build();

                Log.Output($"CliWrap: Run app: {cliWrapCommand.App}; Arguments {commandArguments}; WorkingDir {cliWrapCommand.WorkingDir};");


                // CliWrap
                Command action = Cli.Wrap(cliWrapCommand.App)
                    .WithWorkingDirectory(cliWrapCommand.WorkingDir)
                    .WithArguments(commandArguments)
                    .WithValidation(cliWrapCommand.Validation);
                BufferedCommandResult result = await action.ExecuteBufferedAsync();

                // Variables
                string stdout = result.StandardOutput.Trim();
                if (!cliWrapCommand.ReloadExplorer) Console.WriteLine(stdout);

                string stderr = result.StandardError.Trim();
                if (!cliWrapCommand.ReloadExplorer) Console.WriteLine(stderr);

                // StandardOutput
                string stdoutLine = !string.IsNullOrEmpty(stdout) ? $"\nSTDOUT: {stdout}" : "";
                string stderrLine = !string.IsNullOrEmpty(stderr) ? $"\nSTDERR: {stderr}" : "";
                Log.Output($"CliWrap: Successfully executed {cliWrapCommand.App}; Exit code: {result.ExitCode}; Start time: {result.StartTime}; Exit time: {result.ExitTime}{stdoutLine}{stderrLine};");

                // Success?
                if (result.ExitCode == 0 || (result.ExitCode == 2 && cliWrapCommand.ReloadExplorer)) return;


                // StandardError
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

                string errorMessage = $"Command execution failed because the underlying process ({cliWrapCommand.App}) returned a non-zero exit code - {result.ExitCode}.\n\n{info}";
                Log.ThrowErrorString(errorMessage);
            }
            catch (Exception ex)
            {
                Log.ThrowError(ex);
            }
        }

        public class CliWrap
        {
            public string App { get; set; }
            public string WorkingDir { get; set; }
            public ArgumentsBuilder Arguments { get; set; }
            public CommandResultValidation Validation { get; set; }
            public bool ReloadExplorer { get; set; }
        }
    }
}
