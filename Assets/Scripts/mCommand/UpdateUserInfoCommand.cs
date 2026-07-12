using mEvent;
using QFramework;

namespace mCommand
{
    public class UpdateUserInfoCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.SendEvent(new UpdateUserInfoEvent());
        }
    }
}