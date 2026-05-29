using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Masterstrap
{
    internal class OverlayWindow : Window, INotifyPropertyChanged
    {
        private int _frames;
        private int _fps;
        private readonly Stopwatch _fpsStopwatch = Stopwatch.StartNew();
        private TextBlock _fpsTextBlock;
        private TextBlock _pingTextBlock;
        private TextBlock _locationTextBlock;
        private TextBlock _timeTextBlock;
        private readonly DispatcherTimer _updateTimer;

        private bool _showFPS;
        private bool _showPing;
        private bool _showTime;
        private bool _showLocation;
        private double _brightness = 50.0;

        private Border _darkOverlay;
        private Border _brightOverlay;
        private string _serverIp;
        private string _lastServerIp;
        private bool _locationFetching;
        private string _serverLocation = "Location: --";
        private static readonly HttpClient Http = new HttpClient();

        public OverlayWindow()
        {
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight;
            this.Left = 0;
            this.Top = 0;
            this.AllowsTransparency = true;
            this.Background = Brushes.Transparent;
            this.WindowStyle = WindowStyle.None;
            this.Topmost = true;
            this.ShowInTaskbar = false;
            this.IsHitTestVisible = false;

            Grid grid = new Grid();

            _darkOverlay = new Border { Background = Brushes.Black, Opacity = 0 };
            _brightOverlay = new Border { Background = Brushes.White, Opacity = 0 };

            grid.Children.Add(_darkOverlay);
            grid.Children.Add(_brightOverlay);

            StackPanel infoPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 40, 20, 0)
            };

            _fpsTextBlock = CreateTextBlock(Brushes.Lime);
            _pingTextBlock = CreateTextBlock(Brushes.LightSkyBlue);
            _locationTextBlock = CreateTextBlock(Brushes.LightGreen);
            _timeTextBlock = CreateTextBlock(Brushes.Cyan);

            infoPanel.Children.Add(_fpsTextBlock);
            infoPanel.Children.Add(_pingTextBlock);
            infoPanel.Children.Add(_locationTextBlock);
            infoPanel.Children.Add(_timeTextBlock);

            grid.Children.Add(infoPanel);
            this.Content = grid;

            CompositionTarget.Rendering += OnRendering;

            _updateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            _updateTimer.Tick += async (s, e) => await UpdateStatsAsync();
            _updateTimer.Start();

            this.Loaded += (s, e) => {
                MakeClickThrough();
                ApplyBrightness();
            };
        }

        protected override void OnClosed(EventArgs e)
        {
            CompositionTarget.Rendering -= OnRendering;
            if (_updateTimer != null)
            {
                _updateTimer.Stop();
            }
            base.OnClosed(e);
        }

        public double Brightness
        {
            get => _brightness;
            set
            {
                _brightness = Math.Clamp(value, 0, 100);
                ApplyBrightness();
                OnPropertyChanged(nameof(Brightness));
            }
        }

        public void UpdateFromSettings(MainWindow.ModsUiSettings settings)
        {
            _showFPS = settings.FpsCounter;
            _showPing = settings.ServerDetails;
            _showTime = settings.LightingOverlays;
            _showLocation = settings.ServerDetails;
            this.Brightness = settings.Brightness;

            _fpsTextBlock.Visibility = _showFPS ? Visibility.Visible : Visibility.Collapsed;
            _pingTextBlock.Visibility = _showPing ? Visibility.Visible : Visibility.Collapsed;
            _locationTextBlock.Visibility = _showLocation ? Visibility.Visible : Visibility.Collapsed;
            _timeTextBlock.Visibility = _showTime ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ApplyBrightness()
        {
            if (_brightness == 50)
            {
                _darkOverlay.Opacity = 0;
                _brightOverlay.Opacity = 0;
            }
            else if (_brightness < 50)
            {
                _darkOverlay.Opacity = (50 - _brightness) / 50.0;
                _brightOverlay.Opacity = 0;
            }
            else
            {
                _brightOverlay.Opacity = (_brightness - 50) / 50.0;
                _darkOverlay.Opacity = 0;
            }
        }

        private void OnRendering(object sender, EventArgs e)
        {
            if (_showFPS)
            {
                _frames++;
                if (_fpsStopwatch.ElapsedMilliseconds >= 1000)
                {
                    _fps = _frames;
                    _frames = 0;
                    _fpsStopwatch.Restart();
                    _fpsTextBlock.Text = $"FPS: {_fps}";
                }
            }

            if (!IsRobloxForeground())
            {
                if (this.Visibility == Visibility.Visible) this.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (this.Visibility != Visibility.Visible) this.Visibility = Visibility.Visible;
            }
        }

        private async Task UpdateStatsAsync()
        {
            if (_showTime)
                _timeTextBlock.Text = DateTime.Now.ToString("h:mm tt");

            if (!_showPing && !_showLocation) return;

            _serverIp = GetRobloxServerIp();
            if (string.IsNullOrEmpty(_serverIp))
            {
                _pingTextBlock.Text = "Ping: --";
            }
            else
            {
                if (_showPing)
                {
                    int pingValue = await PingServerAsync(_serverIp);
                    _pingTextBlock.Text = pingValue <= 0 ? "Ping: --" : $"Ping: {pingValue} ms";
                }

                if (_showLocation && !_locationFetching && _serverIp != _lastServerIp)
                {
                    _lastServerIp = _serverIp;
                    _locationFetching = true;
                    _locationTextBlock.Text = "Location: Fetching...";
                    string loc = await GetServerLocationAsync(_serverIp);
                    _locationTextBlock.Text = loc;
                    _locationFetching = false;
                }
            }
        }

        private string GetRobloxServerIp()
        {
            try
            {
                var connections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();
                return connections.FirstOrDefault(c => c.State == TcpState.Established && !IPAddress.IsLoopback(c.RemoteEndPoint.Address))?.RemoteEndPoint.Address.ToString();
            }
            catch { return null; }
        }

        private async Task<int> PingServerAsync(string ip)
        {
            try
            {
                using (Ping p = new Ping())
                {
                    var reply = await p.SendPingAsync(ip, 1000);
                    return reply.Status == IPStatus.Success ? (int)reply.RoundtripTime : -1;
                }
            }
            catch { return -1; }
        }

        private async Task<string> GetServerLocationAsync(string ip)
        {
            try
            {
                var res = await Http.GetStringAsync($"https://ipinfo.io/{ip}/json");
                using (JsonDocument doc = JsonDocument.Parse(res))
                {
                    var root = doc.RootElement;
                    string city = root.TryGetProperty("city", out var c) ? c.GetString() : null;
                    string country = root.TryGetProperty("country", out var co) ? co.GetString() : null;
                    return city != null ? $"Location: {city} ({country})" : "Location: Unknown";
                }
            }
            catch { return "Location: Unknown"; }
        }

        private static TextBlock CreateTextBlock(Brush color)
        {
            return new TextBlock
            {
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Foreground = color,
                Effect = new System.Windows.Media.Effects.DropShadowEffect { BlurRadius = 4, ShadowDepth = 1, Opacity = 0.8 }
            };
        }

        private void MakeClickThrough()
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, -20);
            SetWindowLong(hwnd, -20, extendedStyle | 0x20 | 0x80);
        }

        private static bool IsRobloxForeground()
        {
            IntPtr foreground = GetForegroundWindow();
            if (foreground == IntPtr.Zero) return false;

            GetWindowThreadProcessId(foreground, out uint pid);
            try
            {
                var proc = Process.GetProcessById((int)pid);
                string name = proc.ProcessName;


                return name.Equals("RobloxPlayerBeta", StringComparison.OrdinalIgnoreCase) ||
                       name.Equals("RobloxPlayer", StringComparison.OrdinalIgnoreCase) ||
                       name.Equals("Windows10Player", StringComparison.OrdinalIgnoreCase) ||
                       name.Equals("Roblox", StringComparison.OrdinalIgnoreCase) ||
                       name.Contains("Roblox", StringComparison.OrdinalIgnoreCase);
            }
            catch { return false; }
        }

        [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll")] private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")] private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
