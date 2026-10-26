using System;
using System.IO;
using System.Text.Json;

namespace ByteRacer.Services
{
    public static class SettingsService
    {
        private class SettingsData
        {
            public double Volume { get; set; } = 50;
        }

        // %AppData%\ByteRacer\settings.json: outside the project so it never
        // ends up in git and rebuilding can't overwrite it
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ByteRacer", "settings.json");

        public static double LoadVolume()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var data = JsonSerializer.Deserialize<SettingsData>(File.ReadAllText(FilePath));
                    if (data != null) return data.Volume;
                }
            }
            catch
            {
                // unreadable or broken file fall back to the default
            }
            return 50;
        }

        public static void SaveVolume(double volume)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
                File.WriteAllText(FilePath, JsonSerializer.Serialize(new SettingsData { Volume = volume }));
            }
            catch
            {
               //so a failure must never crash the game
            }
        }
    }
}