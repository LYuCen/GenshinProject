using GameFSM.FSM;
using GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InAir;
using GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.InGround;
using UnityEngine;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState
{
    /// <summary>
    /// 角色下半身状态层
    /// </summary>
    public class LowerBodyLayer : AbstractMixLevelState<PlayerManager>
    {
        public LowerBodyLayer(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
        }

        protected override void BuildState()
        {
            States.Add("Ground", new PlayerGroundLayer(MyManager, new PlayerGroundLayerCondition(MyManager, "Ground")));
            //States.Add("Air", new PlayerAirLayer(MyManager, new PlayerAirLayerCondition(MyManager, "Air")));
            IsBuilt = true;
        }

        public override void OnEnterState()
        {
            MyManager.gunCube.localPosition = new Vector3(0.000118f, 0.001919f, 0.001026f);
            MyManager.gunCube.localEulerAngles = new Vector3(283.742096f, 323.333099f, 229.175308f);
            MyManager.moveSpeedWeight = 1f;
        }

        public override void OnUpdateState()
        {
            base.OnUpdateState();
            float targetYaw = MyManager.cameraController.Yaw;
            
            // Quaternion.Euler 为旋转矩阵  Quaternion.Euler(0, targetYaw, 0) 是向量为 y轴旋转 targetYaw度
            Vector3 forward = Quaternion.Euler(0, targetYaw, 0) * Vector3.forward;
            Vector3 right   = Quaternion.Euler(0, targetYaw, 0) * Vector3.right;

            Vector3 move =
                forward * MyManager.InputMove.y +
                right   * MyManager.InputMove.x;
            
            // sqrMagnitude向量长度的平方比 magnitude 求向量长度不需要开平方，性能更好
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
                move.normalized * (MyManager.moveSpeed * Time.deltaTime)
            );
        }
    }
}