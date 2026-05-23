using Firearms;
using mEvent;
using QFramework;
using UnityEngine;

namespace GameFSM.FSM.WeaponFSM.SPR2.State
{
    public class Spr2BoltState : AbstractState<AbstractGun>, ICanSendEvent
    {
        private float _boltTime;
        public Spr2BoltState(AbstractGun manager) : base(manager)
        {
            AddCondition(new Spr2IdleCondition(MyManager, "Idle"));
        }

        public override void OnEnterState()
        {
            _boltTime = MyManager.GetWeaponConfig().boltCd;
            this.SendEvent(new BoltEvent());
        }

        public override void OnUpdateState()
        {
            _boltTime -= Time.deltaTime;
            if (_boltTime < 0)
            {
                MyManager.IsBolt = false;
            }
        }

        public override void OnExitState()
        {
            this.SendEvent(new BoltEndEvent());
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}