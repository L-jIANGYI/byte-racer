namespace ByteRacer.Game
{
    public readonly record struct CarInput(bool Gas, bool Brake, bool Left, bool Right)
    {
        public static CarInput None => default;
    }

    class Car
    {
        // State
        public double X { get; set; }
        public double Y { get; set; }
        public double Angle { get; set; }
        public double Speed { get; set; }

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

            Speed = Math.Clamp(Speed, -MaxReverse, MaxSpeed);

            double turnFactor = Math.Clamp(Speed / 150, -1, 1);
            if (input.Left) Angle -= TurnSpeed * turnFactor * dt;
            if (input.Right) Angle += TurnSpeed * turnFactor * dt;

            double radians = Angle * Math.PI / 180;
            X += Math.Cos(radians) * Speed * dt;
            Y += Math.Sin(radians) * Speed * dt;
        }
    }
}