using System;
using System.Text;
using System.Threading.Tasks;
using PrepareStella.Scripts;

namespace PrepareStella
{
    internal static class Start
    {
        private static async Task Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"                             Genshin Impact Stella Mod - Beta release");
            Console.WriteLine($"                                        Version: v{Program.AppVersion}\n");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(@"» Author  : Sefinek [Country: Poland]");
            Console.WriteLine(@"» Website : " + Program.AppWebsite);
            Console.WriteLine(@"» Discord : " + Program.DiscordUrl);
            Console.ResetColor();
            Console.WriteLine(Program.Line);

            Console.Title = $@"{Program.AppName} • v{Program.AppVersion}";

            if (!Utils.IsRunAsAdmin())
            {
                Log.ErrorAndExit(new Exception("» This application requires administrator privileges to run."), false, false);
                return;
            }

            try
            {
                await Program.Start();
            }
            catch (Exception ex)
            {
                Log.ErrorAndExit(ex, false, false);
            }
        }
    }
}
