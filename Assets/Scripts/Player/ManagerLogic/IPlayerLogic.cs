using GameFSM.LevelFSM.PlayerFSM;
using mSystem.InputSystem.Context;
using Player.Context;

namespace Player.ManagerLogic
{
    public interface IPlayerLogic
    {
        void Init(PlayerManager manager);
        void OnUpdate();
        void OnFixedUpdate();
        void OnLateUpdate();
    }
}