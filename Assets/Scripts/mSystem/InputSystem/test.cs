using QFramework;
using UnityEngine;

namespace mSystem.InputSystem
{
    public class test : MonoBehaviour, QFramework.IController
    {
        private IInputSystem _inputSystem;
        private void Start()
        {
            _inputSystem = this.GetSystem<IInputSystem>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            _inputSystem.SetKeyCode();
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}