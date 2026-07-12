namespace mSystem.mSettingSystem
{
    public abstract class SettingEditData : ISettingEditData
    {
        public bool IsModify => Compare();
        
        public abstract void Apply();
        public abstract void Cancel();
        public abstract void Close();
        
        /// <summary>
        /// 数据是否发生变更
        /// </summary>
        protected abstract bool Compare();
    }
}