using ByteRacer.Models;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace ByteRacer.Services
{
    /// <summary>
    /// Ghost van de snelste run opslaan en laden in %AppData%\ByteRacer\ghost.json.
    /// </summary>
    public static class GhostService
    {
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ByteRacer", "ghost.json");

        public static void Save(GhostData ghost)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
                File.WriteAllText(FilePath, JsonSerializer.Serialize(ghost));
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                Debug.WriteLine($"Ghost opslaan mislukt: {ex.Message}");
            }
        }

        /// <summary>
        /// Geeft de ghost van de huidige #1, of null als er geen (passende) is.
        /// </summary>
        public static GhostData? LoadBest()
        {
            PlayResult? best = LeaderboardService.GetTop(1).FirstOrDefault();
            if (best == null || !File.Exists(FilePath)) return null;

            try
            {
                var ghost = JsonSerializer.Deserialize<GhostData>(File.ReadAllText(FilePath));
                if (ghost == null || ghost.Samples.Count < 2) return null;

                // hoort bij een andere run dan de huidige #1: niet gebruiken
                if (Math.Abs(ghost.RaceTime - best.RaceTime) > 0.001) return null;

                return ghost;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
            {
                Debug.WriteLine($"Ghost laden mislukt: {ex.Message}");
                return null;
            }
        }
    }
}