using GameFSM.LevelFSM.PlayerFSM;
using mSystem.InputSystem.Context;

namespace mController.Player.InputController.Inputs
{
    public abstract class InputActionHandler
    {
        protected PlayerManager MyManager;

        public void Init(PlayerManager manager) => MyManager = manager;

        /// <summary>
        /// 核心执行方案
        /// </summary>
        /// <param name="context"></param>
        public abstract void Handle(InputContext context);
    }
}