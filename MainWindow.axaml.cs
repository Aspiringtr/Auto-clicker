using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Gma.System.MouseKeyHook;


namespace Autoclicker
{
    public partial class MainWindow : Window
    {
        private bool _isCapturing = false;
        private CancellationTokenSource _cts;

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out POINT lpPoint);

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

        private void OnStartClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
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

                await Task.Delay(10); // Update every 100ms
            }
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q && _isCapturing)
            {
                if (GetCursorPos(out POINT point))
                {
                    CapturedText.Text = $"Captured at X: {point.X}, Y: {point.Y}";
                }
            }
            else if (e.Key == Key.Enter && _isCapturing)
            {
                if (GetCursorPos(out POINT point))
                {
                    CapturedText.Text = $"{point.X} {point.Y}";
                }

                _isCapturing = false;
                _cts?.Cancel();
            }
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            this.Focus(); // Give keyboard focus to the window
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _cts?.Cancel();
        }
    }
}