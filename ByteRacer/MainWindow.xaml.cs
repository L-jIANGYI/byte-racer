using System.Windows;
using ByteRacer.Models;
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
            SoundService.PlayMusic("menu-music.mp3");
        }

        public void ShowStart() => PageHost.Content = new StartPage(this);
        public void ShowGame(string playerName = "") => PageHost.Content = new GamePage(this, playerName);
        public void ShowLeaderboard(PlayResult? latest = null) => PageHost.Content = new LeaderboardPage(this, latest);
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

        private const double DesignWidth = 800;
        private const double DesignHeight = 450;

        private void Root_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width <= 0 || e.NewSize.Height <= 0) return;

            double scale = Math.Min(e.NewSize.Width / DesignWidth, e.NewSize.Height / DesignHeight);
            PageScale.ScaleX = scale;
            PageScale.ScaleY = scale;

            PageHost.Width = e.NewSize.Width / scale;
            PageHost.Height = e.NewSize.Height / scale;
        }
    }
}