using GameFSM.FSM;
using GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer;
using mEnum.Flags;
using mEnum.Query;

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
            WeaponQuery query = MyManager.CombatModule.GetQuery();
            MyManager.gunCube.SetParent(MyManager.hand);
            MyManager.MoveModule.SetSpeed(0f);
            MyManager.IkModule.SetHandIk(!query.HasState(WeaponFlags.Reload) ? 1 : 0);
            MyManager.IkModule.SetAimIk(query.HasState(WeaponFlags.Reload) ? 0 : 1);
        }
    }
}