using System.Collections.Generic;
using mSystem.InputSystem.mEvent;
using mSystem.InputSystem.mKey;
using QFramework;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace mSystem.InputSystem.KeyHandlers.Handlers
{
    public class MoveHandler : KeyHandler, ICanSendEvent
    {
        private MoveEvent _moveEvent = new MoveEvent();
        private Vector2 _move = Vector2.zero;
        
        public override void Handle(List<KeyState> states)
        {
            if (!Enable) return;
            
            _move = Vector2.zero;
            foreach (var key in states)
            {
                if (!key.GetIsPress()) continue;
                if (key.GetKeyName() == "MoveUp") _move.y += 1;
                else if (key.GetKeyName() == "MoveDown") _move.y -= 1;
                else if (key.GetKeyName() == "MoveLeft") _move.x -= 1;
                else if (key.GetKeyName() == "MoveRight") _move.x += 1;
            }

            if (LengthSquared(_move) > 1)
            {
                Normalize(_move);
            }
            _moveEvent.MoveVec = _move;
            //Debug.Log(_move);
            this.SendEvent(_moveEvent);
        }

        float LengthSquared(Vector2 v) {
            return v.x * v.x + v.y * v.y;
        }

        void Normalize(Vector2 v) {
            float len2 = LengthSquared(v);
            if (len2 == 0.0f) return; // 避免除以 0
            float invLen = 1.0f / Mathf.Sqrt(len2);
            v.x *= invLen;
            v.y *= invLen;
        }
        

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}