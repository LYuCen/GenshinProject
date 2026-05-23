namespace mWeapon
{
    public class WeaponTaskRunner
    {
        private IWeaponTask _currentTask;

        public void Run(IWeaponTask task)
        {
            _currentTask?.Cancel();

            _currentTask = task;

            _currentTask.Start();
        }

        public void Tick()
        {
            _currentTask?.Tick();

            if (_currentTask != null && _currentTask.Finished)
            {
                _currentTask = null;
            }
        }

        public void CancelCurrent()
        {
            _currentTask?.Cancel();

            _currentTask = null;
        }
    }
}