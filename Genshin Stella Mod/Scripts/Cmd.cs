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
            if (!string.IsNullOrEmpty($"{cliWrapCommand.Arguments}")) commandArguments = cliWrapCommand.Arguments.Build();

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
                Log.Output($"CliWrap: Successfully executed {cliWrapCommand.App}; Exit code: {result.ExitCode}; Start time: {result.StartTime}; Exit time: {result.ExitTime}{stdoutLine}{stderrLine};");

                // StandardError
                if (result.ExitCode == 0) return true;

                string showCommand = !string.IsNullOrEmpty(cliWrapCommand.App) ? $"\n\n» Executed command:\n{cliWrapCommand.App} {cliWrapCommand.Arguments?.Build()}" : "";
                string showWorkingDir = !string.IsNullOrEmpty(cliWrapCommand.WorkingDir)
                    ? $"\n\n» Working directory: {cliWrapCommand.WorkingDir}"
                    : "";
                string showExitCode = !double.IsNaN(result.ExitCode) ? $"\n\n» Exit code: {result.ExitCode}" : "";
                string showError = !string.IsNullOrEmpty(stderr) ? $"\n\n» Error:\n{stderr}" : "";
                string info = $"{showCommand}{showWorkingDir}{showExitCode}{showError}";

                switch (result.ExitCode)
                {
                    case 3010:
                    {
                        Log.Output($"{cliWrapCommand.App} {result.ExitCode}");
                        return false;
                    }

                    case 5:
                        Log.Output($"CliWrap: {cliWrapCommand.App} installed. Exit code: {result.ExitCode}\nThe requested operation is successful. Changes will not be effective until the system is rebooted");
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
