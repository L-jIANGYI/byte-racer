using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ByteRacer.Game;

namespace ByteRacer.Views
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : UserControl
    {
        private readonly MainWindow _main;

        // Settings
        private const double WorldWidth = 3000;
        private const double WorldHeight = 2000;
        private const double MetersPerPixel = 0.1;
        private const double CameraFollowSpeed = 8;

        // Game loop
        private readonly Stopwatch _clock = new Stopwatch();
        private TimeSpan _lastFrameTime;
        private TimeSpan _lastRenderingTime;

        // Key
        private readonly HashSet<Key> _pressedKeys = new HashSet<Key>();

        // Car
        private readonly Car _car = new Car();

        public GamePage(MainWindow main)
        {
            InitializeComponent();
            _main = main;

            World.Width = WorldWidth;
            World.Height = WorldHeight;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _car.X = WorldWidth / 2;
            _car.Y = WorldHeight / 2;
            _car.Angle = 0;
            _car.Speed = 0;

            UpdateCamera(0, true);

            _main.KeyDown += OnKeyDown;
            _main.KeyUp += OnKeyUp;
            _main.Deactivated += OnWindowDeactivated;

            _clock.Start();
            _lastFrameTime = _clock.Elapsed;
            CompositionTarget.Rendering += OnFrame;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering -= OnFrame;

            _main.KeyDown -= OnKeyDown;
            _main.KeyUp -= OnKeyUp;
            _main.Deactivated -= OnWindowDeactivated;

            _clock.Stop();
            _pressedKeys.Clear();
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            _pressedKeys.Add(e.Key);

            // voor vragen beloning later: test speelboost en afremmen 
            if (e.IsRepeat) return;
            if (e.Key == Key.B) _car.ApplyEffect(1.5, 3);
            if (e.Key == Key.N) _car.ApplyEffect(0.5, 3);
        }

        private void OnKeyUp(object? sender, KeyEventArgs e) => _pressedKeys.Remove(e.Key);

        private void OnWindowDeactivated(object? sender, EventArgs e) => _pressedKeys.Clear();

        private bool IsDown(Key a, Key b) => _pressedKeys.Contains(a) || _pressedKeys.Contains(b);

        private CarInput ReadInput() => new CarInput(
            Gas: IsDown(Key.Up, Key.W),
            Brake: IsDown(Key.Down, Key.S),
            Left: IsDown(Key.Left, Key.A),
            Right: IsDown(Key.Right, Key.D));

        private void OnFrame(object? sender, EventArgs e)
        {
            TimeSpan renderingTime = ((RenderingEventArgs)e).RenderingTime;
            if (renderingTime == _lastRenderingTime) return;
            _lastRenderingTime = renderingTime;

            TimeSpan now = _clock.Elapsed;
            double dt = (now - _lastFrameTime).TotalSeconds;
            _lastFrameTime = now;
            if (dt > 0.05) dt = 0.05;

            CarInput input = ReadInput();

            _car.Update(dt, input, true);
            KeepCarInWorld();

            DrawCar();
            UpdateCamera(dt);
            DrawDebug();
        }

        private void KeepCarInWorld()
        {
            double maxY = WorldHeight;
            double maxX = WorldWidth;

            if (_car.X < 0 || _car.X > maxX || _car.Y < 0 || _car.Y > maxY)
            {
                _car.X = Math.Clamp(_car.X, 0, maxX);
                _car.Y = Math.Clamp(_car.Y, 0, maxY);
            }
        }

        private void DrawCar()
        {
            Canvas.SetLeft(CarVisual, _car.X - CarVisual.Width / 2);
            Canvas.SetTop(CarVisual, _car.Y - CarVisual.Height / 2);
            CarRotation.Angle = _car.Angle;
        }

        private void UpdateCamera(double dt, bool snap = false)
        {
            double targetX = Viewport.ActualWidth / 2 - _car.X;
            double targetY = Viewport.ActualHeight / 2 - _car.Y;

            if (snap)
            {
                Camera.X = targetX;
                Camera.Y = targetY;
                return;
            }

            double t = 1 - Math.Exp(-CameraFollowSpeed * dt);
            Camera.X += (targetX - Camera.X) * t;
            Camera.Y += (targetY - Camera.Y) * t;
        }

        private void DrawDebug()
        {
            double kmh = Math.Abs(_car.Speed) * MetersPerPixel * 3.6;
            DebugText.Text = $"speed  {_car.Speed,6:0} px/s   {kmh:0} km/h\n" +
                             $"SpeelMultiplier {_car.SpeedMultiplier}\n" +
                             $"EffectTimeLeft {_car.EffectTimeLeft}";
        }
    }
}