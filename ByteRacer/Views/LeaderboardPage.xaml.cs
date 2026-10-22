using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ByteRacer.Views
{
    /// <summary>
    /// Interaction logic for LeaderboardPage.xaml
    /// </summary>
    public partial class LeaderboardPage : UserControl
    {
        private readonly MainWindow _main;
        private const int ShownCount = 10;

        public LeaderboardPage(MainWindow main)
        {
            InitializeComponent();
            _main = main;

            Loaded += (s, e) => Focus();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) => _main.ShowStart();

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Enter or Key.Escape)
            {
                e.Handled = true;
                _main.ShowStart();
            }
        }
    }
}