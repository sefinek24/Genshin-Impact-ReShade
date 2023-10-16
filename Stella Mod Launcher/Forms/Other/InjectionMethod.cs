using System;
using System.Windows.Forms;
using StellaLauncher.Scripts;
using StellaLauncher.Scripts.Forms.MainForm;

namespace StellaLauncher.Forms.Other
{
    public partial class InjectionMethod : Form
    {
        public InjectionMethod()
        {
            InitializeComponent();
        }

        private void InjectionMethod_Load(object sender, EventArgs e)
        {
            switch (Run.InjectType)
            {
                case "exe":
                    comboBox1.SelectedIndex = 0;
                    break;
                case "cmd":
                    comboBox1.SelectedIndex = 1;
                    break;
            }
        }

        private void Method_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    Program.Settings.WriteString("Launcher", "InjectType", "exe");
                    Run.InjectType = "exe";
                    break;


                case 1:
                    if (Secret.IsMyPatron)
                    {
                        Program.Settings.WriteString("Launcher", "InjectType", "cmd");
                        Program.Settings.Save();

                        Run.InjectType = "cmd";
                    }
                    else
                    {
                        MessageBox.Show("You're not my patron.");
                    }

                    break;
            }
        }

        private void SaveData_Click(object sender, EventArgs e)
        {
        }
    }
}
