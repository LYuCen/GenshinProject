using UnityEngine;

namespace Connect.HitRewind
{
    public class HitHistoryBuffer
    {
        private int _head;
        private int _count;
        private int _size;
        private HitSnapshot[] _queue;

        public HitHistoryBuffer(int size = 10)
        {
            _size = size;
            _head = 0;
            _count = 0;
            _queue = new HitSnapshot[size];
        }

        public void Add(HitSnapshot add)
        {
            var copy = new HitSnapshot
            {
                ServerTick = add.ServerTick,
                Shapes = new HitShape[add.Shapes.Length]     
            };
            System.Array.Copy(add.Shapes, copy.Shapes, add.Shapes.Length);
            _queue[_head] = copy;
            _head = (_head + 1) % _size;
            if (_count < _size) _count++;

        }

        public bool FindClosest(uint targetTick, out HitSnapshot result)
        {
            result = default;
            if (_count == 0) return false;
            
            int l = (_head - _count + _size) % _size;        
            result = _queue[l];
            for (int i = 0; i < _count; i++)
            {
                int idx = (l + i) % _size;
                if (TickDisk(result.ServerTick, targetTick) > TickDisk(_queue[idx].ServerTick, targetTick))
                {
                    result = _queue[idx];
                }
            }
            return true;
        }

        private long TickDisk(uint a, uint b)
        {
            return a >= b ? (long)(a - b) : (long)(b - a);
        }
    }
}