using System;
using System.IO;
using System.Windows.Media;

namespace ByteRacer.Services
{
    public static class SoundService
    {
        private static readonly MediaPlayer _music = new MediaPlayer();
        private static readonly List<MediaPlayer> _activeEffects = new();

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

        public static void PlayEffect(string fileName, Action? onFinished = null)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "Assets", "Sounds", fileName);

            var effect = new MediaPlayer();
            effect.Volume = _music.Volume;  
            _activeEffects.Add(effect);

            void Finish()
            {
                effect.Close();
                _activeEffects.Remove(effect);
                onFinished?.Invoke();
            }

            effect.MediaEnded += (s, e) => Finish();
            effect.MediaFailed += (s, e) => Finish();

            effect.Open(new Uri(path, UriKind.Absolute));
            effect.Play();
        }
    }
}