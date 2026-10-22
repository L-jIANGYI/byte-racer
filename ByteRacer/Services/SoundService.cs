using System;
using System.IO;
using System.Windows.Media;

namespace ByteRacer.Services
{
    public static class SoundService
    {
        private static readonly MediaPlayer _music = new MediaPlayer();

        static SoundService()
        {
            _music.MediaEnded += (s, e) =>
            {
                _music.Position = TimeSpan.Zero;
                _music.Play();
            };
        }

        public static void PlayMusic(string fileName)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "Assets", "Sounds", fileName);
            _music.Open(new Uri(path, UriKind.Absolute));
            _music.Play();
        }

        public static void StopMusic()
        {
            _music.Stop();
        }
        public static double MusicVolume
        {
            get => _music.Volume * 100;
            set => _music.Volume = Math.Clamp(value, 0, 100) / 100;
        }
    }
}