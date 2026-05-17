using System.Collections.Generic;
using mSystem.InputSystem.Context;
using mSystem.InputSystem.KeyHandlers;
using mSystem.InputSystem.KeyHandlers.Handlers;
using mSystem.InputSystem.mKey;

namespace mSystem.InputSystem
{
    public interface IInputSystem
    {
        public bool Enable { get; set; }
        void SetKeyCode();
    }

    public class InputSystem : IInputSystem 
    {
        private List<KeyHandler> _handlers;
        private List<KeyState> _keyStates;
        public bool Enable { get; set; }
        public InputSystem(InputContext context)
        {
            KeyModel model = new KeyModel();
            _handlers = new List<KeyHandler>();
            _keyStates = model.GetKeys();
            _handlers.Add(new MoveHandler(context));
            _handlers.Add(new AttackHandler(context));
            _handlers.Add(new ShootHandler(context));
            _handlers.Add(new JumpHandler(context));
            _handlers.Add(new MouseScrollHandler(context));
            _handlers.Add(new ReloadHandler(context));
            _handlers.Add(new ChangeWeaponHandler(context));
            _handlers.Add(new RunHandler(context));
            _handlers.Add(new ThrowGrenadeHandler(context));
            _handlers.Add(new CrouchHandler(context));
            Enable = true;
        }

        public void SetKeyCode()
        {
            if (!Enable) return;
            foreach (var key in _keyStates)
            {
                key.UpdateState();
            }
            foreach (var handler in _handlers)
            {
                handler.Handle(_keyStates);
            }
        }
    }
}