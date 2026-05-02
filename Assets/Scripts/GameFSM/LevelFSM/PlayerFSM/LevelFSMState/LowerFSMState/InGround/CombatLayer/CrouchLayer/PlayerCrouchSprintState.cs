using Common.LevelFSM;
using GameFSM.FSM;
using UnityEngine;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer.CrouchLayer
{
    /// <summary>
    /// 玩家蹲姿冲刺状态
    /// </summary>
    public class PlayerCrouchSprintState : AbstractLevelState<PlayerManager>
    {
        public PlayerCrouchSprintState(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new PlayerCrouchWalkStateCondition(MyManager, "Walk"));
        }
        
        public override void OnEnterState()
        {
            MyManager.speed = 3.5f;
            MyManager.gunCube.SetParent(MyManager.hand);
            MyManager.aimIkWeight = 0;
            MyManager.stateName = "奔跑";
        }

        public override void OnUpdateState()
        {
            float targetYaw = MyManager.cameraController.Yaw;
            
            // Quaternion.Euler 为旋转矩阵  Quaternion.Euler(0, targetYaw, 0) 是向量为 y轴旋转 targetYaw度
            Vector3 forward = Quaternion.Euler(0, targetYaw, 0) * Vector3.forward;
            Vector3 right   = Quaternion.Euler(0, targetYaw, 0) * Vector3.right;

            Vector3 move =
                forward * MyManager.InputMove.y +
                right   * MyManager.InputMove.x;

            MyManager.characterController.Move(
                move.normalized * (MyManager.speed * Time.deltaTime)
            );
            
            
        }

        public override void OnExitState()
        {
        }
    }
}