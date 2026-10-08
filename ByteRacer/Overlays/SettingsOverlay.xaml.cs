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

namespace ByteRacer.Overlays
{
    /// <summary>
    /// Interaction logic for SettingsOverlay.xaml
    /// </summary>
    public partial class SettingsOverlay : UserControl
    {
        public SettingsOverlay()
        {
            InitializeComponent();
            UpdateVolumeIcon(VolumeSlider.Value);
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateVolumeIcon(e.NewValue);
        }

        private void UpdateVolumeIcon(double volume)
        {
            if (Wave1 == null || Wave2 == null || Wave3 == null) return;

            Wave1.Visibility = volume > 0 ? Visibility.Visible : Visibility.Collapsed;
            Wave2.Visibility = volume > 33 ? Visibility.Visible : Visibility.Collapsed;
            Wave3.Visibility = volume > 66 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
