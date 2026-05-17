using mEvent;
using Player.Context;
using QFramework;

namespace Player.Module
{
    public class PlayerViewModule : IModule, ICanSendEvent
    {
        private PlayerContext _context;
        private int _chose;
        private int _weapon;
        public PlayerViewModule(PlayerContext context)
        {
            _context = context;
        }
        
        public void Init()
        {
            _chose = 0;
            _weapon = 0;
            this.SendEvent(new ViewInitEvent());
        }

        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
            
        }

        public void GetPrimaryWeapon()
        {
            _weapon = 0;
            this.SendEvent(new ViewUpdateWeaponEvent
            {
                Index = 0
            });
        }

        public void GetSecondaryWeapon()
        {
            _weapon = 1;
            this.SendEvent(new ViewUpdateWeaponEvent
            {
                Index = 1
            });
        }
        
        public void ChangeElement(int offset)
        {
            _chose = (_chose + offset + 3) % 3;
            this.SendEvent(new ViewUpdateElement
            {
                Offset = offset
            });
        }

        public int GetChose()
        {
            return _chose;
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}