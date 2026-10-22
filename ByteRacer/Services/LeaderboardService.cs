using ByteRacer.Models;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace ByteRacer.Services
{
    /// <summary>
    /// Ophalen en opslaan alle resultaten in %AppData%\ByteRacer\leaderboard.json.
    /// </summary>
    public static class LeaderboardService
    {
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ByteRacer",
            "leaderboard.json");

        private static readonly List<PlayResult> _results = Load();

        /// <summary>
        /// Nieuw resultaat toevoegen en opslaan.
        /// </summary>
        /// <param name="result"></param>
        public static void Add(PlayResult result)
        {
            _results.Add(result);
            Save();
        }

        /// <summary>
        /// Haalt top resultaten volgens 'count' op en sorteren op snelheid
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<PlayResult> GetTop(int count) => Ranked().Take(count).ToList();

        /// <summary>
        /// Haalt specifiek volgens rank het resultaat op
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static int GetRank(PlayResult result) => Ranked().ToList().IndexOf(result) + 1;

        /// <summary>
        /// Resultaten sorteren van snelste naar langzaamste
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<PlayResult> Ranked() =>
            _results.OrderBy(r => r.RaceTime).ThenBy(r => r.Date);

        /// <summary>
        /// Resultaten uit AppData laden
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Resultaten opslaan in AppData
        /// </summary>
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