using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace StellaLauncher.Forms.Other
{
	public partial class Language : Form
	{
		private readonly bool _isInitializing;

		public Language()
		{
			InitializeComponent();

			_isInitializing = true;
			int selected = Program.Settings.ReadInt("Language", "ID", 0);
			comboBox1.SelectedIndex = selected;
			_isInitializing = false;
		}

		private void Lang_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_isInitializing) return;

			string selectedLang;
			switch (comboBox1.SelectedIndex)
			{
				case 0:
					selectedLang = "en";
					Thread.CurrentThread.CurrentUICulture = new CultureInfo(selectedLang);
					break;
				case 1:
					selectedLang = "pl";
					Thread.CurrentThread.CurrentUICulture = new CultureInfo(selectedLang);
					break;
				default:
					selectedLang = CultureInfo.InstalledUICulture.Name.Substring(0, 2);
					Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture;
					break;
			}

			ReloadOpenForms();
			Program.Settings.WriteString("Language", "UI", selectedLang);
			Program.Settings.WriteString("Language", "ID", comboBox1.SelectedIndex.ToString());
		}


		private void ReloadOpenForms()
		{
			Version.Text = @"Status: Please wait...";
			foreach (Form form in Application.OpenForms)
			{
				form.SuspendLayout();

				ComponentResourceManager resources = new ComponentResourceManager(form.GetType());
				resources.ApplyResources(form, "$this");

				UpdateControlLanguage(resources, form.Controls);

				form.ResumeLayout(false);
				form.PerformLayout();
			}

			Version.Text = $@"Status: Changed to {comboBox1.SelectedItem}. Restart is optional.";
		}

		private static void UpdateControlLanguage(ComponentResourceManager resources, Control.ControlCollection controls)
		{
			foreach (Control control in controls)
			{
				resources.ApplyResources(control, control.Name);
				if (control.Controls.Count > 0) UpdateControlLanguage(resources, control.Controls);
			}
		}
	}
}
