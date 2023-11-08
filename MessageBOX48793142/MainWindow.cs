using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace InformationWindow
{
    public partial class MainWindow : Form
    {
        private readonly Timer _timer;
        private int _displayCount;

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private const int HwndTopmost = -1;
        private const uint SwpNoSize = 0x0001;
        private const uint SwpNoMove = 0x0002;

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
    }
}
