using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassLibrary;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using StellaLauncher.Forms;
using StellaLauncher.Properties;
using StellaLauncher.Scripts.Forms;
using File = System.IO.File;

namespace StellaLauncher.Scripts
{
	internal static class Utils
	{
		public static async Task<string> GetGame(string type)
		{
			string fullGamePath = null;
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Program.RegistryPath))
			{
				if (key != null) fullGamePath = (string)key.GetValue("GamePath");
			}

			if (!File.Exists(fullGamePath))
			{
				DialogResult result = MessageBox.Show(string.Format(Resources.Utils_FileWithGamePathWasNotFoundIn_DoYouWantToResetAllSMSettings, fullGamePath), Program.AppNameVer, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				Program.Logger.Info($"File with game path was not found in: {fullGamePath}");

				if (result != DialogResult.Yes) return string.Empty;
				using (RegistryKey newKey = Registry.CurrentUser.CreateSubKey(Program.RegistryPath))
				{
					newKey?.SetValue("AppIsConfigured", 0);
				}

				_ = Cmd.Execute(new Cmd.CliWrap { App = Program.ConfigurationWindow });

				Environment.Exit(99587986);
				return string.Empty;
			}


			switch (type)
			{
				case "giDir":
				{
					string path = Path.GetDirectoryName(Path.GetDirectoryName(fullGamePath));
					Program.Logger.Info($"giDir: {path}");

					return path;
				}

				case "giGameDir":
				{
					string path = Path.GetDirectoryName(fullGamePath);
					string giGameDir = Path.Combine(path);
					if (Directory.Exists(giGameDir)) return giGameDir;

					Program.Logger.Info($"giGameDir: {giGameDir}");
					return string.Empty;
				}

				case "giExe":
				{
					return fullGamePath;
				}

				case "giLauncher":
				{
					string giDir = await GetGame("giDir");

					string genshinImpactExe = Path.Combine(giDir, "launcher.exe");
					if (!File.Exists(genshinImpactExe))
					{
						MessageBox.Show(string.Format(Resources.Utils_LauncherFileDoesNotExist, genshinImpactExe), Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						Program.Logger.Info($"Launcher file does not exists in: {genshinImpactExe} [giLauncher]");
						return string.Empty;
					}

					Program.Logger.Info($"giLauncher: {genshinImpactExe}");
					return genshinImpactExe;
				}

				default:
				{
					Log.ThrowError(new Exception(Resources.Utils_WrongParameter));
					return string.Empty;
				}
			}
		}

		public static async Task<string> GetGameVersion()
		{
			string gvSfn = Path.Combine(Program.AppData, "game-version.sfn");
			string exePath = await GetGame("giExe");
			string exe = Path.GetFileName(exePath);

			string number = exe == "GenshinImpact.exe" ? "1" : "2";
			if (!File.Exists(gvSfn)) File.WriteAllText(gvSfn, number);

			return number;
		}

		public static void OpenUrl(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				Log.ThrowError(new Exception(Resources.Utils_URLIsNullOrEmpty));
				return;
			}

			Music.PlaySound("winxp", "minimize");

			try
			{
				Process.Start(url);
				Program.Logger.Info($"Opened '{url}' in default browser");
			}
			catch (Exception ex)
			{
				Log.ThrowError(new Exception($"Failed to open '{url}' in default browser\n\n{ex}"));
			}
		}

		public static void RemoveClickEvent(Label button)
		{
			FieldInfo eventClickField = typeof(Control).GetField("EventClick", BindingFlags.Static | BindingFlags.NonPublic);
			object eventHandler = eventClickField?.GetValue(button);
			if (eventHandler == null) return;

			PropertyInfo eventsProperty = button.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
			EventHandlerList eventHandlerList = (EventHandlerList)eventsProperty?.GetValue(button, null);

			eventHandlerList?.RemoveHandler(eventHandler, eventHandlerList[eventHandler]);
		}

		public static bool CheckFileExists(string fileName)
		{
			string filePath = Path.Combine(fileName);
			bool fileExists = File.Exists(filePath);

			Program.Logger.Info(fileExists ? $"File '{fileName}' was found at '{filePath}'" : $"File '{fileName}' was not found at '{filePath}'");

			return fileExists;
		}

		public static bool CreateShortcut()
		{
			try
			{
				WshShell shell = new WshShell();
				IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(Shortcut.ScPath);
				shortcut.Description = Resources.Utils_RunOfficialLauncherForStellaModMadeBySefinek;
				shortcut.WorkingDirectory = Program.AppPath;
				shortcut.TargetPath = Shortcut.ProgramExe;
				shortcut.Save();

				Program.Logger.Info($"Desktop shortcut has been created in: {Shortcut.ScPath}");
				return true;
			}
			catch (Exception ex)
			{
				Log.ThrowError(new Exception($"An error occurred while creating the shortcut.\n\n{ex}"));
				return false;
			}
		}

		public static void ShowStartGameBts()
		{
			Default._startGame_LinkLabel.Visible = true;
			Default._injectReShade_LinkLabel.Visible = true;
			Default._runFpsUnlocker_LinkLabel.Visible = true;
			Default._only3DMigoto_LinkLabel.Visible = true;
			Default._runGiLauncher_LinkLabel.Visible = true;
			if (!Secret.IsStellaPlusSubscriber) Default._becomeMyPatron_LinkLabel.Visible = true;
		}

		public static void HideProgressBar(bool error)
		{
			if (error)
			{
				Default.UpdateIsAvailable = false;

				Default._updates_LinkLabel.LinkColor = Color.Red;
				Default._updates_LinkLabel.Text = Resources.Utils_OopsAnErrorOccurred;

				TaskbarProgress.SetProgressValue(100);
				TaskbarProgress.SetProgressState(TaskbarProgress.Flags.Error);
			}

			Default._progressBar1.Hide();
			Default._preparingPleaseWait.Hide();

			Default._progressBar1.Value = 0;
		}

		public static async Task<Image> LoadImageAsync(string url)
		{
			Program.Logger.Info($"Downloading image via LoadImageAsync(): {url}");

			try
			{
				HttpClient httpClient = Program.WbClient.Value;
				HttpResponseMessage response = await httpClient.GetAsync(url);
				byte[] bytes = await response.Content.ReadAsByteArrayAsync();

				MemoryStream ms = new MemoryStream(bytes);
				return Image.FromStream(ms);
			}
			catch (Exception ex)
			{
				BalloonTip.Show(ex.Message, "Something went wrong while attempting to retrieve the image.");
				Program.Logger.Error(ex);
				return null;
			}
		}
	}
}
