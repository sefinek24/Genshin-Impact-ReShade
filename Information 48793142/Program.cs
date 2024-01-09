namespace InformationWindow;

internal static class Program
{
	/// <summary>
	///    The
	///    main
	///    entry
	///    point
	///    for
	///    the
	///    application.
	/// </summary>
	[STAThread]
	private static void Main()
	{
		try
		{
			ApplicationConfiguration.Initialize();
			Application.Run(new MainWindow());
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
