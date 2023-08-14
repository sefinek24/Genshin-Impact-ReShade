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
        private static readonly string MigotoDir = Path.Combine(Default.ResourcesPath, "3DMigoto", "");

        public static async Task RunAsync()
        {
            DeleteRegistryKey();

            // Delete presets for patrons
            if (Directory.Exists(Program.PresetsPatronsPath)) await DeleteDirectoryAsync(Program.PresetsPatronsPath);

            // Delete 3DMigoto files
            string[] filesToDelete = { "d3d11.dll", "d3dcompiler_46.dll", "loader.exe", "nvapi64.dll" };
            DeleteFilesInFolder(MigotoDir, filesToDelete);
        }

        private static void DeleteFilesInFolder(string folderPath, string[] filesToDelete)
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

            Directory.Delete(directoryPath);
            Log.Output($"Deleted folder: {directoryPath}");
        }

        private static void DeleteRegistryKey()
        {
            string keyPath = $"{Secret.RegistryKeyPath}/Secret";

            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath, true))
                {
                    if (key == null) return;

                    foreach (string subKeyName in key.GetSubKeyNames()) key.DeleteSubKeyTree(subKeyName);

                    Registry.LocalMachine.DeleteSubKeyTree(keyPath);
                    Log.Output($"Deleted key: {keyPath}");
                }
            }
            catch (Exception ex)
            {
                Log.ThrowError(ex);
            }
        }
    }
}
