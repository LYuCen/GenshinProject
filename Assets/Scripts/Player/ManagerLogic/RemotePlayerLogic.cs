using Connect.Converter;
using Connect.DTO;
using GameFSM.FSM.PlayerFSM;
using UnityEngine;

namespace Player.ManagerLogic
{
    public class RemotePlayerLogic : IPlayerLogic
    {
        private PlayerManager _m;
        private Vector3 _targetPos;
        
        // --- 预测相关变量 ---
        private Vector3 _lastServerPos;      // 上一帧服务器发来的位置
        private Vector3 _currentVelocity;    // 当前预测的速度
        private Quaternion _targetRot;       // 目标旋转
        private float _lastPacketTime;       // 上次收到包的时间
        private bool _isFirstPacket = true;  // 是否是第一个包

        // 最大纠偏距离：如果预测位置离真实位置太远（比如超过2米），直接瞬移，不插值
        private const float MaxSnapDistance = 2.0f; 

        public void Init(PlayerManager manager)
        {
            _m = manager;
            _lastPacketTime = Time.time;
            GameObject go = new GameObject("Aim");
            _m.aim = go.transform;
            _m.aimIK.solver.target = _m.aim;
        }

        // 关键：由外部 (PlayerPool) 调用，填入从服务器收到的 PlayerStateData
        public void SyncFromServer(PlayerDataDto dto)
        {
            PlayerDataConverter.FillData(dto, _m.state);
            Vector3 newPos = _m.state.position;
            _targetRot = _m.state.rotation;

            if (_isFirstPacket)
            {
                _m.transform.position = newPos;
                _m.transform.rotation = _targetRot;
                _lastServerPos = newPos;
                _isFirstPacket = false;
                return;
            }

            // 1. 计算预测速度 (Velocity = Displacement / Time)
            // 注意：这里最好使用服务器真实的间隔时间，如果没有，可以用我们的平均发包间隔 0.05s
            float timeSinceLastPacket = Time.time - _lastPacketTime;
            // 限制一下分母，防止除以0或极小值导致速度飙升
            timeSinceLastPacket = Mathf.Max(timeSinceLastPacket, 0.01f);

            _currentVelocity = (newPos - _lastServerPos) / timeSinceLastPacket;
            
            // 2. 更新基础参考位
            _lastServerPos = newPos;
            _lastPacketTime = Time.time;
            
            // 更新 aimPosition
            _m.aim.position = new Vector3(dto.ax, dto.ay, dto.az);
            
            // 更新 IK
            _m.aimIK.enabled = _m.state.isShoot;

            // 4. 距离过大补偿 (防止预测出错导致玩家穿墙太远)
            if (Vector3.Distance(_m.transform.position, newPos) > MaxSnapDistance)
            {
                _m.transform.position = newPos;
            }
        }

        public void OnUpdate()
        {
            // --- 预测移动逻辑 ---

            // A. 基础移动：按惯性（速度）一直跑
            // 这样即便网络卡了 100ms 没发包，由于速度还在，模型会继续匀速前进
            Vector3 predictionMove = _currentVelocity * Time.deltaTime;
            
            // B. 纠偏：向服务器最新位置做轻微拉动
            // 这步很关键。预测位置和服务器真实位置总有误差，我们每帧都往服务器位置“挪”一点点。
            // 0.1f 是纠偏力度，值越小越平滑，值越大越精准但可能会有轻微抖动。
            Vector3 correction = (_lastServerPos - _m.transform.position) * 0.15f;

            // 应用位置更新
            _m.transform.position += predictionMove + correction;

            // C. 旋转插值
            _m.transform.rotation = Quaternion.Slerp(_m.transform.rotation, _targetRot, Time.deltaTime * 15f);

            // D. 动画更新
            _m.AnimController.Update();
            
            // E. 预测衰减（可选）
            // 如果很长时间（比如超过 200ms）没收到新包，说明玩家可能停了或者掉线了，让速度衰减，防止一直跑出地图
            if (Time.time - _lastPacketTime > 0.2f)
            {
                _currentVelocity = Vector3.Lerp(_currentVelocity, Vector3.zero, Time.deltaTime * 5f);
            }
        }


        public void OnFixedUpdate() { }
        public void OnLateUpdate() { }
    }
}