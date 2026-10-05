using System.Windows;
using ByteRacer.Views;

namespace ByteRacer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowStart();
        }

        public void ShowStart() => PageHost.Content = new StartPage(this);
        public void ShowGame() => PageHost.Content = new GamePage(this);
        public void ShowLeaderboard() => PageHost.Content = new LeaderboardPage(this);
        public void ShowNameEntry() => PageHost.Content = new NameEntryPage(this);
    }
}