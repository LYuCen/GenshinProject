using System;
using mView.TipView;

namespace mEvent
{
    public class InputTipEvent
    {
        public string TitleStr;
        public string PlaceholderStr;
        public Action<string> Apply;
        public Action Abandon;
    }
}