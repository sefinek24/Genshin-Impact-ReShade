using StellaModLauncher.Scripts;
using StellaModLauncher.Scripts.Forms.MainForm;

namespace StellaModLauncher.Forms.Other;

public sealed partial class InjectionMethod : Form
{
	private bool _isLoading;

	public InjectionMethod()
	{
		InitializeComponent();

		DoubleBuffered = true;
	}

	private void InjectionMethod_Load(object sender, EventArgs e)
	{
		_isLoading = true;

		switch (Run.InjectType)
		{
			case "exe":
				comboBox1.SelectedIndex = 0;
				break;
			case "cmd":
				comboBox1.SelectedIndex = 1;
				break;
		}

		_isLoading = false;
	}

	private void Method_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (_isLoading) return;

		switch (comboBox1.SelectedIndex)
		{
			case 0:
				SetToGenshinStellaMod();
				break;


			case 1:
				if (Secret.IsStellaPlusSubscriber)
				{
					SetToBatchFiles();

					MessageBox.Show(@"WARNING! SETTING CHANGED TO ONE DIFFERENT FROM THE RECOMMENDED ONE! I HOPE YOU KNOW WHAT YOU'RE DOING! 

This option is intended for users with any knowledge in the IT field and in the Batch scripting language. With this option, you can have full control over the injection process in the .cmd file.

!!! THE CREATOR OF THIS SOFTWARE TAKES NO RESPONSIBILITY FOR ANY BANS IN THE GAME. BY USING THIS FUNCTION, YOU ACCEPT THIS RISK !!!", Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
				else
				{
					SetToGenshinStellaMod();

					MessageBox.Show(@"Only Stella Mod Plus subscribers have the ability to change the injection method.

Subscribe to have full control over what is to be injected using batch files. Remember not to break the game rules and avoid cheating!", Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}

				break;

			default:
				SetToGenshinStellaMod();

				MessageBox.Show(@"Unknown choice. Data has been restored to the default value.", Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Warning);

				break;
		}
	}

	private void SetToGenshinStellaMod()
	{
		comboBox1.SelectedIndex = 0;
		Program.Settings.WriteString("Injection", "Method", "exe");
		Program.Settings.Save();
		Run.InjectType = "exe";
	}

	private void SetToBatchFiles()
	{
		comboBox1.SelectedIndex = 1;
		Program.Settings.WriteString("Injection", "Method", "cmd");
		Program.Settings.Save();
		Run.InjectType = "cmd";
	}
}
