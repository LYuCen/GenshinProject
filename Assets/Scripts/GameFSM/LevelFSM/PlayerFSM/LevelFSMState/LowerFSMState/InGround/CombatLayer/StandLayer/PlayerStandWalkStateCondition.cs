using GameFSM.FSM;
using UnityEngine;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround.CombatLayer.StandLayer
{
    /// <summary>
    /// 玩家站立行走判定 (Walk)
    /// </summary>
    public class PlayerStandWalkStateCondition : AbstractCondition<PlayerManager>
    {
        public PlayerStandWalkStateCondition(PlayerManager manager, string nextStateName) : base(manager, nextStateName)
        {
        }

        public override bool CheckCondition(out string nextState)
        {
            nextState = "Walk";
            return !MyManager.IsSprint || 
                    !(MyManager.InputMove == Vector2.up || 
                      MyManager.InputMove == new Vector2(1, 1) || 
                      MyManager.InputMove == new Vector2(-1, 1));
        }
    }
}