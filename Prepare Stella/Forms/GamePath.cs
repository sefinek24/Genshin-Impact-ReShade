using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace PrepareStella.Forms
{
	public partial class GamePath : Form
	{
		private readonly string _inputString;

		public GamePath(string inputString)
		{
			InitializeComponent();
			_inputString += inputString;
		}

		private void SelectPath_Load(object sender, EventArgs e)
		{
			label4.Text += _inputString;
		}

		private void Browse_Click(object sender, EventArgs e)
		{
			string filePath = string.Empty;

			Thread t = new Thread(() =>
			{
				using (OpenFileDialog dialog = new OpenFileDialog())
				{
					dialog.InitialDirectory = Program.ProgramFiles;
					dialog.Filter = @"Process (*.exe)|*.exe";
					dialog.FilterIndex = 0;
					dialog.RestoreDirectory = true;

					if (dialog.ShowDialog() != DialogResult.OK) return;
					string selectedFile = dialog.FileName;

					if (!selectedFile.Contains("GenshinImpact.exe") && !selectedFile.Contains("YuanShen.exe"))
					{
						MessageBox.Show("Please select the game exe.\n\n* GenshinImpact.exe for OS version\n* YuanShen.exe for CN version", Start.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
						return;
					}

					string directory = Path.GetDirectoryName(selectedFile);
					if (!File.Exists(Path.Combine(directory, "UnityPlayer.dll")))
					{
						MessageBox.Show($"That's not the right place.\n\nSelected path:\n{directory}", Start.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return;
					}

					filePath = selectedFile;
				}
			});

			t.SetApartmentState(ApartmentState.STA);
			t.Start();
			t.Join();

			if (string.IsNullOrEmpty(filePath)) return;
			comboBox1.Items.Clear();
			comboBox1.Items.Add(filePath);
			comboBox1.SelectedIndex = 0;
		}

		private void SaveSettings_Click(object sender, EventArgs e)
		{
			string selectedFile = comboBox1.GetItemText(comboBox1.SelectedItem);
			if (!selectedFile.Contains("GenshinImpact.exe") && !selectedFile.Contains("YuanShen.exe"))
			{
				MessageBox.Show("We can't save your settings. Please select the game exe.\n\n* GenshinImpact.exe for OS version (main)\n* YuanShen.exe for CN (Chinese) version", Start.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			string directory = Path.GetDirectoryName(selectedFile);
			if (!File.Exists(Path.Combine(directory, "UnityPlayer.dll")))
			{
				MessageBox.Show(@"That's not the right place. UnityPlayer.dll file was not found.", Start.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			using (RegistryKey key = Registry.CurrentUser.CreateSubKey(Program.RegistryPath))
			{
				key?.SetValue("GameVersion", Path.GetFileName(selectedFile) == "GenshinImpact.exe" ? "1" : "2");
			}

			Program.SavedGamePath = selectedFile;
			Close();
		}

		private void Help_Click(object sender, EventArgs e)
		{
			new Help { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) }.Show();
		}
	}
}
