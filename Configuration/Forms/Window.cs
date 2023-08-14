using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Windows.Storage;
using Configuration.Properties;
using Configuration.Scripts;

namespace Configuration.Forms
{
    public partial class Window : Form
    {
        private static readonly string AppData = GetAppData();
        private static bool _msStore;
        public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
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
        public Window()
        {
            _prepareIni = new IniFile(Path.Combine(AppData, "prepare-stella.ini"));

            InitializeComponent();
        }

        private static string GetAppData()
        {
            try
            {
                _msStore = true;
                return Path.Combine(ApplicationData.Current?.LocalFolder?.Path);
            }
            catch (InvalidOperationException)
            {
                _msStore = false;
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stella Mod Launcher");
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (_msStore)
            {
                checkBox2.Checked = false;
                checkBox3.Checked = false;
            }
            else
            {
                int data1 = _prepareIni.ReadInt("PrepareStella", "NewShortcutsOnDesktop", 1);
                checkBox2.Checked = data1 != 0;

                int data2 = _prepareIni.ReadInt("PrepareStella", "InternetShortcutsInStartMenu", 1);
                checkBox3.Checked = data2 != 0;
            }

            if (!File.Exists(Path.Combine(AppData, "resources-path.sfn")) && !checkBox7.Checked)
            {
                checkBox7.Checked = true;
            }
            else
            {
                bool isMyPatron = CheckData.IsUserMyPatron();
                if (isMyPatron)
                {
                    checkBox7.Checked = false;
                    checkBox7.Enabled = false;
                }
                else
                {
                    int data3 = _prepareIni.ReadInt("PrepareStella", "DownloadOrUpdateShaders", 1);
                    checkBox7.Checked = data3 != 0;
                }
            }

            int data4 = _prepareIni.ReadInt("PrepareStella", "UpdateReShadeConfig", 1);
            checkBox4.Checked = data4 != 0;

            int data5 = _prepareIni.ReadInt("PrepareStella", "UpdateFpsUnlockerConfig", 1);
            checkBox5.Checked = data5 != 0;

            int data6 = _prepareIni.ReadInt("PrepareStella", "DeleteReShadeCache", 1);
            checkBox6.Checked = data6 != 0;

            int data7 = _prepareIni.ReadInt("PrepareStella", "InstOrUpdWT", 1);
            checkBox1.Checked = data7 != 0;

            SaveIniData();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveIniData();
        }

        // Checkboxes
        private void NewShortcutsOnDesktop_CheckedChanged(object sender, EventArgs e)
        {
            if (_msStore && checkBox2.Checked)
            {
                checkBox2.Checked = false;
                MessageBox.Show(Resources.YouCannotCreateANewIconOnYourDesktopWhenAnAppIsInstalledFromTheMStore, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _newShortcutsOnDesktop = sender is CheckBox checkbox && checkbox.Checked ? 1 : 0;
        }

        private void InternetShortcutsInStartMenu_CheckedChanged(object sender, EventArgs e)
        {
            if (_msStore && checkBox3.Checked)
            {
                checkBox3.Checked = false;
                MessageBox.Show(Resources.YouCannotCreateANewIconsInTheSMWhenAnAppIsInstalledFromTheMStore, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _newInternetShortcutsOnDesktop = sender is CheckBox checkbox && checkbox.Checked ? 1 : 0;
        }

        private void DownloadOrUpdateShaders(object sender, EventArgs e)
        {
            if (!File.Exists(Path.Combine(AppData, "resources-path.sfn")) && !checkBox7.Checked)
            {
                checkBox7.Checked = true;
                MessageBox.Show(Resources.TheStellaResourcesDirWasNotFoundOnYourPC, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            string prepareStellaExe = Path.Combine(AppPath, "Prepare Stella Mod.exe");
            if (!File.Exists(prepareStellaExe))
            {
                MessageBox.Show(string.Format(Resources.RequiredFile_WasNotFound, prepareStellaExe), AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = prepareStellaExe,
                WorkingDirectory = AppPath,
                Verb = "runas",
                UseShellExecute = true
            });

            Environment.Exit(0);
        }
    }
}
