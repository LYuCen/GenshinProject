using GameFSM.FSM;
using UnityEngine;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InAir
{
    public class PlayerJumpState : AbstractLevelState<PlayerManager>
    {
        public PlayerJumpState(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new PlayerFallStateCondition(MyManager, "Fall"));
        }

        public override void OnExitState()
        {
        }
    }
}