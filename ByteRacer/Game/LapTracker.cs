using System;
using System.Collections.Generic;
using System.Text;

namespace ByteRacer.Game
{
        class LapTracker
        {
            public int LapCount { get; private set; } = 1;
            public int LapMax { get; } = 3;

            private bool _wasAtFinishLine = false;

            public bool Update(double carX, double carY)
            {
                // Finishdetectie:
                bool isAtFinishLine =
                    carX >= 1880 && carX <= 1930 &&
                    carY >= 160 && carY <= 220;

                // Tel alleen wanneer de auto de finishzone binnenrijdt.
                if (isAtFinishLine && !_wasAtFinishLine)
                {
                    LapCount++;

                    if (LapCount > LapMax)
                    {
                        _wasAtFinishLine = isAtFinishLine;
                        return true;
                    }
                }

                _wasAtFinishLine = isAtFinishLine;
                return false;
            }

            public void Reset()
            {
                LapCount = 1;
                _wasAtFinishLine = false;
            }
        }
    }

