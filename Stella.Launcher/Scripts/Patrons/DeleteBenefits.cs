using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using StellaLauncher.Forms;

namespace StellaLauncher.Scripts.Patrons
{
	internal static class DeleteBenefits
	{
		public static async void Start()
		{
			// Delete presets
			string presets = Path.Combine(Default.ResourcesPath, "ReShade", "Presets", "3. Only for patrons");
			if (Directory.Exists(presets)) await DeleteDirectory(presets);

			// Delete addons
			string addons = Path.Combine(Default.ResourcesPath, "ReShade", "Addons");
			if (Directory.Exists(addons))
			{
				string[] cmdFilesToDelete = { "ReshadeEffectShaderToggler.addon64", "version.json" };
				DeleteFiles(addons, cmdFilesToDelete);
			}

			// Delete 3DMigoto files
			string migotoDir = Path.Combine(Default.ResourcesPath, "3DMigoto");
			string[] filesToDelete = { "d3d11.dll", "d3dcompiler_46.dll", "loader.exe", "nvapi64.dll", "3dmigoto.dll" };
			DeleteFiles(migotoDir, filesToDelete);

			// Delete 3DMigoto default mod pack
			string migotoCharsMods = Path.Combine(Default.ResourcesPath, "3DMigoto", "Mods", "1. Characters");
			if (Directory.Exists(migotoCharsMods)) await DeleteDirectory(migotoCharsMods);
			string migotoOtherMods = Path.Combine(Default.ResourcesPath, "3DMigoto", "Mods", "2. Other");
			if (Directory.Exists(migotoOtherMods)) await DeleteDirectory(migotoOtherMods);

			// Delete cmd (batch) files
			string cmdPath = Path.Combine(Program.AppPath, "data", "cmd", "patrons");
			if (Directory.Exists(cmdPath)) await DeleteDirectory(cmdPath);


			// Update ReShade.ini config
			await RsConfig.Prepare();
		}

		// Delete specific files in a folder
		private static void DeleteFiles(string folderPath, IEnumerable<string> filesToDelete)
		{
			Program.Logger.Info($"Deleting files in folder: {folderPath}");

			try
			{
				foreach (string fileName in filesToDelete)
				{
					string filePath = Path.Combine(folderPath, fileName);

					if (File.Exists(filePath))
					{
						File.Delete(filePath);
						Program.Logger.Info($"Deleted file: {fileName}");
					}
					else
					{
						Program.Logger.Info($"File not found: {fileName}");
					}
				}
			}
			catch (Exception ex)
			{
				Program.Logger.Info($"An error occurred while deleting files in folder: {Path.GetDirectoryName(folderPath)}");
				Program.Logger.Error(ex.ToString());
			}
		}

		// Delete directory & content
		public static async Task DeleteDirectory(string directoryPath)
		{
			try
			{
				await Task.Run(() => { Directory.Delete(directoryPath, true); });

				Program.Logger.Info($"Deleted directory: {directoryPath}");
			}
			catch (Exception ex)
			{
				Program.Logger.Info($"An error occurred while deleting folder: {Path.GetDirectoryName(directoryPath)}");

				Program.Logger.Error(ex.ToString());
			}
		}
	}
}
