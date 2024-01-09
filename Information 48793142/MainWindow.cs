using System.Diagnostics;
using System.Runtime.InteropServices;
using InformationWindow.Properties;
using NAudio.Wave;
using Timer = System.Windows.Forms.Timer;

namespace InformationWindow;

public partial class MainWindow : Form
{
    private const int HwndTopmost = -1;
    private const uint SwpNoSize = 0x0001;
    private const uint SwpNoMove = 0x0002;

    private readonly Timer _autoCloseTimer;
    private readonly Timer _timer;
    private int _displayCount;
    private bool _openedUrl;
    private int _remainingSeconds = 23;

    public MainWindow()
    {
        InitializeComponent();

        FormBorderStyle = FormBorderStyle.None;
        WindowState = FormWindowState.Maximized;
        TopMost = true;
        ShowInTaskbar = false;

        _timer = new Timer { Interval = 1000 };
        _timer.Tick += Timer_Tick;
        _timer.Start();

        _autoCloseTimer = new Timer { Interval = 1000 };
        _autoCloseTimer.Tick += AutoCloseTimer_Tick;
        _autoCloseTimer.Start();

        TimeSpan time = TimeSpan.FromSeconds(_remainingSeconds);
        label4.Text = string.Format(Resources.PleaseWait, $"{time.Seconds:D2}");
    }

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial void SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

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
            label4.Text = string.Format(Resources.PleaseWait, $"{time.Seconds:D2}");
        }
        else
        {
            _autoCloseTimer.Stop();
            label4.Text = Resources.Closing;
            Application.Exit();
        }
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        SetWindowPos(Handle, HwndTopmost, 0, 0, 0, 0, SwpNoMove | SwpNoSize);
    }

    // Sound Effect from <a href="https://pixabay.com/sound-effects/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=47485">Pixabay</a>
    private async void MeowButton_Click(object sender, EventArgs e)
    {
        Random random = new();
        string mp3FilePath = Path.Combine(Directory.GetCurrentDirectory(), "sound", $"meow{random.Next(1, 5)}.mp3");

        try
        {
            using AudioFileReader audioFile = new(mp3FilePath);
            using WaveChannel32 volumeStream = new(audioFile);
            volumeStream.Volume = 0.68f;
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
        if (_openedUrl) return;

        Process.Start(new ProcessStartInfo("https://sefinek.net") { UseShellExecute = true });
        label3.Visible = true;

        _openedUrl = true;
    }
}
