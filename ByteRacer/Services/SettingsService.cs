using System.IO;
using System.Text.Json;
using ByteRacer.Models;

namespace ByteRacer.Services
{
    public static class SettingsService
    {
        // %AppData%\ByteRacer\settings.json: outside the project so it never
        // ends up in git and rebuilding can't overwrite it
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ByteRacer", "settings.json");

        public static UserSettings Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var data = JsonSerializer.Deserialize<UserSettings>(File.ReadAllText(FilePath));
                    if (data != null)
                    {
                        data.MusicVolume = Math.Clamp(data.MusicVolume, 0, 100);
                        data.SfxVolume = Math.Clamp(data.SfxVolume, 0, 100);
                        return data;
                    }
                }
            }
            catch
            {
                // unreadable or broken file fall back to the default
            }

            return new UserSettings();
        }

        public static void Save(double music, double sfx)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
                File.WriteAllText(FilePath, JsonSerializer.Serialize(new UserSettings
                {
                    MusicVolume = music,
                    SfxVolume = sfx
                }));
            }
            catch
            {
                //so a failure must never crash the game
            }
        }
    }
}