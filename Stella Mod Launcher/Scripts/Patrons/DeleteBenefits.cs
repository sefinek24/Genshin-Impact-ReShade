using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Microsoft.Win32;
using StellaLauncher.Forms;

namespace StellaLauncher.Scripts.Patrons
{
    internal static class DeleteBenefits
    {
        public static async void Run()
        {
            // Delete presets for patrons
            string presets = Path.Combine(Default.ResourcesPath, "ReShade", "Presets", "3. Only for patrons");
            if (Directory.Exists(presets)) DeleteDirectory(presets);

            // Delete addons for patrons
            string addons = Path.Combine(Default.ResourcesPath, "ReShade", "Addons");
            if (Directory.Exists(addons)) DeleteDirectory(addons);

            // Delete 3DMigoto files
            string migotoDir = Path.Combine(Default.ResourcesPath, "3DMigoto");
            string[] filesToDelete = { "d3d11.dll", "d3dcompiler_46.dll", "loader.exe", "nvapi64.dll" };
            DeleteFiles(migotoDir, filesToDelete);

            // Delete cmd files: data/cmd
            string cmdPath = Path.Combine(Program.AppPath, "data", "cmd");
            if (Directory.Exists(cmdPath))
            {
                string[] cmdFilesToDelete = { "run.cmd", "run-patrons.cmd" };
                DeleteFiles(cmdPath, cmdFilesToDelete);

                // Delete: data/cmd/start
                string cmdDir = Path.Combine(cmdPath, "start");
                if (Directory.Exists(cmdDir)) DeleteDirectory(cmdDir);
            }

            // Delete registry value for patrons
            DeleteRegistryValue();

            // Update ReShade.ini config
            await RsConfig.Prepare();
        }


        // Delete specific files in a folder
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
                Log.Output($"An error occurred while deleting files in folder: {Path.GetDirectoryName(folderPath)}");
                Log.SaveError(ex.ToString());
            }
        }

        // Delete a directory and its contents
        private static void DeleteDirectory(string directoryPath)
        {
            Log.Output($"Deleting files in folder: {directoryPath}");

            try
            {
                Directory.Delete(directoryPath, true);
            }
            catch (Exception ex)
            {
                Log.Output($"An error occurred while deleting folder: {Path.GetDirectoryName(directoryPath)}");
                Log.SaveError(ex.ToString());
            }
        }

        // Delete registry key for patrons. Written by ChatGPT.
        private static void DeleteRegistryValue()
        {
            const string secret = "Secret";

            try
            {
                string registryKeyPath = Secret.RegistryKeyPath;
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryKeyPath, true))
                {
                    if (key != null)
                    {
                        object value = key.GetValue(secret);
                        if (value != null)
                        {
                            key.DeleteValue(secret);
                            Log.Output($"Deleted REG_SZ value '{secret}' from the registry.");
                        }
                        else
                        {
                            Log.Output($"REG_SZ value '{secret}' not found in the registry.");
                        }
                    }
                    else
                    {
                        Log.Output($"Registry key '{secret}' not found.");
                    }
                }
            }
            catch (SecurityException securityEx)
            {
                Log.Output($"Permission error while deleting registry value '{secret}': {securityEx.Message}");
                Log.SaveError(securityEx.ToString());
            }
            catch (IOException ioEx)
            {
                Log.Output($"I/O error while deleting registry value '{secret}': {ioEx.Message}");
                Log.SaveError(ioEx.ToString());
            }
            catch (Exception ex)
            {
                Log.Output($"An error occurred while deleting registry value '{secret}': {ex.Message}");
                Log.SaveError(ex.ToString());
            }
        }
    }
}
