using System;
using System.Windows.Forms;
using Configuration.Forms;
using Configuration.Properties;

namespace Configuration
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Application.Run(new Window { Icon = Resources.icon });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Window.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
