using mEvent;
using Player.Context;
using QFramework;
using UnityEngine;

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
            Debug.Log("发送更新面板");
            this.SendEvent(new WeaponDataUpdateEvent());
        }

        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
            
        }

        public void Reset()
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