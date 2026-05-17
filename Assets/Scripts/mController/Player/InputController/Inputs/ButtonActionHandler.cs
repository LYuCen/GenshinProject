using mSystem.InputSystem;
using mSystem.InputSystem.Context;

namespace mController.Player.InputController.Inputs
{
    public abstract class ButtonActionHandler : InputActionHandler
    {
        public override void Handle(InputContext context)
        {
            var state = GetButtonState(context);
            if (state.Down) OnDown();
            if (state.Press) OnPress();
            if (state.Up) OnUp();
        }

        /// <summary>
        /// 子类决定监听 Context 的哪一个状态
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected abstract ButtonState GetButtonState(InputContext context);

        protected virtual void OnDown() { }
        protected virtual void OnPress() { }
        protected virtual void OnUp() { }
    }
}