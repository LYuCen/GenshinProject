using Common.LevelFSM.EnemyFSM.LevelFSMState;

namespace Common.LevelFSM
{
    public abstract class AbstractLeafLevelState<T> : AbstractLevelState<T>
    {
        protected AbstractLeafLevelState(T manager) : base(manager)
        {
            
        }
    }
}