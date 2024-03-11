using LibVLCSharp.Shared;
using Timer = System.Windows.Forms.Timer;

namespace StellaModLauncher.Forms.Other;

public partial class VideoShake : Form
{
	private static Random? _random;
	private static Timer? _shakeTimer;
	private static Timer? _stopShakeTimer;
	private readonly LibVLC _libVlc;
	private readonly MediaPlayer _mp;

	public VideoShake()
	{
		InitializeComponent();

		_libVlc = new LibVLC(enableDebugLogs: true);
		_libVlc.Log += (_, args) => Program.Logger.Debug(args.Message);
		_mp = new MediaPlayer(_libVlc);
		videoView1.MediaPlayer = _mp;
	}

	private void WebViewWindow_Load(object sender, EventArgs e)
	{
		_shakeTimer = new Timer { Interval = 99 };
		_shakeTimer.Tick += ShakeTimer_Tick;

		_stopShakeTimer = new Timer { Interval = 10000 };
		_stopShakeTimer.Tick += StopShakeTimer_Tick;

		_random = new Random();
	}

	public void Navigate(string? path, FromType type)
	{
		if (string.IsNullOrEmpty(path))
		{
			Program.Logger.Error("`path` is null or empty at VideoShake.Navigate");
			return;
		}

		_mp.Play(new Media(_libVlc, path, type));
	}

	private static void StartShaking()
	{
		_shakeTimer?.Start();
		_stopShakeTimer?.Start();
	}

	private void ShakeTimer_Tick(object? sender, EventArgs e)
	{
		Left += _random!.Next(-6, 7);
		Top += _random.Next(-6, 7);
	}

	private static void StopShakeTimer_Tick(object? sender, EventArgs e)
	{
		_shakeTimer?.Stop();
		_stopShakeTimer?.Stop();
	}

	private void WebViewWindow_Shown(object sender, EventArgs e)
	{
		StartShaking();
	}

	private void VideoShake_FormClosing(object sender, FormClosingEventArgs e)
	{
		_libVlc.Dispose();
		_mp.Dispose();
	}
}
