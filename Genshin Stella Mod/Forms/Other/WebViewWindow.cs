using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace StellaLauncher.Forms.Other
{
    public partial class WebViewWindow : Form
    {
        private readonly Random _random;
        private readonly Timer _shakeTimer;

        public WebViewWindow()
        {
            InitializeComponent();

            _shakeTimer = new Timer();
            _shakeTimer.Interval = 99;
            _shakeTimer.Tick += ShakeTimer_Tick;

            _random = new Random();
        }

        public async void WebView2(string webView)
        {
            try
            {
                CoreWebView2Environment coreWebView2Env = await CoreWebView2Environment.CreateAsync(null, Program.AppData, new CoreWebView2EnvironmentOptions());
                await webView21.EnsureCoreWebView2Async(coreWebView2Env);

                webView21.CoreWebView2.Navigate(webView);
            }
            catch (Exception ex)
            {
                Scripts.WebView2.HandleError(ex);
            }
        }

        public void Navigate(string url)
        {
            WebView2(url);
        }

        private void StartShaking()
        {
            _shakeTimer.Start();
        }

        // private void StopShaking()
        // {
        //     shakeTimer.Stop();
        //     Left = 100;
        //     Top = 100;
        // }

        private void ShakeTimer_Tick(object sender, EventArgs e)
        {
            int deltaX = _random.Next(-4, 5);
            int deltaY = _random.Next(-4, 5);
            Left += deltaX;
            Top += deltaY;
        }

        private void WebViewWindow_Shown(object sender, EventArgs e)
        {
            StartShaking();
        }
    }
}
