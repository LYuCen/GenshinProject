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

        public PlayerInputController(PlayerManager manager, FsmContext fsmContext)
        {
            Register(new Ability1ActionHandler(), manager, fsmContext);
            Register(new CrouchActionHandler(), manager, fsmContext);
            Register(new GetPrimaryWeaponActionHandler(), manager, fsmContext);
            Register(new GetSecondaryWeaponActionHandler(), manager, fsmContext);
            Register(new MouseScrollActionHandler(), manager, fsmContext);
            Register(new MoveActionHandler(), manager, fsmContext);
            Register(new PrimaryActionHandler(), manager, fsmContext);
            Register(new ReloadActionHandler(), manager, fsmContext);
            Register(new SecondaryActionHandler(), manager, fsmContext);
            Register(new SprintActionHandler(), manager, fsmContext);
            Register(new JumpActionHandler(), manager, fsmContext);
        }

        private void Register(InputActionHandler handler, PlayerManager manager, FsmContext fsmContext)
        {
            handler.Init(manager, fsmContext);
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