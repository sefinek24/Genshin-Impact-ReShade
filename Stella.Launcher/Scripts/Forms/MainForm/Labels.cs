using StellaModLauncher.Forms;
using StellaModLauncher.Properties;
using StellaPLFNet;

namespace StellaModLauncher.Scripts.Forms.MainForm;

internal static class Labels
{
	public static void ShowStartGameBtns()
	{
		Default._startGame_LinkLabel!.Visible = true;
		Default._injectReShade_LinkLabel!.Visible = true;
		Default._runFpsUnlocker_LinkLabel!.Visible = true;
		Default._only3DMigoto_LinkLabel!.Visible = true;
		Default._runGiLauncher_LinkLabel!.Visible = true;
	}

	public static void HideStartGameBtns()
	{
		Default._startGame_LinkLabel!.Visible = false;
		Default._injectReShade_LinkLabel!.Visible = false;
		Default._runFpsUnlocker_LinkLabel!.Visible = false;
		Default._only3DMigoto_LinkLabel!.Visible = false;
		Default._runGiLauncher_LinkLabel!.Visible = false;
	}

	public static void ShowStartGameBts()
	{
		Default._startGame_LinkLabel!.Visible = true;
		Default._injectReShade_LinkLabel!.Visible = true;
		Default._runFpsUnlocker_LinkLabel!.Visible = true;
		Default._only3DMigoto_LinkLabel!.Visible = true;
		Default._runGiLauncher_LinkLabel!.Visible = true;

		if (!Secret.IsStellaPlusSubscriber) Default._becomeMyPatron_LinkLabel!.Visible = true;
	}

	public static void ShowProgressbar()
	{
		Default._progressBar1!.Show();
		Default._preparingPleaseWait!.Show();

		Default._discordServerIco_Picturebox!.Hide();
		Default._discordServer_LinkLabel!.Hide();
		Default._supportMeIco_PictureBox!.Hide();
		Default._supportMe_LinkLabel!.Hide();
		Default._youtubeIco_Picturebox!.Hide();
		Default._youTube_LinkLabel!.Hide();
	}

	public static void HideProgressbar(string? successText, bool error)
	{
		Default._progressBar1!.Hide();
		Default._progressBar1.Value = 0;
		Default._preparingPleaseWait!.Hide();

		Default._discordServerIco_Picturebox!.Show();
		Default._discordServer_LinkLabel!.Show();
		Default._supportMeIco_PictureBox!.Show();
		Default._supportMe_LinkLabel!.Show();
		Default._youtubeIco_Picturebox!.Show();
		Default._youTube_LinkLabel!.Show();

		if (!string.IsNullOrEmpty(successText) && !error) Default._status_Label!.Text += $"[✓] {successText}\n";

		if (!error) return;
		Default.UpdateIsAvailable = false;
		Default._checkForUpdates_LinkLabel!.LinkColor = Color.Red;
		Default._checkForUpdates_LinkLabel.Text = Resources.Utils_OopsAnErrorOccurred;

		TaskbarProgress.SetProgressValue(100);
		TaskbarProgress.SetProgressState(TaskbarProgress.Flags.Error);
	}
}
