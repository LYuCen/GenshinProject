using Common.LevelFSM;
using GameFSM.FSM;
using UnityEngine;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer.StandLayer
{
    /// <summary>
    /// 玩家站立行走状态
    /// </summary>
    public class PlayerStandWalkState : AbstractLevelState<PlayerManager>
    {
        public PlayerStandWalkState(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new PlayerStandSprintStateCondition(MyManager, "Sprint"));
        }
        
        public override void OnEnterState()
        {
            MyManager.speed = 1.6f;
            MyManager.gunCube.SetParent(MyManager.hand);
            MyManager.aimIkWeight = 0;
            MyManager.stateName = "移动";
        }

        public override void OnUpdateState()
        {
            float targetYaw = MyManager.cameraController.Yaw;
            
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