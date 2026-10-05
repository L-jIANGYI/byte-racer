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

        public void ShowStart() => PageHost.Content = new StartPage();
        public void ShowNameEntry() => PageHost.Content = new NameEntryPage(this);
    }
}