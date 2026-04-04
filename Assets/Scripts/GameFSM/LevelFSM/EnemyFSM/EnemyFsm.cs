using Common.LevelFSM.EnemyFSM.LevelFSMState;

namespace Common.LevelFSM.EnemyFSM
{
    public class EnemyFsm
    {
        private EnemyManager _myManager;
        private AbstractMixLevelState<EnemyManager> _root;

        public EnemyFsm(EnemyManager manager)
        {
            _myManager = manager;
            Init();
        }

        private void Init()
        {
            _root = new RootState(_myManager, null);
        }

        public void Update()
        {
            _root.OnUpdateState();
        }
    }
}