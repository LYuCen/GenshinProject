using System.Collections.Generic;
using Connect.Converter;

namespace Server
{
    public class PlayerInputBuffer
    {
        private SortedDictionary<uint, InputDataConverter.InputSample> _pending = new();
        private Queue<InputDataConverter.InputSample> _ready = new();
        private uint _nextTick = 1;
        private InputDataConverter.InputSample _lastSample = new InputDataConverter.InputSample
        {
            InputX = 127, // 摇杆居中 (0+1)*127
            InputY = 127,
        };
        

        public int ReadyCount => _ready.Count;

        // 收到网络包时调用，将网络包加入消费队列
        public void Enqueue(InputDataConverter.InputSample samples)
        {
            if (_pending.Count == 0 && _ready.Count == 0) _nextTick = samples.Tick;
            
            _pending[samples.Tick] = samples;

            while (_pending.TryGetValue(_nextTick, out var next))
            {
                _ready.Enqueue(next);
                _pending.Remove(_nextTick);
                _nextTick++;
            }
        }

        private const int MinBuffer = 0;

        // 将数据进行消费，在 fixUpdate 里调用
        public bool TryDequeue(out InputDataConverter.InputSample sample)
        {
            if (_ready.Count > MinBuffer)
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