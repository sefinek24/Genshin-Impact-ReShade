using System.Linq;
using System.Management;

namespace GenshinStellaMod.Scripts
{
    internal static class ComputerInfo
    {
        private static string Identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            string result = string.Empty;
            using (ManagementClass mc = new ManagementClass(wmiClass))
            {
                using (ManagementObjectCollection moc = mc.GetInstances())
                {
                    foreach (ManagementObject mo in moc.Cast<ManagementObject>())
                    {
                        if (wmiMustBeTrue != null && mo[wmiMustBeTrue]?.ToString() != "True")
                            continue;

                        if (!string.IsNullOrEmpty(result))
                            continue;

                        result = mo[wmiProperty]?.ToString() ?? string.Empty;
                        break;
                    }
                }
            }

            return result;
        }

        public static string GetMotherboardSerialNumber()
        {
            return Identifier("Win32_BaseBoard", "SerialNumber", null);
        }

        public static string GetMacAddress()
        {
            return Identifier("Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled");
        }

        public static string GetCpuSerialNumber()
        {
            string result = Identifier("Win32_Processor", "ProcessorId", null);
            if (string.IsNullOrEmpty(result)) result = Identifier("Win32_Processor", "UniqueId", null);

            return result;
        }

        public static string GetHardDriveSerialNumber()
        {
            return Identifier("Win32_PhysicalMedia", "SerialNumber", null);
        }
    }
}
