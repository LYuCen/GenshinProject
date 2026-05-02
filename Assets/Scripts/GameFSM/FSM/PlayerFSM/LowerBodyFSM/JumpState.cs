using GameFSM.LevelFSM.PlayerFSM;
using UnityEngine;

namespace GameFSM.FSM.PlayerFSM.LowerBodyFSM
{
    public class JumpState : AbstractState<PlayerManager>
    {
        private Vector3 _v3;
        private Vector2 _move;
        public JumpState(PlayerManager manager) : base(manager)
        {
            AddCondition(new IdleCondition(MyManager,"Idle"));
            AddCondition(new WalkCondition(MyManager,"Walk"));
        }

        public override void OnEnterState()
        {
            
            _move = MyManager.InputMove * (MyManager.speed * Time.deltaTime);
            
        }

        public override void OnUpdateState()
        {
            if (MyManager.IsGround) MyManager.characterController.Move(_v3);
            MyManager.characterController.Move(new Vector3(_move.x, 0, _move.y));
        }

        public override void OnExitState()
        {
            
        }
    }
}