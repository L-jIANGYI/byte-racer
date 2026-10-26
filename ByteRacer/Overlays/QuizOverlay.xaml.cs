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
using ByteRacer.Services;
using Path = System.IO.Path;

namespace ByteRacer.Overlays
{
    /// <summary>
    /// methode om window te showen, maak json option, wat vars en een random aan voor zo, lees de json, roep loadquestion method op
    /// </summary>
    public partial class QuizOverlay : UserControl
    {
        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            ReadCommentHandling = JsonCommentHandling.Skip // zodat de comments in de json het niet verneuken :-)
        };

        private List<Question> questions = new();
        private Question currentQuestion = null!;
        private Random random = new Random();

        public QuizOverlay()
        {
            InitializeComponent();

            string path = Path.Combine(AppContext.BaseDirectory, "Data", "questions.json");
            string json = File.ReadAllText(path);

            questions = JsonSerializer.Deserialize<List<Question>>(json, JsonOptions)
                        ?? throw new InvalidOperationException("questions.json failed to parse");


            LoadQuestion();
        }

        /// <summary>
        /// variables uit de json, hebben we ook hier nodig
        /// </summary>
        public class Question
        {
            public string question { get; set; } = string.Empty;
            public List<string> answers { get; set; } = new();
            public int correctAnswer { get; set; }
        }

        /// <summary>
        /// pak een random vraag uit de json, en zet alle labels
        /// </summary>
        private void LoadQuestion()
        {
            currentQuestion = questions[random.Next(questions.Count)];
            QuestionText.Text = currentQuestion.question;
            Answer1.Content = currentQuestion.answers[0];
            Answer2.Content = currentQuestion.answers[1];
            Answer3.Content = currentQuestion.answers[2];
        }

        /// <summary>
        /// maak een event aan die we in GamePage verbinden met de OnQuizAnswered function, en invoke deze wanneer correct
        /// </summary>
        public event Action<bool>? AnswerChecked;

        private void CheckAnswer(int answer)
        {
            bool correct = answer == currentQuestion.correctAnswer;

            Visibility = Visibility.Collapsed;
            LoadQuestion();

            SoundService.PlayEffect(correct ? "correct.wav" : "wrong.wav");
            AnswerChecked?.Invoke(correct);
        }

        /// <summary>
        /// roep CheckAnswer op met de bijbehorende answer button Tag property
        /// </summary>
        private void Click_Ans(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && int.TryParse(button.Tag?.ToString(), out int answer))
                CheckAnswer(answer);
        }
    }
}