using UnityEngine;

namespace Common.FSM.PlayerFSM.LowerBodyFSM
{
    public class ShootWalkState : AbstractState<PlayerManager>
    {
        public ShootWalkState(PlayerManager manager) : base(manager)
        {
            Conditions.Add(new WalkCondition(MyManager,"Walk"));
            Conditions.Add(new ShootCondition(MyManager,"Shoot"));
            Conditions.Add(new RunCondition(MyManager,"Run"));
        }

        public override void OnEnterState()  
        {
            MyManager.speed = 1.6f;
            MyManager.PlayAnimController.LeftHandIk = 1;
            MyManager.PlayAnimController.RightHandIk = 1;
            MyManager.stateName = "据枪移动";
        }

        public override void OnUpdateState()
        {
            MyManager.PlayAnimController.LeftHandIk = 1;
            MyManager.PlayAnimController.RightHandIk = 1;
            float targetYaw = MyManager.cameraController.Yaw;
            
            Vector3 forward = Quaternion.Euler(0, targetYaw, 0) * Vector3.forward;
            Vector3 right   = Quaternion.Euler(0, targetYaw, 0) * Vector3.right;

            Vector3 move =
                forward * MyManager.inputMove.y +
                right   * MyManager.inputMove.x;
            
            if (move.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(forward);
                MyManager.transform.rotation = Quaternion.Slerp(
                    MyManager.transform.rotation,
                    targetRot,
                    Time.deltaTime * 10f
                );
            }

            MyManager.characterController.Move(
                move.normalized * (MyManager.speed * Time.deltaTime)
            );
        }

        public override void OnExitState()
        {
            MyManager.PlayAnimController.RightHandIk = 0;
        }
    }
}