using UnityEngine;

namespace Common.FSM.PlayerFSM.LowerBodyFSM
{
    public class IdleState : AbstractState<PlayerManager>
    {
        
        public IdleState(PlayerManager manager) : base(manager)
        { 
            Conditions.Add(new WalkCondition(MyManager,"Walk"));
            Conditions.Add(new JumpCondition(MyManager,"Jump"));
            Conditions.Add(new ShootCondition(MyManager, "Shoot"));
            
        }

        public override void OnEnterState()
        {
            MyManager.PlayAnimController.LeftHandIk = 1;
            MyManager.PlayAnimController.RightHandIk = 0;
            MyManager.gunCube.SetParent(MyManager.hand);
            MyManager.gunCube.localPosition = new Vector3(0.00016f, 0.00203f, 0.00078f);
            MyManager.gunCube.localEulerAngles = new Vector3(-90, 180, 0);
            MyManager.stateName = "待机";
        }

        public override void OnUpdateState()
        {
            
        }

        public override void OnExitState()
        {
            MyManager.PlayAnimController.LeftHandIk = 0;
        }
    }
}