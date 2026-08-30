using System;
using mEvent;
using QFramework;

namespace mCommand
{
    public class InputTipCommand : AbstractCommand
    {
        private string _titleStr;
        private string _placeholderStr;
        private Action<string> _apply;
        private Action _abandon;

        public InputTipCommand(string titleStr, string placeholderStr, Action<String> apply, Action abandon)
        {
            _titleStr = titleStr;
            _placeholderStr = placeholderStr;
            _apply = apply;
            _abandon = abandon;
        }
        
        protected override void OnExecute()
        {
            this.SendEvent(new InputTipEvent
            {
                TitleStr = _titleStr,
                PlaceholderStr = _placeholderStr,
                Apply = _apply,
                Abandon = _abandon
            });
        }
    }
}