using System;
using System.Windows.Forms;

namespace StellaLauncher.Scripts.Forms.MainForm
{
	internal class Tray
	{
		private readonly Form _mainForm;
		private readonly NotifyIcon _trayIcon;

		public Tray(NotifyIcon trayIcon, Form mainForm)
		{
			_trayIcon = trayIcon;
			_mainForm = mainForm;
		}

		public void ToggleMinimizeRestore(object sender, EventArgs e)
		{
			if (_mainForm.WindowState == FormWindowState.Normal || _mainForm.WindowState == FormWindowState.Maximized)
			{
				Music.PlaySound("winxp", "minimize");
				_mainForm.WindowState = FormWindowState.Minimized;
			}
			else
			{
				Music.PlaySound("winxp", "restore");
				_mainForm.WindowState = FormWindowState.Normal;
				_mainForm.Activate();
			}

			UpdateToggleButtonText();
		}

		private void UpdateToggleButtonText()
		{
			ToolStripMenuItem toggleButton = _trayIcon.ContextMenuStrip.Items[0] as ToolStripMenuItem;

			if (_mainForm.WindowState == FormWindowState.Normal || _mainForm.WindowState == FormWindowState.Maximized)
				toggleButton.Text = @"Minimize";
			else
				toggleButton.Text = @"Restore";
		}


		public void ReloadForm(object sender, EventArgs e)
		{
			Music.PlaySound("winxp", "feed_discovered");
			_mainForm.Refresh();
		}

		public static void OfficialWebsite(object sender, EventArgs e)
		{
			Utils.OpenUrl(Program.AppWebsiteFull);
		}

		public static void StellaModPlus(object sender, EventArgs e)
		{
			Utils.OpenUrl($"{Program.AppWebsiteFull}/subscription");
		}

		public static void DiscordServer(object sender, EventArgs e)
		{
			Utils.OpenUrl(Discord.Invitation);
		}

		public static void Support(object sender, EventArgs e)
		{
			Utils.OpenUrl($"{Program.AppWebsiteFull}/support");
		}

		public static void Feedback(object sender, EventArgs e)
		{
			Utils.OpenUrl($"{Program.AppWebsiteFull}/feedback");
		}

		public static void Donations(object sender, EventArgs e)
		{
			Utils.OpenUrl("https://sefinek.net/support-me");
		}


		public static void OnQuitClick(object sender, EventArgs e)
		{
			Application.Exit();
		}
	}
}
