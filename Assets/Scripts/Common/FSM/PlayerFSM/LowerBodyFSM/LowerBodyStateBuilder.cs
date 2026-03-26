using System.Collections.Generic;

namespace Common.FSM.PlayerFSM.LowerBodyFSM
{
    public class LowerBodyStateBuilder : AbstractStateBuilder<PlayerManager>
    {
        private Dictionary<string, AbstractState<PlayerManager>> _dictionary = new();
        public LowerBodyStateBuilder(PlayerManager manager) : base(manager)
        {
            _dictionary.TryAdd("Idle", new IdleState(MyManager));
            _dictionary.TryAdd("Walk", new WalkState(MyManager));
            _dictionary.TryAdd("Jump", new JumpState(MyManager));
            _dictionary.TryAdd("Shoot", new ShootState(MyManager));
            _dictionary.TryAdd("ShootWalk", new ShootWalkState(MyManager));
            _dictionary.TryAdd("Run", new RunState(MyManager));
            _dictionary.TryAdd("Sliding", new SlidingState(MyManager));
            IsBuilt = true;
        }

        public override Dictionary<string, AbstractState<PlayerManager>> CreateStates()
        {
            return _dictionary;
        }
    }
}