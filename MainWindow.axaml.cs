using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System.Linq;
using System.Collections.Generic;

namespace Autoclicker
{
    public partial class MainWindow : Window
    {
        private bool _clk = false;
        private bool _letsStartClicking = false;
        private bool _isCapturing = false;
        private CancellationTokenSource _cts;

        const int MOUSEEVENTF_LEFTDOWN = 0x02;
        const int MOUSEEVENTF_LEFTUP = 0x04;
        public int posX;
        public int posY;
        public static List<string> delay = new List<string>() {"10 ms","20 ms","30 ms","40 ms","50 ms","100 ms","150 ms","200 ms"};
        public int num = 0;

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void delayIncrement(object? sender,RoutedEventArgs e)
        {
            if (num < 8)
            {
                delayText.Text = delay[num];
                num = num + 1;
            }
            else
            {
                num = 0;
                delayText.Text = delay[num];
            }
        }

        private void delayDecrement(object? sender, RoutedEventArgs e)
        {
            if (num !=0 ) 
            {
                num=num - 1;    
                delayText.Text = delay[num];
            }
            else 
            {
                num = 7;
                delayText.Text = delay[num];
            }
        }

        private void trackMouse(object sender, RoutedEventArgs e)
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
            }else if(e.Key == Key.Q && _letsStartClicking)
            {
                _clk = true;
            }
        }
        private async void startClicking(object? sender, RoutedEventArgs e)
        {
            _letsStartClicking = true;
            for (int i = 0; i < 500; i++)
            {
                if (_clk)
                    break;
                SetCursorPos(posX, posY);
                await Task.Delay(10);
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                await Task.Delay(10);
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                await Task.Delay(50);
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