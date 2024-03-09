using CliWrap.Builders;
using StellaModLauncher.Forms;
using StellaModLauncher.Properties;
using StellaModLauncher.Scripts.StellaPlus;

namespace StellaModLauncher.Scripts.Forms.MainForm;

internal static class Run
{
	// Exe files
	public static readonly string GsmPath = Path.Combine(Program.AppPath, "Genshin Stella Mod.exe");
	public static readonly string Terminal = Path.Combine(Program.AppPath, "dependencies", "terminal-1.19.10573.0", "wt.exe");

	// Batch files
	public static readonly string BatchDir = Path.Combine(Program.AppPath, "data", "cmd");
	private static readonly string BatchDirPatrons = Path.Combine(BatchDir, "patrons");
	private static readonly string BatchRunPatrons = Path.Combine(BatchDirPatrons, "run.cmd");

	// Variables
	public static string? InjectType;

	public static async Task StartGame()
	{
		Cmd.CliWrap? command = InjectType switch
		{
			"exe" => new Cmd.CliWrap
			{
				App = Terminal,
				Arguments = new ArgumentsBuilder().Add(GsmPath) // 0
					.Add(Program.AppFileVersion!) // 1
					.Add(Data.ReShadeVer) // 2
					.Add(Data.UnlockerVer) // 3
					.Add(Secret.IsStellaPlusSubscriber ? 1 : 6) // 4
			},
			"cmd" => new Cmd.CliWrap
			{
				App = Terminal,
				WorkingDir = Program.AppPath,
				Arguments = new ArgumentsBuilder().Add(BatchRunPatrons) // 0
					.Add(Program.AppFileVersion!) // 1
					.Add(Data.ReShadeVer) // 2
					.Add(Data.UnlockerVer) // 3
					.Add(Secret.IsStellaPlusSubscriber ? 1 : 6) // 4
					.Add(Secret.IsStellaPlusSubscriber ? $"\"{Default.ResourcesPath}\\3DMigoto\"" : "0") // 5 
					.Add(await Utils.GetGameVersion().ConfigureAwait(false)) // 6
					.Add(Log.CmdLogs) // 7
					.Add(Program.AppPath) // 8
					.Add(Path.GetDirectoryName(Program.FpsUnlockerExePath) ?? string.Empty) // 9
			},
			_ => null
		};

		bool res = await Cmd.Execute(command, false).ConfigureAwait(false);
		if (res) Application.Exit();
	}

	public static async Task ReShade()
	{
		Cmd.CliWrap? command = InjectType switch
		{
			"exe" => new Cmd.CliWrap
			{
				App = Terminal,
				Arguments = new ArgumentsBuilder().Add(GsmPath) // 0
					.Add(Program.AppFileVersion!) // 1
					.Add(Data.ReShadeVer) // 2
					.Add(Data.UnlockerVer) // 3
					.Add(3) // 4
			},
			"cmd" => new Cmd.CliWrap
			{
				App = Terminal,
				WorkingDir = Program.AppPath,
				Arguments = new ArgumentsBuilder().Add(BatchRunPatrons) // 0
					.Add(Program.AppFileVersion!) // 1
					.Add(Data.ReShadeVer) // 2
					.Add(Data.UnlockerVer) // 3
					.Add(3) // 4
					.Add(0) // 5 
					.Add(await Utils.GetGameVersion().ConfigureAwait(false)) // 6
					.Add(Log.CmdLogs) // 7
					.Add(Program.AppPath) // 8
					.Add(Path.GetDirectoryName(Program.FpsUnlockerExePath) ?? string.Empty) // 9
			},
			_ => null
		};

		bool res = await Cmd.Execute(command, false).ConfigureAwait(false);
		if (res) Application.Exit();
	}

	public static async Task FpsUnlocker()
	{
		Cmd.CliWrap? command = InjectType switch
		{
			"exe" => new Cmd.CliWrap
			{
				App = Terminal,
				Arguments = new ArgumentsBuilder().Add(GsmPath) // 0
					.Add(Program.AppFileVersion!) // 1
					.Add(Data.ReShadeVer) // 2
					.Add(Data.UnlockerVer) // 3
					.Add(4) // 4
			},
			"cmd" => new Cmd.CliWrap
			{
				App = Terminal,
				WorkingDir = Program.AppPath,
				Arguments = new ArgumentsBuilder().Add(BatchRunPatrons) // 0
					.Add(Program.AppFileVersion!) // 1
					.Add(Data.ReShadeVer) // 2
					.Add(Data.UnlockerVer) // 3
					.Add(4) // 4
					.Add(0) // 5 
					.Add(await Utils.GetGameVersion().ConfigureAwait(false)) // 6
					.Add(Log.CmdLogs) // 7
					.Add(Program.AppPath) // 8
					.Add(Path.GetDirectoryName(Program.FpsUnlockerExePath) ?? string.Empty) // 9
			},
			_ => null
		};

		bool res = await Cmd.Execute(command, false).ConfigureAwait(false);
		if (res) Application.Exit();
	}

	public static async Task Migoto()
	{
		if (!Secret.IsStellaPlusSubscriber)
		{
			DialogResult result = MessageBox.Show(Resources.Default_ThisFeatureIsAvailableOnlyForStellaModPlusSubscribers, Program.AppNameVer, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (result == DialogResult.Yes) Utils.OpenUrl("https://sefinek.net/genshin-impact-reshade/subscription");
			return;
		}

		Cmd.CliWrap? command = InjectType switch
		{
			"exe" => new Cmd.CliWrap
			{
				App = Terminal,
				Arguments = new ArgumentsBuilder().Add(GsmPath) // 0
					.Add(Program.AppVersion!) // 1
					.Add(Data.ReShadeVer) // 2
					.Add(Data.UnlockerVer) // 3
					.Add(5) // 4
			},
			"cmd" => new Cmd.CliWrap
			{
				App = Terminal,
				WorkingDir = Program.AppPath,
				Arguments = new ArgumentsBuilder().Add(BatchRunPatrons) // 0
					.Add(Program.AppVersion!) // 1
					.Add(Data.ReShadeVer) // 2
					.Add(Data.UnlockerVer) // 3
					.Add(5) // 4
					.Add(Secret.IsStellaPlusSubscriber ? $"\"{Default.ResourcesPath}\\3DMigoto\"" : "0") // 5 
					.Add(await Utils.GetGameVersion().ConfigureAwait(false)) // 6
					.Add(Log.CmdLogs) // 7
					.Add(Program.AppPath) // 8
			},
			_ => null
		};

		bool res = await Cmd.Execute(command, false).ConfigureAwait(false);
		if (res) Application.Exit();
	}
}
