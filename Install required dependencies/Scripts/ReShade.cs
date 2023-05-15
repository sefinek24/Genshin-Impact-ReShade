using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Prepare_mod.Scripts
{
    internal abstract class ReShade
    {
        public static async Task DownloadFiles()
        {
            WebClient wbClient1 = new WebClient();
            wbClient1.Headers.Add("user-agent", Program.UserAgent);
            await wbClient1.DownloadFileTaskAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/reshade/ReShade.ini", Program.ReShadeConfig);

            WebClient wbClient2 = new WebClient();
            wbClient2.Headers.Add("user-agent", Program.UserAgent);
            await wbClient2.DownloadFileTaskAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/reshade/ReShade.log", Program.ReShadeLogFile);

            if (File.Exists(Program.ReShadeConfig) && File.Exists(Program.ReShadeLogFile))
                Log.Output(
                    "ReShade.ini and ReShade.log was successfully downloaded.\n" +
                    "» Source 1: https://cdn.sefinek.net/resources/v3/genshin-stella-mod/reshade/ReShade.ini\n" +
                    "» Source 2: https://cdn.sefinek.net/resources/v3/genshin-stella-mod/reshade/ReShade.log"
                );
            else
                Log.ThrowError(new Exception($"Something went wrong. Config or log file for ReShade was not found in: {Program.ReShadeConfig}"), true);
        }
    }
}
