using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using StellaLauncher.Forms;
using StellaLauncher.Models;

namespace StellaLauncher.Scripts.Patrons
{
    internal static class CheckForUpdatesOfBenefits
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public static async Task<int> Analyze()
        {
            BenefitVersions versions = await GetVersions();

            // Mods
            string modsVersionPath = Path.Combine(Default.ResourcesPath, "3DMigoto", "version.json");
            string modsJson = File.ReadAllText(modsVersionPath);
            BenefitsJsonVersion modsJsonConverted = JsonConvert.DeserializeObject<BenefitsJsonVersion>(modsJson);
            if (versions.Message.Resources.Mods != modsJsonConverted.Version)
            {
                Default._version_LinkLabel.Text = $@"v{modsJsonConverted.Version} → v{versions.Message.Resources.Mods}";

                MessageBox.Show(
                    $"New 3DMigoto mod updates for patrons detected! Your current mods will NOT be removed.\n\nClick OK to proceed with the update.\n\nNew version: v{versions.Message.Resources.Mods}\nYour version: v{modsJsonConverted.Version} from {modsJsonConverted.Date}\n",
                    Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                UpdateBenefits.Download("3DMigoto mods.zip", "3dmigoto");
                return 1;
            }

            // Addons
            string addonsVersionPath = Path.Combine(Default.ResourcesPath, "ReShade", "Addons", "version.json");
            string addonsJson = File.ReadAllText(addonsVersionPath);
            BenefitsJsonVersion addonsJsonConverted = JsonConvert.DeserializeObject<BenefitsJsonVersion>(addonsJson);
            if (versions.Message.Resources.Addons != addonsJsonConverted.Version)
            {
                Default._version_LinkLabel.Text = $@"v{addonsJsonConverted.Version} → v{versions.Message.Resources.Addons}";

                MessageBox.Show(
                    $"A new version of the Addons is available.\n\nCurrent version: {addonsJsonConverted.Version}\nNew version: {versions.Message.Resources.Addons}",
                    Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                UpdateBenefits.Download("Addons.zip", "addons");
                return 1;
            }

            // Presets
            string presetsVersionPath = Path.Combine(Default.ResourcesPath, "ReShade", "Presets", "3. Only for patrons", "version.json");
            string presetsJson = File.ReadAllText(presetsVersionPath);
            BenefitsJsonVersion presetsJsonConverted = JsonConvert.DeserializeObject<BenefitsJsonVersion>(presetsJson);
            if (versions.Message.Resources.Presets != presetsJsonConverted.Version)
            {
                Default._version_LinkLabel.Text = $@"v{presetsJsonConverted.Version} → v{versions.Message.Resources.Presets}";

                MessageBox.Show(
                    $"A new version of the Presets is available.\n\nCurrent version: {presetsJsonConverted.Version}\nNew version: {versions.Message.Resources.Presets}",
                    Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                UpdateBenefits.Download("Presets.zip", "presets");
                return 1;
            }


            // Shaders
            string shadersVersionPath = Path.Combine(Default.ResourcesPath, "ReShade", "Shaders", "version.json");
            string shadersJson = File.ReadAllText(shadersVersionPath);
            BenefitsJsonVersion shadersJsonConverted = JsonConvert.DeserializeObject<BenefitsJsonVersion>(shadersJson);
            if (versions.Message.Resources.Shaders != shadersJsonConverted.Version)
            {
                Default._version_LinkLabel.Text = $@"v{shadersJsonConverted.Version} → v{versions.Message.Resources.Shaders}";

                MessageBox.Show(
                    $"A new version of the Shaders is available.\n\nCurrent version: {shadersJsonConverted.Version}\nNew version: {versions.Message.Resources.Shaders}",
                    Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                UpdateBenefits.Download("Shaders.zip", "shaders");
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
                    webClient.Headers.Add("Authorization", $"Bearer {Secret.JwtToken}");
                    webClient.Headers.Add("User-Agent", Program.UserAgent);

                    string jsonResponse = await webClient.DownloadStringTaskAsync($"{Program.WebApi}/genshin-stella-mod/patrons/benefits/version");

                    return JsonConvert.DeserializeObject<BenefitVersions>(jsonResponse);
                }
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
