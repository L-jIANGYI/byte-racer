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

        // ---------- Settings ----------
        private const double WorldWidth = 3000;
        private const double WorldHeight = 2000;
        private const double MetersPerPixel = 0.1;
        private const double CameraFollowSpeed = 8;

        // ---------- Game loop ----------
        private readonly Stopwatch _clock = new Stopwatch();
        private TimeSpan _lastFrameTime;
        private TimeSpan _lastRenderingTime;

        // ---------- Key ----------
        private readonly HashSet<Key> _pressedKeys = new HashSet<Key>();

        // ---------- Car ----------
        private readonly Car _car = new Car();

        public GamePage(MainWindow main)
        {
            InitializeComponent();
            _main = main;

            // Hier wordt de grootte van "World" gedefinieerd
            World.Width = WorldWidth;
            World.Height = WorldHeight;
        }

        #region Start / Stop

        /// <summary>
        /// Init game bij laden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Alles clean up wanneer ontladen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering -= OnFrame;

            _main.KeyDown -= OnKeyDown;
            _main.KeyUp -= OnKeyUp;
            _main.Deactivated -= OnWindowDeactivated;

            _clock.Stop();
            _pressedKeys.Clear();
        }

        #endregion

        #region Key Methods

        /// <summary>
        /// Key down event, voeg gedrukte key in _pressedKeys.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            _pressedKeys.Add(e.Key);

            // voor vragen beloning later: test speelboost en afremmen 
            if (e.IsRepeat) return;
            switch (e.Key)
            {
                case Key.B: _car.ApplyEffect(1.5, 3); break;
                case Key.N: _car.ApplyEffect(0.5, 3); break;
            }
        }

        /// <summary>
        /// Key up event, verwijder gedrukte key.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyUp(object? sender, KeyEventArgs e) => _pressedKeys.Remove(e.Key);

        /// <summary>
        /// Zorg voor dat van pagina verandert, auto niet door rijden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowDeactivated(object? sender, EventArgs e) => _pressedKeys.Clear();

        /// <summary>
        /// Helper function, zorg input voor WASD en pijlen beide werkt.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private bool IsDown(Key a, Key b) => _pressedKeys.Contains(a) || _pressedKeys.Contains(b);

        /// <summary>
        /// Leest de huidige input van de speler.
        /// </summary>
        /// <returns>De rijinvoer op basis van de ingedrukte toetsen.</returns>
        private CarInput ReadInput() => new CarInput(
            Gas: IsDown(Key.Up, Key.W),
            Brake: IsDown(Key.Down, Key.S),
            Left: IsDown(Key.Left, Key.A),
            Right: IsDown(Key.Right, Key.D));

        #endregion

        #region Elke frame gebeurt

        /// <summary>
        /// Game loop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFrame(object? sender, EventArgs e)
        {
            // Zorg dat niet 2x aangeroepen in een frame. Kleine bug van CompositionTarget.Rendering
            TimeSpan renderingTime = ((RenderingEventArgs)e).RenderingTime;
            if (renderingTime == _lastRenderingTime) return;
            _lastRenderingTime = renderingTime;

            // Bereken delta time
            TimeSpan now = _clock.Elapsed;
            double dt = (now - _lastFrameTime).TotalSeconds;
            _lastFrameTime = now;
            if (dt > 0.05) dt = 0.05; // Niet flash wanneer lang vast zit.

            _car.Update(dt, ReadInput(), true);
            KeepCarInWorld();

            DrawCar();
            UpdateCamera(dt);
            DrawDebug();
        }

        /// <summary>
        /// Houd de auto in het wereld.
        /// </summary>
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

        /// <summary>
        /// Tekent de auto op basis van de huidige positie en rotatie.
        /// </summary>
        private void DrawCar()
        {
            Canvas.SetLeft(CarVisual, _car.X - CarVisual.Width / 2);
            Canvas.SetTop(CarVisual, _car.Y - CarVisual.Height / 2);
            CarRotation.Angle = _car.Angle;
        }

        /// <summary>
        /// Update camera zodat de auto volgt.
        /// </summary>
        /// <param name="dt">delta time, in sec</param>
        /// <param name="snap">Of de camera direct naar de doelpositie</param>
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

            // Berekent hoe snel de camera naar de doelpositie beweegt.
            double t = 1 - Math.Exp(-CameraFollowSpeed * dt);

            // Verplaatst de camera geleidelijk naar de doelpositie.
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

        #endregion
    }
}