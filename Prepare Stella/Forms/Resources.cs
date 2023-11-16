using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace PrepareStella.Forms
{
    public partial class Resources : Form
    {
        public Resources()
        {
            InitializeComponent();
        }

        private void SelectShadersPath_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add(Path.Combine(@"C:\", "Stella-Mod-Resources"));
            comboBox1.SelectedIndex = 0;
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            string folderPath = string.Empty;

            Thread t = new Thread(() =>
            {
                using (FolderBrowserDialog dialog = new FolderBrowserDialog())
                {
                    dialog.SelectedPath = Program.ProgramFiles;
                    dialog.Description = @"Select a custom folder for your mod resources.";

                    if (dialog.ShowDialog() != DialogResult.OK) return;
                    string selectedFolder = dialog.SelectedPath;

                    if (File.Exists(Path.Combine(selectedFolder, "UnityPlayer.dll")) || File.Exists(Path.Combine(selectedFolder, "launcher.exe")))
                    {
                        MessageBox.Show(@"That's not the right place.", Start.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    folderPath = selectedFolder;
                }
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            if (string.IsNullOrEmpty(folderPath)) return;
            comboBox1.Items.Add(Path.Combine(folderPath, "Stella-Mod-Resources"));
            string bottomItem = comboBox1.Items[comboBox1.Items.Count - 1].ToString();
            comboBox1.Items.Insert(0, bottomItem);
            comboBox1.Items.RemoveAt(comboBox1.Items.Count - 1);
            comboBox1.SelectedIndex = 0;
        }

        private void SaveSettings_Click(object sender, EventArgs e)
        {
            string selectedFile = comboBox1.GetItemText(comboBox1.SelectedItem);

            Program.ResourcesGlobal = selectedFile;
            File.WriteAllText(Path.Combine(Start.AppData, "resources-path.sfn"), Program.ResourcesGlobal);

            Close();
        }
    }
}
