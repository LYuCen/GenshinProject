using UnityEngine;

namespace Connect.Sync
{
    public struct PlayerSnapshot
    {
        public float receiveTime;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
    }
    
    public class SnapshotBuffer
    {
        // 缓冲区
        private const int MaxSnapshots = 32;
        // 渲染落后服务端 100ms
        public float InterpolationDelay = 0.1f;
        // 最多落后时长 200ms
        public float MaxExtrapolationTime = 0.2f;

        // 数组模拟缓冲队列
        private PlayerSnapshot[] _buffer = new PlayerSnapshot[MaxSnapshots];
        private int _head;
        private int _count;
        
        public void AddSnapshot(Vector3 pos, Quaternion rot, Vector3 vel)
        {
            if (_count > 0)
            {
                var prev = _buffer[(_head - 1 + MaxSnapshots) % MaxSnapshots];
                float dt = Time.time - prev.receiveTime;
                // 求速度
                if (dt > 0.001f)
                {
                    vel = (pos - prev.position) / dt;
                }
            }

            _buffer[_head] = new PlayerSnapshot()
            {
                receiveTime = Time.time,
                position = pos,
                rotation = rot,
                velocity = vel
            };

            _head = (_head + 1) % MaxSnapshots;
            if (_count < MaxSnapshots) _count++;
        }

        /// <summary>
        /// 每帧渲染时调用，根据渲染时间获取插值后的 Transform 数据
        /// </summary>
        /// <param name="renderTime"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        public bool TryGetInterpolated(float renderTime, out Vector3 pos, out Quaternion rot)
        {
            pos = default;
            rot = default;
            if (_count == 0) return false;

            if (_count == 1)
            {
                var only = _buffer[(_head - 1 + MaxSnapshots) % MaxSnapshots];
                float extrapTime = renderTime - only.receiveTime;

                // 太久没更新快照就冻结位置
                if (extrapTime > MaxExtrapolationTime)
                {
                    pos = only.position;
                    rot = only.rotation;
                    return true;
                }
            }

            int oldest = (_head - _count + MaxSnapshots) % MaxSnapshots;
            // 如果渲染时间早于最旧的快照，就使用最旧的快照
            if (renderTime <= _buffer[oldest].receiveTime)
            {
                pos = _buffer[oldest].position;
                rot = _buffer[oldest].rotation;
                return true;
            }

            for (int i = 0; i < _count - 1; i++)
            {
                int left = (oldest + i) % MaxSnapshots;
                int right = (oldest + i + 1) % MaxSnapshots;

                var l = _buffer[left];
                var r = _buffer[right];

                if (renderTime >= l.receiveTime && renderTime <= r.receiveTime)
                {
                    float range = r.receiveTime - l.receiveTime;
                    float t = range > 0.001f ? (renderTime - l.receiveTime) / range : 0f;
                    t = Mathf.Clamp01(t);
                    
                    pos = Vector3.Lerp(l.position, r.position, t);
                    rot = Quaternion.Slerp(l.rotation, r.rotation, t);
                    return true;
                }
            }

            // 渲染时间比最新快照还要晚，那就延续最新的快照
            var latest = _buffer[(_head - 1 + MaxSnapshots) % MaxSnapshots];
            float extraTime = renderTime - latest.receiveTime;
            if (extraTime > MaxExtrapolationTime) extraTime = MaxExtrapolationTime;
            pos = latest.position + latest.velocity * extraTime;
            rot = latest.rotation;
            return true;
        }
    }
}