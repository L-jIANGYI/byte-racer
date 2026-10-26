using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ByteRacer.Services;

namespace ByteRacer.Overlays
{
    /// <summary>
    /// Interaction logic for SettingsOverlay.xaml
    /// </summary>
    public partial class SettingsOverlay : UserControl
    {
        // false while the overlay is being built, so the slider's starting
        // value from the XAML (50) doesn't overwrite the real volume
        private bool _ready = false;

        private MainWindow _main => (MainWindow)Application.Current.MainWindow;

        public SettingsOverlay()
        {
            InitializeComponent();

            VolumeSlider.Value = SoundService.MusicVolume;
            UpdateVolumeIcon(VolumeSlider.Value);

            SfxSlider.Value = SoundService.SfxVolume;
            UpdateSfxIcon(SfxSlider.Value);

            _ready = true;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        private void FullscreenButton_Click(object sender, RoutedEventArgs e) => _main.SetFullscreen();

        private void BorderlessButton_Click(object sender, RoutedEventArgs e) => _main.SetBorderless();

        private void BorderedButton_Click(object sender, RoutedEventArgs e) => _main.SetBordered();

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_ready) return;

            SoundService.MusicVolume = e.NewValue;
            UpdateVolumeIcon(e.NewValue);
        }

        private void UpdateVolumeIcon(double volume)
        {
            if (Wave1 == null || Wave2 == null || Wave3 == null) return;

            Wave1.Visibility = volume > 0 ? Visibility.Visible : Visibility.Collapsed;
            Wave2.Visibility = volume > 33 ? Visibility.Visible : Visibility.Collapsed;
            Wave3.Visibility = volume > 66 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SfxSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_ready) return;

            SoundService.SfxVolume = e.NewValue;
            UpdateSfxIcon(e.NewValue);
        }

        private void SfxSlider_Released(object sender, MouseButtonEventArgs e)
        {
            SoundService.PlayEffect("checkpoint.wav");
        }

        private void UpdateSfxIcon(double volume)
        {
            if (SfxWave1 == null || SfxWave2 == null || SfxWave3 == null) return;

            SfxWave1.Visibility = volume > 0 ? Visibility.Visible : Visibility.Collapsed;
            SfxWave2.Visibility = volume > 33 ? Visibility.Visible : Visibility.Collapsed;
            SfxWave3.Visibility = volume > 66 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}