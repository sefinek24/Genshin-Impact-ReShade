using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StellaLauncher.Models;

namespace StellaLauncher.Scripts.Patrons
{
    internal abstract class DownloadCmd
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public static readonly string RunCmdPatrons = Path.Combine(Program.AppPath, "data", "cmd", "run_patrons.cmd");

        public static async Task Run()
        {
            string apiCheckUrl = $"{Program.WebApi}/genshin-stella-mod/patrons/sha256/static?file=run_patrons.cmd";
            string apiDownloadUrl = $"{Program.WebApi}/genshin-stella-mod/patrons/static/run_patrons.cmd";

            try
            {
                if (File.Exists(RunCmdPatrons))
                {
                    Console.WriteLine("Plik istnieje lokalnie. Wysyłanie żądania do sprawdzenia hashu na serwer...");

                    FileHash remoteFileHash = await GetRemoteFileHash(apiCheckUrl);

                    if (!string.IsNullOrEmpty(remoteFileHash?.Hash))
                    {
                        string localFileHash = CalculateFileHash(RunCmdPatrons);

                        if (remoteFileHash.Hash == localFileHash)
                            Console.WriteLine("Hash pliku się zgadza. Nie ma potrzeby pobierania pliku.");
                        else
                            await DownloadFile(apiDownloadUrl);
                    }
                    else
                    {
                        Console.WriteLine("Nie można pobrać hasha pliku z serwera.");
                    }
                }
                else
                {
                    await DownloadFile(apiDownloadUrl);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Wystąpił błąd podczas komunikacji z serwerem: {ex}");
            }
        }

        private static async Task<FileHash> GetRemoteFileHash(string apiCheckUrl)
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Secret.JwtToken);

            using (HttpResponseMessage response = await HttpClient.GetAsync(apiCheckUrl))
            {
                response.EnsureSuccessStatusCode();
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<FileHash>(json);
            }
        }

        private static async Task DownloadFile(string apiDownloadUrl)
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Secret.JwtToken);

            using (HttpResponseMessage downloadResponse = await HttpClient.GetAsync(apiDownloadUrl))
            {
                downloadResponse.EnsureSuccessStatusCode();

                byte[] fileBytes = await downloadResponse.Content.ReadAsByteArrayAsync();
                File.WriteAllBytes(RunCmdPatrons, fileBytes);

                Console.WriteLine("Plik został pomyślnie pobrany i zapisany na dysku.");
            }
        }

        private static string CalculateFileHash(string filePath)
        {
            using (SHA256 sha256 = SHA256.Create())
            using (FileStream stream = File.OpenRead(filePath))
            {
                byte[] hashBytes = sha256.ComputeHash(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
