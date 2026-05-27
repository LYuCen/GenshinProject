using System;
using System.Collections.Generic;
using GameFSM.LevelFSM.PlayerFSM;
using mEnum;
using Player.AnimPro;
using UnityEngine.Playables;

namespace Player.AnimTree
{
    public class PlayerAnimController
    {
        private PlayerManager _manager;
        private PlayerAnimTreeBuild _animBuild;
        private WeaponType _currentWeaponType;
        private Dictionary<string, Func<PlayerManager, AnimTreeNode<PlayerManager>>> _slotFactories = new();
        public PlayerAnimSlotTreeRoot Root;
        public PlayerAnimSlot Slot;

        public PlayerAnimController(PlayerManager manager, AnimGraphData defaultTree)
        {
            _manager = manager;
            Slot = new PlayerAnimSlot();
            manager.AnimController = this;
            _animBuild = new PlayerAnimTreeBuild(manager, defaultTree, Slot);
            Root = _animBuild.RootNode;
        }

        /// <summary>
        /// 武器类型变化时，切换动画树。调用方不需要知道 PlayerManager。
        /// </summary>
        public void SwitchWeaponGraph(AnimGraphData weaponGraph, WeaponType weaponType)
        {
            if (_currentWeaponType == weaponType) return;
            _currentWeaponType = weaponType;
            _slotFactories.Clear();
            Slot.Clear();
            Root.InstallSubtree(AnimGraphRuntimeBuilder.Build(weaponGraph, _manager, Slot));
        }

        /// <summary>
        /// 给指定槽安装特殊动作节点（换弹、拉栓等），并缓存 factory 以便后续重装
        /// </summary>
        public void InstallSlotAction(string slotName, Func<PlayerManager, AnimTreeNode<PlayerManager>> factory)
        {
            _slotFactories[slotName] = factory;
            var slot = Slot.Get(slotName);
            if (slot != null) slot.InstallSubtree(factory(_manager));
        }

        /// <summary>
        /// 重新安装之前装过的槽位动作，利用 InstallSubtree 的 crossfade 避免硬跳
        /// </summary>
        public void ReinstallSlotAction(string slotName)
        {
            if (_slotFactories.TryGetValue(slotName, out var factory))
            {
                var slot = Slot.Get(slotName);
                if (slot != null) slot.InstallSubtree(factory(_manager));
            }
        }

        public PlayableGraph Graph => _animBuild.Graph;
        public void Update() => _animBuild.Update();
        public void Destroy() => _animBuild.DestroyGraph();
    }
}