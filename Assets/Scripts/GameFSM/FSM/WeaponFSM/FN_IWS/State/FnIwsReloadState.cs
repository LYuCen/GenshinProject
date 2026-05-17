using Firearms;
using mEvent;
using QFramework;
using UnityEngine;

namespace GameFSM.FSM.WeaponFSM.FN_IWS.State
{
    public class FnIwsReloadState : AbstractState<AbstractGun>, ICanSendEvent
    {
        private float _timer;
        private bool _end;
        public FnIwsReloadState(AbstractGun manager) : base(manager)
        {
            AddCondition(new FnIwsIdleCondition(MyManager, "Idle"));
            AddCondition(new FnIwsShootCondition(MyManager, "Shoot"));
        }

        public override void OnEnterState()
        {
            _timer = MyManager.GetWeaponConfig().reloadCd;
            _end = false;
        }

        public override void OnUpdateState()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0 && !_end)
            {
                _end = true;
                MyManager.ExecuteActionSuccess();
                this.SendEvent(new ReloadEndEvent());
            }
        }

        public override void OnExitState()
        {
            if (!_end) MyManager.CancelAction();
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}