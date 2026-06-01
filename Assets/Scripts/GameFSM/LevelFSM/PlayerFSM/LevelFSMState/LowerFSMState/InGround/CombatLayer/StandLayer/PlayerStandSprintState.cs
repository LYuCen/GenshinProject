using GameFSM.FSM;
using mEnum.Flags;
using mEnum.Query;
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
            
            MyManager.MoveModule.SetSpeed(4f);
            MyManager.gunCube.SetParent(MyManager.hand);
            MyManager.IkModule.SetAimIk(MyManager.GetAimIkTarget());
        }

        public override void OnUpdateState()
        {
            MyManager.MoveModule.Sprint();
        }

        public override void OnExitState()
        {
            MyManager.MoveModule.Sprint();
            MyManager.IkModule.SetAimIk(MyManager.GetAimIkTarget());
        }
    }
}