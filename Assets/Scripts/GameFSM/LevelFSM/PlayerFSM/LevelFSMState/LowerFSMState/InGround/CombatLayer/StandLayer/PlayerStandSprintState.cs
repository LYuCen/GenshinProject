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