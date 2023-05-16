using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Windows.Storage;
using First_app_launch.Scripts;

namespace Conf_window.Forms
{
    public partial class Main : Form
    {
        private static readonly string AppData = GetAppData();
        private static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
        private static readonly string AppPath = AppDomain.CurrentDomain.BaseDirectory;

        private static IniFile _prepareIni;

        private static int _newShortcutsOnDesktop;
        private static int _newInternetShortcutsOnDesktop;
        private static int _downloadOrUpdateShaders;
        private static int _updateReShadeConfig;
        private static int _updateFpsUnlockerConfig;
        private static int _deleteReShadeCache;
        private static int _instOrUpdWt;

        // Main
        public Main()
        {
            _prepareIni = new IniFile(Path.Combine(AppData, "prepare-stella.ini"));

            InitializeComponent();
        }

        private static string GetAppData()
        {
            try
            {
                return Path.Combine(ApplicationData.Current?.LocalFolder?.Path);
            }
            catch (InvalidOperationException)
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Genshin Stella Mod");
            }
        }


        private void Main_Load(object sender, EventArgs e)
        {
            int data1 = _prepareIni.ReadInt("PrepareStella", "NewShortcutsOnDesktop", 1);
            checkBox2.Checked = data1 != 0;

            int data2 = _prepareIni.ReadInt("PrepareStella", "InternetShortcutsInStartMenu", 1);
            checkBox3.Checked = data2 != 0;

            int data3 = _prepareIni.ReadInt("PrepareStella", "DownloadOrUpdateShaders", 1);
            checkBox7.Checked = data3 != 0;

            int data4 = _prepareIni.ReadInt("PrepareStella", "UpdateReShadeConfig", 1);
            checkBox4.Checked = data4 != 0;

            int data5 = _prepareIni.ReadInt("PrepareStella", "UpdateFpsUnlockerConfig", 1);
            checkBox5.Checked = data5 != 0;

            int data6 = _prepareIni.ReadInt("PrepareStella", "DeleteReShadeCache", 0);
            checkBox6.Checked = data6 != 0;

            int data7 = _prepareIni.ReadInt("PrepareStella", "InstOrUpdWT", 1);
            checkBox1.Checked = data7 != 0;

            if (!File.Exists(Path.Combine(AppData, "resources-path.sfn")) && !checkBox7.Checked) checkBox7.Checked = true;

            SaveIniData();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveIniData();
        }

        // Checkboxes
        private void NewShortcutsOnDesktop_CheckedChanged(object sender, EventArgs e)
        {
            _newShortcutsOnDesktop = sender is CheckBox checkbox && checkbox.Checked ? 1 : 0;
        }

        private void InternetShortcutsInStartMenu_CheckedChanged(object sender, EventArgs e)
        {
            _newInternetShortcutsOnDesktop = sender is CheckBox checkbox && checkbox.Checked ? 1 : 0;
        }

        private void DownloadOrUpdateShaders(object sender, EventArgs e)
        {
            if (!File.Exists(Path.Combine(AppData, "resources-path.sfn")) && !checkBox7.Checked)
            {
                checkBox7.Checked = true;
                MessageBox.Show(@"The directory with mod resources was not found on your computer. You cannot enable this checkbox.", AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _downloadOrUpdateShaders = checkBox7.Checked ? 1 : 0;
        }


        private void UpdateReShadeConfig_CheckedChanged(object sender, EventArgs e)
        {
            _updateReShadeConfig = sender is CheckBox checkbox && checkbox.Checked ? 1 : 0;
        }

        private void UpdateFpsUnlockerConfig_CheckedChanged(object sender, EventArgs e)
        {
            _updateFpsUnlockerConfig = sender is CheckBox checkbox && checkbox.Checked ? 1 : 0;
        }

        private void DeleteReShadeCache_CheckedChanged(object sender, EventArgs e)
        {
            _deleteReShadeCache = sender is CheckBox checkbox && checkbox.Checked ? 1 : 0;
        }

        private void InstOrUpdWT_CheckedChanged(object sender, EventArgs e)
        {
            _instOrUpdWt = sender is CheckBox checkbox && checkbox.Checked ? 1 : 0;
        }


        // Done! Save data.
        private static void SaveIniData()
        {
            _prepareIni.WriteInt("PrepareStella", "NewShortcutsOnDesktop", _newShortcutsOnDesktop);
            _prepareIni.WriteInt("PrepareStella", "InternetShortcutsInStartMenu", _newInternetShortcutsOnDesktop);
            _prepareIni.WriteInt("PrepareStella", "DownloadOrUpdateShaders", _downloadOrUpdateShaders);
            _prepareIni.WriteInt("PrepareStella", "UpdateReShadeConfig", _updateReShadeConfig);
            _prepareIni.WriteInt("PrepareStella", "UpdateFpsUnlockerConfig", _updateFpsUnlockerConfig);
            _prepareIni.WriteInt("PrepareStella", "DeleteReShadeCache", _deleteReShadeCache);
            _prepareIni.WriteInt("PrepareStella", "InstOrUpdWT", _instOrUpdWt);
        }

        private void LetsGo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaveIniData();

            string exeFile = Path.Combine(AppPath, "Prepare Stella Mod.exe");
            if (!File.Exists(exeFile))
            {
                MessageBox.Show($@"Required file '{exeFile}' was not found. Please reinstall this app or join our Discord server.", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = exeFile,
                WorkingDirectory = AppPath,
                Verb = "runas",
                UseShellExecute = true
            });

            Environment.Exit(0);
        }
    }
}
