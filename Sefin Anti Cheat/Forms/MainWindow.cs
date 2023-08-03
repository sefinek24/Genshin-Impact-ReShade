using System;
using System.Drawing;
using System.Windows.Forms;
using SefinAntiCheat.Properties;

namespace SefinAntiCheat.Forms
{
    public partial class MainWindow : Form
    {
        private const int MinimizeMargin = 9;
        private NotifyIcon _trayIcon;
        private ContextMenu _trayMenu;

        public MainWindow()
        {
            InitializeComponent();
            InitializeWindow();
            InitializeTray();
        }

        private void InitializeWindow()
        {
            Rectangle workingArea = Screen.GetWorkingArea(this);
            Location = new Point(workingArea.Right - Width - MinimizeMargin, workingArea.Bottom - Height - MinimizeMargin);
            label2.Text = $@"v{Program.AppVersion}";
        }

        private void InitializeTray()
        {
            _trayMenu = new ContextMenu();
            _trayMenu.MenuItems.Add("Quit", OnQuitClick);

            _trayIcon = new NotifyIcon
            {
                ContextMenu = _trayMenu,
                Icon = Resources.icon,
                Text = Program.AppName,
                Visible = true
            };

            _trayIcon.MouseClick += OnTrayIconClick;
        }

        private void OnTrayIconClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Show();
                WindowState = FormWindowState.Normal;
            }
        }

        private void OnQuitClick(object sender, EventArgs e)
        {
            _trayIcon.Visible = false;
            _trayIcon.Dispose(); // Dispose the NotifyIcon when the application exits
            Environment.Exit(0);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                _trayIcon.ShowBalloonTip(2000, "Information", "The application has been minimized to the tray.", ToolTipIcon.Info);
            }
        }
    }
}
