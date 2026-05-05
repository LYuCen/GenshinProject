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
        }

        public override void OnUpdateState()
        {
            MyManager.IsSprint = MyManager.InputMove == Vector2.up || MyManager.InputMove == new Vector2(-1, 1) ||
                                   MyManager.InputMove == new Vector2(1, 1);
        }

        public override void OnExitState()
        {
        }
    }
}