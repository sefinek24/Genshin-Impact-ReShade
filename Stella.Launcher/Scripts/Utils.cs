using System.Diagnostics;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using StellaModLauncher.Forms;
using StellaModLauncher.Properties;
using StellaModLauncher.Scripts.Forms;
using File = System.IO.File;

namespace StellaModLauncher.Scripts;

internal static class Utils
{
	public enum StatusType
	{
		Info,
		Success,
		Error
	}

	private static readonly Dictionary<LinkLabel, LinkLabelLinkClickedEventHandler?> LinkClickedHandlers = [];

	public static async Task<string?> GetGame(string type)
	{
		string? fullGamePath = null;
		using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(Program.RegistryPath))
		{
			if (key != null) fullGamePath = key.GetValue("GamePath") as string;
		}

		if (!File.Exists(fullGamePath))
		{
			DialogResult result = MessageBox.Show(string.Format(Resources.Utils_FileWithGamePathWasNotFoundIn_DoYouWantToResetAllSMSettings, fullGamePath), Program.AppNameVer, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			Program.Logger.Info($"File with game path was not found in: {fullGamePath}");

			if (result != DialogResult.Yes) return string.Empty;
			using (RegistryKey newKey = Registry.CurrentUser.CreateSubKey(Program.RegistryPath))
			{
				newKey.SetValue("AppIsConfigured", 0);
			}

			_ = Cmd.Execute(new Cmd.CliWrap { App = Program.ConfigurationWindow });

			Environment.Exit(99587986);
			return string.Empty;
		}


		switch (type)
		{
			case "giDir":
			{
				string? path = Path.GetDirectoryName(Path.GetDirectoryName(fullGamePath));
				Program.Logger.Info($"giDir: {path}");

				return path;
			}

			case "giGameDir":
			{
				string? path = Path.GetDirectoryName(fullGamePath);
				string giGameDir = Path.Combine(path!);
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
				string? giDir = await GetGame("giDir").ConfigureAwait(false);

				string genshinImpactExe = Path.Combine(giDir!, "launcher.exe");
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
				Log.ThrowError(new Exception("Wrong parameter."));
				return string.Empty;
			}
		}
	}

	public static async Task<string> GetGameVersion()
	{
		string gvSfn = Path.Combine(Program.AppData, "game-version.sfn");
		string? exePath = await GetGame("giExe").ConfigureAwait(false);
		string? exe = Path.GetFileName(exePath);

		string number = exe == "GenshinImpact.exe" ? "1" : "2";
		if (!File.Exists(gvSfn)) await File.WriteAllTextAsync(gvSfn, number).ConfigureAwait(false);

		return number;
	}

	public static void OpenUrl(string? url)
	{
		if (string.IsNullOrEmpty(url))
		{
			Log.ThrowError(new Exception("URL is null or empty."));
			return;
		}

		Music.PlaySound("winxp", "minimize");

		try
		{
			ProcessStartInfo psi = new()
			{
				FileName = url,
				UseShellExecute = true
			};

			Process.Start(psi);
			Program.Logger.Info($"Opened '{url}' in default browser");
		}
		catch (Exception ex)
		{
			Log.ThrowError(new Exception($"Failed to open '{url}' in default browser\n\n{ex}"));
		}
	}

	public static void AddLinkClickedEventHandler(LinkLabel linkLabel, LinkLabelLinkClickedEventHandler? handler)
	{
		RemoveLinkClickedEventHandler(linkLabel);
		linkLabel.LinkClicked += handler;
		LinkClickedHandlers[linkLabel] = handler;
	}

	public static void RemoveLinkClickedEventHandler(LinkLabel linkLabel)
	{
		if (!LinkClickedHandlers.TryGetValue(linkLabel, out LinkLabelLinkClickedEventHandler? existingHandler)) return;

		linkLabel.LinkClicked -= existingHandler;
		LinkClickedHandlers.Remove(linkLabel);
	}

	public static bool CheckFileExists(string? fileName)
	{
		string filePath = Path.Combine(fileName!);
		bool fileExists = File.Exists(filePath);

		Program.Logger.Info(fileExists ? $"File '{fileName}' was found at '{filePath}'" : $"File '{fileName}' was not found at '{filePath}'");

		return fileExists;
	}

	public static bool CreateShortcut()
	{
		try
		{
			WshShell shell = new();
			IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(Shortcut.ScPath);
			shortcut.Description = Resources.Utils_RunOfficialLauncherForStellaModMadeBySefinek;
			shortcut.WorkingDirectory = Program.AppPath;
			shortcut.TargetPath = Shortcut.ProgramExe;
			shortcut.Save();

			Program.Logger.Info("Desktop shortcut has been created");
			Program.Logger.Info($"CreateShortcut(): {Shortcut.ScPath}");
			Program.Logger.Info($"WorkingDirectory(): {Shortcut.ScPath}");
			Program.Logger.Info($"TargetPath(): {Shortcut.ProgramExe}");
			return true;
		}
		catch (Exception ex)
		{
			Log.ThrowError(new Exception($"An error occurred while creating the shortcut.\n\n{ex}"));
			return false;
		}
	}

	public static async Task<Image?> LoadImageAsync(string? url)
	{
		Program.Logger.Info($"Downloading image via LoadImageAsync(): {url}");

		try
		{
			HttpClient httpClient = Program.SefinWebClient;
			HttpResponseMessage response = await httpClient.GetAsync(url).ConfigureAwait(true);
			byte[] bytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

			MemoryStream ms = new(bytes);
			return Image.FromStream(ms);
		}
		catch (Exception ex)
		{
			BalloonTip.Show(ex.Message, "Something went wrong while attempting to retrieve the image.");
			Program.Logger.Error(ex);
			return null;
		}
	}

	public static void UpdateStatusLabel(string text, StatusType status, bool playSound = true)
	{
		string prefix = status switch
		{
			StatusType.Info => "[i]",
			StatusType.Success => "[✓]",
			StatusType.Error => "[X]",
			_ => "[-]"
		};

		Default._status_Label!.Text += @$"{prefix} {text}{Environment.NewLine}";

		Action<string> action = status switch
		{
			StatusType.Error => Program.Logger.Error,
			_ => Program.Logger.Info
		};
		action(text);

		string[] lines = Default._status_Label.Text.Split('\n');
		if (lines.Length > 10) Default._status_Label.Text = string.Join("\n", lines.Skip(lines.Length - 10));

		if (playSound) Music.PlaySound("winxp", "balloon");
	}
}
