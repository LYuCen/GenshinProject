using mEvent;
using QFramework;

namespace mCommand
{
    public class OpenSettingCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.SendEvent<OpenSettingEvent>();
        }
    }
}