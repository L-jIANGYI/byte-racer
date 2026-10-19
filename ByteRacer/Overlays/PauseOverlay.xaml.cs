using System.Windows;
using System.Windows.Controls;

namespace ByteRacer.Overlays
{
    public partial class PauseOverlay : UserControl
    {
        public PauseOverlay()
        {
            InitializeComponent();
        }

        public void Show() => Visibility = Visibility.Visible;

        public void Hide() => Visibility = Visibility.Collapsed;
    }
}