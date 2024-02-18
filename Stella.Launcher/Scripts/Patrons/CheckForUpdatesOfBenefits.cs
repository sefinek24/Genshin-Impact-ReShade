using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using StellaLauncher.Forms;
using StellaLauncher.Models;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts.Patrons
{
	internal static class CheckForUpdatesOfBenefits
	{
		public static async Task<int> Analyze()
		{
			BenefitVersions remoteVersions = await GetVersions();


			// 3DMigoto
			string migotoVerPath = Path.Combine(Default.ResourcesPath, "3DMigoto", "3dmigoto-version.json");
			if (File.Exists(migotoVerPath))
			{
				string migotoJson = File.ReadAllText(migotoVerPath);
				LocalBenefitsVersion migotoJsonConverted = JsonConvert.DeserializeObject<LocalBenefitsVersion>(migotoJson);

				if (remoteVersions.Message.Resources.Migoto != migotoJsonConverted.Version)
				{
					Default._version_LinkLabel.Text = $@"v{migotoJsonConverted.Version} → v{remoteVersions.Message.Resources.Migoto}";

					MessageBox.Show(
						$"The new update for 3DMigoto has been detected. This update will not affect any downloaded mods.\n\nClick OK to proceed with the update.\n\nYour version: v{migotoJsonConverted.Version} from {migotoJsonConverted.Date}\nNew version: v{remoteVersions.Message.Resources.Migoto}",
						Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Information);

					UpdateBenefits.Download("3dmigoto", $"3DMigoto Update - v{remoteVersions.Message.Resources.Migoto}.zip", Path.GetDirectoryName(migotoVerPath));
					return 1;
				}
			}
			else
			{
				UpdateBenefits.Download("3dmigoto", $"3DMigoto Software - v{remoteVersions.Message.Resources.Migoto}.zip", Path.GetDirectoryName(migotoVerPath));
				return 1;
			}


			// Mods for 3DMigoto
			string modsVerPath = Path.Combine(Default.ResourcesPath, "3DMigoto", "mods-version.json");
			if (File.Exists(modsVerPath))
			{
				string modsJson = File.ReadAllText(modsVerPath);
				LocalBenefitsVersion modsJsonConverted = JsonConvert.DeserializeObject<LocalBenefitsVersion>(modsJson);
				if (remoteVersions.Message.Resources.Mods != modsJsonConverted.Version)
				{
					Default._version_LinkLabel.Text = $@"v{modsJsonConverted.Version} → v{remoteVersions.Message.Resources.Mods}";

					MessageBox.Show(
						$"A new version of the default mod pack for Stella Mod Plus subscribers has been detected. Your custom mods will not be deleted, provided they are in folder number 3. The software creator is not responsible for accidental deletion of mods.\nIf character models seem strange after the update, remove the default mod from the package. Each character can have only 1 mod. In any case, if you encounter any problems, visit the Genshin Stella Mod Discord server and ask for help.\n\nClick the OK button to continue.\n\nYour version: v{modsJsonConverted.Version} from {modsJsonConverted.Date}\nNew version: v{remoteVersions.Message.Resources.Mods}",
						Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Information);

					// Delete old mods
					Default._preparingPleaseWait.Text = Resources.StellaResources_DeletingThePreviousVersionOfTheModPack;
					string migotoCharsMods = Path.Combine(Default.ResourcesPath, "3DMigoto", "Mods", "1. Characters");
					if (Directory.Exists(migotoCharsMods)) await DeleteBenefits.DeleteDirectory(migotoCharsMods);
					string migotoOtherMods = Path.Combine(Default.ResourcesPath, "3DMigoto", "Mods", "2. Other");
					if (Directory.Exists(migotoOtherMods)) await DeleteBenefits.DeleteDirectory(migotoOtherMods);

					// Update
					UpdateBenefits.Download("3dmigoto-mods", $"3DMigoto Mods Update - v{remoteVersions.Message.Resources.Mods}.zip", Path.GetDirectoryName(modsVerPath));
					return 1;
				}
			}


			// Addons
			string addonsVersionPath = Path.Combine(Default.ResourcesPath, "ReShade", "Addons", "version.json");
			if (File.Exists(addonsVersionPath))
			{
				string addonsJson = File.ReadAllText(addonsVersionPath);
				LocalBenefitsVersion addonsJsonConverted = JsonConvert.DeserializeObject<LocalBenefitsVersion>(addonsJson);
				if (remoteVersions.Message.Resources.Addons != addonsJsonConverted.Version)
				{
					Default._version_LinkLabel.Text = $@"v{addonsJsonConverted.Version} → v{remoteVersions.Message.Resources.Addons}";

					MessageBox.Show(
						$"The new update for ReShade addons is available!\n\nClick the OK button to proceed with the update.\n\nYour version: v{addonsJsonConverted.Version} from {addonsJsonConverted.Date}\nNew version: v{remoteVersions.Message.Resources.Addons}",
						Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Information);

					UpdateBenefits.Download("addons", $"Addons update - v{remoteVersions.Message.Resources.Addons}.zip", Path.GetDirectoryName(addonsVersionPath));
					return 1;
				}
			}
			else
			{
				UpdateBenefits.Download("addons", $"Addons - v{remoteVersions.Message.Resources.Addons}.zip", Path.GetDirectoryName(addonsVersionPath));
				return 1;
			}


			// Presets
			string presetsVersionPath = Path.Combine(Default.ResourcesPath, "ReShade", "Presets", "3. Stella Mod Plus", "version.json");
			if (File.Exists(presetsVersionPath))
			{
				string presetsJson = File.ReadAllText(presetsVersionPath);
				LocalBenefitsVersion presetsJsonConverted = JsonConvert.DeserializeObject<LocalBenefitsVersion>(presetsJson);
				if (remoteVersions.Message.Resources.Presets != presetsJsonConverted.Version)
				{
					Default._version_LinkLabel.Text = $@"v{presetsJsonConverted.Version} → v{remoteVersions.Message.Resources.Presets}";

					MessageBox.Show(
						$"A new version of presets has been detected.\n\nYour version: v{presetsJsonConverted.Version} from {presetsJsonConverted.Date}\nNew version: v{remoteVersions.Message.Resources.Presets}",
						Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Information);

					string presets = Path.Combine(Default.ResourcesPath, "ReShade", "Presets", "3. Stella Mod Plus");
					if (Directory.Exists(presets)) await DeleteBenefits.DeleteDirectory(presets);

					UpdateBenefits.Download("presets", $"Presets update - v{remoteVersions.Message.Resources.Presets}.zip", Path.GetDirectoryName(presetsVersionPath));
					return 1;
				}
			}
			else
			{
				UpdateBenefits.Download("presets", $"Presets - v{remoteVersions.Message.Resources.Presets}.zip", Path.GetDirectoryName(presetsVersionPath));
				return 1;
			}


			// Shaders
			string shadersVersionPath = Path.Combine(Default.ResourcesPath, "ReShade", "Shaders", "version.json");
			if (File.Exists(shadersVersionPath))
			{
				string shadersJson = File.ReadAllText(shadersVersionPath);
				LocalBenefitsVersion shadersJsonConverted = JsonConvert.DeserializeObject<LocalBenefitsVersion>(shadersJson);
				if (remoteVersions.Message.Resources.Shaders != shadersJsonConverted.Version)
				{
					Default._version_LinkLabel.Text = $@"v{shadersJsonConverted.Version} → v{remoteVersions.Message.Resources.Shaders}";

					MessageBox.Show(
						$"New version of shaders is available!\n\nYour version: v{shadersJsonConverted.Version} from {shadersJsonConverted.Date}\nNew version: v{remoteVersions.Message.Resources.Shaders}",
						Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Information);

					UpdateBenefits.Download("shaders", $"Shaders update - v{remoteVersions.Message.Resources.Shaders}.zip", Path.GetDirectoryName(shadersVersionPath));
					return 1;
				}
			}
			else
			{
				UpdateBenefits.Download("shaders", $"Shaders - v{remoteVersions.Message.Resources.Shaders}.zip", Path.GetDirectoryName(shadersVersionPath));
				return 1;
			}


			// Cmd files
			string cmdVersionPath = Path.Combine(Program.AppPath, "data", "cmd", "patrons", "version.json");
			if (File.Exists(cmdVersionPath))
			{
				string cmdJson = File.ReadAllText(cmdVersionPath);
				LocalBenefitsVersion cmdJsonConverted = JsonConvert.DeserializeObject<LocalBenefitsVersion>(cmdJson);
				if (remoteVersions.Message.Resources.Cmd != cmdJsonConverted.Version)
				{
					Default._version_LinkLabel.Text = $@"v{cmdJsonConverted.Version} → v{remoteVersions.Message.Resources.Cmd}";

					MessageBox.Show(
						$"A new version of the CMD files is available!\n\nYour version: v{cmdJsonConverted.Version} from {cmdJsonConverted.Date}\nNew version: v{remoteVersions.Message.Resources.Cmd}",
						Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Information);

					string cmdPath = Path.Combine(Program.AppPath, "data", "cmd", "patrons");
					if (Directory.Exists(cmdPath)) await DeleteBenefits.DeleteDirectory(cmdPath);

					UpdateBenefits.Download("cmd", $"Batch files update - {remoteVersions.Message.Resources.Cmd}.zip", Path.GetDirectoryName(cmdVersionPath));
					return 1;
				}
			}
			else
			{
				UpdateBenefits.Download("cmd", $"Batch files - {remoteVersions.Message.Resources.Cmd}.zip", Path.GetDirectoryName(cmdVersionPath));
				return 1;
			}


			return 0;
		}

		private static async Task<BenefitVersions> GetVersions()
		{
			try
			{
				Program.SefinWebClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Secret.BearerToken);

				string jsonResponse = await Program.SefinWebClient.GetStringAsync($"{Program.WebApi}/genshin-stella-mod/patrons/benefits/version");
				return JsonConvert.DeserializeObject<BenefitVersions>(jsonResponse);
			}
			catch (WebException webEx)
			{
				if (webEx.Response is HttpWebResponse response)
				{
					using (StreamReader reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
					{
						string errorResponse = await reader.ReadToEndAsync();
						MessageBox.Show($@"An error occurred: {errorResponse}", Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Error);
						Log.ErrorAndExit(new Exception(errorResponse), false);
					}
				}
				else
				{
					string msg = $@"An unrecoverable error occurred: {webEx.Message}";
					MessageBox.Show(msg, Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Error);
					Program.Logger.Error(msg);
				}

				return null;
			}
			catch (Exception ex)
			{
				string msg = $"An unrecoverable error occurred while communicating with the API interface. The application must be closed immediately.\n\n{ex.InnerException ?? ex}";
				Program.Logger.Error(msg);
				MessageBox.Show(msg, Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Error);

				Environment.Exit(23451);
				return null;
			}
		}
	}
}
