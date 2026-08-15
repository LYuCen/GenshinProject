using System.Collections.Generic;
using Connect.Converter;

namespace Server
{
    public class PlayerInputBuffer
    {
        private SortedDictionary<uint, InputDataConverter.InputSample[]> _pending = new();
        private Queue<InputDataConverter.InputSample> _ready = new();
        private uint _nextSequence = 1;
        private InputDataConverter.InputSample _lastSample = new InputDataConverter.InputSample
        {
            InputX = 127, // 摇杆居中 (0+1)*127
            InputY = 127,
        };

        public int ReadyCount => _ready.Count;

        // 收到网络包时调用，将网络包加入消费队列
        public void Enqueue(uint sequence, InputDataConverter.InputSample[] samples)
        {
            if (_pending.Count == 0 && _ready.Count == 0)
            {
                _nextSequence = sequence;
            }
            if (sequence < _nextSequence) return; // 过期包丢弃

            _pending[sequence] = samples;

            while (_pending.TryGetValue(_nextSequence, out var next))
            {
                foreach (var s in next)
                {
                    _ready.Enqueue(s);
                }

                _pending.Remove(_nextSequence);
                _nextSequence++;
            }
        }

        private const int MinBuffer = 0;

        // 将数据进行消费，在 fixUpdate 里调用
        public bool TryDequeue(out InputDataConverter.InputSample sample)
        {
            if (_ready.Count >= MinBuffer)
            {
                sample = _ready.Dequeue();
                _lastSample = sample;
                return true;
            }

            sample = default;
            return false;
        }
    }
}