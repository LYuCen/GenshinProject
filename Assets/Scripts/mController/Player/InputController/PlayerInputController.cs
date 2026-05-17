using System.Collections.Generic;
using GameFSM.LevelFSM.PlayerFSM;
using mController.Player.InputController.Inputs;
using mController.Player.InputController.Inputs.ActionHandler;
using mSystem.InputSystem.Context;

namespace mController.Player.InputController
{
    public class PlayerInputController
    {
        private List<InputActionHandler> _handlers = new();

        public PlayerInputController(PlayerManager manager)
        {
            Register(new Ability1ActionHandler(), manager);
            Register(new CrouchActionHandler(), manager);
            Register(new GetPrimaryWeaponActionHandler(), manager);
            Register(new GetSecondaryWeaponActionHandler(), manager);
            Register(new MouseScrollActionHandler(), manager);
            Register(new MoveActionHandler(), manager);
            Register(new PrimaryActionHandler(), manager);
            Register(new ReloadActionHandler(), manager);
            Register(new SecondaryActionHandler(), manager);
            Register(new SprintActionHandler(), manager);
        }

        private void Register(InputActionHandler handler, PlayerManager manager)
        {
            handler.Init(manager);
            _handlers.Add(handler);
        }

        public void Process(InputContext context)
        {
            if (context == null) return;
        
            // 遍历执行所有已注册的处理器
            for (int i = 0; i < _handlers.Count; i++)
            {
                _handlers[i].Handle(context);
            }
        }
    }
}