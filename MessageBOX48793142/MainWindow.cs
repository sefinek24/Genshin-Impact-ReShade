using System.Runtime.InteropServices;
using NAudio.Wave;
using Timer = System.Windows.Forms.Timer;

namespace InformationWindow;

public partial class MainWindow : Form
{
    private const int HwndTopmost = -1;
    private const uint SwpNoSize = 0x0001;
    private const uint SwpNoMove = 0x0002;
    private readonly Timer _timer;
    private int _displayCount;

    public MainWindow()
    {
        InitializeComponent();

        _timer = new Timer();
        _timer.Interval = 1000;
        _timer.Tick += Timer_Tick;

        FormBorderStyle = FormBorderStyle.None;
        WindowState = FormWindowState.Maximized;
        TopMost = true;
        ShowInTaskbar = false;

        _timer.Start();

        Timer autoCloseTimer = new();
        autoCloseTimer.Interval = 10000;
        autoCloseTimer.Tick += AutoCloseTimer_Tick;
        autoCloseTimer.Start();
    }

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    private void Timer_Tick(object sender, EventArgs e)
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

    private static void AutoCloseTimer_Tick(object sender, EventArgs e)
    {
        Application.Exit();
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
        int randomNumber = random.Next(1, 5);
        string mp3FilePath = @$"sound\meow{randomNumber}.mp3";

        try
        {
            using (AudioFileReader audioFile = new(mp3FilePath))
            using (WaveChannel32 volumeStream = new(audioFile))
            {
                volumeStream.Volume = 0.3f;
                using (WaveOutEvent outputDevice = new())
                {
                    outputDevice.Init(volumeStream);
                    outputDevice.Play();

                    await Task.Delay(TimeSpan.FromSeconds(audioFile.TotalTime.TotalSeconds)).ConfigureAwait(false);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}
