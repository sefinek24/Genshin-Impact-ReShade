using StellaModLauncherNET.Forms;

namespace StellaModLauncherNET.Scripts.Forms.MainForm;

internal static class Labels
{
	public static void ShowStartGameBtns()
	{
		Default._startGame_LinkLabel.Visible = true;
		Default._injectReShade_LinkLabel.Visible = true;
		Default._runFpsUnlocker_LinkLabel.Visible = true;
		Default._only3DMigoto_LinkLabel.Visible = true;
		Default._runGiLauncher_LinkLabel.Visible = true;
	}

	public static void HideStartGameBtns()
	{
		Default._startGame_LinkLabel.Visible = false;
		Default._injectReShade_LinkLabel.Visible = false;
		Default._runFpsUnlocker_LinkLabel.Visible = false;
		Default._only3DMigoto_LinkLabel.Visible = false;
		Default._runGiLauncher_LinkLabel.Visible = false;
	}
}
