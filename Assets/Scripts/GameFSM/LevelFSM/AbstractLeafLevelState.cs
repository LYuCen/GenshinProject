using GameFSM.FSM;

namespace Common.LevelFSM
{
    public abstract class AbstractLeafLevelState<T> : AbstractLevelState<T>
    {
        protected AbstractLeafLevelState(T manager, AbstractCondition<T> selfCondition) : base(manager, selfCondition)
        {
        }
    }
}