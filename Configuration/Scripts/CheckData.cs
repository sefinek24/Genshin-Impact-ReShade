using Microsoft.Win32;

namespace Configuration.Scripts
{
    internal static class CheckData
    {
        private const string RegistryPath = @"Software\Stella Mod Launcher";

        public static bool IsUserMyPatron()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath))
            {
                if (key == null) return false;

                string data = (string)key.GetValue("Secret");
                return !string.IsNullOrEmpty(data);
            }
        }
    }
}
