using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PrepareStella.Models;

namespace PrepareStella.Scripts.Preparing
{
    internal static class DownloadUpdateResources
    {
        public static async void Run()
        {
            string resourcesGlobalPath = Program.ResourcesGlobal;

            if (!Directory.Exists(resourcesGlobalPath))
            {
                Directory.CreateDirectory(resourcesGlobalPath);
                Console.WriteLine($@"Created folder: {resourcesGlobalPath}");
            }
            else
            {
                Console.WriteLine($@"Found: {resourcesGlobalPath}");
            }

            // Checking current version of resources
            Console.WriteLine(@"Checking current version of resources...");
            StellaResources json;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", Program.UserAgent);
                HttpResponseMessage response = await httpClient.GetAsync("https://api.sefinek.net/api/v4/genshin-stella-mod/version/app/launcher/resources");
                string responseContent = await response.Content.ReadAsStringAsync();
                json = JsonConvert.DeserializeObject<StellaResources>(responseContent);
            }

            // Deleting existing resources zip file
            string zipPath = Path.Combine(resourcesGlobalPath, $"Stella resources - v{json.Message}.zip");

            if (File.Exists(zipPath))
            {
                Console.WriteLine($@"Deleting {zipPath}...");
                File.Delete(zipPath);
            }

            // Downloading resources zip file
            Console.WriteLine(@"Downloading resources...");

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("user-agent", Program.UserAgent);

                using (Stream stream = await httpClient.GetStreamAsync("https://github.com/sefinek24/Genshin-Stella-Resources/releases/latest/download/resources.zip"))
                using (FileStream fs = File.Create(zipPath))
                {
                    await stream.CopyToAsync(fs);
                }
            }

            // Unpacking resources
            Console.WriteLine(@"Unpacking resources...");

            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string fullPath = Path.Combine(resourcesGlobalPath, entry.FullName);

                    if (entry.Name == "")
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                        continue;
                    }

                    await Task.Run(() => entry.ExtractToFile(fullPath, true));
                }
            }
        }
    }
}
