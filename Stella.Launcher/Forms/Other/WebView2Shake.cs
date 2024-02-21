using StellaModLauncher.Scripts.Forms;
using Timer = System.Windows.Forms.Timer;

namespace StellaModLauncher.Forms.Other;

public partial class WebView2Shake : Form
{
	private static Random? _random;
	private static Timer? _shakeTimer;
	private static Timer? _stopShakeTimer;

	public WebView2Shake()
	{
		InitializeComponent();
	}

	private void WebViewWindow_Load(object sender, EventArgs e)
	{
		_shakeTimer = new Timer { Interval = 99 };
		_shakeTimer.Tick += ShakeTimer_Tick;

		_stopShakeTimer = new Timer { Interval = 10000 };
		_stopShakeTimer.Tick += StopShakeTimer_Tick;

		_random = new Random();
	}

	private async void WebView2(string? webView)
	{
		await WebViewHelper.Initialize(webView21, webView).ConfigureAwait(false);
	}

	public void Navigate(string? url)
	{
		WebView2(url);
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
}
