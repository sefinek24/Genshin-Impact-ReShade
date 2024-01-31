using ConfigurationNC.Forms;
using ConfigurationNC.Properties;

namespace ConfigurationNC;

internal static class Program
{
	/// <summary>
	///    The main entry point for the application.
	/// </summary>
	[STAThread]
	private static void Main()
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);

		try
		{
			Application.Run(new Window { Icon = Resources.catIcon });
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, Window.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
