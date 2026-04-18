using System.Collections.Generic;
using Firearms;
using GameFSM.FSM.PlayerFSM;
using mEnum;
using Player.AnimPro;
using UnityEngine;

namespace Factory.Greade
{
    public class GrenadesFactory : IWeaponTypeFactory
    {
        private PlayerManager _myManager;
        private Dictionary<string, IWeaponFactory> _gunFactories = new();

        public GrenadesFactory(PlayerManager manager)
        {
            _myManager = manager;
            // 注册具体投掷物生产线
           _gunFactories.Add("Grenade", new GrenadeFactory(_myManager));
        }

        public IEnumerable<string> GetSupportedWeapons() => _gunFactories.Keys;

        public WeaponType GetWeaponType() => WeaponType.Grenade;

        public IWeapon GetWeapon(string name)
        {
            return _gunFactories[name].GetWeapon();
        }

        public void UpdateTypeAnimations()
        {
            var anyGun = _gunFactories.Values.GetEnumerator();
            anyGun.MoveNext();
            var cfg = anyGun.Current.GetConfig();

            Debug.Log($"[RifleFactory] 检测到类型切换，更新移动动画集");
            
            var slots = _myManager.AnimController.Slot;
        }
        
        public AnimTreeNode<PlayerManager> CreateSpecialActionNode(string name)
        {
            if (_gunFactories.TryGetValue(name, out var factory))
            {
                return factory.CreateActionNode();
            }
            return null;
        }
    }
}