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
    /// <summary>
    /// Interaction logic for LeaderboardPage.xaml
    /// </summary>
    public partial class LeaderboardPage : UserControl
    {
        private readonly MainWindow _main;

        // TESTDATA VOOR DE LEADERBOARD
        private readonly List<(int Rank, string Gebruikersnaam, string Tijd)> testData =
            new List<(int Rank, string Gebruikersnaam, string Tijd)>
            {
                (1, "Gebruikersnaam 1", "00:42"),
                (2, "Gebruikersnaam 2", "00:57"),
                (3, "Gebruikersnaam 3", "01:13")
            };

        public LeaderboardPage(MainWindow main)
        {
            InitializeComponent();
            _main = main;

            foreach (var speler in testData)
            {
                TextBlock rank = new TextBlock
                {
                    Text = speler.Rank.ToString(),
                    FontFamily = new FontFamily("./Fonts/#Press Start 2P"),
                    FontSize = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                TextBlock gebruikersnaam = new TextBlock
                {
                    Text = speler.Gebruikersnaam,
                    FontFamily = new FontFamily("./Fonts/#Press Start 2P"),
                    FontSize = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                TextBlock tijd = new TextBlock
                {
                    Text = speler.Tijd,
                    FontFamily = new FontFamily("./Fonts/#Press Start 2P"),
                    FontSize = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                int row = LeaderboardGrid.RowDefinitions.Count;

                LeaderboardGrid.RowDefinitions.Add(
                    new RowDefinition { Height = new GridLength(60) }
                );

                Grid.SetRow(rank, row);
                Grid.SetColumn(rank, 0);

                Grid.SetRow(gebruikersnaam, row);
                Grid.SetColumn(gebruikersnaam, 1);

                Grid.SetRow(tijd, row);
                Grid.SetColumn(tijd, 2);

                LeaderboardGrid.Children.Add(rank);
                LeaderboardGrid.Children.Add(gebruikersnaam);
                LeaderboardGrid.Children.Add(tijd);
            }
        }
    }
}