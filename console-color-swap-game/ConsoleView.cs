using CCSG.Core;
using CCSG.Utils;

namespace CCSG.View
{
    public class ConsoleView
    {
        private int _beakerIndex;
        private int _prevIndex;
        private Game _game;

        public ConsoleView(int count, int capacity)
        {
            _game = new Game(count, capacity);
            Draw();
        }

        internal void HandleInput(ConsoleKey key)
        {
            switch(key)
            {
                case ConsoleKey.D or ConsoleKey.RightArrow:
                    _prevIndex = _beakerIndex;
                    _beakerIndex = _beakerIndex == _game.BeakersCount - 1
                        ? 0
                        : _beakerIndex + 1;
                    break;
                case ConsoleKey.A or ConsoleKey.LeftArrow:
                    _prevIndex = _beakerIndex;
                    _beakerIndex = _beakerIndex == 0 
                        ? _game.BeakersCount - 1 
                        : _beakerIndex - 1;
                    break;
                case ConsoleKey.Spacebar when _game.Hold == 0:
                    _game.TryTake(_beakerIndex);
                    break;
                case ConsoleKey.Spacebar when _game.Hold != 0:
                    _game.TryPut(_beakerIndex);
                    break;
                case ConsoleKey.R:
                    _game = new Game(_game.BeakersCount, _game.BeakerCapacity);
                    break;
            }
            Draw();
        }

        private void Draw()
        {
            int offsetX = 3;
            int offsetY = 2;
            for (int i = 0; i < _game.BeakersCount; i++)
            {
                for(int j = 0; j < _game.BeakerCapacity; j++)
                {
                    ConsoleExtensions.SetCursorPosition(offsetX + (i * (offsetX + 3)), offsetY + _game.BeakerCapacity - j);
                    ConsoleExtensions.SetColors(ConsoleColor.White, ConsoleColor.Black);
                    ConsoleExtensions.Write("║");
                    ConsoleExtensions.SetColors(GetColor(_game[i, j]), ConsoleColor.Black);
                    ConsoleExtensions.Write(GetBall(_game[i, j]));
                    ConsoleExtensions.SetColors(ConsoleColor.White, ConsoleColor.Black);
                    ConsoleExtensions.Write("║");
                }
                ConsoleExtensions.SetCursorPosition(offsetX + (i * (offsetX + 3)), offsetY + _game.BeakerCapacity + 1);
                ConsoleExtensions.SetColors(ConsoleColor.White, ConsoleColor.Black);
                ConsoleExtensions.Write("╚══╝");
            }

            ConsoleExtensions.SetCursorPosition(offsetX + (_prevIndex * (offsetX + 3)), offsetY - 1);
            ConsoleExtensions.SetColors(ConsoleColor.Black, ConsoleColor.Black);
            ConsoleExtensions.Write("    ");
            ConsoleExtensions.SetCursorPosition(offsetX + (_beakerIndex * (offsetX + 3)), offsetY - 1);
            ConsoleExtensions.SetColors(ConsoleColor.White, ConsoleColor.Black);
            ConsoleExtensions.Write(">");
            ConsoleExtensions.SetColors(GetColor(_game.Hold), ConsoleColor.Black);
            ConsoleExtensions.Write(GetBall(_game.Hold));
            ConsoleExtensions.SetColors(ConsoleColor.White, ConsoleColor.Black);
            ConsoleExtensions.Write("<");
        }

        private string GetBall(int number) => number switch
        {
            0 => "  ",
            <= 6 => "()",
            <= 12 => "[]",
            <= 18 => "{}",
        };

        private ConsoleColor GetColor(int number) => number switch
        {
            0 => ConsoleColor.Black,
            1 or 7 or 13 => ConsoleColor.Yellow,
            2 or 8 or 14 => ConsoleColor.Blue,
            3 or 9 or 15 => ConsoleColor.Red,
            4 or 10 or 16 => ConsoleColor.Cyan,
            5 or 11 or 17 => ConsoleColor.Magenta,
            6 or 12 or 18 => ConsoleColor.Green,
        };
    }
}
