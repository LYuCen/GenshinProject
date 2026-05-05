using GameFSM.FSM;
using UnityEngine;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.UpperFSMState
{
    /// <summary>
    /// 玩家瞄准状态
    /// </summary>
    public class PlayerAimState : AbstractLevelState<PlayerManager>
    {
        public PlayerAimState(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new PlayerReloadStateCondition(MyManager, "Reload"));
            AddCondition(new PlayerThrowStateCondition(MyManager,  "Throw"));
            AddCondition(new PlayerEmptyStateCondition(MyManager, "Empty"));
        }
        
        public override void OnEnterState()
        {
            MyManager.moveSpeedWeight = 0.9f;
            MyManager.aimIkWeight = !MyManager.IsThrow && !MyManager.IsReload ? 1 : 0;
        }

        public override void OnUpdateState()
        {
            float targetYaw = MyManager.cameraController.Yaw;
            
            Vector3 forward = Quaternion.Euler(0, targetYaw, 0) * Vector3.forward;
            
            Quaternion targetRot = Quaternion.LookRotation(forward);
            MyManager.transform.rotation = Quaternion.Slerp(
                MyManager.transform.rotation,
                targetRot,
                Time.deltaTime * 20f);
        }

        public override void OnExitState()
        {
            float targetYaw = MyManager.cameraController.Yaw;
            
            Vector3 forward = Quaternion.Euler(0, targetYaw, 0) * Vector3.forward;
            
            Quaternion targetRot = Quaternion.LookRotation(forward);
            MyManager.transform.rotation = targetRot;
            MyManager.moveSpeedWeight = 1f;
            MyManager.aimIkWeight = 0;
        }
    }
}