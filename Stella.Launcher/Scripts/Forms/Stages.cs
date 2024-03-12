using StellaModLauncher.Forms;
using StellaModLauncher.Scripts.Forms.MainForm;
using StellaPLFNet;

namespace StellaModLauncher.Scripts.Forms;

internal static class Stages
{
	public const int AllStages = 9;
	public static int _currentStage;

	public static async void UpdateStage(int stage, string progressBarText)
	{
		if (stage is < 1 or > AllStages) throw new ArgumentOutOfRangeException(nameof(stage), @$"The stage must be between 1 and {AllStages} ");

		Default._preparingPleaseWait!.Text = progressBarText;

		int progressValue = (int)((double)stage / AllStages * 100);
		TaskbarProgress.SetProgressValue((ulong)progressValue);
		Default._progressBar1!.Value = progressValue > Default._progressBar1.Maximum ? Default._progressBar1.Maximum : progressValue;

		_currentStage = stage;
		Program.Logger.Debug($"{stage}; {progressValue}; {progressBarText}");

		// Finish
		if (AllStages != stage) return;
		await Task.Delay(1300).ConfigureAwait(true);

		Labels.HideProgressbar(null, false);
		Default._webView21!.Visible = true;

		TaskbarProgress.SetProgressState(TaskbarProgress.Flags.NoProgress);
	}
}
