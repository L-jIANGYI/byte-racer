namespace ByteRacer.Game
{
    public readonly record struct CarInput(bool Gas, bool Brake, bool Left, bool Right)
    {
        public static CarInput None => default;
    }

    public class Car
    {
        // State
        public double X { get; set; }
        public double Y { get; set; }
        public double Angle { get; set; }
        public double Speed { get; set; }

        // Question Effect
        public double SpeedMultiplier { get; private set; } = 1;
        public double EffectTimeLeft { get; private set; }

        // Parameters
        public double MaxSpeed { get; set; } = 450;
        public double MaxReverse { get; set; } = 120;
        public double Acceleration { get; set; } = 250;
        public double BrakePower { get; set; } = 500;
        public double Friction { get; set; } = 120;
        public double TurnSpeed { get; set; } = 160;
        public double GrassMaxSpeed { get; set; } = 140;

        public void Update(double dt, CarInput input, bool onTrack)
        {
            if (EffectTimeLeft > 0)
            {
                EffectTimeLeft -= dt;
                if (EffectTimeLeft <= 0)
                {
                    EffectTimeLeft = 0;
                    SpeedMultiplier = 1;
                }
            }

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

            double maxSpeed = (onTrack ? MaxSpeed : GrassMaxSpeed) * SpeedMultiplier;
            if (Speed > maxSpeed) Speed = Math.Max(maxSpeed, Speed - BrakePower * dt);
            else Speed = Math.Clamp(Speed, -MaxReverse, maxSpeed);

            double turnFactor = Math.Clamp(Speed / 150, -1, 1);
            if (input.Left) Angle -= TurnSpeed * turnFactor * dt;
            if (input.Right) Angle += TurnSpeed * turnFactor * dt;

            double radians = Angle * Math.PI / 180;
            X += Math.Cos(radians) * Speed * dt;
            Y += Math.Sin(radians) * Speed * dt;
        }

        public void ApplyEffect(double speedMultiplier, double seconds)
        {
            SpeedMultiplier = speedMultiplier;
            EffectTimeLeft = seconds;
        }
    }
}