using mView.TipView;

namespace mEvent
{
    public class InputTipEvent
    {
        public InputTipType Type;

        public InputTipEvent(InputTipType type)
        {
            Type = type;
        }
    }
}