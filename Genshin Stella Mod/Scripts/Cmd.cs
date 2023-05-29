using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Toolkit.Uwp.Notifications;
using StellaLauncher;
using StellaLauncher.Forms;

namespace Genshin_Stella_Mod.Scripts
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
                    MessageBox.Show(
                        "Update is required.\n\nI apologize for any inconvenience caused, but it is necessary to install every update. Each update may contain quality improvements, and some updates might even include important security fixes.\n\nIf, for any reason, you are unable to update Stella Mod through the launcher, please uninstall the previous version and reinstall it from the official website.",
                        Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Log.Output("CliWrap: Command execution failed. An update is required.");
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
                Log.Output($"CliWrap: Successfully executed {app}; Exit code: {result.ExitCode}; Start time: {result.StartTime}; Exit time: {result.ExitTime}{stdoutLine}{stderrLine};");

                // StandardError
                if (result.ExitCode == 0) return true;

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
                                .AddText("Reboot is required 📄")
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
                        return false;
                    }

                    case 5:
                        const string mainInfo = "Failed to update. The software was denied access to a location, preventing it from saving, copying, opening or loading files.";
                        MessageBox.Show(mainInfo, Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        Log.SaveErrorLog(new Exception($"{mainInfo}\nRestart your computer or suspend antivirus program and try again.{info}"));
                        return false;

                    default:
                    {
                        if (!downloadSetup)
                            Log.ErrorAndExit(new Exception($"Command execution failed because the underlying process ({app}) returned a non-zero exit code - {result.ExitCode}.\n{info}"));
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
