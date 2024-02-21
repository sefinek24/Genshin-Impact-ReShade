using CliWrap.Builders;
using StellaModLauncher.Forms.Other;
using StellaModLauncher.Properties;
using StellaModLauncher.Scripts;
using StellaModLauncher.Scripts.Forms;
using StellaModLauncher.Scripts.Forms.MainForm;

namespace StellaModLauncher.Forms;

public partial class Tools : Form
{
	private bool _mouseDown;
	private Point _offset;

	public Tools()
	{
		InitializeComponent();

		DoubleBuffered = true;
	}

	private void Tools_Load(object sender, EventArgs e)
	{
		RoundedCorners.Form(this);
	}

	private void Utils_Shown(object sender, EventArgs e)
	{
		Discord.SetStatus(Resources.Tools_BrowsingUtils);

		Program.Logger.Info(string.Format(Resources.Main_LoadedForm_, Text));
	}

	private void MouseDown_Event(object sender, MouseEventArgs e)
	{
		_offset.X = e.X;
		_offset.Y = e.Y;
		_mouseDown = true;
	}

	private void MouseMove_Event(object sender, MouseEventArgs e)
	{
		if (!_mouseDown) return;
		Point currentScreenPos = PointToScreen(e.Location);
		Location = new Point(currentScreenPos.X - _offset.X, currentScreenPos.Y - _offset.Y);
	}

	private void MouseUp_Event(object sender, MouseEventArgs e)
	{
		_mouseDown = false;
	}

	private void Exit_Click(object sender, EventArgs e)
	{
		Program.Logger.Info(string.Format(Resources.Main_ClosedForm_, Text));
		Close();

		Discord.Home();
	}


	// ---------------------------------- Misc ----------------------------------
	private async void ScanSysFiles_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Cmd.CliWrap? command = new()
		{
			App = "wt.exe",
			WorkingDir = Program.AppPath,
			Arguments = new ArgumentsBuilder()
				.Add(Path.Combine(Run.BatchDir, "scan_sys_files.cmd"))
				.Add(Program.AppVersion!)
				.Add(Data.ReShadeVer)
				.Add(Data.UnlockerVer)
		};
		await Cmd.Execute(command);
	}


	// ------------------------------ Config files ------------------------------
	private async void ReShadeConfig_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		string? gamePath = await Utils.GetGame("giGameDir");
		string reShadeIni = Path.Combine(gamePath!, "ReShade.ini");

		if (!File.Exists(reShadeIni))
		{
			MessageBox.Show(string.Format(Resources.Tools_ReShadeConfigFileWasNotFoundIn_, reShadeIni), Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
		else
		{
			Cmd.CliWrap? command = new()
			{
				App = "notepad.exe",
				WorkingDir = Program.AppPath,
				Arguments = new ArgumentsBuilder()
					.Add(reShadeIni)
			};
			await Cmd.Execute(command);
		}
	}

	private async void UnlockerConfig_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Cmd.CliWrap? command = new()
		{
			App = "notepad.exe",
			WorkingDir = Program.AppPath,
			Arguments = new ArgumentsBuilder()
				.Add(Path.Combine(Program.AppPath, "data", "unlocker", "unlocker.config.json"))
		};
		await Cmd.Execute(command);
	}


	// --------------------------------- Cache ---------------------------------
	private async void DeleteCache_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		string? gameDir = await Utils.GetGame("giGameDir");

		Cmd.CliWrap? command = new()
		{
			App = "wt.exe",
			WorkingDir = Program.AppPath,
			Arguments = new ArgumentsBuilder()
				.Add(Path.Combine(Run.BatchDir, "delete_cache.cmd")) // 0
				.Add(Program.AppVersion!) // 1
				.Add(Data.ReShadeVer) // 2
				.Add(Data.UnlockerVer) // 3
				.Add(Program.AppData) // 4
				.Add(Default.ResourcesPath!) // 5
				.Add(gameDir!) // 6
				.Add(Program.AppData) // 7
		};
		await Cmd.Execute(command);
	}

	private async void DeleteWebViewCache_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Cmd.CliWrap? command = new()
		{
			App = "wt.exe",
			WorkingDir = Program.AppPath,
			Arguments = new ArgumentsBuilder()
				.Add(Path.Combine(Run.BatchDir, "delete_webview_cache.cmd")) // 0
				.Add(Program.AppVersion!) // 1
				.Add(Data.ReShadeVer) // 2
				.Add(Data.UnlockerVer) // 3
				.Add(Program.AppData) // 4
		};
		await Cmd.Execute(command);
	}


	// ---------------------------------- Logs ---------------------------------
	private static void LogSharingAlert()
	{
		int logSharingAlert = Program.Settings.ReadInt("Launcher", "LogSharingAlert", 0);
		if (logSharingAlert >= 2) return;

		BalloonTip.Show("Important", "Remember not to share log files with unfamiliar individuals, as they might contain sensitive data. Share them only with the Stella Mod developer.");

		Program.Settings.WriteInt("Launcher", "LogSharingAlert", logSharingAlert + 1);
	}

	private void LogDir_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		if (!Directory.Exists(Log.Folder))
		{
			MessageBox.Show($"Directory with the Stella Logs was not found in:\n\n{Log.Folder}", Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			Program.Logger.Info($"Directory with the Stella Logs was not found in: {Log.Folder}");
			return;
		}

		Cmd.Start(Log.Folder);
		LogSharingAlert();
	}

	private async void LauncherLogs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Cmd.CliWrap? command = new()
		{
			App = "notepad.exe",
			WorkingDir = Program.AppPath,
			Arguments = new ArgumentsBuilder()
				.Add(Path.Combine(Log.Folder!, "launcher.output.log"))
		};
		await Cmd.Execute(command);

		LogSharingAlert();
	}

	private async void GSModLogs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		await Cmd.Execute(new Cmd.CliWrap
		{
			App = "notepad.exe",
			WorkingDir = Program.AppPath,
			Arguments = new ArgumentsBuilder()
				.Add(Path.Combine(Log.Folder, "gsmod.output.log"))
		});

		LogSharingAlert();
	}

	private async void ReShadeLogs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		string? gameDir = await Utils.GetGame("giGameDir");
		string logFile = Path.Combine(gameDir!, "ReShade.log");

		if (!Directory.Exists(gameDir) || !File.Exists(logFile))
			MessageBox.Show(string.Format(Resources.Tools_ReShadeLogFileWasNotFoundIn_, logFile), Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Information);
		else
			await Cmd.Execute(new Cmd.CliWrap
			{
				App = "notepad.exe",
				WorkingDir = Program.AppPath,
				Arguments = new ArgumentsBuilder()
					.Add(logFile)
			});

		LogSharingAlert();
	}


	// -------------------------- Nothing special ((: ---------------------------
	private void Notepad_MouseClick(object sender, MouseEventArgs e)
	{
		string? path = Path.Combine(Program.AppPath, "data", "videos", "poland-strong.mp4");
		if (!Utils.CheckFileExists(path)) return;

		WebView2Shake viewer = new() { DesktopLocation = DesktopLocation, Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) };
		viewer.Navigate(path);
		viewer.Show();
	}
}
