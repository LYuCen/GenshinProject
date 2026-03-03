using UnityEngine;

namespace Common.LevelFSM.EnemyFSM
{
    public class EnemyController : MonoBehaviour
    {
        private EnemyFsm _fsm;
        public EnemyManager enemyManager;
        

        private void Start()
        {
            enemyManager.Init();
            _fsm = new EnemyFsm(enemyManager);
        }

        private void Update()
        {
            //_fsm.Update();
        }
    }
}