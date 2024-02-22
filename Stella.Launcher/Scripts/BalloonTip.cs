namespace StellaModLauncher.Scripts;

internal static class BalloonTip
{
	public static void Show(string header, string desc)
	{
		GlobalHelpers.NotifyIconInstance!.BalloonTipTitle = header;
		GlobalHelpers.NotifyIconInstance.BalloonTipText = desc;
		GlobalHelpers.NotifyIconInstance.ShowBalloonTip(5000);

		Program.Logger.Info($"BalloonTip.Show(): header: {header}; desc: {desc}");
	}
}
