namespace GenshinTest;

internal static class Program
{
	[STAThread]
	private static void Main()
	{
		Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
		ApplicationConfiguration.Initialize();
		Application.Run(new MainForm());
	}
}
