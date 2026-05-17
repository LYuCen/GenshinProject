using System.Collections.Generic;
using Firearms;
using GameFSM.FSM.WeaponFSM.SPR2.State;

namespace GameFSM.FSM.WeaponFSM.SPR2
{
    public class Spr2Builder : AbstractStateBuilder<AbstractGun>
    {
        private Dictionary<string, AbstractState<AbstractGun>> _node;
        
        public Spr2Builder(AbstractGun manager) : base(manager)
        {
            _node = new Dictionary<string, AbstractState<AbstractGun>>();
        }

        public override Dictionary<string, AbstractState<AbstractGun>> CreateStates()
        {
            _node.Add("Idle", new Spr2IdleState(MyManager));
            _node.Add("Reload", new Spr2ReloadState(MyManager));
            _node.Add("Shoot", new Spr2ShootState(MyManager));
            _node.Add("Bolt", new Spr2BoltState(MyManager));
            IsBuilt = true;
            return _node;
        }
    }
}