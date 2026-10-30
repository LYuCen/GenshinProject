using AssetBundleFramework.Core.Resource;
using BaseClass;
using QFramework;
using UnityEngine.Serialization;

namespace mSystem.InputSystem
{
    public class GameEnter : AbstractSingleton<GameEnter>, IController
    {
        private IInputSystem _inputSystem;
        public bool isGame; // 是否在游戏中
        public bool returnFromGame; // 是否从对局返回大厅
        public bool returnIsHost; // 返回后是否为房主

        protected override void Awake()
        {
            base.Awake();
            ResourceManager.EnsureInitialized();
        }

        private void Start()
        {
            _inputSystem = this.GetSystem<IInputSystem>();
        }

        public void Update()
        {
            _inputSystem.SetKeyCode();
        }
        
        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}