using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using StellaLauncher.Forms;
using StellaLauncher.Models;

namespace StellaLauncher.Scripts.Patrons
{
    internal static class CheckForUpdatesOfBenefits
    {
        public static async Task<int> Analyze()
        {
            BenefitVersions versions = await GetVersions();


            // Mods
            string modsVersionPath = Path.Combine(Default.ResourcesPath, "3DMigoto", "version.json");
            if (File.Exists(modsVersionPath))
            {
                string modsJson = File.ReadAllText(modsVersionPath);
                BenefitsJsonVersion modsJsonConverted = JsonConvert.DeserializeObject<BenefitsJsonVersion>(modsJson);
                if (versions.Message.Resources.Mods != modsJsonConverted.Version)
                {
                    Default._version_LinkLabel.Text = $@"v{modsJsonConverted.Version} → v{versions.Message.Resources.Mods}";

                    MessageBox.Show(
                        $"New 3DMigoto mod updates for patrons detected! Your current mods will NOT be removed.\n\nClick OK to proceed with the update.\n\nYour version: v{modsJsonConverted.Version} from {modsJsonConverted.Date}\nNew version: v{versions.Message.Resources.Mods}",
                        Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    UpdateBenefits.Download("3dmigoto", $"3DMigoto mods update - v{versions.Message.Resources.Mods}.zip", Path.GetDirectoryName(modsVersionPath));
                    return 1;
                }
            }
            else
            {
                UpdateBenefits.Download("3dmigoto", $"3DMigoto mods - v{versions.Message.Resources.Mods}.zip", Path.GetDirectoryName(modsVersionPath));
                return 1;
            }


            // Addons
            string addonsVersionPath = Path.Combine(Default.ResourcesPath, "ReShade", "Addons", "version.json");
            if (File.Exists(addonsVersionPath))
            {
                string addonsJson = File.ReadAllText(addonsVersionPath);
                BenefitsJsonVersion addonsJsonConverted = JsonConvert.DeserializeObject<BenefitsJsonVersion>(addonsJson);
                if (versions.Message.Resources.Addons != addonsJsonConverted.Version)
                {
                    Default._version_LinkLabel.Text = $@"v{addonsJsonConverted.Version} → v{versions.Message.Resources.Addons}";

                    MessageBox.Show(
                        $"The new update for ReShade addons is available! Click the OK button to proceed with the update.\n\nCurrent version: v{addonsJsonConverted.Version} from {addonsJsonConverted.Date}\nNew version: v{versions.Message.Resources.Addons}",
                        Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    UpdateBenefits.Download("addons", $"Addons update - v{versions.Message.Resources.Addons}.zip", Path.GetDirectoryName(addonsVersionPath));
                    return 1;
                }
            }
            else
            {
                UpdateBenefits.Download("addons", $"Addons - v{versions.Message.Resources.Addons}.zip", Path.GetDirectoryName(addonsVersionPath));
                return 1;
            }


            // Presets
            string presetsVersionPath = Path.Combine(Default.ResourcesPath, "ReShade", "Presets", "3. Only for patrons", "version.json");
            if (File.Exists(presetsVersionPath))
            {
                string presetsJson = File.ReadAllText(presetsVersionPath);
                BenefitsJsonVersion presetsJsonConverted = JsonConvert.DeserializeObject<BenefitsJsonVersion>(presetsJson);
                if (versions.Message.Resources.Presets != presetsJsonConverted.Version)
                {
                    Default._version_LinkLabel.Text = $@"v{presetsJsonConverted.Version} → v{versions.Message.Resources.Presets}";

                    MessageBox.Show(
                        $"A new version of the Presets is available.\n\nCurrent version: v{presetsJsonConverted.Version} from {presetsJsonConverted.Date}\nNew version: v{versions.Message.Resources.Presets}",
                        Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    UpdateBenefits.Download("presets", $"Presets update - v{versions.Message.Resources.Presets}.zip", Path.GetDirectoryName(presetsVersionPath));
                    return 1;
                }
            }
            else
            {
                UpdateBenefits.Download("presets", $"Presets - v{versions.Message.Resources.Presets}.zip", Path.GetDirectoryName(presetsVersionPath));
                return 1;
            }


            // Shaders
            string shadersVersionPath = Path.Combine(Default.ResourcesPath, "ReShade", "Shaders", "version.json");
            if (File.Exists(shadersVersionPath))
            {
                string shadersJson = File.ReadAllText(shadersVersionPath);
                BenefitsJsonVersion shadersJsonConverted = JsonConvert.DeserializeObject<BenefitsJsonVersion>(shadersJson);
                if (versions.Message.Resources.Shaders != shadersJsonConverted.Version)
                {
                    Default._version_LinkLabel.Text = $@"v{shadersJsonConverted.Version} → v{versions.Message.Resources.Shaders}";

                    MessageBox.Show(
                        $"A new version of the Shaders is available.\n\nCurrent version: v{shadersJsonConverted.Version} from {shadersJsonConverted.Date}\nNew version: v{versions.Message.Resources.Shaders}",
                        Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    UpdateBenefits.Download("shaders", $"Shaders update - v{versions.Message.Resources.Shaders}.zip", Path.GetDirectoryName(shadersVersionPath));
                    return 1;
                }
            }
            else
            {
                UpdateBenefits.Download("shaders", $"Shaders - v{versions.Message.Resources.Shaders}.zip", Path.GetDirectoryName(shadersVersionPath));
                return 1;
            }


            // Cmd files
            string cmdVersionPath = Path.Combine(Program.AppPath, "data", "cmd", "version.json");
            if (File.Exists(cmdVersionPath))
            {
                string cmdJson = File.ReadAllText(cmdVersionPath);
                BenefitsJsonVersion cmdJsonConverted = JsonConvert.DeserializeObject<BenefitsJsonVersion>(cmdJson);
                if (versions.Message.Resources.Cmd != cmdJsonConverted.Version)
                {
                    Default._version_LinkLabel.Text = $@"v{cmdJsonConverted.Version} → v{versions.Message.Resources.Cmd}";

                    MessageBox.Show(
                        $"A new version of the CMD files is available.\n\nCurrent version: v{cmdJsonConverted.Version} from {cmdJsonConverted.Date}\nNew version: v{versions.Message.Resources.Cmd}",
                        Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    UpdateBenefits.Download("cmd", $"Batch files update - {versions.Message.Resources.Cmd}.zip", Path.GetDirectoryName(cmdVersionPath));
                    return 1;
                }
            }
            else
            {
                UpdateBenefits.Download("cmd", $"Batch files - {versions.Message.Resources.Cmd}.zip", Path.GetDirectoryName(cmdVersionPath));
                return 1;
            }


            return 0;
        }

        private static async Task<BenefitVersions> GetVersions()
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add("Authorization", $"Bearer {Secret.BearerToken}");
                    webClient.Headers.Add("User-Agent", Program.UserAgent);

                    string jsonResponse = await webClient.DownloadStringTaskAsync($"{Program.WebApi}/genshin-stella-mod/patrons/benefits/version");
                    return JsonConvert.DeserializeObject<BenefitVersions>(jsonResponse);
                }
            }
            catch (WebException webEx)
            {
                if (webEx.Response is HttpWebResponse response)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string errorResponse = await reader.ReadToEndAsync();
                        MessageBox.Show($@"An error occurred: {errorResponse}", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Log.ErrorAndExit(new Exception(errorResponse));
                    }
                }
                else
                {
                    MessageBox.Show($@"An unrecoverable error occurred: {webEx.Message}", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.ErrorAndExit(webEx);
                }

                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An unrecoverable error occurred while communicating with the API interface in Warsaw, Poland. The application must be closed immediately.\n\n{(ex.InnerException != null ? ex.InnerException : ex)}",
                    Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                Log.ErrorAndExit(ex);
                return null;
            }
        }
    }
}
