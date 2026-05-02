using System.Collections.Generic;
using GameFSM.LevelFSM.PlayerFSM;

namespace GameFSM.FSM.PlayerFSM.UpperBodyFSM
{
    public class UpperBodyStateBuilder : AbstractStateBuilder<PlayerManager>
    {
        private Dictionary<string, AbstractState<PlayerManager>> _dictionary = new();
        public UpperBodyStateBuilder(PlayerManager manager) : base(manager)
        {
            _dictionary.Add("Null",new NullState(MyManager));
            _dictionary.Add("Reload", new ReloadState(MyManager));
            _dictionary.Add("ThrowGrenade", new ThrowGrenadeState(MyManager));
            IsBuilt = true;
        }

        public override Dictionary<string, AbstractState<PlayerManager>> CreateStates()
        {
            return _dictionary;
        }
    }
}