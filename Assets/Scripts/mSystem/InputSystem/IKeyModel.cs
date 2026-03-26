using System.Collections.Generic;
using mSystem.InputSystem.mKey;
using mSystem.InputSystem.mKey.Keys;
using QFramework;
using UnityEngine;

namespace mSystem.InputSystem
{
    public interface IKeyModel : IModel
    {
        List<KeyState> GetKeys();
    }

    public class KeyModel : AbstractModel, IKeyModel
    {
        private List<KeyState> _keys;
        protected override void OnInit()
        {
            _keys = new List<KeyState>();
            LoadKeyState();
        }
        

        public List<KeyState> GetKeys()
        {
            return _keys;
        }

        private void LoadKeyState()
        {
            KeyState state = new MoveKeyUp(KeyCode.W);
            _keys.Add(state);
            KeyState state1 = new MoveKeyLeft(KeyCode.A);
            _keys.Add(state1);
            KeyState state2 = new MoveKeyDown(KeyCode.S);
            _keys.Add(state2);
            KeyState state3 = new MoveKeyRight(KeyCode.D);
            _keys.Add(state3);
            KeyState state4 = new MouseKeyLeft(KeyCode.Mouse0);
            _keys.Add(state4);
            KeyState state5 = new JumpKey(KeyCode.Space);
            _keys.Add(state5);
            KeyState state6 = new InShootKey(KeyCode.Mouse1);
            _keys.Add(state6);
            KeyState state7 = new ReloadKey(KeyCode.R);
            _keys.Add(state7);
            KeyState state8 = new MainGunKey(KeyCode.Alpha1);
            _keys.Add(state8);
            KeyState state9 = new MainGun2Key(KeyCode.Alpha2);
            _keys.Add(state9);
            KeyState state10 = new RunKey(KeyCode.LeftShift);
            _keys.Add(state10);
            KeyState state11= new ThrowGrenadeKey(KeyCode.G);
            _keys.Add(state11);
            KeyState state12= new CrouchKey(KeyCode.C);
            _keys.Add(state12);
        }
    }
}