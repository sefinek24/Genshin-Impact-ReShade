using System.Diagnostics;
using System.Runtime.InteropServices;
using InformationWindow.Properties;
using NAudio.Wave;
using StellaTelemetry;
using Timer = System.Windows.Forms.Timer;

namespace InformationWindow.Forms;

public sealed partial class MainWindow : Form
{
	private const uint SwpNoSize = 0x0001;
	private const uint SwpNoMove = 0x0002;

	private readonly Timer _autoCloseTimer;
	private readonly Timer _timer;
	private int _displayCount;
	private bool _openedDocs;
	private bool _openedOfficialWebsite;
	private int _remainingSeconds = 20;

	public MainWindow()
	{
		InitializeComponent();

		SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
		UpdateStyles();

		_timer = new Timer { Interval = 1000 };
		_timer.Tick += Timer_Tick;
		_timer.Start();

		_autoCloseTimer = new Timer { Interval = 1000 };
		_autoCloseTimer.Tick += AutoCloseTimer_Tick;
		_autoCloseTimer.Start();

		TimeSpan time = TimeSpan.FromSeconds(_remainingSeconds);
		label4.Text = string.Format(Resources.ThisWindowWillCloseIn, $"{time.Seconds:D2}");
	}

	[LibraryImport("user32.dll")]
	private static partial void SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

	private void Timer_Tick(object? sender, EventArgs e)
	{
		if (_displayCount < 3)
		{
			_displayCount++;
		}
		else
		{
			_timer.Stop();
			_timer.Dispose();
		}
	}

	private void AutoCloseTimer_Tick(object? sender, EventArgs e)
	{
		_remainingSeconds--;
		if (_remainingSeconds >= 0)
		{
			TimeSpan time = TimeSpan.FromSeconds(_remainingSeconds);
			label4.Text = string.Format(Resources.ThisWindowWillCloseIn, $"{time.Seconds:D2}");
		}
		else
		{
			label4.Text = Resources.Closing;
			_autoCloseTimer.Stop();

			Application.Exit();
		}
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

		SetWindowPos(Handle, -1, 0, 0, 0, 0, SwpNoMove | SwpNoSize);
	}

	// Sound Effect from <a href="https://pixabay.com/sound-effects/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=47485">Pixabay</a>
	private async void MeowButton_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Random random = new();
		string mp3FilePath = Path.Combine(Directory.GetCurrentDirectory(), "sound", $"meow{random.Next(1, 5)}.mp3");

		if (!File.Exists(mp3FilePath)) return;

		try
		{
			using AudioFileReader audioFile = new(mp3FilePath);
			using WaveChannel32 volumeStream = new(audioFile);
			using WaveOutEvent outputDevice = new();
			outputDevice.Init(volumeStream);
			outputDevice.Play();

			await Task.Delay(TimeSpan.FromSeconds(audioFile.TotalTime.TotalSeconds)).ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	private void Copyright_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		if (_openedOfficialWebsite) return;
		Process.Start(new ProcessStartInfo("https://sefinek.net") { UseShellExecute = true });

		label3.Visible = true;
		_openedOfficialWebsite = true;
	}

	private void ViewDocs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		if (_openedDocs) return;
		Process.Start(new ProcessStartInfo($"{Data.AppWebsiteFull}/docs?page=terms-of-use") { UseShellExecute = true });

		label3.Visible = true;
		_openedDocs = true;
	}
}
