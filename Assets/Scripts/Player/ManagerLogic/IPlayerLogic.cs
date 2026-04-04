using GameFSM.FSM.PlayerFSM;

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