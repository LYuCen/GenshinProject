using UnityEngine;

namespace GameFSM.FSM.PlayerFSM.LowerBodyFSM
{
    public class ShootState : AbstractState<PlayerManager>
    {
        public ShootState(PlayerManager manager) : base(manager)
        {
            AddCondition(new IdleCondition(MyManager,"Idle"));
            AddCondition(new WalkCondition(MyManager,"Walk"));
            AddCondition(new ShootWalkCondition(MyManager,"ShootWalk"));
        }

        public override void OnEnterState()
        {
            MyManager.speed = 0.4f;
            MyManager.aimIkWeight = !MyManager.IsThrow && !MyManager.IsReload ? 1 : 0;
            MyManager.stateName = "据枪待机";
        }

        public override void OnUpdateState()
        {
            float targetYaw = MyManager.cameraController.Yaw;
            
            Vector3 forward = Quaternion.Euler(0, targetYaw, 0) * Vector3.forward;
            
            Quaternion targetRot = Quaternion.LookRotation(forward);
            MyManager.transform.rotation = Quaternion.Slerp(
                MyManager.transform.rotation,
                targetRot,
                Time.deltaTime * 20f);
        }

        public override void OnExitState()
        {
            float targetYaw = MyManager.cameraController.Yaw;
            
            Vector3 forward = Quaternion.Euler(0, targetYaw, 0) * Vector3.forward;
            
            Quaternion targetRot = Quaternion.LookRotation(forward);
            MyManager.transform.rotation = targetRot;
        }
    }
}