using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ByteRacer.Views
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : UserControl
    {
        private readonly MainWindow _main;

        // Game loop
        private readonly Stopwatch _clock = new Stopwatch();
        private TimeSpan _lastFrameTime;
        private TimeSpan _lastRenderingTime;

        // Key
        private Window? _window;
        private readonly HashSet<Key> _pressedKeys = new HashSet<Key>();

        // Test Box
        private double _boxX = 200;
        private double _boxY = 200;
        private const double BoxSpeed = 300;
        private bool _boxIsBlue;

        public GamePage(MainWindow main)
        {
            InitializeComponent();
            _main = main;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _window = Window.GetWindow(this);
            if (_window != null)
            {
                _window.KeyDown += OnKeyDown;
                _window.KeyUp += OnKeyUp;
                _window.Deactivated += OnWindowDeactivated;
            }

            _clock.Start();
            _lastFrameTime = _clock.Elapsed;
            CompositionTarget.Rendering += OnFrame;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering -= OnFrame;

            if (_window != null)
            {
                _window.KeyDown -= OnKeyDown;
                _window.KeyUp -= OnKeyUp;
                _window.Deactivated -= OnWindowDeactivated;
            }

            _clock.Stop();
            _pressedKeys.Clear();
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            _pressedKeys.Add(e.Key);

            if (e.IsRepeat) return;

            if (e.Key == Key.Space)
            {
                _boxIsBlue = !_boxIsBlue;
                RedBox.Fill = _boxIsBlue ? Brushes.Blue : Brushes.Red;
            }
        }

        private void OnKeyUp(object? sender, KeyEventArgs e)
        {
            _pressedKeys.Remove(e.Key);
        }

        private bool IsDown(Key a, Key b) => _pressedKeys.Contains(a) || _pressedKeys.Contains(b);

        private void OnWindowDeactivated(object? sender, EventArgs e)
        {
            _pressedKeys.Clear();
        }

        private void OnFrame(object? sender, EventArgs e)
        {
            TimeSpan renderingTime = ((RenderingEventArgs)e).RenderingTime;
            if (renderingTime == _lastRenderingTime) return;
            _lastRenderingTime = renderingTime;

            TimeSpan now = _clock.Elapsed;
            double dt = (now - _lastFrameTime).TotalSeconds;
            _lastFrameTime = now;
            if (dt > 0.05) dt = 0.05;

            bool up = IsDown(Key.Up, Key.W);
            bool down = IsDown(Key.Down, Key.S);
            bool left = IsDown(Key.Left, Key.A);
            bool right = IsDown(Key.Right, Key.D);

            if (up) _boxY -= BoxSpeed * dt;
            if (down) _boxY += BoxSpeed * dt;
            if (left) _boxX -= BoxSpeed * dt;
            if (right) _boxX += BoxSpeed * dt;

            // Zorg dat blok binnen de wereld is. Math.Max voorkomt bug en zorg dat minimaal 0 is.
            _boxY = Math.Clamp(_boxY, 0, Math.Max(0, (World.ActualHeight - RedBox.Height)));
            _boxX = Math.Clamp(_boxX, 0, Math.Max(0, (World.ActualWidth - RedBox.Width)));

            Canvas.SetTop(RedBox, _boxY);
            Canvas.SetLeft(RedBox, _boxX);

            DebugText.Text = $"dt    {dt:0.0000} s\n" +
                             $"Keys {string.Join(",", _pressedKeys)}";
        }
    }
}