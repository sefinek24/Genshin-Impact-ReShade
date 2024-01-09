using System;

namespace DeviceIdentifier
{
    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine(
                "WARNING: Note: If someone asks you for hardware identifiers from the list below,\n" +
                "please ensure that this individual is genuinely a developer or a collaborator of sefinek24/Genshin-Impact-ReShade."
            );

            Console.WriteLine(
                "\n\n================== Developer ==================\n" +
                "Username: Sefinek\n" +
                "GPG key ID: 376D883EFE071F0D\n\n" +
                "pub   rsa4096 2023-10-19 [SC] [wygasa: 2025-**-**]\n" +
                "      432012BD6823FB93FFF792A0376D883EFE071F0D\n" +
                "uid    [   absolutne   ] Sefinek (Neko cat) <contact@sefinek.net>\n" +
                "sub   rsa4096 2023-10-19 [E] [wygasa: 2025-**-**]"
            );


            Console.WriteLine("\n\n===================== You =====================");
            Console.WriteLine($"Username: {Environment.UserName}");
            Console.WriteLine($"Region: {ComputerInfo.GetSystemRegion()}");
            Console.WriteLine($"Motherboard serial number: {ComputerInfo.GetMotherboardSerialNumber()}");
            Console.WriteLine($"CPU serial number: {ComputerInfo.GetCpuSerialNumber()}");
            Console.WriteLine($"Hard drive serial number: {ComputerInfo.GetHardDriveSerialNumber()}");


            Console.WriteLine($"\n\nPress [ENTER] to exit this program...");
            Console.ReadLine();
        }
    }
}
