using UnityEngine;

namespace GameFSM.FSM.PlayerFSM.LowerBodyFSM
{
    public class SlidingState : AbstractState<PlayerManager>
    {
        private float _rotationY;
        float _rotateY;
        public SlidingState(PlayerManager manager) : base(manager)
        {
            AddCondition(new RunCondition(MyManager, "Run"));
            AddCondition(new IdleCondition(MyManager, "Idle"));
            AddCondition(new WalkCondition(MyManager,"Walk"));
            AddCondition(new ShootCondition(MyManager, "Shoot"));
            AddCondition(new ShootWalkCondition(MyManager, "ShootWalk"));
        }

        private Vector3 _lockedSlideDirection; // 用于存储锁定的方向

        public override void OnEnterState()
        {
            MyManager.speed = 5f;
            MyManager.gunCube.SetParent(MyManager.hand);
            MyManager.aimIK.enabled = true;
            MyManager.stateName = "滑铲";
            MyManager.isSlidingThrow = true;
            
            // 获取进入瞬间的相机正前方作为滑动方向
            _lockedSlideDirection = MyManager.transform.forward;

            // 让角色瞬间转向滑铲方向
            MyManager.transform.rotation = Quaternion.LookRotation(_lockedSlideDirection);
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

            // 使用进入时锁定的方向进行移动，不随相机转动而改变
            MyManager.characterController.Move(
                _lockedSlideDirection * (MyManager.speed * Time.deltaTime)
            );
            
        }

        public override void OnExitState()
        {
            
        }
    }
}