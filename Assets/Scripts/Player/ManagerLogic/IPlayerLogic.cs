using GameFSM.FSM.PlayerFSM;
using GameFSM.LevelFSM.PlayerFSM;

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