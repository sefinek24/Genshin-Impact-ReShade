using StellaModLauncher.Forms;

namespace StellaModLauncher.Scripts.Forms;

internal static class Stages
{
	private const int AllStages = 12;

	public static void UpdateStage(int stage, string progressBarText)
	{
		if (stage is < 1 or > AllStages) throw new ArgumentOutOfRangeException(nameof(stage), @$"The stage must be between 1 and {AllStages} ");

		Default._preparingPleaseWait!.Text = progressBarText;

		int progressValue = (int)((double)stage / AllStages * 100);
		Default._progressBar1!.Value = progressValue > Default._progressBar1.Maximum ? Default._progressBar1.Maximum : progressValue;

		if (AllStages == stage) Utils.HideProgressBar(false);
	}
}
