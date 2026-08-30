using GameFSM.FSM;
using UnityEngine;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState.GlobalInterruptState
{
    public class PlayerDeadState : AbstractLeafLevelState<PlayerManager>
    {
        private float _timer;
        private bool _reviveSent;
        
        public PlayerDeadState(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
        }

        public override void Enter()
        {
            _timer = 0f;
            _reviveSent = false;
            MyManager.DisableModule();
            MyManager.CreateDeathSnapshot();

            if (MyManager.IsLocal)
            {
                Debug.Log("你已死亡");
                MyManager.DisableVisual();
            }
            else
            {
                Debug.Log($"远程玩家{MyManager.EntityId}已死亡");
                MyManager.DisableVisual();
            }
        }

        public override void Update()
        {
            if (!MyManager.IsLocal) return;
            if (_reviveSent) return;

            _timer += Time.deltaTime;
            if (_timer >= 3f)
            {
                _reviveSent = true;
                MyManager.SendReviveRequest();
            }
        }

        public override void OnExitState()
        {
            MyManager.EnableModule();
            MyManager.EnableVisual();
            Debug.Log("玩家复活");
        }
    }
}
