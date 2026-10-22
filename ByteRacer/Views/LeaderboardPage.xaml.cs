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

            ShowResults(latest);

            Loaded += (s, e) => Focus();
        }

        private void ShowResults(PlayResult? latest)
        {
            List<PlayResult> top = LeaderboardService.GetTop(ShownCount);

            var rows = new List<LeaderboardRow>();
            for (int i = 0; i < ShownCount; i++)
            {
                int rank = i + 1;

                if (i < top.Count)
                {
                    rows.Add(new LeaderboardRow
                    {
                        Rank = rank,
                        Name = top[i].Name,
                        Time = FormatTime(top[i].RaceTime),
                        IsLatest = top[i] == latest
                    });
                }
                else
                {
                    rows.Add(new LeaderboardRow { Rank = rank, IsEmpty = true });
                }
            }

            ApplyRows(rows);

            if (latest != null && !top.Contains(latest))
            {
                YourRankRow.Content = new LeaderboardRow
                {
                    Rank = LeaderboardService.GetRank(latest),
                    Name = latest.Name,
                    Time = FormatTime(latest.RaceTime),
                    IsLatest = true
                };
                YourRankPanel.Visibility = Visibility.Visible;
            }
            else
            {
                YourRankPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void ApplyRows(List<LeaderboardRow> rows)
        {
            FirstPlace.Content = rows[0];
            SecondPlace.Content = rows[1];
            ThirdPlace.Content = rows[2];
            RestList.ItemsSource = rows.Skip(3).ToList();
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