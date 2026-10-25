using ByteRacer.Game;
using ByteRacer.Models;
using ByteRacer.Services;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ByteRacer.Views
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : UserControl
    {
        // even testen voor minimap
        private static readonly Geometry TestTrack = Geometry.Parse(
            "M 1750,300 C 2200,0 3250,300 2650,800 C 2650,800 2800,2000 2350,1100 " +
            "C 1525,1400 1000,1100 1000,1100 C 625,275 1150,50 1750,300 Z");

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

        private readonly LapTracker _lapTracker = new LapTracker();
        private readonly List<Ellipse> _checkpointMarkers = new List<Ellipse>();
        private readonly List<TextBlock> _checkpointLabels = new List<TextBlock>();
        private const double CheckpointVisualRadius = 165;

        private readonly List<QuizBlock> _blocks = new List<QuizBlock>();

        // ---------- Ghost ----------
        private readonly GhostRecorder _recorder = new GhostRecorder();
        private readonly GhostPlayer? _ghostPlayer;

        #endregion
        public GamePage(MainWindow main, string playerName = "")
        {
            InitializeComponent();
            _main = main;
            _playerName = playerName;

            World.Width = WorldWidth;
            World.Height = WorldHeight;

            InitializeCheckpoints();

            PauseMenu.ResumeClicked += ResumeGame;
            PauseMenu.RestartClicked += RestartGame;
            PauseMenu.ExitClicked += ExitToStart;
            PauseMenu.VolumeChanged += SetVolume;

            Quiz.AnswerChecked += OnQuizAnswered;
            CreateBlocks();

            GhostData? ghost = GhostService.LoadBest();
            _ghostPlayer = ghost == null ? null : new GhostPlayer(ghost.Samples);
        }

        #region ------------------------- Start / Stop -------------------------

        /// <summary>
        /// Init game bij laden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _car.X = 1975;
            _car.Y = 204;
            _car.Angle = 0;
            _car.Speed = 0;

            MiniMap.Setup(TestTrack, 160);

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
        /// 


        private void OnFrame(object? sender, EventArgs e)
        {
            // Voorkom dubbele updates in hetzelfde frame
            TimeSpan renderingTime = ((RenderingEventArgs)e).RenderingTime;
            if (renderingTime == _lastRenderingTime) return;
            _lastRenderingTime = renderingTime;

            // Delta time
            TimeSpan now = _clock.Elapsed;
            double dt = (now - _lastFrameTime).TotalSeconds;
            _lastFrameTime = now;

            _gameState.Update(dt);

            switch (_gameState.State)
            {
                case RaceState.Racing:
                    {
                        _car.Update(dt, _input.Read(), onTrack: true);
                        KeepCarInWorld();

                        // Quizblokken bijwerken en botsingen controleren.
                        UpdateBlocks(dt);

                        // Controleren of de auto de start-/finishlijn bereikt.
                        bool isAtStartFinishLine = IsCarAtStartFinishLine();

                        bool raceFinished = _lapTracker.Update(
                            _car.X,
                            _car.Y,
                            isAtStartFinishLine);

                        // positie opnemen voor de ghost; bij de finish altijd het laatste punt
                        _recorder.Record(_gameState.RaceTime, _car, force: raceFinished);

                        if (raceFinished)
                        {
                            _gameState.Finish();
                        }

                        break;
                    }

                case RaceState.Finished:
                    {
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
            }

            UpdateCheckpoints();
            DrawCar();
            DrawGhost();
            UpdateCamera(dt);
            DrawHud();
            DrawDebug();
        }


        /// <summary>
        /// Controleert of de auto zich binnen het gebied van de start-/finishlijn bevindt.
        /// Zet de positie van de auto om naar de coördinaten van de start-/finishlijn
        /// en controleert of deze binnen de breedte en hoogte van de lijn valt.
        /// Geeft false terug als de start-/finishlijn niet geladen is of de transformatie mislukt.
        /// </summary>
        private bool IsCarAtStartFinishLine()
        {
            if (!StartFinishLine.IsLoaded)
                return false;

            GeneralTransform lineToWorld =
                StartFinishLine.TransformToAncestor(World);

            GeneralTransform worldToLine = lineToWorld.Inverse;

            if (worldToLine == null)
                return false;

            Point carPosition = worldToLine.Transform(
                new Point(_car.X, _car.Y));

            return carPosition.X >= 0 &&
                   carPosition.X <= StartFinishLine.ActualWidth &&
                   carPosition.Y >= 0 &&
                   carPosition.Y <= StartFinishLine.ActualHeight;
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
        /// Tekent de ghost op de plek waar de #1 op dit moment van de race was.
        /// </summary>
        private void DrawGhost()
        {
            if (_ghostPlayer == null) return;

            if (_gameState.State == RaceState.Finished ||
                !_ghostPlayer.TryGetPose(_gameState.RaceTime, out double x, out double y, out double angle))
            {
                GhostVisual.Visibility = Visibility.Collapsed;
                return;
            }

            Canvas.SetLeft(GhostVisual, x - GhostVisual.Width / 2);
            Canvas.SetTop(GhostVisual, y - GhostVisual.Height / 2);
            GhostRotation.Angle = angle;
            GhostVisual.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Maakt voor elk checkpoint een zichtbare cirkel. Dit gebeurt één keer.
        /// Geel = nog te bereiken; groen = al bereikt.
        /// </summary>
        private void InitializeCheckpoints()
        {
            _checkpointMarkers.Clear();
            _checkpointLabels.Clear();

            for (int i = 0; i < Track.Checkpoints.Count; i++)
            {
                var checkpoint = Track.Checkpoints[i];

                // Cirkel toont de zone waar de auto het checkpoint kan halen.
                var marker = new Ellipse
                {
                    Width = CheckpointVisualRadius * 2,
                    Height = CheckpointVisualRadius * 2,
                    StrokeThickness = 5,
                    Stroke = Brushes.Yellow,
                    Fill = new SolidColorBrush(Color.FromArgb(45, 255, 255, 0)),
                    IsHitTestVisible = false
                };

                Canvas.SetLeft(marker, checkpoint.X - marker.Width / 2);
                Canvas.SetTop(marker, checkpoint.Y - marker.Height / 2);
                World.Children.Add(marker);
                _checkpointMarkers.Add(marker);

                // Nummer in het midden zodat de volgorde duidelijk is.
                var label = new TextBlock
                {
                    Text = $"CHECKPOINT {i + 1}",
                    Foreground = Brushes.Black,
                    Background = Brushes.Yellow,
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Padding = new Thickness(5, 3, 5, 3),
                    IsHitTestVisible = false
                };

                Canvas.SetLeft(label, checkpoint.X - 55);
                Canvas.SetTop(label, checkpoint.Y - 13);
                World.Children.Add(label);
                _checkpointLabels.Add(label);
            }
        }

        /// <summary>
        /// Werkt de kleur van de checkpoints bij op basis van de voortgang.
        /// </summary>
        private void UpdateCheckpoints()
        {
            for (int i = 0; i < _checkpointMarkers.Count; i++)
            {
                bool passed = i < _lapTracker.NextCheckpoint;
                _checkpointMarkers[i].Stroke = passed
                    ? Brushes.LimeGreen
                    : Brushes.Yellow;
                _checkpointMarkers[i].Fill = passed
                    ? new SolidColorBrush(Color.FromArgb(45, 0, 255, 0))
                    : new SolidColorBrush(Color.FromArgb(45, 255, 255, 0));

                _checkpointLabels[i].Text = passed
                    ? $"CHECKPOINT {i + 1} ✓"
                    : $"CHECKPOINT {i + 1}";
                _checkpointLabels[i].Background = passed
                    ? Brushes.LimeGreen
                    : Brushes.Yellow;
            }
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

        #endregion

        #region ------------------------- Start / Finish -------------------------

        /// <summary>
        /// Verwerk de racetijd en CenterText o.b.v. status aanpassen 
        /// </summary>
        private void DrawHud()
        {
            RaceTimeText.Text = FormatTime(_gameState.RaceTime);
            DrawStartLights();
            MiniMap.UpdateCar(_car.X, _car.Y, _car.Angle);
            // toon het huidige lap en het max aantal lap
            LapText.Text = $"Lap {Math.Min(_lapTracker.LapCount, _lapTracker.LapMax)} / {_lapTracker.LapMax}";

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

            // nieuwe #1: deze run wordt de ghost
            if (LeaderboardService.GetRank(result) == 1)
            {
                GhostService.Save(new GhostData
                {
                    Name = result.Name,
                    RaceTime = result.RaceTime,
                    Samples = _recorder.Samples
                });
            }

            _main.ShowLeaderboard(result);
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

        /// <summary>
        /// Zet de blocks op de baan, in sets van 3, 
        /// en op de positions die gebaseerd zijn op hoeveel we willen in BlockSetsCount,
        /// eerlijk verpsreid over de volledige TrackPath (de Path voor het asfalt).
        /// </summary>
        private void CreateBlocks()
        {
            var sprite = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/Icons/quizblock.png"));
            RenderOptions.SetBitmapScalingMode(sprite, BitmapScalingMode.NearestNeighbor);
            var track = TrackPath.Data.GetFlattenedPathGeometry();

            for (int set = 0; set < QuizBlock.BlockSetsCount; set++)
            {
                double position = (set + 0.5) / QuizBlock.BlockSetsCount;

                track.GetPointAtFractionLength(position, out Point p, out Point direction);

                double sideX = -direction.Y;
                double sideY = direction.X;

                for (int i = -1; i <= 1; i++)
                {
                    double x = p.X + sideX * i * 45;
                    double y = p.Y + sideY * i * 45;

                    var block = new QuizBlock(x, y, sprite, set);
                    Panel.SetZIndex(block.Visual, 1);
                    World.Children.Add(block.Visual);
                    _blocks.Add(block);
                }
            }
        }

        /// <summary>
        /// Check of we een blok rammen met onze dikke waggie, en update voortdurend de status/downtime van de blocks
        /// </summary>
        private void UpdateBlocks(double dt)
        {
            foreach (var block in _blocks) block.Update(dt);

            foreach (var block in _blocks)
            {
                if (!block.IsActive) continue;

                double dx = _car.X - block.X;
                double dy = _car.Y - block.Y;
                double distance = Math.Sqrt(dx * dx + dy * dy); // pythagoras heb ik oprecht al 4 jaar niet meer gebruikt, shit's crazy

                if (distance > QuizBlock.HitDistance) continue;
                if (!_gameState.CanStartQuiz) return;

                foreach (var other in _blocks)
                {
                    if (other.Row == block.Row)
                        other.Hit();
                }

                OpenQuiz();
                return;
            }
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