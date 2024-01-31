using System;
using System.Reflection;
using System.Windows.Forms;
using SefinAntiCheat.Forms;
using SefinAntiCheat.Properties;

namespace SefinAntiCheat
{
	internal static class Program
	{
		public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
		public static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

		/// <summary>
		///    The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			try
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new MainWindow { Icon = Resources.icon });
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
