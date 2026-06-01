using GameFSM.FSM;
using UnityEngine;

namespace GameFSM.LevelFSM.PlayerFSM.LevelFSMState.UpperFSMState
{
    public class PlayerBoltState : AbstractLevelState<PlayerManager>
    {
        public PlayerBoltState(PlayerManager manager, AbstractCondition<PlayerManager> selfCondition) : base(manager, selfCondition)
        {
            AddCondition(new PlayerEmptyStateCondition(MyManager, "Empty"));
            AddCondition(new PlayerReloadStateCondition(MyManager, "Reload"));
        }
        
        public override void OnEnterState()
        {
            Transform gun = MyManager.CombatModule.GetWeapon();
            if (gun)
            {
                gun.SetParent(MyManager.weaponPivot);
                gun.localPosition = Vector3.zero;
                gun.localEulerAngles = Vector3.zero;
            }
            MyManager.IkModule.SetAimIk(0, true);
        }

        public override void OnUpdateState()
        {
        }

        public override void OnExitState()
        {
            MyManager.CombatModule.ResetGun();
            MyManager.IkModule.SetAimIk(MyManager.GetAimIkTarget());
        }
    }
}