using System.Collections.Generic;
using mModel.SettingModel;
using mSystem.InputSystem.Context;
using mSystem.InputSystem.KeyHandlers;
using mSystem.InputSystem.KeyHandlers.Handlers;
using mSystem.InputSystem.mKey;
using QFramework;

namespace mSystem.InputSystem
{
    public interface IInputSystem : ISystem
    {
        public bool Enable { get; set; }
        public bool GamePlayEnable { get; set; }
        void SetKeyCode();
        void InitHandler(InputContext context);
    }

    public class InputSystem : AbstractSystem, IInputSystem
    {
        private List<KeyHandler> _gameplayHandlers;
        private List<KeyHandler> _globalHandlers;
        private List<KeyState> _keyStates;
        private IKeyModel _keyModel;
        public bool Enable { get; set; }
        public bool GamePlayEnable { get; set; }
        
        protected override void OnInit()
        {
            _keyModel = this.GetModel<IKeyModel>();
            _keyModel.LoadKeyState(this.GetModel<ISettingModel>());
            _globalHandlers = new List<KeyHandler>();
            _gameplayHandlers = new List<KeyHandler>();
            _globalHandlers.Add(new SettingHandler());
            _keyStates = _keyModel.GetKeys();
            Enable = true;
            GamePlayEnable = true;
        }
        
        public void InitHandler(InputContext context)
        {
            _gameplayHandlers.Add(new MoveHandler(context));
            _gameplayHandlers.Add(new AttackHandler(context));
            _gameplayHandlers.Add(new ShootHandler(context));
            _gameplayHandlers.Add(new JumpHandler(context));
            _gameplayHandlers.Add(new MouseScrollHandler(context));
            _gameplayHandlers.Add(new ReloadHandler(context));
            _gameplayHandlers.Add(new ChangeWeaponHandler(context));
            _gameplayHandlers.Add(new RunHandler(context));
            _gameplayHandlers.Add(new ThrowGrenadeHandler(context));
            _gameplayHandlers.Add(new CrouchHandler(context));
        }

        public void SetKeyCode()
        {
            if (!Enable) return;
            foreach (var key in _keyStates)
            {
                key.UpdateState();
            }
            foreach (var handler in _globalHandlers)
            {
                handler.Handle(_keyStates);
            }

            if (!GamePlayEnable) return;
            foreach (var handler in _gameplayHandlers)
            {
                handler.Handle(_keyStates);
            }
        }
    }
}