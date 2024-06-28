using CCSG.Utils;

namespace CCSG.Core
{
    public class Game
    {
        private int[,] _beakers;
        private int[] _unknownBeakers;

        private int _hold;
        private int _holdPosition;

        private int _completedCount = 0;
        private (int from, int to) _historyBuffer;
        private Stack<(int from, int to)> _history;
    
        public int Hold => _hold;
        public int HoldPosition => _holdPosition;
        public int BeakersCount => _beakers.GetLength(0);
        public int BeakerCapacity => _beakers.GetLength(1);
        public int TargetFilledCount => BeakersCount - Consts.ADDITIONAL_BEAKERS;
        public bool IsCompleted => _completedCount == TargetFilledCount;
        public int CompletedCount => _completedCount;

        public const int UNKNOWN_SYMBOL = 999;
        public int this[int x, int y]
        {
            get => y < _unknownBeakers[x] ? UNKNOWN_SYMBOL : _beakers[x, y];
        }

        public Game(int count, int capcity, bool bottomUnknown)
        {
            _history = new Stack<(int from, int to)>();
            Init(count, capcity, bottomUnknown);
        }

        private void Init(int count, int capcity, bool bottomUnknown)
        {
            _beakers = new int[count, capcity];
            _unknownBeakers = new int[count];
            var circlesBuffer = new int[TargetFilledCount * capcity];

            int n = 0;
            for(int i = 0; i < TargetFilledCount; i++)
            {
                for(int j = 0; j < BeakerCapacity; j++)
                {
                    circlesBuffer[n] = i + 1;
                    n++;
                }
            }

            Extensions.Shuffle(circlesBuffer);

            n = 0;
            for(int i = 0; i < TargetFilledCount; i++)
            {
                _unknownBeakers[i] = bottomUnknown ? BeakerCapacity - 1 : 0;
                for (int j = 0; j < BeakerCapacity; j++)
                {
                    _beakers[i, j] = circlesBuffer[n];
                    n++;
                }
            }
        }

        public bool TryTake(int beakerIndex, bool writeToHistory = true)
        {
            if(_hold != 0)
            {
                return false;
            }

            int? index = GetHeighestInBeaker(beakerIndex);
            if (index.HasValue)
            {
                _hold = _beakers[beakerIndex, index.Value];
                _beakers[beakerIndex, index.Value] = 0;
                _holdPosition = beakerIndex;
                if(writeToHistory)
                {
                    _historyBuffer.from = beakerIndex;
                }
                return true;
            }
            
            return false;
        }

        public bool TryPut(int beakerIndex, bool writeToHistory = true, bool force = false)
        {
            if(_hold == 0)
            {
                return false;
            }

            int? index = GetHeighestInBeaker(beakerIndex);
            if (index.GetValueOrDefault(0) != BeakerCapacity - 1 
                && (force == true || _holdPosition == beakerIndex 
                || index.HasValue == false || _beakers[beakerIndex, index.Value] == _hold))
            {
                _beakers[beakerIndex, index.GetValueOrDefault(-1) + 1] = _hold;
                _hold = 0;
                TryComplete();
                if(_historyBuffer.from != beakerIndex && writeToHistory)
                {
                    _unknownBeakers[_historyBuffer.from] = 
                        Math.Min(GetHeighestInBeaker(_historyBuffer.from) ?? 0, _unknownBeakers[_historyBuffer.from]);
                    
                    _historyBuffer.to = beakerIndex;
                    _history.Push(_historyBuffer);
                }
                return true;
            }

            return false;
        }

        public void Undo()
        {
            if (_history.TryPop(out var lastMove))
            {
                TryTake(lastMove.to, false);
                TryPut(lastMove.from, false, true);
            }
        }

        public int? GetHeighestInBeaker(int beakerIndex)
        {
            int? index = null;
            for(int i = 0; i < BeakerCapacity; i++)
            {
                if (_beakers[beakerIndex, i] == 0)
                {
                    break;
                }
                index = i;
            }

            return index;
        }

        private void TryComplete()
        {
            _completedCount = 0;
            for (int i = 0; i < BeakersCount; i++)
            {
                var isCompleted = true;
                for(int j = 0; j < BeakerCapacity - 1; j++)
                {
                    if (_beakers[i, j] != _beakers[i, j + 1] || _beakers[i, j] == 0)
                    {
                        isCompleted = false;
                        break;
                    }
                }

                if(isCompleted)
                {
                    _completedCount++;
                }
            }
        }
    }
}
