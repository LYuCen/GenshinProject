using System.Collections.Generic;
using Connect.Data;
using Factory.Greade;
using Factory.Rifle;
using Firearms;
using GameFSM.FSM.PlayerFSM;
using mEnum;
using UnityEngine;

namespace Factory
{
    public class WeaponFactory
    {
        // 映射：武器名 -> 负责它的类型工厂
        private Dictionary<string, IWeaponTypeFactory> _nameToTypeFactory = new();
        private WeaponType? _lastType;
        private PlayerManager _myManager;

        public WeaponFactory(PlayerManager manager)
        {
            _myManager = manager;
            // 注册具体的类型工厂
            RegisterTypeFactory(new RifleFactory(manager));
            RegisterTypeFactory(new GrenadesFactory(manager));
        }

        private void RegisterTypeFactory(IWeaponTypeFactory typeFactory)
        {
            foreach (var weaponName in typeFactory.GetSupportedWeapons())
            {
                _nameToTypeFactory[weaponName] = typeFactory;
            }
        }

        public IWeapon GetWeapon(string name)
        {
            if (_nameToTypeFactory.TryGetValue(name, out var factory))
            {
                // 获取武器实例
                IWeapon weapon = factory.GetWeapon(name);
                return weapon;
            }

            Debug.LogError($"[WeaponFactory] 找不到名为 {name} 的武器配置");
            return null;
        }

        public IWeapon GetWeapon(WeaponStateData data)
        {
            IWeapon weapon = GetWeapon(data.WeaponName);
            AbstractMagazine magazine = weapon.GetMagazine();
            if (magazine != null)
            {
                magazine.Element = data.CurrentElement;
                magazine.BulletNumber = data.CurrentBullets;
            }
            else Debug.LogWarning($"[WeaponFactory] 找不到 {data.WeaponName} 的弹匣");

            return weapon;
        }
        
        public void SyncWeaponAnimation(IWeapon weapon)
        {
            var config = weapon.GetWeaponConfig();
            
            if(config == null)Debug.LogWarning("config 为空");
            if (_nameToTypeFactory.TryGetValue(config.weaponName, out var factory))
            {
                // 1. 下放给工厂创建特殊动画树 (不管是 Reload 还是 Throw)
                var actionNode = factory.CreateSpecialActionNode(config.weaponName);
        
                if (actionNode != null)
                {
                    _myManager.AnimController.Slot.UpperSlot.InstallSubtree(actionNode);
                }

                // 2. 同步移动动作 (下半身 - 内部有类型检查)
                WeaponType newType = factory.GetWeaponType();
                if (_lastType != newType)
                {
                    factory.UpdateTypeAnimations();
                    _lastType = newType;
                }
            }
        }
    }
}