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
		public static readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
		private static readonly string AppPath = AppDomain.CurrentDomain.BaseDirectory;
		private static readonly string AppData = GetAppData();
		private static bool _msStore;

		private static int _newShortcutsOnDesktop;
		private static int _newInternetShortcutsOnDesktop;
		private static int _downloadOrUpdateShaders;
		private static int _updateReShadeConfig;
		private static int _updateFpsUnlockerConfig;
		private static int _deleteReShadeCache;
		private static int _instOrUpdWt;

		private static IniFile _prepareIni;

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
				checkBox2.Checked = _prepareIni.ReadInt("PrepareStella", "NewShortcutsOnDesktop", 1) != 0;
				checkBox3.Checked = _prepareIni.ReadInt("PrepareStella", "InternetShortcutsInStartMenu", 1) != 0;
			}

			bool foundResources = CheckData.ResourcesPath();
			if (!foundResources)
			{
				checkBox7.Checked = true;
				checkBox7.Enabled = false;
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
					checkBox7.Checked = _prepareIni.ReadInt("PrepareStella", "DownloadOrUpdateShaders", 1) != 0;
				}
			}

			checkBox4.Checked = _prepareIni.ReadInt("PrepareStella", "UpdateReShadeConfig", 1) != 0;
			checkBox5.Checked = _prepareIni.ReadInt("PrepareStella", "UpdateFpsUnlockerConfig", 1) != 0;
			checkBox6.Checked = _prepareIni.ReadInt("PrepareStella", "DeleteReShadeCache", 1) != 0;
			checkBox1.Checked = _prepareIni.ReadInt("PrepareStella", "InstOrUpdWT", 1) != 0;

			SaveIniData();
		}

		private void Main_FormClosing(object sender, FormClosingEventArgs e)
		{
			SaveIniData();
		}

		private void NewShortcutsOnDesktop_CheckedChanged(object sender, EventArgs e)
		{
			if (_msStore && checkBox2.Checked)
			{
				checkBox2.Checked = false;
				MessageBox.Show(Resources.YouCannotCreateANewIconOnYourDesktopWhenAnAppIsInstalledFromTheMStore, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			_newShortcutsOnDesktop = checkBox2.Checked ? 1 : 0;
		}

		private void InternetShortcutsInStartMenu_CheckedChanged(object sender, EventArgs e)
		{
			if (_msStore && checkBox3.Checked)
			{
				checkBox3.Checked = false;
				MessageBox.Show(Resources.YouCannotCreateANewIconsInTheSMWhenAnAppIsInstalledFromTheMStore, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			_newInternetShortcutsOnDesktop = checkBox3.Checked ? 1 : 0;
		}

		private void DownloadOrUpdateShaders(object sender, EventArgs e)
		{
			bool foundResources = CheckData.ResourcesPath();
			if (!foundResources && !checkBox7.Checked)
			{
				checkBox7.Checked = true;
				MessageBox.Show(Resources.TheStellaResourcesDirWasNotFoundOnYourPC, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			_downloadOrUpdateShaders = checkBox7.Checked ? 1 : 0;
		}

		private void UpdateReShadeConfig_CheckedChanged(object sender, EventArgs e)
		{
			_updateReShadeConfig = checkBox4.Checked ? 1 : 0;
		}

		private void UpdateFpsUnlockerConfig_CheckedChanged(object sender, EventArgs e)
		{
			_updateFpsUnlockerConfig = checkBox5.Checked ? 1 : 0;
		}

		private void DeleteReShadeCache_CheckedChanged(object sender, EventArgs e)
		{
			_deleteReShadeCache = checkBox6.Checked ? 1 : 0;
		}

		private void InstOrUpdWT_CheckedChanged(object sender, EventArgs e)
		{
			_instOrUpdWt = checkBox1.Checked ? 1 : 0;
		}

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
