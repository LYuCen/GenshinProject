using GameFSM.FSM;
using GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InAir;
using GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.GlobalInterruptState
{
    /// <summary>
    /// 玩家全局打断层
    /// </summary>
    public class GlobalInterruptLayer : AbstractMixLevelState<PlayerManager>
    {
        public GlobalInterruptLayer(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new PlayerGroundLayerCondition(MyManager, "Ground"));
            AddCondition(new PlayerAirLayerCondition(MyManager, "Air"));
        }

        public override bool CheckConditions(out string nextStateName)
        {
            if (MyManager.HpModule.IsDead())
            {
                nextStateName = string.Empty;
                return false;
            }
            return base.CheckConditions(out nextStateName);
        }

        protected override void BuildState()
        {
            States.Add("Dead", new PlayerDeadState(MyManager, new PlayerDeadCondition(MyManager, "Dead")));
            IsBuilt = true;
        }
    }
}
