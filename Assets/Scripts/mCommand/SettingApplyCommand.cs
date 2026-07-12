using mEvent;
using QFramework;

namespace mCommand
{
    public class SettingApplyCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.SendEvent(new SettingApplyEvent());
        }
    }
}