using Common.LevelFSM;
using GameFSM.FSM;
using UnityEngine;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer.StandLayer
{
    /// <summary>
    /// 玩家站立冲刺状态
    /// </summary>
    public class PlayerStandSprintState : AbstractLevelState<PlayerManager>
    {
        public PlayerStandSprintState(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new PlayerStandWalkStateCondition(MyManager, "Walk"));
        }
        
        public override void OnEnterState()
        {
            MyManager.speed = 4f;
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
            
            // sqrMagnitude向量长度的平方比 magnitude 求向量长度不需要开平方，性能更好
            if (move.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(move);
                MyManager.transform.rotation = Quaternion.Slerp(
                    MyManager.transform.rotation,
                    targetRot,
                    Time.deltaTime * 10f
                );
            }

            MyManager.characterController.Move(
                move.normalized * (MyManager.speed * Time.deltaTime)
            );
        }

        public override void OnExitState()
        {
        }
    }
}