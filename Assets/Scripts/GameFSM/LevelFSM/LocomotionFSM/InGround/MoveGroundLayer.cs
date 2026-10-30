using GameFSM.FSM;
using GameFSM.LevelFSM.LocomotionFSM.InAir;
using GameFSM.LevelFSM.LocomotionFSM.InGround.CrouchLayer;
using GameFSM.LevelFSM.LocomotionFSM.InGround.StandLayer;
using UnityEngine;

namespace GameFSM.LevelFSM.LocomotionFSM.InGround
{
    /// <summary>
    /// 移动地面状态层
    /// </summary>
    public class MoveGroundLayer : AbstractMixLevelState<Locomotion>
    {
        public MoveGroundLayer(Locomotion manager, AbstractCondition<Locomotion> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new MoveAirLayerCondition(MyManager, "Air"));
        }

        protected override void BuildState()
        {
            States.Add("Idle", new MoveIdleState(MyManager, new MoveIdleStateCondition(MyManager, "Idle")));
            States.Add("Crouch", new MoveCrouchLayer(MyManager, new MoveCrouchLayerCondition(MyManager, "Crouch")));
            States.Add("Stand", new MoveStandLayer(MyManager, new MoveStandLayerCondition(MyManager, "Stand")));
            IsBuilt = true;
        }

        public override void OnUpdateState()
        {
            if (MyManager.CrouchDown)
            {
                MyManager.IsCrouch = !MyManager.IsCrouch;
                MyManager.CrouchDown = false;
            }

            if (MyManager.SprintDown && MyManager.IsMoveForward && !MyManager.IsAiming)
            {
                MyManager.IsSprint = true;
            }
            
            Vector3 forward = Quaternion.Euler(0, MyManager.Yaw, 0) * Vector3.forward;
            Vector3 right   = Quaternion.Euler(0, MyManager.Yaw, 0) * Vector3.right;
            MyManager.MoveVec = forward * MyManager.InputMoveVec.y + right * MyManager.InputMoveVec.x;
        }
    }
}
