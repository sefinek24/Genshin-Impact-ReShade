using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StellaLauncher.Forms;
using StellaLauncher.Models;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts.Patrons
{
    internal abstract class DownloadCmd
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public static readonly string RunCmdPatrons = Path.Combine(Program.AppPath, "data", "cmd", "run_patrons.cmd");
        private static readonly string ApiCheckUrl = $"{Program.WebApi}/genshin-stella-mod/patrons/sha256/static?file=run_patrons.cmd";
        private static readonly string ApiDownloadUrl = $"{Program.WebApi}/genshin-stella-mod/patrons/static/run_patrons.cmd";

        public static async Task Run()
        {
            Log.Output($"{ApiCheckUrl}\n{ApiDownloadUrl}");

            try
            {
                if (File.Exists(RunCmdPatrons))
                {
                    Log.Output($"DownloadCmd.Run(): File exists locally in {RunCmdPatrons}. Sending a request to check the hash on the server...");

                    FileHash remoteFileHash = await GetRemoteFileHash(ApiCheckUrl);
                    if (!string.IsNullOrEmpty(remoteFileHash?.Hash))
                    {
                        string localFileHash = CalculateFileHash(RunCmdPatrons);

                        if (remoteFileHash.Hash == localFileHash)
                            Log.Output($"DownloadCmd.Run(): The file hash matches. There's no need to download the file.\nLocal: {localFileHash}\nRemote: {remoteFileHash.Hash}");
                        else
                            await DownloadFile(ApiDownloadUrl);
                    }
                    else
                    {
                        Log.Output("DownloadCmd.Run(): Unable to download the file hash from the server.");
                    }
                }
                else
                {
                    await DownloadFile(ApiDownloadUrl);
                }
            }
            catch (HttpRequestException ex)
            {
                Log.SaveError($"DownloadCmd.Run(): HttpRequestException: An error occurred while communicating with the server: {ex}");
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

                Log.Output("DownloadCmd.DownloadFile(): The file has been successfully downloaded and saved to the disk.");
                Default._status_Label.Text = string.Format(Resources.DownloadCmd_SuccesfullyDownloadedOrUpdatedFile_, "[i]", Path.GetFileName(RunCmdPatrons));
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
