using System;
using System.Collections.Generic;
using System.Text;

namespace ByteRacer.Game
{
    class LapTracker
    {
        public int LapCount { get; private set; } = 1;
        public int LapMax { get; } = 3;

        public int NextCheckpoint { get; private set; } = 0;

        private bool _wasAtFinishLine = false;


        public bool Update(double carX, double carY, bool isAtStartFinishLine)
        {
            // Controleer het volgende checkpoint.
            if (NextCheckpoint < Track.Checkpoints.Count)
            {
                var checkpoint = Track.Checkpoints[NextCheckpoint];
                // afstand bereken van auto naar checkpoint
                double dx = carX - checkpoint.X;
                double dy = carY - checkpoint.Y;
                double distance = Math.Sqrt(dx * dx + dy * dy);

                if (distance <= Track.CheckpointRadius)
                {
                    NextCheckpoint++;
                }
            }

            // Controleer of alle checkpoints zijn bereikt.
            bool allCheckpointsPassed = NextCheckpoint >= Track.Checkpoints.Count;

            // Tel alleen bij het binnenkomen van de finishzone.
            bool crossedFinishEntry = isAtStartFinishLine && !_wasAtFinishLine;

            if (crossedFinishEntry && allCheckpointsPassed)
            {
                LapCount++;
                NextCheckpoint = 0;

                if (LapCount > LapMax)
                {
                    _wasAtFinishLine = isAtStartFinishLine;
                    return true;
                }
            }

            _wasAtFinishLine = isAtStartFinishLine;
            return false;
        }

        public void Reset()
        {
            LapCount = 1;
            NextCheckpoint = 0;
            _wasAtFinishLine = false;
        }
    }
}
