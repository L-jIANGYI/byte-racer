using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace ByteRacer.Game
{
    class Track
    {
        public static Geometry Centerline { get; } =
            Geometry.Parse(
                "M 1750,300 " +
                "C 2200,0 3250,300 2650,800 " +
                "C 2650,800 2800,2000 2350,1100 " +
                "C 1525,1400 1000,1100 1000,1100 " +
                "C 625,275 1150,50 1750,300"
            );
        public const double EdgeWidth = 190;
        public const double RoadWidth = 160;
    }
}

