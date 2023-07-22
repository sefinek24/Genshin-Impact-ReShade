using System;
using System.Diagnostics;
using System.IO;
using StellaLauncher.Forms;
using StellaLauncher.Properties;
using StellaLauncher.Scripts.Download;

namespace StellaLauncher.Scripts.Forms.MainForm
{
    internal static class Files
    {
        public static void Scan(string formName)
        {
            Default._version_LinkLabel.Text = $@"v{Program.AppVersion}";
            Log.Output(string.Format(Resources.Main_LoadedForm_, formName));

            if (!File.Exists(Program.FpsUnlockerExePath) && !Debugger.IsAttached)
                Default._status_Label.Text += $"[x]: {string.Format(Resources.Default_File_WasNotFound, Program.FpsUnlockerExePath)}\n";

            if (!File.Exists(Program.InjectorPath) && !Debugger.IsAttached)
                Default._status_Label.Text += $"[x]: {string.Format(Resources.Default_File_WasNotFound, Program.InjectorPath)}\n";

            if (!File.Exists(Program.ReShadePath) && !Debugger.IsAttached)
                Default._status_Label.Text += $"[x]: {string.Format(Resources.Default_File_WasNotFound, Program.ReShadePath)}\n";

            if (!File.Exists(Program.FpsUnlockerCfgPath) && !Debugger.IsAttached) FpsUnlockerCfg.Run(Default._status_Label);

            if (Default._status_Label.Text.Length > 0) Log.SaveError(Default._status_Label.Text);
        }

        public static void DeleteSetup()
        {
            if (!File.Exists(NormalRelease.SetupPathExe)) return;

            try
            {
                File.Delete(NormalRelease.SetupPathExe);
                Default._status_Label.Text += $"[i] {Resources.Default_DeletedOldSetupFromTempDirectory}\n";
                Log.Output(string.Format(Resources.Default_DeletedOldSetupFromTempFolder, NormalRelease.SetupPathExe));
            }
            catch (Exception ex)
            {
                Default._status_Label.Text += $"[x] {ex.Message}\n";
                Log.SaveError(ex.ToString());
            }
        }
    }
}
