namespace ByteRacer.Models
{
    /// <summary>
    /// Vaste spelregels
    /// </summary>
    public static class GameSettings
    {
        // ---------- Race ----------
        public const double FinishScreenSeconds = 3;
        public const double MetersPerPixel = 0.1;

        // ---------- Startlichten ----------
        public const int StartLightCount = 5;
        public const double StartLightInterval = 1.0;
        public const double StartMinHoldTime = 0.2;
        public const double StartMaxHoldTime = 2.0;

        // ---------- Quiz ----------
        public const double QuizBoost = 1.5;
        public const double QuizPenalty = 0.5;
        public const double QuizEffectSeconds = 3;
    }
}