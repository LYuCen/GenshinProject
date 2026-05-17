using GameFSM.LevelFSM.PlayerFSM;
using mSystem.InputSystem;
using mSystem.InputSystem.Context;
using QFramework;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour, IController
    {
        [SerializeField] private Transform player;
        
        public PlayerManager manager;
        private PlayerFsm _playerFsm;
        private IInputSystem _inputSystem;
        private InputContext _context;
        [SerializeField] private InputContextHolder holder;

        private void Start()
        {
            _context = new InputContext();
            _inputSystem = new InputSystem(_context);
            holder.Context = _context;
            manager.Init(true, _context);
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
            _inputSystem.SetKeyCode();
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}
