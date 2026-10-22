using ByteRacer.Models;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace ByteRacer.Services
{
    public static class LeaderboardService
    {
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ByteRacer",
            "leaderboard.json");

        private static readonly List<PlayResult> _results = Load();

        public static void Add(PlayResult result)
        {
            _results.Add(result);
            Save();
        }

        public static List<PlayResult> GetTop(int count) => Ranked().Take(count).ToList();

        public static int GetRank(PlayResult result) => Ranked().ToList().IndexOf(result) + 1;

        private static IEnumerable<PlayResult> Ranked() =>
            _results.OrderBy(r => r.RaceTime).ThenBy(r => r.Date);

        private static List<PlayResult> Load()
        {
            try
            {
                if (!File.Exists(FilePath)) return new List<PlayResult>();
                string json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<List<PlayResult>>(json) ?? new List<PlayResult>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Leaderboard laden mislukt: {ex.Message}");
                return new List<PlayResult>();
            }
        }

        private static void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(FilePath, JsonSerializer.Serialize(_results, options));
                Debug.WriteLine("Leaderboard opslaan gelukt");
            }
            catch (Exception ex)
            {
                // Opslaan mislukt (bijv. geen schrijfrechten): spel gaat gewoon door
                Debug.WriteLine($"Leaderboard opslaan mislukt: {ex.Message}");
            }
        }
    }
}