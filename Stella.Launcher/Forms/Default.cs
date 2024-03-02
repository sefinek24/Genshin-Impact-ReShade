using System.Diagnostics;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Win32;
using Newtonsoft.Json;
using NuGet.Versioning;
using StellaModLauncher.Forms.Other;
using StellaModLauncher.Models;
using StellaModLauncher.Properties;
using StellaModLauncher.Scripts;
using StellaModLauncher.Scripts.Forms;
using StellaModLauncher.Scripts.Forms.MainForm;
using StellaModLauncher.Scripts.Patrons;
using StellaModLauncher.Scripts.Remote;
using StellaPLFNet;
using Shortcut = StellaModLauncher.Scripts.Shortcut;
using Telemetry = StellaTelemetry.Telemetry;

namespace StellaModLauncher.Forms;

public partial class Default : Form
{
	// New update?
	public static bool UpdateIsAvailable;

	// Main
	public static Label? _status_Label;
	public static Label? _preparingPleaseWait;
	public static ProgressBar? _progressBar1;

	// Left
	public static PictureBox? _discordServerIco_Picturebox;
	public static LinkLabel? _discordServer_LinkLabel;
	public static PictureBox? _supportMeIco_PictureBox;
	public static LinkLabel? _supportMe_LinkLabel;
	public static PictureBox? _youtubeIco_Picturebox;
	public static LinkLabel? _youTube_LinkLabel;

	// Start the game
	public static LinkLabel? _startGame_LinkLabel;
	public static LinkLabel? _injectReShade_LinkLabel;
	public static LinkLabel? _runFpsUnlocker_LinkLabel;
	public static LinkLabel? _only3DMigoto_LinkLabel;
	public static LinkLabel? _runGiLauncher_LinkLabel;
	public static LinkLabel? _becomeMyPatron_LinkLabel;

	// Right
	public static LinkLabel? _version_LinkLabel;
	public static LinkLabel? _checkForUpdates_LinkLabel;
	public static PictureBox? _updateIco_PictureBox;

	// Bottom
	public static WebView2? _webView21;

	// Path
	public static string? ResourcesPath;

	// Window
	private bool _mouseDown;
	private Point _offset;

	public Default()
	{
		InitializeComponent();

		DoubleBuffered = true;
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
		TaskbarProgress.MainWinHandle = Handle;
	}

	private void Default_Load(object sender, EventArgs e)
	{
		// Set background
		Image? newBackground = Background.OnStart(toolTip1, changeBg_LinkLabel);
		if (newBackground != null) BackgroundImage = newBackground;

		RoundedCorners.Form(this);
		Music.LinkLabelSfx(this);
	}

	private async void Main_Shown(object sender, EventArgs e)
	{
		// First
		_status_Label = status_Label;
		_preparingPleaseWait = PreparingPleaseWait;
		_progressBar1 = progressBar1;

		_discordServerIco_Picturebox = discordServerIco_Picturebox;
		_discordServer_LinkLabel = discordServer_LinkLabel;
		_supportMeIco_PictureBox = supportMeIco_PictureBox;
		_supportMe_LinkLabel = supportMe_LinkLabel;
		_youtubeIco_Picturebox = webIco_Picturebox;
		_youTube_LinkLabel = web_LinkLabel;

		_startGame_LinkLabel = startGame_LinkLabel;
		_injectReShade_LinkLabel = injectReShade_LinkLabel;
		_runFpsUnlocker_LinkLabel = runFpsUnlocker_LinkLabel;
		_only3DMigoto_LinkLabel = only3DMigoto_LinkLabel;
		_runGiLauncher_LinkLabel = runGiLauncher_LinkLabel;
		_becomeMyPatron_LinkLabel = becomeMyPatron_LinkLabel;

		_version_LinkLabel = version_LinkLabel;
		_checkForUpdates_LinkLabel = checkForUpdates_LinkLabel;
		_updateIco_PictureBox = updateIco_PictureBox;

		_webView21 = webView21;


		// Registry
		Stages.UpdateStage(1, "Updating registry...");

		string? resourcesPath = null;
		using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(Program.RegistryPath))
		{
			if (key != null) resourcesPath = key.GetValue("ResourcesPath") as string;
		}

		if (string.IsNullOrEmpty(resourcesPath))
		{
			Program.Logger.Error("Path of the resources was not found. Is null or empty.");
			MessageBox.Show(Resources.Default_ResourceDirNotFound, Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		if (!Directory.Exists(resourcesPath))
		{
			Program.Logger.Error($"Directory with the resources '{resourcesPath}' was not found.");
			MessageBox.Show(string.Format(Resources.Default_Directory_WasNotFound, resourcesPath), Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		if (string.IsNullOrEmpty(resourcesPath) || !Directory.Exists(resourcesPath))
		{
			_ = Cmd.Execute(new Cmd.CliWrap { App = Program.ConfigurationWindow });
			Application.Exit();
		}

		ResourcesPath = resourcesPath;

		Utils.AddLinkClickedEventHandler(_checkForUpdates_LinkLabel, CheckForUpdates.CheckUpdates_Click);


		// App version
		Stages.UpdateStage(2, "Loading Stella Mod into the Tray...");
		GlobalHelpers.NotifyIconInstance = new NotifyIcon
		{
			Icon = Program.Ico,
			Text = Program.AppNameVer,
			Visible = true,
			ContextMenuStrip = new ContextMenuStrip()
		};

		Tray trayHandler = new(GlobalHelpers.NotifyIconInstance, this);
		GlobalHelpers.NotifyIconInstance.ContextMenuStrip.Items.AddRange(new ToolStripItem[]
		{
			new ToolStripMenuItem("Toggle Minimize/Restore", null, trayHandler.ToggleMinimizeRestore),
			new ToolStripMenuItem("Reload window", null, trayHandler.ReloadForm),
			new ToolStripMenuItem("Official website", null, Tray.OfficialWebsite),
			new ToolStripMenuItem("Stella Mod Plus", null, Tray.StellaModPlus),
			new ToolStripMenuItem("Visit Discord server", null, Tray.DiscordServer),
			new ToolStripMenuItem("Support", null, Tray.Support),
			new ToolStripMenuItem("Leave your feedback", null, Tray.Feedback),
			new ToolStripMenuItem("Donations", null, Tray.Donations),
			new ToolStripMenuItem("Quit", null, Tray.OnQuitClick)
		});


		// Is user my Patron?
		Stages.UpdateStage(3, "Checking Stella Mod Plus subscription...");
		string? registrySecret = Secret.GetTokenFromRegistry();


		Stages.UpdateStage(4, "Verifying Stella Mod Plus subscription...");
		if (registrySecret != null)
		{
			label1.Text = @"/ᐠ. ｡.ᐟ\ᵐᵉᵒʷˎˊ˗";

			Secret.GetDeviceId();

			string? data = await Secret.VerifyToken(registrySecret).ConfigureAwait(true);
			if (data == null)
			{
				Secret.IsStellaPlusSubscriber = false;

				Program.Logger.Info("Received null from the server. Deleting benefits in progress...");
				DeleteBenefits.Start();
				Delete.RegistryKey("Secret");

				MessageBox.Show(
					"The customer received zero data. Your subscription cannot be verified for some reason. No further details are available. Please contact the software creator for more information.\n\nThe launcher will be started without the benefits of a subscription.",
					Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Error);

				label1.Text = @"null :c";
			}
			else
			{
				VerifyToken? remote = JsonConvert.DeserializeObject<VerifyToken>(data);

				if (remote!.DeleteBenefits) DeleteBenefits.Start();
				if (remote.DeleteTokens) Delete.RegistryKey("Secret");

				switch (remote.Status)
				{
					case 200:
						Secret.IsStellaPlusSubscriber = true;
						label1.Text = @"Genshin Stella Mod Plus Launcher";
						madeBySefinek_Label.Text = @"What will we be doing today?";

						Image? avatar = null;
						if (!string.IsNullOrEmpty(remote.AvatarUrl)) avatar = await Utils.LoadImageAsync(remote.AvatarUrl).ConfigureAwait(true);

						if (avatar == null)
						{
							Program.Logger.Warn($"remote.AvatarUrl is still null or empty: {remote.AvatarUrl}");
							avatar = await Utils.LoadImageAsync("https://i.pinimg.com/originals/17/e8/90/17e890c5c6bdb755d58b8bf975861198.jpg").ConfigureAwait(true);
						}

						pictureBox4.Visible = true;
						pictureBox4.Image = RoundedCorners.Picture(avatar, 12);

						clickMe_LinkLabel.Location = new Point(1034, 163);
						paimon_PictureBox.Location = new Point(1173, 157);

						label2.Text = string.Format(label2.Text, remote.Username);
						label2.Visible = true;
						Secret.Username = remote.Username;

						Program.Logger.Info($"The user ({Secret.Username}) is a subscriber of Stella Mod Plus ({Secret.IsStellaPlusSubscriber}). Tier {remote.TierId}. Benefits have been unlocked!");
						Secret.BearerToken = remote.Token;
						break;

					case 400:
						Secret.IsStellaPlusSubscriber = false;
						label1.Text = @"Something went wrong /ᐠﹷ ‸ ﹷ ᐟ\ﾉ";

						MessageBox.Show(
							$"Oh, it looks like something went wrong during the verification of your subscription. The client sent incorrect information to the server. An error with code {remote.Status} has been received. Please check if you are not using VPNs or proxies.\n\nUnfortunately, the benefits of the subscription will not be available at this time. Please try again or contact the software creator (preferably on the Discord server or via email: {Data.Email}).\n\n\n{remote.Message}",
							Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Error);
						break;

					case 500:
						Secret.IsStellaPlusSubscriber = false;
						label1.Text = @"Fatal server error /ᐠ_ ꞈ _ᐟ\";

						MessageBox.Show(
							$"Unfortunately, there was a server-side error during the verification of your benefits. Please report this error on the Discord server or via email. Remember to provide your `backup code` as well.\nIf you launch the game after closing this message, you will be playing the free version.\n\n{remote.Message}",
							Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Error);
						break;

					default:
						Secret.IsStellaPlusSubscriber = false;
						label1.Text = @"Oh nooo... Sad cat... ( ̿–ᆺ ̿–)";

						MessageBox.Show(
							$"An error occurred while verifying the benefits of your subscription (error code {remote.Status}). The server informed the client that it sent an invalid request. If you launch the game after closing this message, you will be playing the free version. Please contact Sefinek for more information. Error details can be found below.\n\n{remote.Message}",
							Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						break;
				}
			}
		}
		else
		{
			Secret.IsStellaPlusSubscriber = false;

			DeleteBenefits.Start();
			Delete.RegistryKey("Secret");
		}


		// Check if all required files exists
		Stages.UpdateStage(5, "Verifying required files...");
		await Files.ScanAsync().ConfigureAwait(true);


		// Delete setup file from Temp directory
		Stages.UpdateStage(6, "Checking the installation file after the update...");
		Files.DeleteSetupAsync();

		// Loaded form
		Program.Logger.Info(string.Format(Resources.Main_LoadedForm_, Text));


		// Check ReShade & FPS Unlock version
		Stages.UpdateStage(7, "Checking ReShade & FPS Unlock version...");
		NuGetVersion reshadeVersion = NuGetVersion.Parse(FileVersionInfo.GetVersionInfo(Program.ReShadePath).ProductVersion!);
		NuGetVersion fpsUnlockVersion = NuGetVersion.Parse(FileVersionInfo.GetVersionInfo(Program.FpsUnlockerExePath).ProductVersion!);
		Data.ReShadeVer = reshadeVersion.ToString();
		Data.UnlockerVer = fpsUnlockVersion.ToString();


		// Discord RPC
		Stages.UpdateStage(9, "Initializing Discord RPC...");
		Discord.InitRpc();

		// Telemetry
		// Telemetry.Opened();

		// Updated?
		int updatedLauncher = Program.Settings.ReadInt("Updates", "UpdateAvailable", 0);
		string oldVersion = Program.Settings.ReadString("Updates", "OldVersion");
		if (updatedLauncher == 1 && oldVersion != Program.AppFileVersion)
		{
			Program.Settings.WriteInt("Updates", "UpdateAvailable", 0);
			Program.Settings.Save();

			Utils.UpdateStatusLabel(Resources.Default_Congratulations, Utils.StatusType.Success, false);
			Utils.UpdateStatusLabel(string.Format(Resources.Default_SMLSuccessfullyUpdatedToVersion_, Program.AppFileVersion), Utils.StatusType.Info);

			pictureBox5.Show();
			viewChangelog_LinkLabel.Show();
		}


		// Check InjectType
		string injectMode = Program.Settings.ReadString("Injection", "Method", "exe");
		switch (injectMode)
		{
			case "exe":
				Run.InjectType = "exe";
				break;
			case "cmd" when Secret.IsStellaPlusSubscriber:
				Run.InjectType = "cmd";
				break;
			default:
			{
				Run.InjectType = "exe";
				Program.Settings.WriteString("Injection", "Method", "exe");
				Program.Settings.Save();

				if (!Secret.IsStellaPlusSubscriber)
				{
					Utils.UpdateStatusLabel("Batch file usage in Genshin Stella Mod is exclusive to Stella Mod Plus subscribers.", Utils.StatusType.Error);
					Program.Logger.Error("To utilize batch files, a subscription to Stella Mod Plus is required.");
				}

				break;
			}
		}


		// Check for updates
		Stages.UpdateStage(10, "Checking for updates...");
		int found = await CheckForUpdates.Analyze().ConfigureAwait(true);
		if (found == 2) return;

		Stages.UpdateStage(11, "Checking Genshin Stella Mod.exe and ReShade.ini...");

		// Check Genshin Stella Mod.exe
		if (!File.Exists(Run.GsmPath))
		{
			string fileName = Path.GetFileName(Run.GsmPath);

			Program.Logger.Error($"{fileName} was not found in {Run.GsmPath}");
			Utils.UpdateStatusLabel($"Not found `{fileName}` in:", Utils.StatusType.Error, false);
			Utils.UpdateStatusLabel(Path.GetDirectoryName(Run.GsmPath)!, Utils.StatusType.Error);

			MessageBox.Show($"Required file {fileName} was not found.\n\nReinstalling the application may be necessary.", Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Error);
			Environment.Exit(89675);
		}

		// Check if ReShade.ini exists
		await ReShadeIni.CheckIfExists().ConfigureAwait(true);

		// Check shortcut
		Shortcut.Check();


		Stages.UpdateStage(12, "Finishing...");

		// Music
		Music.PlayBg();

		// Update LastRunTime
		using (RegistryKey key2 = Registry.CurrentUser.CreateSubKey(Program.RegistryPath, true))
		{
			key2.SetValue("LastRunTime", DateTime.Now);
		}


		// Launch count
		int launchCount = await LaunchCountHelper.CheckLaunchCountAndShowMessages().ConfigureAwait(true);
		await WebViewHelper.Initialize(webView21, $"https://api.sefinek.net/api/v2/moecounter?number={launchCount}&length=7&theme=rule34", true).ConfigureAwait(true);

		if (launchCount == 100)
		{
			MessageBox.Show("This is your hundredth (100th) launch of the program by you on your device.\n\nThank you!", Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Information);

			string path = Path.Combine(Program.AppPath, "data", "videos", "theannoyingcat.mp4");
			if (Utils.CheckFileExists(path)) Utils.OpenUrl(path);
		}

		// Stats
		var telemetryData = new
		{
			Program.AppVersion,
			Program.AppVersionFull,
			Program.AppName,
			Program.AppPath,
			Program.AppData
		};

		string jsonData = JsonConvert.SerializeObject(telemetryData);
		await Telemetry.SendStatsAsync(jsonData).ConfigureAwait(true);

		Stages.UpdateStage(13, Secret.IsStellaPlusSubscriber ? $"Welcome {Secret.Username} to the Stella Mod Launcher app!" : "Welcome to the Stella Mod Launcher app!");
		if (Secret.IsStellaPlusSubscriber) Music.PlaySound("other", "interface-welcome", 0.12f);


		// 2024
		// status_Label.Text += @"[i] Happy New Year 2024! Thank you all for your trust ~~ Sefinek";
	}

	private void Exit_Click(object sender, EventArgs e)
	{
		Application.Exit();
	}

	private void Default_FormClosing(object sender, FormClosingEventArgs e)
	{
		Program.Logger.Info(string.Format(Resources.Main_ClosingForm_, Text));

		if (GlobalHelpers.NotifyIconInstance == null) return;
		GlobalHelpers.NotifyIconInstance.Visible = false;
		GlobalHelpers.NotifyIconInstance.Dispose();
		Program.Logger.Info("Disposed NotifyIconInstance");
	}

	private void Default_FormClosed(object sender, FormClosedEventArgs e)
	{
		Program.Logger.Info(Resources.Main_Closed);
	}

	private void MouseDown_Event(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Left) return;
		_offset = new Point(e.X, e.Y);
		_mouseDown = true;
	}

	private void MouseMove_Event(object sender, MouseEventArgs e)
	{
		if (!_mouseDown) return;
		Point currentScreenPos = PointToScreen(e.Location);
		Location = new Point(currentScreenPos.X - _offset.X, currentScreenPos.Y - _offset.Y);
	}

	private void MouseUp_Event(object sender, MouseEventArgs e)
	{
		_mouseDown = false;
	}

	private void ChangeBg_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Image? newBackground = Background.Change(toolTip1, changeBg_LinkLabel);
		if (newBackground != null) BackgroundImage = newBackground;

		Music.PlaySound("other", "multi-pop", 0.09f);
	}


	// ------- Body -------
	private void GitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Utils.OpenUrl("https://github.com/sefinek24/Genshin-Impact-ReShade");
	}


	// ------- Start the game -------
	// 1 = ReShade       + 3DMigoto      + FPS Unlocker  = 1 (default for Stella Mod Plus)
	// 2 = ReShade       + 3DMigoto                      = 2
	// 6 = ReShade       + FPS Unlocker                  = 6 (default)
	// 4 = FPS Unlocker                                  = 4
	// 5 = 3DMigoto                                      = 5
	// 3 = ReShade                                       = 3

	/* 1 */
	private async void StartGame_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		await Run.StartGame().ConfigureAwait(false);
	}

	/* 3 */
	private async void OnlyReShade_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		await Run.ReShade().ConfigureAwait(false);
	}

	/* 4 */
	private async void OnlyUnlocker_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		await Run.FpsUnlocker().ConfigureAwait(false);
	}

	/* 5 */
	private async void Only3DMigoto_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		await Run.Migoto().ConfigureAwait(false);
	}

	private async void OpenGILauncher_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		string? giLauncher = await Utils.GetGame("giLauncher").ConfigureAwait(false);
		if (string.IsNullOrEmpty(giLauncher))
		{
			MessageBox.Show(Resources.Default_GameLauncherWasNotFound, Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			Program.Logger.Error(string.Format(Resources.Default_GameLauncherWasNotFoundIn, giLauncher));
			return;
		}

		await Cmd.Execute(new Cmd.CliWrap { App = giLauncher }).ConfigureAwait(false);
	}


	// ------- Footer -------
	private void Subscription_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Utils.OpenUrl("https://sefinek.net/genshin-impact-reshade/subscription?referrer=launcher");
	}

	private void SupportMe_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		if (Application.OpenForms.OfType<SupportMe>().Any()) return;
		new SupportMe { DesktopLocation = DesktopLocation, Icon = Program.Ico }.Show();
	}

	private void DiscordServer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Utils.OpenUrl(Discord.Invitation);
	}

	private void Website_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Utils.OpenUrl(Program.AppWebsiteFull);
	}

	private void Tools_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		if (Application.OpenForms.OfType<Tools>().Any()) return;
		new Tools { DesktopLocation = DesktopLocation, Icon = Program.Ico }.Show();
		Music.PlaySound("winxp", "navigation_start");
	}

	private void Settings_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		if (Application.OpenForms.OfType<Settings>().Any()) return;
		new Settings { DesktopLocation = DesktopLocation, Icon = Program.Ico }.Show();
		Music.PlaySound("winxp", "navigation_start");
	}

	private void ViewResources_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Cmd.Start(ResourcesPath);
	}

	private void Gameplay_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		if (Application.OpenForms.OfType<Gameplay>().Any()) return;
		new Gameplay { DesktopLocation = DesktopLocation, Icon = Program.Ico }.Show();
		Music.PlaySound("winxp", "navigation_start");
	}

	private void Links_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		if (Application.OpenForms.OfType<Links>().Any()) return;
		new Links { DesktopLocation = DesktopLocation, Icon = Program.Ico }.Show();
		Music.PlaySound("winxp", "navigation_start");
	}

	private void Version_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Utils.OpenUrl("https://sefinek.net/genshin-impact-reshade/docs?page=changelog_v7");
	}

	private void MadeBySefinek_Click(object sender, EventArgs e)
	{
		MessageBox.Show(Resources.Default_ItsJustText_WhatMoreDoYouWant, Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Question);
		Utils.OpenUrl("https://www.youtube.com/watch?v=RpDf3XFHVNI");
	}

	private void W_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Music.PlaySound("winxp", "pop-up_blocked");

		if (ComputerInfo.GetSystemRegion() == "PL")
		{
			WebView2Shake viewer = new() { DesktopLocation = DesktopLocation, Icon = Program.Ico };
			viewer.Navigate("https://www.youtube.com/embed/2F2DdXUNyaQ?autoplay=1");
			viewer.Show();

			MessageBox.Show(@"Pamiętaj by nie grać w lola, gdyż to grzech ciężki.", @"kurwa");
		}
		else
		{
			WebView2Shake viewer = new() { DesktopLocation = DesktopLocation, Icon = Program.Ico };
			viewer.Navigate("https://www.youtube.com/embed/L3ky4gZU5gY?autoplay=1");
			viewer.Show();
		}
	}

	private void Paimon_Click(object sender, EventArgs e)
	{
		if (Application.OpenForms.OfType<RandomImages>().Any()) return;
		new RandomImages { Icon = Program.Ico }.Show();
		Music.PlaySound("winxp", "navigation_start");
	}

	private void ViewChangelog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Utils.OpenUrl("https://sefinek.net/genshin-impact-reshade/docs?page=changelog_v7");
	}
}
