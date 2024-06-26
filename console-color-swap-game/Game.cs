using CCSG.Utils;

namespace CCSG.Core
{
    public class Game
    {
        private int[,] _beakers;

        private int _hold;
        private int _holdPosition;

        private bool _isCompleted = false;
    
        public int Hold => _hold;
        public int HoldPosition => _holdPosition;
        public int BeakersCount => _beakers.GetLength(0);
        public int BeakerCapacity => _beakers.GetLength(1);
        public int TargetFilledCount => BeakersCount - Consts.ADDITIONAL_BEAKERS;
        public bool IsCompleted => _isCompleted;

        public int this[int x, int y]
        {
            get => _beakers[x, y];
        }

        public Game(int count, int capcity)
        {
            Init(count, capcity);
        }

        private void Init(int count, int capcity)
        {
            _beakers = new int[count, capcity];
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
                for (int j = 0; j < BeakerCapacity; j++)
                {
                    _beakers[i, j] = circlesBuffer[n];
                    n++;
                }
            }
        }

        public bool TryTake(int beakerIndex)
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
                return true;
            }
            
            return false;
        }

        public bool TryPut(int beakerIndex)
        {
            if(_hold == 0)
            {
                return false;
            }

            int? index = GetHeighestInBeaker(beakerIndex);
            if (index.GetValueOrDefault(0) != BeakerCapacity - 1 
                && (_holdPosition == beakerIndex || index.HasValue == false || _beakers[beakerIndex, index.Value] == _hold))
            {
                _beakers[beakerIndex, index.GetValueOrDefault(-1) + 1] = _hold;
                _hold = 0;
                TryComplete();
                return true;
            }

            return false;
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
            for(int i = 0; i < TargetFilledCount; i++)
            {
                for(int j = 0; j < BeakerCapacity - 1; j++)
                {
                    if (_beakers[i, j] != _beakers[i, j + 1])
                    {
                        _isCompleted = false;
                        return;
                    }
                }
            }
            _isCompleted = true;
        }
    }
}
