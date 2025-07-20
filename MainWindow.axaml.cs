using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;


namespace Autoclicker
{
    public partial class MainWindow : Window
    {
        private bool _isCapturing = false;
        private CancellationTokenSource _cts;
        const int MOUSEEVENTF_LEFTDOWN = 0x02;
        const int MOUSEEVENTF_LEFTUP = 0x04;
        public int posX;
        public int posY;

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void startClicking(object? sender,RoutedEventArgs e)
        {
            for (int i = 0; i < 500; i++)
            {
                SetCursorPos(posX,posY);
                await Task.Delay(10);
                mouse_event(MOUSEEVENTF_LEFTDOWN,0,0,0,0);
                await Task.Delay(10);
                mouse_event(MOUSEEVENTF_LEFTUP,0,0,0,0);
                await Task.Delay(50);
            }

        }

        private void OnStartClick(object sender, RoutedEventArgs e)
        {
            if (_isCapturing)
                return;

            _isCapturing = true;
            _cts = new CancellationTokenSource();
            Task.Run(() => TrackMousePosition(_cts.Token));
        }

        private async Task TrackMousePosition(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (GetCursorPos(out POINT point))
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        PositionText.Content = $"{point.X} {point.Y}";
                    });
                }

                await Task.Delay(10); 
            }
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q && _isCapturing)
            {
                if (GetCursorPos(out POINT point))
                {
                    CapturedText.Text = $"{point.X} {point.Y}";
                    posX = point.X;
                    posY = point.Y;
                    PositionText.Content = "Capture position";
                }
                _isCapturing = false;
                _cts?.Cancel();
            }
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            this.Focus();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _cts?.Cancel();
        }
    }
}