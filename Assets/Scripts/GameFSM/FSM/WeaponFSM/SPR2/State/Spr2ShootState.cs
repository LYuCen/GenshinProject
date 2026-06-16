using Firearms;
using mEvent;
using QFramework;
using UnityEngine;

namespace GameFSM.FSM.WeaponFSM.SPR2.State
{
    public class Spr2ShootState : AbstractState<AbstractGun>, ICanSendEvent
    {
        
        public Spr2ShootState(AbstractGun manager) : base(manager)
        {
            AddCondition(new Spr2BoltCondition(MyManager, "Bolt"));
            AddCondition(new Spr2ReloadCondition(MyManager, "Reload"));
        }
        
        public override void OnEnterState()
        {
            if (!MyManager.Magazine.IsEmpty())
            {
                this.SendEvent<ShootEvent>();
                MyManager.Shoot();
            }
            else
            {
                this.SendEvent<ShootEvent>();
            }
            
        }

        public override void OnUpdateState()
        {
            
            
        }

        public override void OnExitState()
        {
            
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}