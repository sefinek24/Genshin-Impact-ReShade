using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace StellaLauncher.Scripts
{
    internal static class Secret
    {
        private const string RegistryKeyPath = @"SOFTWARE\Stella Mod Launcher";
        public static bool IsMyPatron = false;

        public static string GetTokenFromRegistry()
        {
            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
            {
                object value = registryKey?.GetValue("Secret");
                if (value is string token) return token;
            }

            return null;
        }


        public static async Task<string> VerifyToken(string mainPcKey)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
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
                    HttpResponseMessage response = await httpClient.PostAsync($"{Program.WebApi}/genshin-stella-mod/access/launcher/verify", content).ConfigureAwait(false);

                    string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Log.Output(json);
                    return json;
                }
            }
            catch (Exception ex)
            {
                Log.SaveError(ex.ToString());

                return null;
            }
        }
    }
}
