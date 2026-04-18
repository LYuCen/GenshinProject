using System.Collections.Generic;
using Connect.Data;
using Factory.Greade;
using Factory.Rifle;
using Firearms;
using GameFSM.FSM.PlayerFSM;
using mEnum;
using mModel.ScriptableConfig;
using Player.AnimTree;
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
            if (config == null) return;

            if (_nameToTypeFactory.TryGetValue(config.weaponName, out var factory))
            {
                WeaponType newType = factory.GetWeaponType();
                var animCtrl = _myManager.AnimController;

                // 1. 架构级切换 (比如：步枪 -> 手雷)
                if (_lastType != newType)
                {
                    // 清空旧的插槽字典，准备迎接新架构
                    animCtrl.Slot.Clear();

                    // 构建新树，解析器会自动填充新的 slot 引用
                    var newFullTree = AnimGraphRuntimeBuilder.Build(
                        config.wholeTreeGraph, 
                        _myManager, 
                        animCtrl.Slot
                    );

                    // 根节点平滑切换
                    animCtrl.Root.InstallSubtree(newFullTree);
                    _lastType = newType;
                }

                // 3. 特殊插槽处理
                var actionNode = factory.CreateSpecialActionNode(config.weaponName);
                if (actionNode != null)
                {
                    var targetSlot = animCtrl.Slot.Get("UpperSlot"); 
        
                    if (targetSlot != null)
                    {
                        targetSlot.InstallSubtree(actionNode);
                    }
                    else
                    {
                        Debug.LogWarning("在当前的动画树中找不到名为 UpperSlot 的插槽节点");
                    }
                }
            }
        }
    }
}