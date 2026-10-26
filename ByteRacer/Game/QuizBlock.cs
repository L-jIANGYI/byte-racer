using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ByteRacer.Game
{
    public class QuizBlock
    {
        /// <summary>
        /// Hier wat properties die je kan aanpassen
        /// </summary>
        public const int Size = 32; // Hoe groot moeten de blocks zijn
        public const double HitDistance = 44; // Afstand tussen car en block om OpenQuiz te triggeren
        public const int BlockSetsCount = 4; // Hoeveel sets van 3 blocks moeten er over de track verspreid zijn

        /// <summary>
        /// Zet wat vars klaar voor zo
        /// </summary>
        public double X { get; }
        public double Y { get; }
        public int Row { get; }
        public Image Visual { get; }

        private double _timeLeft;

        public bool IsActive => _timeLeft <= 0;

        /// <summary>
        /// Maak QuizBlock aan, geef x y en row property voor wanneer we ze gaan createn, Quizblock draagt de sprite,
        /// en maak open-ended posities op de canvas die in gamepage ingevuld worden
        /// </summary>
        public QuizBlock(double x, double y, ImageSource sprite, int row)
        {
            X = x;
            Y = y;
            Row = row;

            Visual =  new Image { Width = Size, Height = Size, Source = sprite };
            Canvas.SetLeft(Visual, x - Size / 2);
            Canvas.SetTop(Visual, y - Size / 2);
        }

        /// <summary>
        /// Hit gaan we callen bij collision, dan verdwijnt ie voor 2 seconden zodat we geen drie gazillion OpenQuiz calls krijgen van hetzelfde blokje
        /// </summary>
        public void Hit()
        {
            _timeLeft = 2;
            Visual.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Update de timeLeft bool met de game clock, als ie 0 raakt is de downtime klaar en mag het blokje weer züruckkommen
        /// </summary>
        public void Update(double dt)
        {
            if (IsActive) return;

            _timeLeft -= dt;
            if (IsActive) Visual.Visibility = Visibility.Visible;
        }
    }
}
