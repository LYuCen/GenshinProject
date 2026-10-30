using GameFSM.FSM;
using GameFSM.LevelFSM.LocomotionFSM.InGround.CrouchLayer;
using GameFSM.LevelFSM.LocomotionFSM.InGround.StandLayer;

namespace GameFSM.LevelFSM.LocomotionFSM.InGround
{
    /// <summary>
    /// 移动待机节点
    /// </summary>
    public class MoveIdleState : AbstractLevelState<Locomotion>
    {
        public MoveIdleState(Locomotion manager, AbstractCondition<Locomotion> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new MoveStandLayerCondition(MyManager, "Stand"));
            AddCondition(new MoveCrouchLayerCondition(MyManager, "Crouch"));
        }

        public override void OnEnterState()
        {
            MyManager.LevelSpeed = 0f;
            MyManager.IsSprint = false;
        }

        public override void OnUpdateState()
        {
            if (MyManager.JumpDown)
            {
                if (MyManager.IsCrouch)
                {
                    MyManager.IsCrouch = false;
                }
                else
                {
                    MyManager.IsJump = true;
                    MyManager.VerticalSpeed = 4f;
                }
                MyManager.JumpDown = false;
            }
            
        }
    }
}
