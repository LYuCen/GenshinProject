using GameFSM.LevelFSM.PlayerFSM;
using mSystem.InputSystem.Context;
using Player.Context;

namespace Player.ManagerLogic
{
    public interface IPlayerLogic
    {
        void Init(PlayerManager manager);
        void OnUpdate();
        void OnClient50HzTick();
        void OnClient20HzTick();
        void OnLateUpdate();
    }
}