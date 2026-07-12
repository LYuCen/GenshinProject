using mEvent;
using QFramework;

namespace mCommand
{
    public class SettingDataResetCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.SendEvent<SettingDataResetEvent>();
        }
    }
}