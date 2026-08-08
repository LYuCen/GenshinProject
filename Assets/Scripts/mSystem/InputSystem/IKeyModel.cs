using System.Collections.Generic;
using mModel.SettingModel;
using mSystem.InputSystem.mKey;
using mSystem.InputSystem.mKey.Keys;
using QFramework;
using UnityEngine;

namespace mSystem.InputSystem
{
    public interface IKeyModel : IModel
    {
        List<KeyState> GetKeys();
        void LoadKeyState(ISettingModel model);
    }

    public class KeyModel : AbstractModel, IKeyModel
    {
        private List<KeyState> _keys = new();

        protected override void OnInit()
        {
        }

        public List<KeyState> GetKeys()
        {
            return _keys;
        }

        public void LoadKeyState(ISettingModel model)
        {
            _keys.Clear();
            _keys.Add(new MoveKeyUp(      model.GetKey("MoveForward", model.BaseConfig)));
            _keys.Add(new MoveKeyLeft(    model.GetKey("MoveLeft", model.BaseConfig)));
            _keys.Add(new MoveKeyDown(    model.GetKey("MoveBack", model.BaseConfig)));
            _keys.Add(new MoveKeyRight(   model.GetKey("MoveRight", model.BaseConfig)));
            _keys.Add(new RunKey(         model.GetKey("Sprint", model.BaseConfig)));
            _keys.Add(new CrouchKey(      model.GetKey("Crouch", model.BaseConfig)));
            _keys.Add(new JumpKey(        model.GetKey("Jump", model.BaseConfig)));

            _keys.Add(new MouseKeyLeft(   model.GetKey("Shoot", model.CombatConfig)));
            _keys.Add(new InShootKey(     model.GetKey("OpenShot", model.CombatConfig)));
            _keys.Add(new ReloadKey(      model.GetKey("Reload", model.CombatConfig)));
            _keys.Add(new MainGunKey(     model.GetKey("Weapon1", model.CombatConfig)));
            _keys.Add(new MainGun2Key(    model.GetKey("Weapon2", model.CombatConfig)));
            _keys.Add(new ThrowGrenadeKey(model.GetKey("Grenade", model.CombatConfig)));
            
            _keys.Add(new SettingKey(KeyCode.Escape));
        }
    }
}