using ByteRacer.Models;

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
        private readonly double _lightsOutTime;

        public int LightsOn => State == RaceState.Countdown
            ? Math.Min(GameSettings.StartLightCount, (int)(StateTime / GameSettings.StartLightInterval))
            : 0;

        // GameState
        public RaceState State { get; private set; } = RaceState.Countdown;
        public double StateTime { get; private set; }
        public double RaceTime { get; private set; }

        // Pause
        private RaceState _stateBeforePause;
        private double _stateTimeBeforePause;
        public bool CanPause => State == RaceState.Countdown || State == RaceState.Racing;
        public bool IsPaused => State == RaceState.Paused;

        // Quiz
        public bool CanStartQuiz => State == RaceState.Racing;
        public bool IsInQuiz => State == RaceState.Quiz;

        public GameState()
        {
            double holdTime = GameSettings.StartMinHoldTime
                              + Random.Shared.NextDouble() *
                              (GameSettings.StartMaxHoldTime - GameSettings.StartMinHoldTime);
            _lightsOutTime = GameSettings.StartLightCount * GameSettings.StartLightInterval + holdTime;
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

        public void StartQuiz()
        {
            if (!CanStartQuiz) return;
            ChangeState(RaceState.Quiz);
        }

        public void EndQuiz()
        {
            if (!IsInQuiz) return;
            ChangeState(RaceState.Racing);
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