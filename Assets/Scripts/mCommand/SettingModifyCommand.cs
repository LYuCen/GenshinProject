using mEvent;
using QFramework;

namespace mCommand
{
    public class SettingModifyCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.SendEvent(new SettingModifyEvent());
        }
    }
}