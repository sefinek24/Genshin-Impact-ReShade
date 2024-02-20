using StellaModLauncher.Forms;

namespace StellaModLauncher.Scripts;

internal static class BalloonTip
{
	public static void Show(string header, string desc)
	{
		if (Default.Global.NotifyIconInstance != null)
		{
			Default.Global.NotifyIconInstance.BalloonTipTitle = header;
			Default.Global.NotifyIconInstance.BalloonTipText = desc;
			Default.Global.NotifyIconInstance.ShowBalloonTip(5000);
		}

		Program.Logger.Info($"BalloonTip.Show(): header: {header}; desc: {desc}");
	}
}
