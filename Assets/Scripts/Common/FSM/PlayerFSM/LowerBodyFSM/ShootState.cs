using UnityEngine;

namespace Common.FSM.PlayerFSM.LowerBodyFSM
{
    public class ShootState : AbstractState<PlayerManager>
    {
        public ShootState(PlayerManager manager) : base(manager)
        {
            Conditions.Add(new IdleCondition(MyManager,"Idle"));
            Conditions.Add(new WalkCondition(MyManager,"Walk"));
            Conditions.Add(new ShootWalkCondition(MyManager,"ShootWalk"));
        }

        public override void OnEnterState()
        {
            MyManager.speed = 0.4f;
            MyManager.PlayAnimController.RightHandIk = 1;
            MyManager.aimIK.enabled = true;
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
                Time.deltaTime * 10f);
        }

        public override void OnExitState()
        {
            MyManager.PlayAnimController.RightHandIk = 0;
            float targetYaw = MyManager.cameraController.Yaw;
            
            Vector3 forward = Quaternion.Euler(0, targetYaw, 0) * Vector3.forward;
            
            Quaternion targetRot = Quaternion.LookRotation(forward);
            MyManager.transform.rotation = targetRot;
        }
    }
}