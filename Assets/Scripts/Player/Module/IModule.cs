namespace Player.Module
{
    public interface IModule
    {
        void Init();
        void OnUpdate();
        void OnFixedUpdate();
    }
}