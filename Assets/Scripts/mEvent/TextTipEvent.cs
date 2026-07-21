using System;

namespace mEvent
{
    public class TextTipEvent
    {
        public Action ApplyAction;
        public Action AbandonAction;
        public Action CloseTip;
        public string TipText;
    }
}