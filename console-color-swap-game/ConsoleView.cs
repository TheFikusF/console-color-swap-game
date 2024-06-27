using CCSG.Core;
using CCSG.Utils;

namespace CCSG.View
{
    public class ConsoleView
    {
        private bool _drawing;
        private int _beakerIndex;
        private int _prevIndex;
        private int _timer;
        private bool _interactable;
        private Game _game;
        private CancellationTokenSource _cts;

        private const int OFFSET_X = 3;
        private const int OFFSET_Y = 4;

        public ConsoleView(int count, int capacity)
        {
            _game = new Game(count, capacity);
            _cts = new CancellationTokenSource();
            InitDraw();
            Task.Run(() => Update(_cts.Token));
        }

        private async void Update(CancellationToken token)
        {
            _interactable = false;

            try
            {
                while (true)
                {
                    await Task.Delay(1000, token);
                    _interactable = true;
                    _timer++;
                    TryDraw();
                }
            }
            catch
            {
                return;
            }
        }

        private void InitDraw()
        {
            Console.Clear();

            ConsoleExtensions.SetColors(ConsoleColor.White, ConsoleColor.Black);
            ConsoleExtensions.SetCursorPosition(0, 1);
            ConsoleExtensions.Write(new string('=', OFFSET_X * 7 * (_game.BeakerCapacity / 2) + 4));
            ConsoleExtensions.SetCursorPosition(0, _game.BeakerCapacity * 2 + OFFSET_Y * 3);
            ConsoleExtensions.Write("[a][d][w][s] - move, [space] - take/put, \n[u] - undo, [r] - restart");

            int offsetX2 = 0;
            int offsetY2 = 0;
            for (int i = 0; i < _game.BeakersCount; i++)
            {
                ConsoleExtensions.SetCursorPosition(OFFSET_X + (i * (OFFSET_X + 3)) + offsetX2,
                    OFFSET_Y + offsetY2);
                ConsoleExtensions.Write("╖  ╓");

                for (int j = 0; j < _game.BeakerCapacity; j++)
                {
                    ConsoleExtensions.SetCursorPosition(OFFSET_X + (i * (OFFSET_X + 3)) + offsetX2,
                        OFFSET_Y + _game.BeakerCapacity - j + offsetY2);
                    ConsoleExtensions.Write("║  ║");
                }

                ConsoleExtensions.SetCursorPosition(OFFSET_X + (i * (OFFSET_X + 3)) + offsetX2,
                    OFFSET_Y + _game.BeakerCapacity + 1 + offsetY2);
                ConsoleExtensions.Write("╚══╝");

                if (_game.BeakersCount > 7 && offsetY2 == 0 && _game.BeakersCount - i <= i + 2)
                {
                    offsetY2 = _game.BeakerCapacity + OFFSET_Y;
                    offsetX2 = -(OFFSET_X + (i * (OFFSET_X + 3))) - 3;
                }
            }
        }

        public void HandleInput(ConsoleKey key)
        {
            if(_interactable == false)
            {
                return;
            }

            switch(key)
            {
                case ConsoleKey.D or ConsoleKey.RightArrow:
                    Move(1);
                    break;
                case ConsoleKey.A or ConsoleKey.LeftArrow:
                    Move(-1);
                    break;
                case ConsoleKey.W or ConsoleKey.UpArrow:
                    Move(-_game.BeakersCount / 2);
                    break;
                case ConsoleKey.S or ConsoleKey.DownArrow:
                    Move(_game.BeakersCount / 2);
                    break;
                case ConsoleKey.Spacebar when _game.Hold == 0:
                    _game.TryTake(_beakerIndex);
                    break;
                case ConsoleKey.Spacebar when _game.Hold != 0:
                    _game.TryPut(_beakerIndex);
                    break;
                case ConsoleKey.U:
                    _game.Undo();
                    break;
                case ConsoleKey.R:
                    _drawing = false;
                    _timer = 0;
                    _cts.Cancel();
                    _cts = new CancellationTokenSource();
                    _game = new Game(_game.BeakersCount, _game.BeakerCapacity);
                    Task.Run(() => Update(_cts.Token));
                    break;
            }
            TryDraw();
        }

        private void Move(int amount)
        {
            _prevIndex = _beakerIndex;
            _beakerIndex += amount;

            if(_beakerIndex >= _game.BeakersCount)
            {
                _beakerIndex = _beakerIndex - _game.BeakersCount;
            }

            if (_beakerIndex < 0)
            {
                _beakerIndex = _game.BeakersCount + _beakerIndex;
            }
        }

        public async void TryDraw()
        {
            if (_drawing == false)
            {
                _drawing = true;
                Draw();
                _drawing = false;
            }
        }

        private void Draw()
        {
            ConsoleExtensions.SetCursorPosition(0, 0);
            ConsoleExtensions.SetColors(ConsoleColor.White, ConsoleColor.Black);
            ConsoleExtensions.Write($"DONE: {_game.CompletedCount}/{_game.TargetFilledCount}");
            ConsoleExtensions.SetCursorPosition(18, 0);
            ConsoleExtensions.Write($"TIME: {MathF.Floor((float)_timer/60):00}:{_timer % 60:00}");

            bool secondLine = _game.BeakersCount - _beakerIndex <= _beakerIndex + 1;
            bool prevSecondLine = _game.BeakersCount - _prevIndex <= _prevIndex + 1;

            int offsetX2 = 0;
            int offsetY2 = 0;
            for (int i = 0; i < _game.BeakersCount; i++)
            {
                ConsoleExtensions.SetColors(ConsoleColor.White, ConsoleColor.Black);
                ConsoleExtensions.SetCursorPosition(OFFSET_X + (i * (OFFSET_X + 3)) + offsetX2,
                    OFFSET_Y + offsetY2 - 1);

                if(_beakerIndex == i)
                {
                    ConsoleExtensions.SetColors(ConsoleColor.White, ConsoleColor.Black);
                    ConsoleExtensions.Write(">");
                    ConsoleExtensions.SetColors(GetColor(_game.Hold), ConsoleColor.Black);
                    ConsoleExtensions.Write(GetBall(_game.Hold));
                    ConsoleExtensions.SetColors(ConsoleColor.White, ConsoleColor.Black);
                    ConsoleExtensions.Write("<");
                }
                else
                {
                    ConsoleExtensions.Write("    ");
                }
                
                for (int j = 0; j < _game.BeakerCapacity; j++)
                {
                    ConsoleExtensions.SetCursorPosition(OFFSET_X + (i * (OFFSET_X + 3)) + offsetX2 + 1,
                        OFFSET_Y + _game.BeakerCapacity - j + offsetY2);
                    ConsoleExtensions.SetColors(GetColor(_game[i, j]), ConsoleColor.Black);
                    ConsoleExtensions.Write(GetBall(_game[i, j]));
                }

                if(_game.BeakersCount > 7 && offsetY2 == 0 && _game.BeakersCount - i <= i + 2)
                {
                    offsetY2 = _game.BeakerCapacity + OFFSET_Y;
                    offsetX2 = -(OFFSET_X + (i * (OFFSET_X + 3))) - 3;
                }
            }
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
