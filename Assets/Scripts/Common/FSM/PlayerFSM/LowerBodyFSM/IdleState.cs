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
            MyManager.aimIK.enabled = false;
            MyManager.gunCube.localPosition = new Vector3(0.000118f, 0.001919f, 0.001026f);
            MyManager.gunCube.localEulerAngles = new Vector3(283.742096f, 323.333099f, 229.175308f);
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