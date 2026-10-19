using System.Windows;
using System.Windows.Controls;

namespace ByteRacer.Overlays
{
    public partial class PauseOverlay : UserControl
    {
        public event Action? ResumeClicked;
        public event Action? RestartClicked;
        public event Action? ExitClicked;
        public event Action<double>? VolumeChanged;
        private bool _isSettingVolume;

        public PauseOverlay()
        {
            InitializeComponent();
            VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
            UpdateVolumeIcon(VolumeSlider.Value);
        }

        public void Show(double volume)
        {
            _isSettingVolume = true;
            VolumeSlider.Value = volume;
            _isSettingVolume = false;
            UpdateVolumeIcon(VolumeSlider.Value);

            Visibility = Visibility.Visible;

            //Dispatcher.InvokeAsync(() => ResumeButton.Focus(), DispatcherPriority.Input);
        }

        public void Hide() => Visibility = Visibility.Collapsed;

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateVolumeIcon(e.NewValue);
            if (!_isSettingVolume)
                VolumeChanged?.Invoke(e.NewValue);
        }

        private void UpdateVolumeIcon(double volume)
        {
            Wave1.Visibility = volume > 0 ? Visibility.Visible : Visibility.Hidden;
            Wave2.Visibility = volume > 33 ? Visibility.Visible : Visibility.Hidden;
            Wave3.Visibility = volume > 66 ? Visibility.Visible : Visibility.Hidden;
        }

        private void Resume_Click(object sender, RoutedEventArgs e) => ResumeClicked?.Invoke();

        private void Restart_Click(object sender, RoutedEventArgs e) => RestartClicked?.Invoke();

        private void Exit_Click(object sender, RoutedEventArgs e) => ExitClicked?.Invoke();
    }
}