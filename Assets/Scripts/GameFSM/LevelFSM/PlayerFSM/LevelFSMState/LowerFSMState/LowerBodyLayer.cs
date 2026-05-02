using Common.LevelFSM;
using GameFSM.FSM;
using GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InAir;
using GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround;
using UnityEngine;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState
{
    /// <summary>
    /// 角色下半身状态层
    /// </summary>
    public class LowerBodyLayer : AbstractMixLevelState<PlayerManager>
    {
        public LowerBodyLayer(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
        }

        protected override void BuildState()
        {
            States.Add("Ground", new PlayerGroundLayer(MyManager, new PlayerGroundLayerCondition(MyManager, "Ground")));
            States.Add("Air", new PlayerAirLayer(MyManager, new PlayerAirLayerCondition(MyManager, "Air")));
            IsBuilt = true;
        }

        public override void OnEnterState()
        {
            MyManager.gunCube.localPosition = new Vector3(0.000118f, 0.001919f, 0.001026f);
            MyManager.gunCube.localEulerAngles = new Vector3(283.742096f, 323.333099f, 229.175308f);
        }
    }
}