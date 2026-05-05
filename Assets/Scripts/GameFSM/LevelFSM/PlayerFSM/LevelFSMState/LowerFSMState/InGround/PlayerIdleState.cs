using GameFSM.FSM;
using GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround
{
    /// <summary>
    /// 玩家待机节点
    /// </summary>
    public class PlayerIdleState : AbstractLevelState<PlayerManager>
    {
        public PlayerIdleState(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new PlayerCombatLayerCondition(MyManager, "Combat"));
        }
        
        public override void OnEnterState()
        {
            MyManager.gunCube.SetParent(MyManager.hand);
            MyManager.speed = 0f;
            MyManager.rHeadIkWeight = !MyManager.IsReload ? 1 : 0;
        }
    }
}