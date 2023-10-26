using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;
using Microsoft.Win32;

namespace StellaLauncher.Scripts
{
    internal static class Secret
    {
        public const string RegistryKeyPath = @"Software\Stella Mod Launcher";
        public static bool IsMyPatron = false;
        public static string BearerToken;

        public static string GetTokenFromRegistry()
        {
            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
            {
                object value = registryKey?.GetValue("Secret");
                if (!(value is string token)) return null;

                Log.Output("Found token for patrons in the registry.");
                return token;
            }
        }

        public static async Task<string> VerifyToken(string mainPcKey)
        {
            try
            {
                List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("token", mainPcKey),
                    new KeyValuePair<string, string>("MACAddress", ComputerInfo.GetMacAddress()),
                    new KeyValuePair<string, string>("motherboardId", ComputerInfo.GetMotherboardSerialNumber()),
                    new KeyValuePair<string, string>("cpuId", ComputerInfo.GetCpuSerialNumber()),
                    new KeyValuePair<string, string>("diskId", ComputerInfo.GetHardDriveSerialNumber())
                };

                FormUrlEncodedContent content = new FormUrlEncodedContent(postData);
                HttpResponseMessage response = await Program.SefinWebClient.PostAsync($"{Program.WebApi}/genshin-stella-mod/access/launcher/verify", content).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return json;
                }

                // TODO
                string msg =
                    $"Your subscription couldn't be verified by the server for some reason. The verification server is probably temporarily unavailable, or there might be another network error. Please try again or get in touch with Sefinek if the problem continues.\n\nHTTP Status Code: {response.StatusCode}, Reason Phrase: {response.ReasonPhrase}";

                MessageBox.Show(msg, Program.AppName, MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                Log.SaveError(msg);
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An unrecoverable error occurred while communicating with the API interface in Warsaw, Poland. The application must be closed immediately.\n\n{ex.InnerException ?? ex}",
                    Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                Log.ErrorAndExit(ex);
                return null;
            }
        }
    }
}
