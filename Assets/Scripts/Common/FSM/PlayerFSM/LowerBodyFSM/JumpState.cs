using UnityEngine;

namespace Common.FSM.PlayerFSM.LowerBodyFSM
{
    public class JumpState : AbstractState<PlayerManager>
    {
        private Vector3 _v3;
        private Vector2 _move;
        private Vector3 _leftHandPosition = new(-0.037f, 0.072f, 0.203f);
        private Vector3 _leftHandRotation = new(34.336f, 57.213f, 181.076f);
        public JumpState(PlayerManager manager) : base(manager)
        {
            Conditions.Add(new IdleCondition(MyManager,"Idle"));
            Conditions.Add(new WalkCondition(MyManager,"Walk"));
        }

        public override void OnEnterState()
        {
            MyManager.PlayAnimController.LeftHandEffector.localPosition = _leftHandPosition;
            MyManager.PlayAnimController.LeftHandEffector.localEulerAngles = _leftHandRotation;
            
            MyManager.f = 1.5f;
            _v3.y = MyManager.f * Time.deltaTime;
            _move = MyManager.inputMove * (MyManager.speed * Time.deltaTime);
            
            Debug.Log("进入跳跃状态");
        }

        public override void OnUpdateState()
        {
            if (MyManager.isGround) MyManager.characterController.Move(_v3);
            MyManager.characterController.Move(new Vector3(_move.x, 0, _move.y));
        }

        public override void OnExitState()
        {
            
        }
    }
}