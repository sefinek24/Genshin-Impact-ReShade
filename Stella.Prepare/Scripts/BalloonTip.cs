namespace PrepareStella.Scripts;

internal static class BalloonTip
{
	public static void Show(string header, string desc)
	{
		if (Start.NotifyIconInstance != null)
		{
			Start.NotifyIconInstance.BalloonTipTitle = header;
			Start.NotifyIconInstance.BalloonTipText = desc;
			Start.NotifyIconInstance.ShowBalloonTip(5000);
		}

		Start.Logger.Info($"BalloonTip.Show(): header: {header}; desc: {desc}");
	}
}
