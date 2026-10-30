using GameFSM.FSM;
using UnityEngine;

namespace GameFSM.LevelFSM.LocomotionFSM.InGround.StandLayer
{
    /// <summary>
    /// 移动站立冲刺状态
    /// </summary>
    public class MoveStandSprintState : AbstractLevelState<Locomotion>
    {
        public MoveStandSprintState(Locomotion manager, AbstractCondition<Locomotion> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new MoveStandWalkStateCondition(MyManager, "Walk"));
        }

        public override void OnEnterState()
        {
            MyManager.LevelSpeed = 4f;
            MyManager.IsSprint = true;
        }

        public override void OnUpdateState()
        {
            if (!MyManager.IsMoveForward) MyManager.IsSprint = false;
            if (MyManager.IsAiming) MyManager.IsSprint = false;
        }

        public override void OnExitState()
        {
            
        }
    }
}
