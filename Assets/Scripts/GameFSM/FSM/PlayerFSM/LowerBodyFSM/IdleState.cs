using UnityEngine;

namespace GameFSM.FSM.PlayerFSM.LowerBodyFSM
{
    public class IdleState : AbstractState<PlayerManager>
    {
        
        public IdleState(PlayerManager manager) : base(manager)
        { 
            AddCondition(new WalkCondition(MyManager,"Walk"));
            AddCondition(new JumpCondition(MyManager,"Jump"));
            AddCondition(new ShootCondition(MyManager, "Shoot"));
            AddCondition(new RunCondition(MyManager, "Run"));
            
        }

        public override void OnEnterState()
        {
            MyManager.gunCube.SetParent(MyManager.hand);
            MyManager.aimIkWeight = 0;
            MyManager.rHeadIkWeight = !MyManager.IsReload ? 1 : 0;
            MyManager.gunCube.localPosition = new Vector3(0.000118f, 0.001919f, 0.001026f);
            MyManager.gunCube.localEulerAngles = new Vector3(283.742096f, 323.333099f, 229.175308f);
            MyManager.stateName = "待机";
        }

        public override void OnUpdateState()
        {
            
        }

        public override void OnExitState()
        {
            
        }
    }
}