namespace ByteRacer.Models
{
    /// <summary>
    /// Format van resultaat van één race.
    /// </summary>
    public class PlayResult
    {
        public string Name { get; set; } = "";
        public double RaceTime { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}