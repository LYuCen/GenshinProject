using UnityEngine;

namespace GameFSM.FSM.PlayerFSM.UpperBodyFSM
{
    public class ThrowGrenadeState : AbstractState<PlayerManager>
    {
        public ThrowGrenadeState(PlayerManager manager) : base(manager)
        {
            Conditions.Add(new NullCondition(MyManager,"Null"));
        }
        
        public override void OnEnterState()
        {
            MyManager.upperName = "投弹";
            MyManager.bodyIk.enabled = false;
        }

        public override void OnUpdateState()
        {
            /*
            float targetYaw = MyManager.cameraController.Yaw;
            
            Vector3 forward = Quaternion.Euler(0, targetYaw, 0) * Vector3.forward;
            
            Quaternion targetRot = Quaternion.LookRotation(forward);
            MyManager.transform.rotation = Quaternion.Slerp(
                MyManager.transform.rotation,
                targetRot,
                Time.deltaTime * 20f);
                */
        }

        public override void OnExitState()
        {
            MyManager.upperName = "投弹结束";
            MyManager.bodyIk.enabled = true;
        }
    }
}