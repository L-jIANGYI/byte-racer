using ByteRacer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;

namespace ByteRacer.Overlays
{
    /// <summary>
    /// methode om window te showen, maak wat vars aan en een random voor zo, lees de json, roep loadquestion method op
    /// </summary>
    public partial class QuizOverlay : UserControl
    {
        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        private List<Question> questions;
        private Question currentQuestion;
        private Random random = new Random();

        public QuizOverlay()
        {
            InitializeComponent();

            string json = File.ReadAllText("Data/questions.json");
            questions = JsonSerializer.Deserialize<List<Question>>(
                json,
                new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip // zodat de comments in de json het niet verneuken :-)
                }
            );

            LoadQuestion();
        }

        public class Question
        {
            public string question {  get; set; }
            public List<string> answers { get; set; }
            public int correctAnswer { get; set; }
        }

        private void LoadQuestion()
        {
            currentQuestion = questions[random.Next(questions.Count)];
            QuestionText.Text = currentQuestion.question;
            Answer1.Content = currentQuestion.answers[0];
            Answer2.Content = currentQuestion.answers[1];
            Answer3.Content = currentQuestion.answers[2];
        }

        private void CheckAnswer(int answer)
        {
            if (answer == currentQuestion.correctAnswer)
                MessageBox.Show("antwoord was correct");
            else
                MessageBox.Show("antwoord was incorrect");

            LoadQuestion();
        }

        private void Click_Ans1(object sender, RoutedEventArgs e)
        {
            CheckAnswer(0);
        }

        private void Click_Ans2(object sender, RoutedEventArgs e)
        {
            CheckAnswer(1);
        }

        private void Click_Ans3(object sender, RoutedEventArgs e)
        {
            CheckAnswer(2);
        }
    }
}
