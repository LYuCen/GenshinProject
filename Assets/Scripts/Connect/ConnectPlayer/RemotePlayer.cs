using UnityEngine;

namespace Connect.ConnectPlayer
{
    public class RemotePlayer : MonoBehaviour
    {
        private Vector3 _lastServerPos;      // 上一次服务器发来的位置
        private Vector3 _currentVelocity;    // 当前预测的速度
        private bool _isNewPacket = false;   // 标记：本帧是否有新包到达

        // 这里的参数建议增加一个 velocity，效果最好
        public void SetTargetPosition(Vector3 newPos, Vector3 velocity)
        {
            _lastServerPos = newPos;
            _currentVelocity = velocity;
            _isNewPacket = true; // 只要这个函数被调用，说明收到了新包
        }

        private void Update()
        {
            if (_isNewPacket)
            {
                // --- 逻辑 A：收到新包时的处理 ---
                
                // 1. 位置校正：不要瞬移，而是向真实位置挪动一点点（纠偏）
                // 0.3f 是纠偏力度，值越大拉扯感越强但越准，越小越顺滑但延迟感强
                transform.position = Vector3.Lerp(transform.position, _lastServerPos, 0.3f);
                
                // 2. 消耗掉这个包标记
                _isNewPacket = false;
            }
            else
            {
                // --- 逻辑 B：没收到新包时的处理（外推预测） ---
                
                // 根据上一次记录的速度，让物体继续按惯性跑
                // 这样即使网络卡了 100ms，物体也不会停下，而是继续匀速前进
                transform.position += _currentVelocity * Time.deltaTime;
            }

            // 可选：如果速度很小，强制归零防止漂移
            if (_currentVelocity.magnitude < 0.01f)
            {
                _currentVelocity = Vector3.zero;
            }
        }
    }
}