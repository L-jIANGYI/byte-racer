using System.Windows.Input;

namespace ByteRacer.Game
{
    /// <summary>
    /// Houdt bij welke toetsen ingedrukt zijn
    /// Pijltjes en WASD werken allebei
    /// </summary>
    class PlayerInput
    {
        private readonly HashSet<Key> _pressedKeys = new HashSet<Key>();

        /// <summary>
        /// Toets ingedrukt
        /// </summary>
        public void Press(Key key) => _pressedKeys.Add(key);

        /// <summary>
        /// Toets los
        /// </summary>
        public void Release(Key key) => _pressedKeys.Remove(key);

        /// <summary>
        /// Alle toetsen clean
        /// </summary>
        public void Clear() => _pressedKeys.Clear();

        /// <summary>
        /// Huidige rijinvoer
        /// </summary>
        public CarInput Read() => new CarInput(
            Gas: IsDown(Key.Up, Key.W),
            Brake: IsDown(Key.Down, Key.S),
            Left: IsDown(Key.Left, Key.A),
            Right: IsDown(Key.Right, Key.D));

        private bool IsDown(Key a, Key b) => _pressedKeys.Contains(a) || _pressedKeys.Contains(b);
    }
}