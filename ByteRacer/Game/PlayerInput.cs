using System.Windows.Input;

namespace ByteRacer.Game
{
    class PlayerInput
    {
        private readonly HashSet<Key> _pressedKeys = new HashSet<Key>();

        public void Press(Key key) => _pressedKeys.Add(key);

        public void Release(Key key) => _pressedKeys.Remove(key);

        public void Clear() => _pressedKeys.Clear();

        public CarInput Read() => new CarInput(
            Gas: IsDown(Key.Up, Key.W),
            Brake: IsDown(Key.Down, Key.S),
            Left: IsDown(Key.Left, Key.A),
            Right: IsDown(Key.Right, Key.D));

        private bool IsDown(Key a, Key b) => _pressedKeys.Contains(a) || _pressedKeys.Contains(b);
    }
}