using System.IO;
using System.Windows.Media;
using System.Windows.Threading;

namespace ByteRacer.Services
{
    public static class SoundService
    {
        private static readonly MediaPlayer _music = new();
        private static readonly MediaPlayer _engine = new();
        private static readonly List<MediaPlayer> _activeEffects = new();

        private static double _musicVolume; // 0 ~ 100
        private static double _sfxVolume; // 0 ~ 100
        private static double _engineLevel; // 0 ~ 1, snelheid bepaalt

        private static readonly DispatcherTimer _saveTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(400)
        };

        static SoundService()
        {
            //start with the volume from the last session
            var settings = SettingsService.Load();
            _musicVolume = settings.MusicVolume;
            _sfxVolume = settings.SfxVolume;
            _music.Volume = _musicVolume / 100;

            _saveTimer.Tick += (s, e) =>
            {
                _saveTimer.Stop();
                SettingsService.Save(_musicVolume, _sfxVolume);
            };

            // loop
            Loop(_music);
            Loop(_engine);
        }

        private static void Loop(MediaPlayer player)
        {
            player.MediaEnded += (s, e) =>
            {
                player.Position = TimeSpan.Zero;
                player.Play();
            };

            _music.MediaFailed += (s, e) =>
                System.Diagnostics.Debug.WriteLine("Music failed: " + e.ErrorException?.Message);
        }

        private static string PathOf(string fileName) =>
            Path.Combine(AppContext.BaseDirectory, "Assets", "Sounds", fileName);

        // -------------------- Volume ---------------------

        // 0 = silent, 100 = full volume 
        public static double MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = Math.Clamp(value, 0, 100);
                _music.Volume = _musicVolume / 100;
                QueueSave();
            }
        }

        public static double SfxVolume
        {
            get => _sfxVolume;
            set
            {
                _sfxVolume = Math.Clamp(value, 0, 100);
                ApplyEngineVolume();
                QueueSave();
            }
        }

        private static void QueueSave()
        {
            _saveTimer.Stop();
            _saveTimer.Start();
        }

        // -------------------- Music ---------------------
        public static void PlayMusic(string fileName)
        {
            _music.Open(new Uri(PathOf(fileName)));
            _music.Play();
        }

        public static void StopMusic() => _music.Stop();

        // -------------------- sound effect ---------------------
        public static void PlayEffect(string fileName, Action? onFinished = null)
        {
            var effect = new MediaPlayer();
            effect.Volume = _sfxVolume / 100;
            _activeEffects.Add(effect);

            void Done()
            {
                effect.Close();
                _activeEffects.Remove(effect);
            }

            effect.MediaEnded += (s, e) => Done();

            // if the file can't be played continue anyway so the game doesn't freeze
            effect.MediaFailed += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"Effect failed: {fileName} - {e.ErrorException?.Message}");
                Done();
            };

            effect.Open(new Uri(PathOf(fileName)));
            effect.Play();
        }

        // -------------------- Engine --------------------

        public static void StartEngine(string fileName)
        {
            _engineLevel = 0;
            _engine.Open(new Uri(PathOf(fileName)));
            ApplyEngineVolume();
            _engine.Play();
        }

        public static void UpdateEngine(double level)
        {
            _engineLevel = Math.Clamp(level, 0, 1);
            ApplyEngineVolume();
            _engine.SpeedRatio = 0.8 + 0.6 * _engineLevel;
        }

        public static void PauseEngine() => _engine.Pause();
        public static void ResumeEngine() => _engine.Play();
        public static void StopEngine() => _engine.Stop();

        private static void ApplyEngineVolume() =>
            _engine.Volume = _sfxVolume / 100 * (0.35 + 0.65 * _engineLevel);
    }
}