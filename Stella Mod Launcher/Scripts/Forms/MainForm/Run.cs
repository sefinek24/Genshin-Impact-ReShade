using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using CliWrap.Builders;
using StellaLauncher.Forms;
using StellaLauncher.Properties;

namespace StellaLauncher.Scripts.Forms.MainForm
{
    internal static class Run
    {
        // Exe files
        private static readonly string GsmPath = Path.Combine(Program.AppPath, "Genshin Stella Mod.exe");

        // Batch files
        public static readonly string BatchDir = Path.Combine(Program.AppPath, "data", "cmd");
        private static readonly string BatchDirPatrons = Path.Combine(BatchDir, "patrons");
        private static readonly string BatchRunPatrons = Path.Combine(BatchDirPatrons, "run.cmd");

        // Variables
        public static string InjectType;

        public static async Task StartGame()
        {
            Cmd.CliWrap command = null;

            switch (InjectType)
            {
                case "exe":
                    command = new Cmd.CliWrap
                    {
                        App = "wt.exe",
                        Arguments = new ArgumentsBuilder()
                            .Add(GsmPath) // 0
                            .Add(Program.AppVersion) // 1
                            .Add(Data.ReShadeVer) // 2
                            .Add(Data.UnlockerVer) // 3
                            .Add(Secret.IsMyPatron ? 1 : 6) // 4
                    };
                    break;
                case "cmd":
                    command = new Cmd.CliWrap
                    {
                        App = "wt.exe",
                        WorkingDir = Program.AppPath,
                        Arguments = new ArgumentsBuilder()
                            .Add(BatchRunPatrons) // 0
                            .Add(Program.AppVersion) // 1
                            .Add(Data.ReShadeVer) // 2
                            .Add(Data.UnlockerVer) // 3
                            .Add(Secret.IsMyPatron ? 1 : 6) // 4
                            .Add(Secret.IsMyPatron ? $"\"{Default.ResourcesPath}\\3DMigoto\"" : "0") // 5 
                            .Add(await Utils.GetGameVersion()) // 6
                            .Add(Log.CmdLogs) // 7
                            .Add(Program.AppPath) // 8
                            .Add(Path.GetDirectoryName(Program.FpsUnlockerExePath) ?? string.Empty) // 9
                    };
                    break;
            }

            bool res = await Cmd.Execute(command);
            if (res) Environment.Exit(0);
        }

        public static async Task ReShade()
        {
            Cmd.CliWrap command = null;

            switch (InjectType)
            {
                case "exe":
                    command = new Cmd.CliWrap
                    {
                        App = "wt.exe",
                        Arguments = new ArgumentsBuilder()
                            .Add(GsmPath) // 0
                            .Add(Program.AppVersion) // 1
                            .Add(Data.ReShadeVer) // 2
                            .Add(Data.UnlockerVer) // 3
                            .Add(3) // 4
                    };
                    break;
                case "cmd":
                    command = new Cmd.CliWrap
                    {
                        App = "wt.exe",
                        WorkingDir = Program.AppPath,
                        Arguments = new ArgumentsBuilder()
                            .Add(Default.ResourcesPath) // 0
                            .Add(Program.AppVersion) // 1
                            .Add(Data.ReShadeVer) // 2
                            .Add(Data.UnlockerVer) // 3
                            .Add(3) // 4
                            .Add(0) // 5 
                            .Add(await Utils.GetGameVersion()) // 6
                            .Add(Log.CmdLogs) // 7
                            .Add(Program.AppPath) // 8
                    };
                    break;
            }

            bool res = await Cmd.Execute(command);
            if (res) Environment.Exit(0);
        }

        public static async Task FpsUnlocker()
        {
            Cmd.CliWrap command = null;
            switch (InjectType)
            {
                case "exe":
                    command = new Cmd.CliWrap
                    {
                        App = "wt.exe",
                        Arguments = new ArgumentsBuilder()
                            .Add(GsmPath) // 0
                            .Add(Program.AppVersion) // 1
                            .Add(Data.ReShadeVer) // 2
                            .Add(Data.UnlockerVer) // 3
                            .Add(4) // 4
                    };
                    break;
                case "cmd":
                    command = new Cmd.CliWrap
                    {
                        App = "wt.exe",
                        WorkingDir = Program.AppPath,
                        Arguments = new ArgumentsBuilder()
                            .Add(BatchRunPatrons) // 0
                            .Add(Program.AppVersion) // 1
                            .Add(Data.ReShadeVer) // 2
                            .Add(Data.UnlockerVer) // 3
                            .Add(4) // 4
                            .Add(0) // 5 
                            .Add(await Utils.GetGameVersion()) // 6
                            .Add(Log.CmdLogs) // 7
                            .Add(Program.AppPath) // 8
                            .Add(Path.GetDirectoryName(Program.FpsUnlockerExePath) ?? string.Empty) // 9
                    };
                    break;
            }


            bool res = await Cmd.Execute(command);
            if (res) Environment.Exit(0);
        }

        public static async Task Migoto()
        {
            if (!Secret.IsMyPatron)
            {
                DialogResult result = MessageBox.Show(Resources.Default_ThisFeatureIsAvailableOnlyForMyPatrons, Program.AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) Utils.OpenUrl("https://www.patreon.com/sefinek");
                return;
            }

            Cmd.CliWrap command = null;
            switch (InjectType)
            {
                case "exe":
                    command = new Cmd.CliWrap
                    {
                        App = "wt.exe",
                        Arguments = new ArgumentsBuilder()
                            .Add(GsmPath) // 0
                            .Add(Program.AppVersion) // 1
                            .Add(Data.ReShadeVer) // 2
                            .Add(Data.UnlockerVer) // 3
                            .Add(5) // 4
                    };
                    break;
                case "cmd":
                    command = new Cmd.CliWrap
                    {
                        App = "wt.exe",
                        WorkingDir = Program.AppPath,
                        Arguments = new ArgumentsBuilder()
                            .Add(BatchRunPatrons) // 0
                            .Add(Program.AppVersion) // 1
                            .Add(Data.ReShadeVer) // 2
                            .Add(Data.UnlockerVer) // 3
                            .Add(5) // 4
                            .Add(Secret.IsMyPatron ? $"\"{Default.ResourcesPath}\\3DMigoto\"" : "0") // 5 
                            .Add(await Utils.GetGameVersion()) // 6
                            .Add(Log.CmdLogs) // 7
                            .Add(Program.AppPath) // 8
                    };
                    break;
            }

            // Run file
            bool res = await Cmd.Execute(command);
            if (res) Environment.Exit(0);
        }
    }
}
