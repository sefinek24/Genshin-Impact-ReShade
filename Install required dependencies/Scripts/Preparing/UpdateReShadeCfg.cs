using System;
using System.IO;
using System.Threading.Tasks;

namespace Prepare_mod.Scripts.Preparing
{
    internal static class UpdateReShadeCfg
    {
        public static async Task Run()
        {
            Console.WriteLine(@"Updating ReShade config... ");

            if (Directory.Exists(Program.GameDirGlobal))
            {
                if (File.Exists(Program.ReShadeConfig)) File.Delete(Program.ReShadeConfig);

                if (File.Exists(Program.ReShadeLogFile)) File.Delete(Program.ReShadeLogFile);

                await ReShade.DownloadFiles();
            }
            else
            {
                Console.WriteLine(@"You must configure some settings manually");
            }
        }
    }
}
