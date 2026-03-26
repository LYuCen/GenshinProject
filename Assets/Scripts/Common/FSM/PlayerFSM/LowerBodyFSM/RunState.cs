using UnityEngine;

namespace Common.FSM.PlayerFSM.LowerBodyFSM
{
    public class RunState : AbstractState<PlayerManager>
    {
        private float _rotationY;
        float _rotateY;
        public RunState(PlayerManager manager) : base(manager)
        {
            Conditions.Add(new IdleCondition(MyManager,"Idle"));
            Conditions.Add(new ShootWalkCondition(MyManager, "ShootWalk"));
            Conditions.Add(new WalkCondition(MyManager, "Walk"));
            AddCondition(new SlidingCondition(MyManager,"Sliding"));
        }

        public override void OnEnterState()
        {
            MyManager.speed = MyManager.isCrouch ? 3.5f : 4f;
            MyManager.gunCube.SetParent(MyManager.hand);
            MyManager.gunCube.localPosition = new Vector3(0.000118f, 0.001919f, 0.001026f);
            MyManager.gunCube.localEulerAngles = new Vector3(283.742096f, 323.333099f, 229.175308f);
            MyManager.aimIK.enabled = false;
            MyManager.stateName = "奔跑";
        }

        public override void OnUpdateState()
        {
            MyManager.speed = MyManager.isCrouch ? 3.5f : 4f;
            float targetYaw = MyManager.cameraController.Yaw;
            
            // Quaternion.Euler 为旋转矩阵  Quaternion.Euler(0, targetYaw, 0) 是向量为 y轴旋转 targetYaw度
            Vector3 forward = Quaternion.Euler(0, targetYaw, 0) * Vector3.forward;
            Vector3 right   = Quaternion.Euler(0, targetYaw, 0) * Vector3.right;

            Vector3 move =
                forward * MyManager.inputMove.y +
                right   * MyManager.inputMove.x;
            
            // sqrMagnitude向量长度的平方比 magnitude 求向量长度不需要开平方，性能更好
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