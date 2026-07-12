using System.Collections.Generic;
using mCommand;
using mSystem.InputSystem.Context;
using mSystem.InputSystem.mKey;
using QFramework;
using UnityEngine;

namespace mSystem.InputSystem.KeyHandlers.Handlers
{
    public class SettingHandler : KeyHandler, IController
    {
        public SettingHandler(InputContext context = null) : base(context)
        {
            
        }

        public override void Handle(List<KeyState> states)
        {
            foreach (var s in states)
            {
                if (s.GetKeyName() == "Setting")
                {
                    if (s.GetIsDown())
                    {
                        this.SendCommand(new OpenSettingCommand());
                    }
                }
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}