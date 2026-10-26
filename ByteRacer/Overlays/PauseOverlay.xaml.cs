using ByteRacer.Services;
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

            SfxSlider.ValueChanged += SfxSlider_ValueChanged;
            SfxSlider.PreviewMouseLeftButtonUp += (s, e) => SoundService.PlayEffect("checkpoint.wav");
        }

        /// <summary>
        /// Pauzemenu tonen met het huidige volume.
        /// </summary>
        /// <param name="volume">0 ~ 100</param>
        public void Show(double volume)
        {
            _isSettingVolume = true;
            VolumeSlider.Value = volume;
            SfxSlider.Value = SoundService.SfxVolume;
            _isSettingVolume = false;
            UpdateVolumeIcon(VolumeSlider.Value);
            UpdateSfxIcon(SfxSlider.Value);

            Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Pauzemenu verbergen
        /// </summary>
        public void Hide() => Visibility = Visibility.Collapsed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateVolumeIcon(e.NewValue);
            if (!_isSettingVolume)
                VolumeChanged?.Invoke(e.NewValue);
        }

        /// <summary>
        /// Golven van volume bepalen.
        /// </summary>
        /// <param name="volume"></param>
        private void UpdateVolumeIcon(double volume)
        {
            Wave1.Visibility = volume > 0 ? Visibility.Visible : Visibility.Hidden;
            Wave2.Visibility = volume > 33 ? Visibility.Visible : Visibility.Hidden;
            Wave3.Visibility = volume > 66 ? Visibility.Visible : Visibility.Hidden;
        }

        private void SfxSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateSfxIcon(e.NewValue);
            if (!_isSettingVolume)
                SoundService.SfxVolume = e.NewValue;
        }

        private void UpdateSfxIcon(double volume)
        {
            SfxWave1.Visibility = volume > 0 ? Visibility.Visible : Visibility.Hidden;
            SfxWave2.Visibility = volume > 33 ? Visibility.Visible : Visibility.Hidden;
            SfxWave3.Visibility = volume > 66 ? Visibility.Visible : Visibility.Hidden;
        }

        private void Resume_Click(object sender, RoutedEventArgs e) => ResumeClicked?.Invoke();

        private void Restart_Click(object sender, RoutedEventArgs e) => RestartClicked?.Invoke();

        private void Exit_Click(object sender, RoutedEventArgs e) => ExitClicked?.Invoke();
    }
}