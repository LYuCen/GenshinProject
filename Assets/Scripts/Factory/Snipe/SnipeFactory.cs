using System.Collections.Generic;
using Factory.Rifle;
using Firearms;
using GameFSM.LevelFSM.PlayerFSM;
using mEnum;
using Player.AnimPro;
using UnityEngine;

namespace Factory.Snipe
{
    public class SnipeFactory : IWeaponTypeFactory
    {
        private PlayerManager _myManager;
        private Dictionary<string, IWeaponFactory> _gunFactories = new();

        public SnipeFactory(PlayerManager manager)
        {
            _myManager = manager;
            // 注册具体步枪生产线
            _gunFactories.Add("SPR2", new SPR2Factory(manager));
        }

        public IEnumerable<string> GetSupportedWeapons() => _gunFactories.Keys;

        public WeaponType GetWeaponType() => WeaponType.Snipe;

        public IWeapon GetWeapon(string name)
        {
            return _gunFactories[name].GetWeapon();
        }

        public void UpdateTypeAnimations()
        {
            var anyGun = _gunFactories.Values.GetEnumerator();
            anyGun.MoveNext();
            var cfg = anyGun.Current.GetConfig();

            Debug.Log($"[Snipe] 检测到类型切换，更新移动动画集");
            
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