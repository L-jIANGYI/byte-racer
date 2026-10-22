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

namespace ByteRacer.Views
{
    public partial class StartPage : UserControl
    {
        private readonly MainWindow _main;

        public StartPage(MainWindow main)
        {
            InitializeComponent();
            _main = main;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _main.ShowNameEntry();
        }

        private void LeaderboardButton_Click(object sender, RoutedEventArgs e)
        {
            _main.ShowLeaderboard();
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            InfoPanel.Visibility = Visibility.Visible;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsPanel.Visibility = Visibility.Visible;
        }
    }
}