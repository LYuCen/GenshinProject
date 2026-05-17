using System.Collections.Generic;
using mSystem.InputSystem.Context;
using mSystem.InputSystem.mKey;
using QFramework;
using Vector2 = UnityEngine.Vector2;

namespace mSystem.InputSystem.KeyHandlers.Handlers
{
    public class MoveHandler : KeyHandler, ICanSendEvent
    {
        private Vector2 _move = Vector2.zero;

        public MoveHandler(InputContext context) : base(context)
        {
        }

        public override void Handle(List<KeyState> states)
        {
            if (!Enable) return;
            
            _move = Vector2.zero;
            Context.Move = new Vector2();
            foreach (var key in states)
            {
                
                if (!key.GetIsPress() && !key.GetIsDown()) continue;
                if (key.GetKeyName() == "MoveUp") _move.y += 1;
                else if (key.GetKeyName() == "MoveDown") _move.y -= 1;
                else if (key.GetKeyName() == "MoveLeft") _move.x -= 1;
                else if (key.GetKeyName() == "MoveRight") _move.x += 1;
            }
            Context.Move = _move;
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}