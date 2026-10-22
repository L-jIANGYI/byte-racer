using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ByteRacer.Models;
using ByteRacer.Services;

namespace ByteRacer.Views
{
    /// <summary>
    /// Interaction logic for LeaderboardPage.xaml
    /// </summary>
    public partial class LeaderboardPage : UserControl
    {
        private readonly MainWindow _main;
        private const int ShownCount = 10;

        public LeaderboardPage(MainWindow main, PlayResult? latest = null)
        {
            InitializeComponent();
            _main = main;


            Loaded += (s, e) => Focus();
        }

        private static string FormatTime(double seconds)
            => TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss\.ff");

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

    public class LeaderboardRow
    {
        public int Rank { get; init; }
        public string Name { get; init; } = "---";
        public string Time { get; init; } = "--:--.--";
        public bool IsEmpty { get; init; }
        public bool IsLatest { get; init; }
    }
}