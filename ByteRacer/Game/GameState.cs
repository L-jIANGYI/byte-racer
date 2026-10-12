namespace ByteRacer.Game
{
    public enum RaceState
    {
        Countdown,
        Racing,
        Quiz,
        Paused,
        Finished
    }

    class GameState
    {
        // Startlichten
        public const int LightCount = 5;
        public const double LightInterval = 1.0;
        public const double MinHoldTime = 0.2;
        public const double MaxHoldTime = 2.0;
        private readonly double _lightsOutTime;

        public int LightsOn => State == RaceState.Countdown
            ? Math.Min(LightCount, (int)(StateTime / LightInterval))
            : 0;

        // GameState
        public RaceState State { get; private set; } = RaceState.Countdown;
        public double StateTime { get; private set; }
        public double RaceTime { get; private set; }
        private RaceState _stateBeforePause;
        private double _stateTimeBeforePause;

        public bool CanPause => State == RaceState.Countdown || State == RaceState.Racing;
        public bool IsPaused => State == RaceState.Paused;

        public GameState()
        {
            double holdTime = MinHoldTime + Random.Shared.NextDouble() * (MaxHoldTime - MinHoldTime);
            _lightsOutTime = LightCount * LightInterval + holdTime;
        }

        public void Update(double dt)
        {
            StateTime += dt;

            switch (State)
            {
                case RaceState.Countdown:
                    if (StateTime >= _lightsOutTime) ChangeState(RaceState.Racing);
                    break;

                case RaceState.Racing:
                    RaceTime += dt;
                    break;
            }
        }

        public void Pause()
        {
            if (!CanPause) return;

            _stateBeforePause = State;
            _stateTimeBeforePause = StateTime;
            ChangeState(RaceState.Paused);
        }

        public void Resume()
        {
            if (!IsPaused) return;

            State = _stateBeforePause;
            StateTime = _stateTimeBeforePause;
        }

        public void Finish()
        {
            if (State != RaceState.Racing) return;
            ChangeState(RaceState.Finished);
        }

        public void ChangeState(RaceState newState)
        {
            State = newState;
            StateTime = 0;
        }
    }
}