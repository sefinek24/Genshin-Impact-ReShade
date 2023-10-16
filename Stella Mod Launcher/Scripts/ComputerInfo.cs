using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;

namespace StellaLauncher.Scripts
{
    internal static class ComputerInfo
    {
        // private static readonly RegistryKey LocalMachine = Environment.Is64BitProcess
        //    ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
        //    : Registry.LocalMachine;


        public static string GetComputerName()
        {
            return Environment.MachineName;
        }

        public static string GetSystemRegion()
        {
            return RegionInfo.CurrentRegion.TwoLetterISORegionName;
        }

        public static string GetMacAddress()
        {
            List<NetworkInterface> networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up && n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .ToList();

            if (networkInterfaces.Count <= 0) return string.Empty;

            PhysicalAddress macAddress = networkInterfaces[0].GetPhysicalAddress();
            return BitConverter.ToString(macAddress.GetAddressBytes()).Replace("-", ":");
        }

        public static string GetMotherboardSerialNumber()
        {
            ManagementObject managementObject = new ManagementObject("Win32_BaseBoard.Tag=\"Base Board\"");
            return managementObject["SerialNumber"].ToString();
        }

        public static string GetCpuSerialNumber()
        {
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor");
            ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
            string cpuSerialNumber = string.Empty;

            foreach (ManagementBaseObject managementObject in managementObjectCollection)
            {
                cpuSerialNumber = managementObject["ProcessorId"].ToString();
                break;
            }

            return cpuSerialNumber;
        }

        public static string GetHardDriveSerialNumber()
        {
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_DiskDrive WHERE MediaType='Fixed hard disk media'");
            ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
            string hardDriveSerialNumber = string.Empty;

            foreach (ManagementBaseObject managementObject in managementObjectCollection)
            {
                hardDriveSerialNumber = managementObject["SerialNumber"].ToString();
                break;
            }

            return hardDriveSerialNumber;
        }
    }
}
