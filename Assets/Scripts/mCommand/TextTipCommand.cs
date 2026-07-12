using System;
using mEvent;
using QFramework;

namespace mCommand
{
    public class TextTipCommand : AbstractCommand
    {
        private Action _applyAction;
        private string _tipText;

        public TextTipCommand(Action applyAction, string str)
        {
            _applyAction = applyAction;
            _tipText = str;
        }
        protected override void OnExecute()
        {
            this.SendEvent(new TextTipEvent()
            {
                ApplyAction = _applyAction,
                TipText = _tipText
            });
        }
    }
}