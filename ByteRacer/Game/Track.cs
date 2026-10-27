using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace ByteRacer.Game
{
    public static class Track
    {
        public static Geometry Centerline { get; } =
            Geometry.Parse(
                "M 1750,300 " +
                "C 2200,0 3250,300 2650,800 " +
                "C 2650,800 2800,2000 2350,1100 " +
                "C 1525,1400 1000,1100 1000,1100 " +
                "C 625,275 1150,50 1750,300 Z"
            );

        public const double EdgeWidth = 190;
        public const double RoadWidth = 160;

        // Maximale afstand waarop een checkpoint wordt geactiveerd.
        public const double CheckpointRadius = 100;

        // Checkpoints in de volgorde van de race.
        public static IReadOnlyList<Point> Checkpoints { get; } =
            new List<Point>
            {
                new Point(2500, 220),
                new Point(2650, 800),
                new Point(2675, 1250),
                new Point(1350, 1200),
                new Point(900, 500)
            };
    }
}