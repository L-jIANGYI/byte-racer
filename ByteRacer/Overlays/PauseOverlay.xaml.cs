using System.Windows;
using System.Windows.Controls;

namespace ByteRacer.Overlays
{
    public partial class PauseOverlay : UserControl
    {
        public event Action? ResumeClicked;
        public event Action? RestartClicked;
        public event Action? ExitClicked;

        public PauseOverlay()
        {
            InitializeComponent();
        }

        public void Show() => Visibility = Visibility.Visible;

        public void Hide() => Visibility = Visibility.Collapsed;

        private void Resume_Click(object sender, RoutedEventArgs e) => ResumeClicked?.Invoke();

        private void Restart_Click(object sender, RoutedEventArgs e) => RestartClicked?.Invoke();

        private void Exit_Click(object sender, RoutedEventArgs e) => ExitClicked?.Invoke();
    }
}