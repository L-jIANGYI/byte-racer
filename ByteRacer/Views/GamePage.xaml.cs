using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ByteRacer.Views
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : UserControl
    {
        private readonly MainWindow _main;
        private readonly Stopwatch _clock = new Stopwatch();
        private TimeSpan _lastFrameTime;
        private TimeSpan _lastRenderingTime;
        private int _frameCount;
        private double _redX;
        private double _yellowX;
        private const double RedSpeed = 200;
        private const double YellowStep = 200.0 / 144;

        public GamePage(MainWindow main)
        {
            InitializeComponent();
            _main = main;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _clock.Start();
            _lastFrameTime = _clock.Elapsed;
            CompositionTarget.Rendering += OnFrame;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering -= OnFrame;
            _clock.Stop();
        }

        private void OnFrame(object? sender, EventArgs e)
        {
            TimeSpan renderingTime = ((RenderingEventArgs)e).RenderingTime;
            if (renderingTime == _lastRenderingTime) return;
            _lastRenderingTime = renderingTime;

            if (LagCheckBox.IsChecked == true) Thread.Sleep(40);

            TimeSpan now = _clock.Elapsed;
            double dt = (now - _lastFrameTime).TotalSeconds;
            _lastFrameTime = now;
            if (dt > 0.05) dt = 0.05;

            _frameCount++;

            _redX += RedSpeed * dt;
            _yellowX += YellowStep;

            if (_redX > World.ActualWidth) _redX = -RedBox.Width;
            if (_yellowX > World.ActualWidth) _yellowX = -YellowBox.Width;

            Canvas.SetLeft(RedBox, _redX);
            Canvas.SetLeft(YellowBox, _yellowX);

            DebugText.Text = $"count  {_frameCount}\n" +
                             $"dt    {dt:0.0000} s\n" +
                             $"FPS   {1 / dt:0}";
        }
    }
}