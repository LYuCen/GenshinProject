namespace mSystem.mSettingSystem
{
    public interface ISettingEditData
    {
        bool IsModify { get; }

        void Apply();
        void Cancel();
        void Close();
    }
}