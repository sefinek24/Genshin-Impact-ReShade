using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;
using StellaLauncher.Forms;

namespace StellaLauncher.Scripts.Patrons
{
    internal static class DeleteBenefits
    {
        public static async Task RunAsync()
        {
            // Delete files for patrons
            string presets = Path.Combine(Default.ResourcesPath, "ReShade", "Presets", "3. Only for patrons");
            if (Directory.Exists(presets)) await DeleteDirectoryAsync(presets);
            string addons = Path.Combine(Default.ResourcesPath, "ReShade", "Addons");
            if (Directory.Exists(addons)) await DeleteDirectoryAsync(addons);

            // Delete 3DMigoto files
            string migotoDir = Path.Combine(Default.ResourcesPath, "3DMigoto");
            string[] filesToDelete = { "d3d11.dll", "d3dcompiler_46.dll", "loader.exe", "nvapi64.dll" };
            DeleteFiles(migotoDir, filesToDelete);

            // Delete cmd files
            string cmdPath = Path.Combine(Program.AppPath, "data", "cmd");
            string[] cmdFilesToDelete = { "run.cmd", "run-patrons.cmd" };
            DeleteFiles(cmdPath, cmdFilesToDelete);

            // Delete key
            DeleteRegistryKey();
        }

        private static void DeleteFiles(string folderPath, IEnumerable<string> filesToDelete)
        {
            Log.Output($"Deleting files in folder: {folderPath}");

            try
            {
                foreach (string fileName in filesToDelete)
                {
                    string filePath = Path.Combine(folderPath, fileName);

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        Log.Output($"Deleted file: {fileName}");
                    }
                    else
                    {
                        Log.Output($"File not found: {fileName}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Output($"An error occurred while deleting files in folder: {folderPath}");
                Log.SaveError(ex.ToString());
            }
        }

        private static async Task DeleteDirectoryAsync(string directoryPath)
        {
            string[] files = Directory.GetFiles(directoryPath);
            string[] subDirectories = Directory.GetDirectories(directoryPath);

            List<Task> deleteTasks = new List<Task>();

            foreach (string file in files) deleteTasks.Add(Task.Run(() => File.Delete(file)));
            foreach (string subDirectory in subDirectories) deleteTasks.Add(DeleteDirectoryAsync(subDirectory));

            await Task.WhenAll(deleteTasks);

            Log.Output($"Deleting folder: {directoryPath}");
            Directory.Delete(directoryPath, true);
        }

        private static void DeleteRegistryKey()
        {
            const string secret = "Secret";

            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Secret.RegistryKeyPath, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue(secret);
                        Log.Output($"Deleted key `{secret}` from the registry.");
                    }
                    else
                    {
                        Log.Output($"Registry key `{secret}` not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Output($"An error occurred while deleting registry key `{secret}`.");
                Log.SaveError(ex.ToString());
            }
        }
    }
}
