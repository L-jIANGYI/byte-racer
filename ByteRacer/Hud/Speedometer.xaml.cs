using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ByteRacer.Game;
using ByteRacer.Models;

namespace ByteRacer.Hud
{
    /// <summary>
    /// Pixel-snelheidsmeter: getal in km/h + oplopende balk van segmenten.
    /// </summary>
    public partial class Speedometer : UserControl
    {
        private const int SegmentCount = 16;
        private const double SegmentWidth = 8;
        private const double SegmentGap = 2;
        private const double MinSegmentHeight = 6;
        private const double MaxSegmentHeight = 20;

        private static readonly Brush OffBrush = Frozen(0x33, 0x33, 0x33);
        private static readonly Brush GreenBrush = Frozen(0x3F, 0xD1, 0x5A);
        private static readonly Brush YellowBrush = Frozen(0xFF, 0xCC, 0x00);
        private static readonly Brush BoostBrush = Frozen(0x38, 0xD9, 0xFF);
        private static readonly Brush SlowBrush = Frozen(0xFF, 0x3B, 0x30);

        private readonly List<Rectangle> _segments = new();
        private readonly List<Brush> _onBrushes = new(); // kleur van elk segment als het aan is

        // Vorige waarden: alleen de UI aanpassen als er echt iets verandert
        private int _shownKmh = -1;
        private int _litCount = -1;

        public Speedometer()
        {
            InitializeComponent();
            CreateSegments();
        }

        /// <summary>
        /// Maakt de segmenten aan, elk iets hoger dan de vorige.
        /// De schaal loopt tot topsnelheid × QuizBoost: het laatste stuk (blauw) brandt alleen met boost.
        /// </summary>
        private void CreateSegments()
        {
            double normalEnd = 1 / GameSettings.QuizBoost; // waar de normale topsnelheid op de schaal ligt

            for (int i = 0; i < SegmentCount; i++)
            {
                double t = (double)i / (SegmentCount - 1);
                double position = (i + 1.0) / SegmentCount;

                var segment = new Rectangle
                {
                    Width = SegmentWidth,
                    Height = MinSegmentHeight + (MaxSegmentHeight - MinSegmentHeight) * t,
                    Margin = new Thickness(0, 0, i < SegmentCount - 1 ? SegmentGap : 0, 0),
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Fill = OffBrush
                };

                SegmentPanel.Children.Add(segment);
                _segments.Add(segment);

                if (position > normalEnd + 0.001) _onBrushes.Add(BoostBrush);
                else if (position > normalEnd * 0.6) _onBrushes.Add(YellowBrush);
                else _onBrushes.Add(GreenBrush);
            }
        }

        /// <summary>
        /// Elke frame aanroepen met de auto van de speler.
        /// </summary>
        public void Update(Car car)
        {
            double speed = Math.Abs(car.Speed);

            // Getal
            int kmh = (int)Math.Round(speed * GameSettings.MetersPerPixel * 3.6);
            if (kmh != _shownKmh)
            {
                _shownKmh = kmh;
                SpeedText.Text = kmh.ToString("000");
            }

            // Segmenten
            double fullScale = car.MaxSpeed * GameSettings.QuizBoost;
            int lit = Math.Clamp((int)Math.Floor(speed / fullScale * SegmentCount), 0, SegmentCount);
            if (lit != _litCount)
            {
                _litCount = lit;
                for (int i = 0; i < SegmentCount; i++)
                    _segments[i].Fill = i < lit ? _onBrushes[i] : OffBrush;
            }

            // Boost / slow
            if (car.HasEffect)
            {
                bool boost = car.SpeedMultiplier > 1;
                EffectText.Text = $"{(boost ? "BOOST" : "SLOW")} {car.EffectTimeLeft:0.0}s";
                EffectText.Foreground = boost ? BoostBrush : SlowBrush;
                EffectText.Visibility = Visibility.Visible;
            }
            else
            {
                EffectText.Visibility = Visibility.Hidden;
            }
        }

        private static Brush Frozen(byte r, byte g, byte b)
        {
            var brush = new SolidColorBrush(Color.FromRgb(r, g, b));
            brush.Freeze(); // frozen brushes zijn sneller en hoeven niet gevolgd te worden
            return brush;
        }
    }
}