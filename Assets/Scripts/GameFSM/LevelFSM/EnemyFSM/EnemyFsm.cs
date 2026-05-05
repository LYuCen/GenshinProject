using GameFSM.LevelFSM.EnemyFSM.LevelFSMState;

namespace GameFSM.LevelFSM.EnemyFSM
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