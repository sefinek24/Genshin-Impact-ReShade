namespace StellaModLauncher.Scripts.Helpers;

internal static class BalloonTip
{
	public static void Show(string header, string desc)
	{
		Global.NotifyIconInstance!.BalloonTipTitle = header;
		Global.NotifyIconInstance.BalloonTipText = desc;
		Global.NotifyIconInstance.ShowBalloonTip(5000);

		Program.Logger.Info($"BalloonTip.Show(): header: {header}; desc: {desc}");
	}
}
