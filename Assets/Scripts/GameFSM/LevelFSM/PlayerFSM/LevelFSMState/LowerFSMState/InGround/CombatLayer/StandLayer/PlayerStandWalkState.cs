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
            MyManager.gunCube.SetParent(MyManager.hand);
        }

        public override void OnUpdateState()
        {
            
        }

        public override void OnExitState()
        {
        }
    }
}