namespace ByteRacer.Game
{
    public readonly record struct CarInput(bool Gas, bool Brake, bool Left, bool Right)
    {
        public static CarInput None => default;
    }

    public class Car
    {
        // ---------- State ----------
        public double X { get; set; }
        public double Y { get; set; }
        public double Angle { get; set; }
        public double Speed { get; set; }

        // ---------- Question Effect ----------
        public double SpeedMultiplier { get; private set; } = 1;
        public double EffectTimeLeft { get; private set; }
        public bool HasEffect => EffectTimeLeft > 0;

        // ---------- Parameters ----------
        public double MaxSpeed { get; set; } = 450;
        public double MaxReverse { get; set; } = 120;
        public double Acceleration { get; set; } = 250;
        public double BrakePower { get; set; } = 500;
        public double Friction { get; set; } = 120;
        public double TurnSpeed { get; set; } = 160;
        public double GrassMaxSpeed { get; set; } = 140;

        /// <summary>
        /// Updates de snelheid, besturing, positie en active effecten van de auto.
        /// </summary>
        /// <param name="dt">De tijd sinds laatste update en nu, in sec</param>
        /// <param name="input">De huidige stuurbeweging van de speler (WASD of pijlen)</param>
        /// <param name="onTrack">Of de auto op het circuit is</param>
        public void Update(double dt, CarInput input, bool onTrack)
        {
            UpdateEffect(dt);

            // Speler input: Gas, remmen, niks = langzaam tot stilstaan
            if (input.Gas)
            {
                Speed += Acceleration * dt;
            }
            else if (input.Brake)
            {
                if (Speed > 0) Speed -= BrakePower * dt;
                else Speed -= Acceleration * dt;
            }
            else
            {
                if (Speed > 0) Speed = Math.Max(0, Speed - Friction * dt);
                else Speed = Math.Min(0, Speed + Friction * dt);
            }

            // Check max speed.
            double maxSpeed = (onTrack ? MaxSpeed : GrassMaxSpeed) * SpeedMultiplier;
            if (Speed > maxSpeed) Speed = Math.Max(maxSpeed, Speed - BrakePower * dt);
            else Speed = Math.Clamp(Speed, -MaxReverse, maxSpeed);

            // Besturen afhankelijk van de snelheid.
            // stilstaan kun je niet besturen. achteruit is besturen omgekeerd.
            double turnFactor = Math.Clamp(Speed / 150, -1, 1);
            if (input.Left) Angle -= TurnSpeed * turnFactor * dt;
            if (input.Right) Angle += TurnSpeed * turnFactor * dt;

            // Update auto positie o.b.v. snelheid en richting.
            double radians = Angle * Math.PI / 180;
            X += Math.Cos(radians) * Speed * dt;
            Y += Math.Sin(radians) * Speed * dt;
        }

        /// <summary>
        /// Update effect duurtijd, alleen als effect is.
        /// </summary>
        /// <param name="dt"></param>
        private void UpdateEffect(double dt)
        {
            if (!HasEffect) return;

            EffectTimeLeft -= dt;
            if (EffectTimeLeft <= 0)
            {
                EffectTimeLeft = 0;
                SpeedMultiplier = 1;
            }
        }

        /// <summary>
        /// Past een snelheidseffect toe op de auto.
        /// </summary>
        /// <param name="speedMultiplier">De factor op max. snelheid van de auto</param>
        /// <param name="seconds">Looptijd van het effect in sec</param>
        public void ApplyEffect(double speedMultiplier, double seconds)
        {
            SpeedMultiplier = speedMultiplier;
            EffectTimeLeft = seconds;
        }
    }
}