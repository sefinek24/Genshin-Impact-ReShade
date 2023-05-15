using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Prepare_mod.Properties;

namespace Prepare_mod.Forms
{
    public partial class SelectGamePath : Form
    {
        public static readonly string SfnFilePath = Path.Combine(Program.AppData, "game-path.sfn");

        public SelectGamePath()
        {
            InitializeComponent();
        }

        private void SelectPath_Load(object sender, EventArgs e)
        {
            if (File.Exists(SfnFilePath))
            {
                string path = File.ReadAllText(SfnFilePath).Trim();
                label4.Text += path;
            }
            else
            {
                label4.Text += $"{Program.GameGenshinImpact} and\n{Program.GameYuanShen}";
            }
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            string filePath = string.Empty;

            Thread t = new Thread(() =>
            {
                using (OpenFileDialog dialog = new OpenFileDialog())
                {
                    dialog.InitialDirectory = Program.ProgramFiles;
                    dialog.Filter = "Process (*.exe)|*.exe";
                    dialog.FilterIndex = 0;
                    dialog.RestoreDirectory = true;

                    if (dialog.ShowDialog() != DialogResult.OK) return;
                    string selectedFile = dialog.FileName;

                    if (!selectedFile.Contains("GenshinImpact.exe") && !selectedFile.Contains("YuanShen.exe"))
                    {
                        MessageBox.Show("Please select the game exe.\n\nGenshinImpact.exe for OS version.\nYuanShen.exe for CN version.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    string directory = Path.GetDirectoryName(selectedFile);
                    if (!File.Exists(Path.Combine(directory, "UnityPlayer.dll")))
                    {
                        MessageBox.Show("That's not the right place.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show(
                    "We can't save your settings. Please select the game exe.\n\nGenshinImpact.exe for OS version.\nYuanShen.exe for CN version.",
                    Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string directory = Path.GetDirectoryName(selectedFile);
            if (!File.Exists($@"{directory}\UnityPlayer.dll"))
            {
                MessageBox.Show("That's not the right place. UnityPlayer.dll file was not found.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Program.GameExeGlobal = selectedFile;
            Program.GameDirGlobal = Path.GetDirectoryName(Path.GetDirectoryName(selectedFile));
            File.WriteAllText(SfnFilePath, Program.GameDirGlobal);

            Console.WriteLine(Program.GameDirGlobal);

            Close();
        }

        private void Help_Click(object sender, EventArgs e)
        {
            new Help { Icon = Resources.icon }.Show();
        }
    }
}
