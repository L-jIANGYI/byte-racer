namespace ByteRacer.Models
{
    /// <summary>
    /// Eén opgenomen moment van de auto.
    /// </summary>
    public class GhostSample
    {
        public double T { get; set; }       // racetijd in seconden
        public double X { get; set; }
        public double Y { get; set; }
        public double Angle { get; set; }
    }

    /// <summary>
    /// Complete opname van een run hoort bij de #1 van het leaderboard.
    /// </summary>
    public class GhostData
    {
        public string Name { get; set; } = "";
        public double RaceTime { get; set; }
        public List<GhostSample> Samples { get; set; } = new();
    }
}