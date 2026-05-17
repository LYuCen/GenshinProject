using System.Collections.Generic;
using mSystem.InputSystem.Context;
using mSystem.InputSystem.mKey;
using QFramework;
using UnityEngine;

namespace mSystem.InputSystem.KeyHandlers.Handlers
{
    public class MouseScrollHandler : KeyHandler, ICanSendEvent
    {
        private float _nextTime;
        private ICanSendEvent _canSendEventImplementation;

        public MouseScrollHandler(InputContext context) : base(context)
        {
        }

        public override void Handle(List<KeyState> states)
        {
            float scrollY = Input.mouseScrollDelta.y;
            if (Time.time > _nextTime && Mathf.Abs(scrollY) > 0.1f)
            {
                // 触发滚动：设置下一次触发时间
                _nextTime = Time.time + 0.3f; 
        
                // 赋予脉冲值 (1 或 -1)
                Context.MouseScroll = scrollY > 0 ? -1f : 1f;
            }
            else
            {
                // 如果没有到达触发条件（包括正在冷却中），必须立刻清零
                Context.MouseScroll = 0;
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}