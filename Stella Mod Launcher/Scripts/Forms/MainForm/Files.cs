using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using StellaLauncher.Forms;
using StellaLauncher.Properties;
using StellaLauncher.Scripts.Download;

namespace StellaLauncher.Scripts.Forms.MainForm
{
    internal static class Files
    {
        public static async Task ScanAsync()
        {
            await CheckFileAsync(Program.FpsUnlockerExePath);
            await CheckFileAsync(Program.InjectorPath);
            await CheckFileAsync(Program.ReShadePath);

            if (!File.Exists(Program.FpsUnlockerCfgPath) && !Debugger.IsAttached)
                await FpsUnlockerCfg.RunAsync(Default._status_Label);

            if (Default._status_Label.Text.Length > 0)
                Log.SaveError(Default._status_Label.Text);
        }

        private static async Task CheckFileAsync(string filePath)
        {
            if (!File.Exists(filePath) && !Debugger.IsAttached)
                await Task.Run(() => { Default._status_Label.Text += $"{string.Format(Resources.Default_File_WasNotFound, filePath)}\n"; });
        }

        public static async Task DeleteSetupAsync()
        {
            if (!File.Exists(NormalRelease.SetupPathExe)) return;

            try
            {
                await Task.Run(() => File.Delete(NormalRelease.SetupPathExe));
                Default._status_Label.Text += $"{Resources.Default_DeletedOldSetupFromTempDirectory}\n";
                Log.Output($"Deleted old setup file from temp folder: {NormalRelease.SetupPathExe}");
            }
            catch (Exception ex)
            {
                Default._status_Label.Text += $"[x] {ex.Message}\n";
                Log.SaveError(ex.ToString());
            }
        }
    }
}
