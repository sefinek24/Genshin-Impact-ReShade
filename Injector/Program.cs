using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Injector.Scripts;

namespace Injector;

internal static class Program
{
    // App
    private static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
    public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;

    // Files
    public static string AppData = Utils.GetAppData();
    private static readonly string AppPath = AppDomain.CurrentDomain.BaseDirectory;
    private static readonly string InjectorDir = Path.Combine(AppPath, "data", "reshade");
    private static readonly string InjectorExe = Path.Combine(AppPath, "data", "reshade", "inject64.exe");
    private static readonly string FpsUnlockerDir = Path.Combine(AppPath, "data", "unlocker");
    private static readonly string FpsUnlockerExe = Path.Combine(AppPath, "data", "unlocker", "unlockfps_clr.exe");

    // Links
    private static readonly string AppWebsite = "https://genshin.sefinek.net";
    public static readonly string DiscordUrl = "https://discord.gg/SVcbaRc7gH";

    // Other
    private static readonly string Line = "===============================================================================================";

    private static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine(
            "⠀   ⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡶⢶⣦⡀\n" +
            "⠀  ⠀⠀⣴⡿⠟⠷⠆⣠⠋⠀⠀⠀⢸⣿\n" +
            "⠀   ⠀⣿⡄⠀⠀⠀⠈⠀⠀⠀⠀⣾⡿                                   Genshin Impact Stella Mod 2023\n" +
            "   ⠀⠀⠹⣿⣦⡀⠀⠀⠀⠀⢀⣾⣿                                            Start the game\n" +
            "⠀   ⠀⠀⠈⠻⣿⣷⣦⣀⣠⣾⡿\n" +
            "     ⠀⠀⠀⠀⠀⠉⠻⢿⡿⠟\n" +
            " ⠀   ⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⢠⠏⡆⠀⠀⠀⠀⠀⢀⣀⣤⣤⣤⣀⡀\n" +
            " ⠀   ⠀⠀⡟⢦⡀⠇⠀⠀⣀⠞⠀⠀⠘⡀⢀⡠⠚⣉⠤⠂⠀⠀⠀⠈⠙⢦⡀\n" +
            "  ⠀ ⠀⠀⠀⡇⠀⠉⠒⠊⠁⠀⠀⠀⠀⠀⠘⢧⠔⣉⠤⠒⠒⠉⠉⠀⠀⠀⠀⠹⣆          » Mod version          : v7.2.0\n" +
            "    ⠀⠀⠀⢰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢻⠀⠀⣤⠶⠶⢶⡄⠀⠀⠀⠀⢹⡆        » ReShade version      : v5.8.0\n" +
            "   ⣀⠤⠒⠒⢺⠒⠀⠀⠀⠀⠀⠀⠀⠀⠤⠊⠀⢸⠀⡿⠀⡀⠀⣀⡟⠀⠀⠀⠀⢸⡇         » FPS Unlocker version : v2.0.9\n" +
            "  ⠈⠀⠀⣠⠴⠚⢯⡀⠐⠒⠚⠉⠀⢶⠂⠀⣀⠜⠀⢿⡀⠉⠚⠉⠀⠀⠀⠀⣠⠟\n" +
            "   ⠠⠊⠀⠀⠀⠀⠙⠂⣴⠒⠒⣲⢔⠉⠉⣹⣞⣉⣈⠿⢦⣀⣀⣀⣠⡴⠟                                                ~ Made by Sefinek\n" +
            Line
        );

        Console.Title = $@"{AppName} • v{AppVersion}";

        Console.ReadLine();

        Cmd.Execute(FpsUnlockerExe, null, FpsUnlockerDir, true, false);
        // Cmd.Execute(InjectorExe, "GenshinImpact.exe", InjectorDir, true, false);
        // Ścieżka do pliku .exe, który chcesz uruchomić

        // Tworzenie procesu
        Process process = new();
        process.StartInfo.FileName = InjectorExe;
        process.StartInfo.Arguments = "GenshinImpact.exe";
        process.StartInfo.WorkingDirectory = InjectorDir;

        // Ustawienie właściwości do przechwytywania wyjścia
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;

        // Subskrypcja zdarzenia do przechwycenia wyjścia
        process.OutputDataReceived += ProcessOutputHandler;

        // Uruchomienie procesu
        process.Start();

        // Rozpoczęcie odczytywania wyjścia asynchronicznie
        process.BeginOutputReadLine();

        // Oczekiwanie na zakończenie procesu
        process.WaitForExit();

        // Zatrzymanie odczytywania wyjścia
        process.CancelOutputRead();

        // Wyświetlenie komunikatu po zakończeniu procesu
        Console.WriteLine("Proces zakończony. Naciśnij dowolny klawisz, aby zakończyć...");
        Console.ReadKey();

        Console.ReadLine();
    }

    private static void ProcessOutputHandler(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Data)) Console.WriteLine(e.Data);
    }
}
