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

        public LeaderboardPage(MainWindow main)
        {
            InitializeComponent();
            _main = main;
        }
    }
}