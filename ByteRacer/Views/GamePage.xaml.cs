using ByteRacer.Game;
using ByteRacer.Models;
using ByteRacer.Services;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ByteRacer.Views
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : UserControl
    {
        #region ---------------------- Properties -------------------------

        private readonly MainWindow _main;
        private readonly string _playerName;

        // ---------- Display ----------
        private const double WorldWidth = 3000;
        private const double WorldHeight = 2000;
        private const double CameraFollowSpeed = 8;
        private const double LightsOutShowSeconds = 0.8;
        private static readonly Brush LightOnColor = new SolidColorBrush(Color.FromRgb(235, 25, 25));
        private static readonly Brush LightOffColor = new SolidColorBrush(Color.FromRgb(45, 12, 12));

        // ---------- Game loop ----------
        private readonly Stopwatch _clock = new Stopwatch();
        private TimeSpan _lastFrameTime;
        private TimeSpan _lastRenderingTime;

        // ---------- Game ----------
        private readonly PlayerInput _input = new PlayerInput();
        private readonly Car _car = new Car();
        private readonly GameState _gameState = new GameState();

        #endregion

        public GamePage(MainWindow main, string playerName = "")
        {
            InitializeComponent();
            _main = main;
            _playerName = playerName;

            World.Width = WorldWidth;
            World.Height = WorldHeight;

            PauseMenu.ResumeClicked += ResumeGame;
            PauseMenu.RestartClicked += RestartGame;
            PauseMenu.ExitClicked += ExitToStart;
            PauseMenu.VolumeChanged += SetVolume;

            Quiz.AnswerChecked += OnQuizAnswered;
        }

        #region ------------------------- Start / Stop -------------------------

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
        /// Clean up bij ontladen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUnloaded(object sender, RoutedEventArgs e) => StopLoop();

        /// <summary>
        /// Loop stop zetten en clean up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopLoop()
        {
            CompositionTarget.Rendering -= OnFrame;

            _main.KeyDown -= OnKeyDown;
            _main.KeyUp -= OnKeyUp;
            _main.Deactivated -= OnWindowDeactivated;

            _clock.Stop();
            _input.Clear();
        }

        #endregion

        #region ------------------------- Key Methods -------------------------

        /// <summary>
        /// Key down event, voeg gedrukte key in _pressedKeys.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            _input.Press(e.Key);

            if (e.IsRepeat) return;

            if (e.Key == Key.Escape) TogglePause();

#if DEBUG
            HandleDebugKeys(e.Key);
#endif
        }

        /// <summary>
        /// Key up event, verwijder gedrukte key.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyUp(object? sender, KeyEventArgs e) => _input.Release(e.Key);

        /// <summary>
        /// Zorg voor dat van pagina verandert, auto niet door rijden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowDeactivated(object? sender, EventArgs e)
        {
            _input.Clear();
            PauseGame();
        }

#if DEBUG
        private void HandleDebugKeys(Key key)
        {
            switch (key)
            {
                case Key.B:
                    _car.ApplyEffect(1.5, 3);
                    break;
                case Key.N:
                    _car.ApplyEffect(0.5, 3);
                    break;
                case Key.F:
                    _gameState.Finish();
                    break;
                case Key.Q:
                    OpenQuiz();
                    break;
                case Key.J:
                    OnQuizAnswered(true);
                    break;
                case Key.K:
                    OnQuizAnswered(false);
                    break;
            }
        }
#endif

        #endregion

        #region ------------------------- Elke frame gebeurt -------------------------

        /// <summary>
        /// Game loop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFrame(object? sender, EventArgs e)
        {
            // Niet 2x aangeroepen per frame
            TimeSpan renderingTime = ((RenderingEventArgs)e).RenderingTime;
            if (renderingTime == _lastRenderingTime) return;
            _lastRenderingTime = renderingTime;

            // Delta time
            TimeSpan now = _clock.Elapsed;
            double dt = (now - _lastFrameTime).TotalSeconds;
            _lastFrameTime = now;
            if (dt > 0.05) dt = 0.05; // Niet flash wanneer lang vast zit.

            _gameState.Update(dt);

            switch (_gameState.State)
            {
                case RaceState.Racing:
                    _car.Update(dt, _input.Read(), onTrack: true);
                    KeepCarInWorld();
                    break;

                case RaceState.Finished:
                    _car.Update(dt, CarInput.None, onTrack: true);
                    KeepCarInWorld();
                    if (_gameState.StateTime >= GameSettings.FinishScreenSeconds)
                    {
                        StopLoop();
                        SaveResultAndShowLeaderboard();
                        return;
                    }

                    break;
            }

            DrawCar();
            UpdateCamera(dt);
            DrawHud();
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
            double kmh = Math.Abs(_car.Speed) * GameSettings.MetersPerPixel * 3.6;
            DebugText.Text = $"state   {_gameState.State}  ({_gameState.StateTime:0.0}s)\n" +
                             $"{kmh:0} km/h\n" +
                             $"effect  x{_car.SpeedMultiplier:0.0}  {_car.EffectTimeLeft:0.0}s\n" +
                             $"volume  {GetVolume():0}\n" + $"{_playerName}"
                ;
        }

        #endregion

        #region ------------------------- Start / Finish -------------------------

        /// <summary>
        /// Verwerk de racetijd en CenterText o.b.v. status aanpassen 
        /// </summary>
        private void DrawHud()
        {
            RaceTimeText.Text = FormatTime(_gameState.RaceTime);
            DrawStartLights();

            switch (_gameState.State)
            {
                case RaceState.Finished:
                    CenterText.Text = $"FINISH\n{FormatTime(_gameState.RaceTime)}";
                    CenterText.Visibility = Visibility.Visible;
                    break;
                default:
                    CenterText.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        /// <summary>
        /// startlichten tekenen, dat 1 op 1 rood wordt en tegelijk uit als 5 aan zijn
        /// </summary>
        private void DrawStartLights()
        {
            bool isInLightsOutShow =
                _gameState.State == RaceState.Racing && _gameState.RaceTime < LightsOutShowSeconds;
            bool show = _gameState.State == RaceState.Countdown || isInLightsOutShow;

            StartLights.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            if (!show) return;

            int lightsOn = _gameState.LightsOn;
            for (int i = 0; i < StartLightsPanel.Children.Count; i++)
            {
                var light = (Ellipse)StartLightsPanel.Children[i];
                light.Fill = i < lightsOn ? LightOnColor : LightOffColor;
            }
        }

        /// <summary>
        /// Helper function, racetijd formaten
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        private static string FormatTime(double seconds)
            => TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss\.ff");

        /// <summary>
        /// Het resultaat opslaan en naar de leaderboard
        /// </summary>
        private void SaveResultAndShowLeaderboard()
        {
            var result = new PlayResult
            {
                Name = _playerName,
                RaceTime = _gameState.RaceTime
            };

            LeaderboardService.Add(result);
            _main.ShowLeaderboard(result);
        }

        #endregion

        #region ------------------------- Pauze -------------------------

        /// <summary>
        /// Pauzeert het spel
        /// </summary>
        private void PauseGame()
        {
            if (!_gameState.CanPause) return;

            _gameState.Pause();
            _input.Clear();
            PauseMenu.Show(GetVolume());
        }

        /// <summary>
        /// Doorgaan
        /// </summary>
        private void ResumeGame()
        {
            if (!_gameState.IsPaused) return;

            _input.Clear();
            _gameState.Resume();
            PauseMenu.Hide();
        }

        /// <summary>
        /// Pauzeren of verder gaan.
        /// </summary>
        private void TogglePause()
        {
            if (_gameState.IsPaused) ResumeGame();
            else PauseGame();
        }

        /// <summary>
        /// Opnieuw
        /// </summary>
        private void RestartGame() => _main.ShowGame(_playerName);

        /// <summary>
        /// Terug naar start
        /// </summary>
        private void ExitToStart() => _main.ShowStart();

        private void PauseButton_Click(object sender, RoutedEventArgs e) => TogglePause();

        #endregion

        #region ------------------------- Quiz -------------------------

        /// <summary>
        /// Stop de game en open de vraag scherm
        /// </summary>
        private void OpenQuiz()
        {
            if (!_gameState.CanStartQuiz) return;
            _gameState.StartQuiz();
            _input.Clear();
            Quiz.Show();
        }

        /// <summary>
        /// Na het antwoord: boost/slowdown en verder racen.
        /// </summary>
        /// <param name="correct"></param>
        private void OnQuizAnswered(bool correct)
        {
            if (!_gameState.IsInQuiz) return;

            double multiplier = correct ? GameSettings.QuizBoost : GameSettings.QuizPenalty;
            _car.ApplyEffect(multiplier, GameSettings.QuizEffectSeconds);

            _input.Clear();
            _gameState.EndQuiz();
        }

        #endregion

        #region ------------------------- Volume tijdelijk -------------------------

        /// <summary>
        /// Huidig volume 0 ~ 100.
        /// </summary>
        private static double GetVolume() => SoundService.MusicVolume;

        /// <summary>
        /// Volume aangepast in het pauzemenu 0 ~ 100.
        /// </summary>
        private static void SetVolume(double volume) => SoundService.MusicVolume = volume;

        #endregion
    }
}