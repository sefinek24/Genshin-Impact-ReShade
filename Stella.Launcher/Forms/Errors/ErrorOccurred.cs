using CliWrap.Builders;
using StellaModLauncher.Properties;
using StellaModLauncher.Scripts;
using StellaModLauncher.Scripts.Forms;
using StellaModLauncher.Scripts.Forms.MainForm;

namespace StellaModLauncher.Forms.Errors;

public sealed partial class ErrorOccurred : Form
{
	public ErrorOccurred()
	{
		InitializeComponent();

		SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
		UpdateStyles();
	}

	private void ErrorOccurred_Load(object sender, EventArgs e)
	{
		label1.Text = string.Format(label1.Text, Data.Discord, Data.Email);

		Music.PlaySound("winxp", "error");
	}

	private void ErrorOccurred_Shown(object sender, EventArgs e)
	{
		Program.Logger.Info(string.Format(Resources.Main_LoadedForm_, Text));
	}

	private void ErrorOccurred_FormClosed(object sender, FormClosedEventArgs e)
	{
		Program.Logger.Info(string.Format(Resources.Main_ClosedForm_, Text));
	}

	private void SeeLogs_Button(object sender, EventArgs e)
	{
		Utils.OpenUrl(Log.Folder);
	}

	private void Reinstall_Button(object sender, EventArgs e)
	{
		Utils.OpenUrl($"{Program.AppWebsiteFull}/download?referrer=error_occurred&hash=null");
	}

	private void Discord_Button(object sender, EventArgs e)
	{
		Utils.OpenUrl(Discord.Invitation);
	}

	private async void SfcScan_Click(object sender, EventArgs e)
	{
		Cmd.CliWrap command = new()
		{
			App = Run.Terminal,
			WorkingDir = Program.AppPath,
			Arguments = new ArgumentsBuilder()
				.Add(Path.Combine(Program.AppPath, "data", "cmd", "scan_sys_files.cmd"))
				.Add(Program.AppVersion!)
				.Add(Data.ReShadeVer)
				.Add(Data.UnlockerVer)
		};
		await Cmd.Execute(command).ConfigureAwait(false);
	}
}
