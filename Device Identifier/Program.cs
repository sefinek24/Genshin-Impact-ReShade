using System;

namespace DeviceIdentifier
{
    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine($"Region: {ComputerInfo.GetSystemRegion()}");
            Console.WriteLine($"Motherboard serial number: {ComputerInfo.GetMotherboardSerialNumber()}");
            Console.WriteLine($"CPU serial number: {ComputerInfo.GetCpuSerialNumber()}");
            Console.WriteLine($"Hard drive serial number: {ComputerInfo.GetHardDriveSerialNumber()}");

            Console.ReadLine();
        }
    }
}
