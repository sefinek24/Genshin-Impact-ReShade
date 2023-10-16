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
                    Program.Settings.WriteString("Injection", "Method", "exe");
                    Run.InjectType = "exe";
                    break;


                case 1:
                    if (Secret.IsMyPatron)
                    {
                        Program.Settings.WriteString("Injection", "Method", "cmd");
                        Program.Settings.Save();
                        Run.InjectType = "cmd";

                        MessageBox.Show(@"WARNING! SETTING CHANGED TO ONE DIFFERENT FROM THE RECOMMENDED ONE! I HOPE YOU KNOW WHAT YOU'RE DOING! 

This option is intended for users with any knowledge in the IT field and in the Batch scripting language. With this option, you can have full control over the injection process in the .cmd file.

!!! THE CREATOR OF THIS SOFTWARE TAKES NO RESPONSIBILITY FOR ANY BANS IN THE GAME. BY USING THIS FUNCTION, YOU ACCEPT THIS RISK !!!", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show(@"Only patrons can use this option.", Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    break;
            }
        }
    }
}
