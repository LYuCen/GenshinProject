using UnityEngine;

namespace Common.FSM.PlayerFSM.LowerBodyFSM
{
    public class WalkState : AbstractState<PlayerManager>
    {
        private float _rotationY;
        private WalkCondition _condition;
        
        public WalkState(PlayerManager manager) : base(manager)
        {
            _condition = new WalkCondition(MyManager, "Walk");
            Conditions.Add(new IdleCondition(MyManager,"Idle"));
            Conditions.Add(new JumpCondition(MyManager,"Jump"));
            Conditions.Add(new ShootCondition(MyManager, "Shoot"));
            Conditions.Add(new ShootWalkCondition(MyManager,"ShootWalk"));
            Conditions.Add(new RunCondition(MyManager, "Run"));
        }

        public override void OnEnterState()
        {
            MyManager.speed = 1.6f;
            MyManager.PlayAnimController.LeftHandIk = 1;
            MyManager.PlayAnimController.RightHandIk = 0;
            MyManager.gunCube.SetParent(MyManager.hand);
            MyManager.gunCube.localPosition = new Vector3(0.00016f, 0.00203f, 0.00078f);
            MyManager.gunCube.localEulerAngles = new Vector3(-90, 180, 0);
            MyManager.stateName = "移动";
        }

        public override void OnUpdateState()
        {
            float targetYaw = MyManager.cameraController.Yaw;
            
            Vector3 forward = Quaternion.Euler(0, targetYaw, 0) * Vector3.forward;
            Vector3 right   = Quaternion.Euler(0, targetYaw, 0) * Vector3.right;

            Vector3 move =
                forward * MyManager.inputMove.y +
                right   * MyManager.inputMove.x;
            
            if (move.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(move);
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
        }
    }
}