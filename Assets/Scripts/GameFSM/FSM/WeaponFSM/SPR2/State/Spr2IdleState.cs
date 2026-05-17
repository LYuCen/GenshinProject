using Firearms;
using UnityEngine;

namespace GameFSM.FSM.WeaponFSM.SPR2.State
{
    public class Spr2IdleState : AbstractState<AbstractGun>
    {
        public Spr2IdleState(AbstractGun manager) : base(manager)
        {
            AddCondition(new Spr2ReloadCondition(MyManager, "Reload"));
            AddCondition(new Spr2ShootCondition(MyManager, "Shoot"));
        }

        public override void OnUpdateState()
        {
            Debug.Log($"[FSM] OnUpdateState - MyManager HashCode:{MyManager.GetHashCode()}, Type:{MyManager.GetType().Name}, IsShoot:{MyManager.IsShoot}, IsReload:{MyManager.IsReload}");
        }
    }
}