using GameFSM.LevelFSM.PlayerFSM;
using UnityEngine;

namespace GameFSM.FSM.PlayerFSM.LowerBodyFSM
{
    public class ShootWalkState : AbstractState<PlayerManager>
    {
        public ShootWalkState(PlayerManager manager) : base(manager)
        {
            AddCondition(new WalkCondition(MyManager,"Walk"));
            AddCondition(new ShootCondition(MyManager,"Shoot"));
            AddCondition(new RunCondition(MyManager,"Run"));
        }

        public override void OnEnterState()  
        {
            MyManager.speed = 1.6f;
            MyManager.aimIkWeight = !MyManager.IsThrow && !MyManager.IsReload ? 1 : 0;
            MyManager.stateName = "据枪移动";
        }

        public override void OnUpdateState()
        {
            float targetYaw = MyManager.cameraController.Yaw;
            
            Vector3 forward = Quaternion.Euler(0, targetYaw, 0) * Vector3.forward;
            Vector3 right   = Quaternion.Euler(0, targetYaw, 0) * Vector3.right;

            Vector3 move =
                forward * MyManager.InputMove.y +
                right   * MyManager.InputMove.x;
            
            if (move.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(forward);
                MyManager.transform.rotation = Quaternion.Slerp(
                    MyManager.transform.rotation,
                    targetRot,
                    Time.deltaTime * 20f
                );
            }

            MyManager.characterController.Move(
                move.normalized * (MyManager.speed * Time.deltaTime)
            );
        }

        public override void OnExitState()
        {
        }
    }
}