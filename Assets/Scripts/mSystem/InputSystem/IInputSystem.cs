using System.Collections.Generic;
using mSystem.InputSystem.KeyHandlers;
using mSystem.InputSystem.KeyHandlers.Handlers;
using mSystem.InputSystem.mKey;
using QFramework;


namespace mSystem.InputSystem
{
    public interface IInputSystem : ISystem
    {
        void SetKeyCode();
    }

    public class InputSystem : AbstractSystem, IInputSystem
    {
        private List<KeyHandler> _handlers;
        private List<KeyState> _keyStates;
        private IKeyModel _keyModel;
        protected override void OnInit()
        {
            _keyModel = this.GetModel<IKeyModel>();
            _handlers = new List<KeyHandler>();
            _keyStates = _keyModel.GetKeys();
            _handlers.Add(new MoveHandler());
            _handlers.Add(new AttackHandler());
            _handlers.Add(new ShootHandler());
            _handlers.Add(new JumpHandler());
            _handlers.Add(new MouseScrollHandler());
            _handlers.Add(new ReloadHandler());
            _handlers.Add(new ChangeWeaponHandler());
            _handlers.Add(new RunHandler());
            _handlers.Add(new ThrowGrenadeHandler());
        }

        public void SetKeyCode()
        {
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