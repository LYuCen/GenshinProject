using GameFSM.FSM.PlayerFSM;
using GameFSM.LevelFSM.PlayerFSM;
using QFramework;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour, IController
    {
        [SerializeField] private Transform player;
        
        public PlayerManager manager;
        private PlayerFsm _playerFsm;

        private void Start()
        {
            manager.Init(true);
            Debug.Log("PlayerController Start 被执行");
            Debug.Log($"manager 是否为空: {manager == null}");
            try
            {
                _playerFsm = new PlayerFsm(manager);
                Debug.Log("_playerFsm 创建成功");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"PlayerFsm 创建失败: {e}");
            }
        }

        private void Update()
        {
            _playerFsm.Update();
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}
