using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace PrepareStella.Forms
{
    public partial class SelectShadersPath : Form
    {
        public SelectShadersPath()
        {
            InitializeComponent();
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            string folderPath = string.Empty;

            Thread t = new Thread(() =>
            {
                using (FolderBrowserDialog dialog = new FolderBrowserDialog())
                {
                    dialog.SelectedPath = Program.ProgramFiles;
                    dialog.Description = "Select the game folder";

                    if (dialog.ShowDialog() != DialogResult.OK) return;
                    string selectedFolder = dialog.SelectedPath;

                    if (File.Exists(Path.Combine(selectedFolder, "UnityPlayer.dll")))
                    {
                        MessageBox.Show("That's not the right place.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    folderPath = selectedFolder;
                }
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            if (string.IsNullOrEmpty(folderPath)) return;
            comboBox1.Items.Clear();
            comboBox1.Items.Add(Path.Combine(folderPath, "Genshin-Stella-Mod"));
            comboBox1.SelectedIndex = 0;
        }

        private void SaveSettings_Click(object sender, EventArgs e)
        {
            string selectedFile = comboBox1.GetItemText(comboBox1.SelectedItem);

            Program.ResourcesGlobal = selectedFile;
            File.WriteAllText(Path.Combine(Program.AppData, "resources-path.sfn"), Program.ResourcesGlobal);

            Close();
        }
    }
}
