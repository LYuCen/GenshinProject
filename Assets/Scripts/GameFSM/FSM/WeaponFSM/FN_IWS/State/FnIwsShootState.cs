using Firearms;
using mEvent;
using QFramework;
using UnityEngine;

namespace GameFSM.FSM.WeaponFSM.FN_IWS.State
{
    public class FnIwsShootState : AbstractState<AbstractGun>, ICanSendEvent
    {
        private float _timer;
        private bool _event;
        public FnIwsShootState(AbstractGun manager) : base(manager)
        {
            AddCondition(new FnIwsIdleCondition(MyManager, "Idle"));
            AddCondition(new FnIwsReloadCondition(MyManager, "Reload"));
        }

        public override void OnEnterState()
        {
            _timer = 0;
            _event = false;
            
        }

        public override void OnUpdateState()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0 && !MyManager.Magazine.IsEmpty())
            {
                _timer = MyManager.GetWeaponConfig().shootCd;
                this.SendEvent<ShootEvent>();
                MyManager.Shoot();
            }
            else if (MyManager.Magazine.IsEmpty())
            {
                this.SendEvent<ShootEvent>();
                this.SendEvent<ReloadEvent>();
            }
            
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