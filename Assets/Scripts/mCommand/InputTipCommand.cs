using mEvent;
using mView.TipView;
using QFramework;

namespace mCommand
{
    public class InputTipCommand : AbstractCommand
    {
        private InputTipType _type;
        
        public InputTipCommand(InputTipType type)
        {
            _type = type;
        }
        
        protected override void OnExecute()
        {
            this.SendEvent(new InputTipEvent(_type));
        }
    }
}