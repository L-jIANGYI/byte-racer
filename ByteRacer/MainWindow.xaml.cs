using System.Windows;
using System.Windows.Controls;
using ByteRacer.Overlays;
using ByteRacer.Services;
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
            //ShowGame();
            SoundService.PlayMusic("menu-music.mp3");
        }

        public void ShowStart() => PageHost.Content = new StartPage(this);
        public void ShowGame(string playerName = "") => PageHost.Content = new GamePage(this, playerName);
        public void ShowLeaderboard() => PageHost.Content = new LeaderboardPage(this);
        public void ShowNameEntry() => PageHost.Content = new NameEntryPage(this);

        public void SetFullscreen()
        {
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            WindowState = WindowState.Maximized;
        }

        public void SetBorderless()
        {
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;

            var area = SystemParameters.WorkArea;
            Left = area.Left;
            Top = area.Top;
            Width = area.Width;
            Height = area.Height;
        }

        public void SetBordered()
        {
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.SingleBorderWindow;
            ResizeMode = ResizeMode.CanResize;

            Width = 816;
            Height = 489;
            var area = SystemParameters.WorkArea;
            Left = area.Left + (area.Width - Width) / 2;
            Top = area.Top + (area.Height - Height) / 2;
        }
    }
}