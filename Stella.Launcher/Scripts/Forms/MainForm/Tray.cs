namespace StellaModLauncher.Scripts.Forms.MainForm;

internal class Tray(NotifyIcon? trayIcon, Form mainForm)
{
	public void ToggleMinimizeRestore(object? sender, EventArgs e)
	{
		if (mainForm.WindowState is FormWindowState.Normal or FormWindowState.Maximized)
		{
			Music.PlaySound("winxp", "minimize");
			mainForm.WindowState = FormWindowState.Minimized;
		}
		else
		{
			Music.PlaySound("winxp", "restore");
			mainForm.WindowState = FormWindowState.Normal;
			mainForm.Activate();
		}

		UpdateToggleButtonText();
	}

	private void UpdateToggleButtonText()
	{
		ToolStripMenuItem? toggleButton = trayIcon.ContextMenuStrip?.Items[0] as ToolStripMenuItem;

		toggleButton.Text = mainForm.WindowState is FormWindowState.Normal or FormWindowState.Maximized ? @"Minimize" : @"Restore";
	}


	public void ReloadForm(object? sender, EventArgs e)
	{
		Music.PlaySound("winxp", "feed_discovered");
		mainForm.Refresh();
	}

	public static void OfficialWebsite(object? sender, EventArgs e)
	{
		Utils.OpenUrl(Program.AppWebsiteFull);
	}

	public static void StellaModPlus(object? sender, EventArgs e)
	{
		Utils.OpenUrl($"{Program.AppWebsiteFull}/subscription");
	}

	public static void DiscordServer(object? sender, EventArgs e)
	{
		Utils.OpenUrl(Discord.Invitation);
	}

	public static void Support(object? sender, EventArgs e)
	{
		Utils.OpenUrl($"{Program.AppWebsiteFull}/support");
	}

	public static void Feedback(object? sender, EventArgs e)
	{
		Utils.OpenUrl($"{Program.AppWebsiteFull}/feedback");
	}

	public static void Donations(object? sender, EventArgs e)
	{
		Utils.OpenUrl("https://sefinek.net/support-me");
	}


	public static void OnQuitClick(object? sender, EventArgs e)
	{
		Application.Exit();
	}
}
