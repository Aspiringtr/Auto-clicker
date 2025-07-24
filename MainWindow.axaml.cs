using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System.Collections.Generic;

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
        public static List<string> delay = new List<string>() {"10 ms","20 ms","30 ms","40 ms","50 ms","100 ms","150 ms","200 ms"};
        public static List<int> delayNo = new List<int>() {10,20,30,40,50,100,150,200};
        public static List<string> time = new List<string>() { "30 sec", "1 min", "5 min", "10 min", "20 min", "30 min"};
        public static List<int> timeNo = new List<int>() {30,60,300,600,1200,1800};
        public int numD = 0;
        public int numT = 0;
        public int numSD = 0;
        public int numST = 0;

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
            }
        }
        private void delayIncrement(object? sender, RoutedEventArgs e)
        {
            if (numD < 8)
            {
                delayText.Text = delay[numD];
                numSD = numD;
                numD = numD + 1;
            }
            else
            {
                numD = 0;
                numSD = numD;
                delayText.Text = delay[numD];
            }
        }
        private void delayDecrement(object? sender, RoutedEventArgs e)
        {
            if (numD != 0)
            {
                numD = numD - 1;
                numSD = numD;
                delayText.Text = delay[numD];
            }
            else
            {
                numD = 7;
                numSD = numD;
                delayText.Text = delay[numD];
            }
        }
        private void timeIncrement(object? sender, RoutedEventArgs e)
        {
            if (numT < 6)
            {
                timeText.Text = time[numT];
                numST = numT;
                numT = numT + 1;
            }
            else
            {
                numT = 0;
                numST = numT;
                timeText.Text = time[numT];
            }
        }
        private void timeDecrement(object? sender, RoutedEventArgs e)
        {
            if (numT != 0)
            {
                numT = numT - 1;
                numST = numT;
                timeText.Text = time[numT];
            }
            else
            {
                numT = 5;
                numST = numT;
                timeText.Text = time[numT];
            }
        }
        private async void startClicking(object? sender, RoutedEventArgs e)
        {
            _cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeNo[numST]));
            var token = _cts.Token;
            try
            {
                while (true) 
                {
                    SetCursorPos(posX, posY);
                    await Task.Delay(10);
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    await Task.Delay(10);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    await Task.Delay(delayNo[numSD]);

                    token.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException) { 
                numD =0;
                numT = 0;
                timeText.Text = "Time";
                delayText.Text = "Delay";
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