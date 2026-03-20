using Common.FSM;
using Common.LevelFSM.EnemyFSM.LevelFSMState.CombatLayer;
using Common.LevelFSM.EnemyFSM.LevelFSMState.GlobalInterruptLayer;
using Common.LevelFSM.EnemyFSM.LevelFSMState.PatrolLayer;
using UnityEngine;

namespace Common.LevelFSM.EnemyFSM.LevelFSMState.ToCoverLayer
{
    /// <summary>
    /// 撤退状态层
    /// </summary>
    public class EnemyRetreatLayer : AbstractMixLevelState<EnemyManager>
    {
        public EnemyRetreatLayer(EnemyManager manager, AbstractCondition<EnemyManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new EnemyPatrolLayerCondition(MyManager,"PatrolLayer"));
            AddCondition(new EnemyGlobalInterruptLayerCondition(MyManager, "GlobalInterruptLayer"));
            AddCondition(new EnemyCombatLayerCondition(MyManager, "Combat"));
        }

        protected override void BuildState()
        {
            States.Add("CoverIdle", new CoverIdleState(MyManager, new CoverIdleCondition(MyManager, "CoverIdle")));
            States.Add("FindCover", new FindCoverState(MyManager, new FindCoverCondition(MyManager, "FindCover")));
            States.Add("RetreatCombat",
                new RetreatCombatState(MyManager, new RetreatCombatCondition(MyManager, "RetreatCombat")));
            IsBuilt = true;
        }

        public override void OnEnterState()
        {
            MyManager.stateStr = "撤退状态";
            MyManager.GridEqsController.enable = true;
        }

        public override void OnUpdateState()
        {
            Vector3 v3 = new Vector3(0, 1.5f, 0);
            var points = MyManager.GridEqsController.GetPoints();
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].IsValid && !MyManager.GridEqsController.findCover)
                {
                    MyManager.GridEqsController.findCover = Physics.Linecast(points[i].Position + v3,
                        MyManager.GridEqsController.target.position + v3, LayerMask.GetMask("Wall"));
                }
            }
            base.OnUpdateState();
        }

        public override void OnExitState()
        {
            MyManager.GridEqsController.enable = false;
        }
    }
}