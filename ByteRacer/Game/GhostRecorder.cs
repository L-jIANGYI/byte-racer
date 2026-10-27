using ByteRacer.Models;

namespace ByteRacer.Game
{
    /// <summary>
    /// Neemt tijdens de race de positie van de auto op.
    /// </summary>
    public class GhostRecorder
    {
        private const double Interval = 0.05;   
        private double _nextSampleTime;

        public List<GhostSample> Samples { get; } = new();

        public void Record(double raceTime, Car car, bool force = false)
        {
            if (raceTime < _nextSampleTime && !force) return;

            Samples.Add(new GhostSample
            {
                T = Math.Round(raceTime, 3),
                X = Math.Round(car.X, 1),
                Y = Math.Round(car.Y, 1),
                Angle = Math.Round(car.Angle, 1)
            });
            _nextSampleTime = raceTime + Interval;
        }
    }

    /// <summary>
    /// Speelt een opname af: geeft bij een racetijd de positie van de ghost.
    /// </summary>
    public class GhostPlayer
    {
        private readonly List<GhostSample> _samples;
        private int _index;

        public GhostPlayer(List<GhostSample> samples) => _samples = samples;

        /// <returns>false als de ghost er niet (meer) is</returns>
        public bool TryGetPose(double raceTime, out double x, out double y, out double angle)
        {
            x = y = angle = 0;
            if (_samples.Count < 2 || raceTime > _samples[^1].T) return false;

            // racetijd gaat alleen vooruit, dus de index ook
            while (_index < _samples.Count - 2 && _samples[_index + 1].T <= raceTime)
                _index++;

            GhostSample a = _samples[_index];
            GhostSample b = _samples[_index + 1];

            double span = b.T - a.T;
            double f = span > 0 ? Math.Clamp((raceTime - a.T) / span, 0, 1) : 0;

            // tussen twee samples vloeiend bewegen
            x = a.X + (b.X - a.X) * f;
            y = a.Y + (b.Y - a.Y) * f;
            angle = a.Angle + (b.Angle - a.Angle) * f;
            return true;
        }
    }
}