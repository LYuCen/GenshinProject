using GameFSM.FSM;
using mEnum.Flags;
using mEnum.Query;
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
            AddCondition(new PlayerBoltStateCondition(MyManager, "Bolt"));
        }
        
        public override void OnEnterState()
        {
            WeaponQuery query = MyManager.CombatModule.GetQuery();
            MyManager.IkModule.SetAimIk(MyManager.GetAimIkTarget());
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
        }
    }
}