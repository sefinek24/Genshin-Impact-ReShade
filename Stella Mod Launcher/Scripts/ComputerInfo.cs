using System;
using System.Globalization;
using System.Linq;
using System.Management;

namespace StellaLauncher.Scripts
{
    internal static class ComputerInfo
    {
        private static string Identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue = null)
        {
            try
            {
                if (!ManagementClassExists(wmiClass))
                {
                    Program.Logger.Error($"WMI class '{wmiClass}' does not exist.");
                    return string.Empty;
                }

                SelectQuery query = new SelectQuery($"SELECT {wmiProperty} FROM {wmiClass}");
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
                    {
                        if (wmiMustBeTrue == null || obj[wmiMustBeTrue]?.ToString() == "True")
                        {
                            return obj[wmiProperty]?.ToString() ?? string.Empty;
                        }
                    }
                }
            }
            catch (ManagementException ex)
            {
                Program.Logger.Error($"ManagementException occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                Program.Logger.Error($"Exception occurred: {ex.Message}");
            }

            return string.Empty;
        }

        private static bool ManagementClassExists(string className)
        {
            try
            {
                using (ManagementClass mc = new ManagementClass(className))
                {
                    mc.GetInstances().Dispose();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static string GetSystemRegion()
        {
            return RegionInfo.CurrentRegion.TwoLetterISORegionName;
        }

        public static string GetMotherboardSerialNumber()
        {
            return Identifier("Win32_BaseBoard", "SerialNumber");
        }

        public static string GetCpuSerialNumber()
        {
            string result = Identifier("Win32_Processor", "ProcessorId");
            if (string.IsNullOrEmpty(result)) result = Identifier("Win32_Processor", "UniqueId");

            return result;
        }

        public static string GetHardDriveSerialNumber()
        {
            return Identifier("Win32_PhysicalMedia", "SerialNumber");
        }
    }
}
