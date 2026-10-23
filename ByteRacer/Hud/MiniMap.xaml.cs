using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ByteRacer.Hud
{
    /// <summary>
    /// Interaction logic for MiniMap.xaml
    /// </summary>
    public partial class MiniMap : UserControl
    {
        private const double MapPadding = 60;
        private const double MarkerSizeRatio = 1.0 / 14;

        public MiniMap()
        {
            InitializeComponent();
        }

        public void Setup(Geometry centerline, double roadWidth)
        {
            Rect bounds = centerline.GetRenderBounds(new Pen(Brushes.Black, roadWidth));
            bounds.Inflate(MapPadding, MapPadding);

            MapArea.Width = bounds.Width;
            MapArea.Height = bounds.Height;

            MapOffset.X = -bounds.X;
            MapOffset.Y = -bounds.Y;

            TrackLine.Data = centerline;
            TrackLine.StrokeThickness = roadWidth;

            double size = bounds.Width * MarkerSizeRatio;
            CarMarker.Width = size;
            CarMarker.Height = size * 0.8;
            CarMarker.StrokeThickness = size / 8;
        }

        public void UpdateCar(double x, double y, double angle)
        {
            Canvas.SetLeft(CarMarker, x - CarMarker.Width / 2);
            Canvas.SetTop(CarMarker, y - CarMarker.Height / 2);
            CarRotation.Angle = angle;
        }
    }
}