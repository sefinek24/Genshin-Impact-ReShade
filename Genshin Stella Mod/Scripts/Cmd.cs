using System;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Buffered;
using CliWrap.Builders;

namespace GenshinStellaMod.Scripts
{
    internal static class Cmd
    {
        public static async Task<bool> Execute(CliWrap cliWrapCommand)
        {
            string commandArguments = string.Empty;
            if (cliWrapCommand.Arguments != null) commandArguments = cliWrapCommand.Arguments.Build();

            try
            {
                Log.Output($"CliWrap: Run app: {cliWrapCommand.App}; Arguments {commandArguments}; WorkingDir {cliWrapCommand.WorkingDir};");

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
                Log.Output($"CliWrap: Successfully executed {cliWrapCommand.App}; Exit code: {result.ExitCode}; Start time: {result.StartTime}; Exit time: {result.ExitTime}{stdoutLine}{stderrLine};");

                // Success?
                if (result.ExitCode == 0) return true;

                // StandardError
                string showCommand = !string.IsNullOrEmpty(cliWrapCommand.App) ? $"\n\n» Executed command:\n{cliWrapCommand.App} {cliWrapCommand.Arguments?.Build()}" : "";
                string showWorkingDir = !string.IsNullOrEmpty(cliWrapCommand.WorkingDir)
                    ? $"\n\n» Working directory: {cliWrapCommand.WorkingDir}"
                    : "";
                string showExitCode = !double.IsNaN(result.ExitCode) ? $"\n\n» Exit code: {result.ExitCode}" : "";
                string showError = !string.IsNullOrEmpty(stderr) ? $"\n\n» Error:\n{stderr}" : "";
                string info = $"{showCommand}{showWorkingDir}{showExitCode}{showError}";

                Log.ErrorAndExit(new Exception($"Command execution failed because the underlying process ({cliWrapCommand.App}) returned a non-zero exit code - {result.ExitCode}.\n\n{info}"));
                return false;
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
        }
    }
}
