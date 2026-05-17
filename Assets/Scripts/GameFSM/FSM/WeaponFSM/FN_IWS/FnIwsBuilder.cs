using System.Collections.Generic;
using Firearms;
using GameFSM.FSM.WeaponFSM.FN_IWS.State;

namespace GameFSM.FSM.WeaponFSM.FN_IWS
{
    public class FnIwsBuilder : AbstractStateBuilder<AbstractGun>
    {
        private Dictionary<string, AbstractState<AbstractGun>> _node;
        
        public FnIwsBuilder(AbstractGun manager) : base(manager)
        {
            _node = new Dictionary<string, AbstractState<AbstractGun>>();
        }

        public override Dictionary<string, AbstractState<AbstractGun>> CreateStates()
        {
            _node.Add("Idle", new FnIwsIdleState(MyManager));
            _node.Add("Reload", new FnIwsReloadState(MyManager));
            _node.Add("Shoot", new FnIwsShootState(MyManager));
            IsBuilt = true;
            return _node;
        }
    }
}