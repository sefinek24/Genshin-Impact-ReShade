using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using IWshRuntimeLibrary;
using File = System.IO.File;

namespace PrepareStella.Scripts.Preparing
{
    internal static class TerminalInstallation
    {
        public static async Task RunAsync()
        {
            string wtAppDataLocal = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Windows Terminal");
            if (!Directory.Exists(wtAppDataLocal))
            {
                Console.WriteLine(@"Config folder not found");
            }
            else
            {
                string wtSettingsJson = Path.Combine(wtAppDataLocal, "settings.json");
                string wtStateJson = Path.Combine(wtAppDataLocal, "state.json");
                string readMeTxt = Path.Combine(wtAppDataLocal, "README.txt");
                Start.Logger.Info($"Files and directories of backup.\n» wtAppDataLocal: {wtAppDataLocal}\n» wtSettingsJson: {wtSettingsJson}\n» wtStateJson: {wtStateJson}\n» readMeTxt: {readMeTxt}");

                string localShortcut = Path.Combine(wtAppDataLocal, "WT Backup Folder.lnk");
                if (File.Exists(localShortcut)) File.Delete(localShortcut);

                string stellaWtBkp = Path.Combine(Start.AppData, "Windows Terminal");
                try
                {
                    using (StreamWriter sw = File.CreateText(readMeTxt))
                    {
                        await sw.WriteAsync(
                            $"⠀   ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀\n⠀  ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿\n⠀   ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                           Genshin Impact Stella Mod Pack 2023\n   ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿                                     Made by Sefinek\n⠀   ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿\n    ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟\n⠀   ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀\n ⠀   ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀\n  ⠀ ⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆\n    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆\n   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇\n  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟\n   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟\n=========================================================================================\n» Windows Terminal application configuration backup files from {DateTime.Now}.");
                    }

                    string zipFile = Path.Combine(Start.AppData, "Windows Terminal", $"wt-config.backup-{DateTime.Now:HHmm_dd.MM.yyyy}.zip");
                    if (!Directory.Exists(stellaWtBkp)) Directory.CreateDirectory(stellaWtBkp);
                    await Zip.CreateAsync(wtAppDataLocal, zipFile);

                    Start.Logger.Info($"The Windows Terminal application configuration files have been backed up.\n» Source: {wtAppDataLocal}\n» Backup: {zipFile}");
                }
                catch (Exception e)
                {
                    Log.ThrowError(e, false);
                }

                if (File.Exists(readMeTxt)) File.Delete(readMeTxt);

                WshShell wshShell = new WshShell();
                IWshShortcut shBkpWt = (IWshShortcut)wshShell.CreateShortcut(localShortcut);
                shBkpWt.Description = "View the backup copy of Windows Terminal created by Genshin Stella Mod.";
                shBkpWt.TargetPath = stellaWtBkp;
                shBkpWt.Save();

                Start.Logger.Info($@"Created: {Path.Combine(wtAppDataLocal, "Stella WT Backup.lnk")}");
                Console.WriteLine(@"Saved in Stella AppData.");
            }

            // Installing
            Console.WriteLine(@"Installing the latest Windows Terminal...");

            if (!File.Exists(Start.WtMsixBundle))
                Log.ErrorAndExit(new Exception($"I can't find a required file: {Start.WtMsixBundle}"), false, false);

            Process[] dllHostName = Process.GetProcessesByName("dllhost");
            if (dllHostName.Length != 0) await Cmd.CliWrap("taskkill", "/F /IM dllhost.exe", null);
            Process[] wtName = Process.GetProcessesByName("WindowsTerminal");
            if (wtName.Length != 0) await Cmd.CliWrap("taskkill", "/F /IM WindowsTerminal.exe", null);

            try
            {
                await Cmd.CliWrap("powershell", $"Add-AppxPackage -Path \"{Start.WtMsixBundle}\"", Start.AppPath);
            }
            catch (Exception ex)
            {
                Log.ThrowError(ex, false);
            }

            // Check installed WT
            Console.WriteLine(@"Checking installed software...");
            string wtProgramFiles = Utils.GetWtProgramFiles();
            if (string.IsNullOrEmpty(wtProgramFiles))
            {
                Log.ErrorAndExit(new Exception($"Windows Terminal directory was not found in: {Program.WindowsApps}"), false, false);
            }
            else
            {
                Start.Logger.Info($"Windows Terminal has been successfully installed in {wtProgramFiles}");

                string wtAppData2 = Utils.GetAppData();
                if (string.IsNullOrEmpty(wtAppData2))
                    Log.ErrorAndExit(new Exception("Fatal error. Code: 3781780149"), false, true);
            }
        }
    }
}
