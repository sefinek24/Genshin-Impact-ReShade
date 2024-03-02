using System.ComponentModel;
using System.Globalization;

namespace StellaModLauncher.Forms.Other;

public sealed partial class Language : Form
{
	private readonly bool _isInitializing;

	public Language()
	{
		InitializeComponent();

		DoubleBuffered = true;

		_isInitializing = true;
		int selected = Program.Settings.ReadInt("Language", "ID", 0);
		comboBox1.SelectedIndex = selected;
		_isInitializing = false;
	}

	private void Lang_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (_isInitializing) return;

		string? selectedLang;
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
				selectedLang = CultureInfo.InstalledUICulture.Name[..2];
				Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture;
				break;
		}

		Program.Settings.WriteString("Language", "UI", selectedLang);
		Program.Settings.WriteString("Language", "ID", comboBox1.SelectedIndex.ToString());

		Application.Restart();
		Environment.Exit(0);
	}
}
