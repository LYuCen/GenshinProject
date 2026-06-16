using mEvent;
using Player.Context;
using QFramework;

namespace Player.Module
{
    public class PlayerViewModule : IModule, ICanSendEvent
    {
        private PlayerContext _context;
        private int _weapon;
        public PlayerViewModule(PlayerContext context)
        {
            _context = context;
        }
        
        public void Init()
        {
            _weapon = 0;
            this.SendEvent(new WeaponDataUpdateEvent());
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

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}