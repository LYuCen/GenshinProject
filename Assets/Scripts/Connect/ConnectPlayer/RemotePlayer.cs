using Connect.Sync;
using GameFSM.LevelFSM.PlayerFSM;
using UnityEngine;

namespace Connect.ConnectPlayer
{
    public class RemotePlayer : MonoBehaviour
    {
        private SnapshotBuffer _buffer = new SnapshotBuffer();
        private PlayerManager _manager;
        public float InterpolationDelay
        {
            get => _buffer.InterpolationDelay;
            set => _buffer.InterpolationDelay = value;
        }

        /// <summary>
        /// 收到服务端更新时调用
        /// </summary>
        public void OnServerUpdate(Vector3 pos, Quaternion rot, Vector2 inputMove, Vector3 aimPos)
        {
            _buffer.AddSnapshot(pos, rot, Vector3.zero);

            if (!_manager) _manager = GetComponent<PlayerManager>();
            
            _manager.state.inputMove = inputMove;
            _manager.state.aimPosition = aimPos;
            _manager.MoveModule?.SyncFromState(_manager.state);  
            _manager.IkModule?.SetAimTargetPosition(aimPos);
        }

        private void Update()
        {
            float renderTime = Time.time - _buffer.InterpolationDelay;
            if (_buffer.TryGetInterpolated(renderTime, out var pos, out var rot))
            {
                transform.position = pos;
                transform.rotation = rot;
            }
        }
    }
}