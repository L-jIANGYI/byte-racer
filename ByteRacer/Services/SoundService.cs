using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Threading;

namespace ByteRacer.Services
{
    public static class SoundService
    {
        private static readonly MediaPlayer _music = new MediaPlayer();
        private static readonly List<MediaPlayer> _activeEffects = new();

        private static readonly DispatcherTimer _saveTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(400)
        };

        static SoundService()
        {
            // start with the volume from the last session
            _music.Volume = Math.Clamp(SettingsService.LoadVolume(), 0, 100) / 100;

            _saveTimer.Tick += (s, e) =>
            {
                _saveTimer.Stop();
                SettingsService.SaveVolume(MusicVolume);
            };

            // loop
            _music.MediaEnded += (s, e) =>
            {
                _music.Position = TimeSpan.Zero;
                _music.Play();
            };

            _music.MediaFailed += (s, e) =>
                System.Diagnostics.Debug.WriteLine("Music failed: " + e.ErrorException?.Message);
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

        // 0 = silent, 100 = full volume 
        public static double MusicVolume
        {
            get => _music.Volume * 100;
            set
            {
                _music.Volume = Math.Clamp(value, 0, 100) / 100;
                _saveTimer.Stop();
                _saveTimer.Start();
            }
        }

        public static void PlayEffect(string fileName, Action? onFinished = null)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "Assets", "Sounds", fileName);

            var effect = new MediaPlayer();
            effect.Volume = _music.Volume;   // follows the volume slider
            _activeEffects.Add(effect);

            void Finish()
            {
                effect.Close();
                _activeEffects.Remove(effect);
                onFinished?.Invoke();
            }

            effect.MediaEnded += (s, e) => Finish();

            // if the file can't be played continue anyway so the game doesn't freeze
            effect.MediaFailed += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine("Effect failed: " + path + " - " + e.ErrorException?.Message);
                Finish();
            };

            effect.Open(new Uri(path, UriKind.Absolute));
            effect.Play();
        }
    }
}