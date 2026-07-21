using System;
using mEvent;
using QFramework;

namespace mCommand
{
    public class TextTipCommand : AbstractCommand
    {
        private Action _applyAction;
        private Action _abandonAction;
        private Action _closeTip;
        private string _tipText;

        public TextTipCommand(string str,Action applyAction, Action abandonAction, Action closeTip)
        {
            _applyAction = applyAction;
            _abandonAction = abandonAction;
            _closeTip = closeTip;
            _tipText = str;
        }
        protected override void OnExecute()
        {
            this.SendEvent(new TextTipEvent()
            {
                ApplyAction = _applyAction,
                AbandonAction = _abandonAction,
                CloseTip = _closeTip,
                TipText = _tipText
            });
        }
    }
}