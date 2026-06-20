using System;
using System.Collections.Generic;
using mEnum;
using mInterface;
using Player.AnimPro;
using UnityEngine.Playables;

namespace Player.AnimTree
{
    public class PlayerAnimController
    {
        private IAnimContext _ctx;
        private PlayerAnimTreeBuild _animBuild;
        private WeaponType _currentWeaponType;
        private Dictionary<string, Func<IAnimContext, AnimTreeNode<IAnimContext>>> _slotFactories = new();
        public PlayerAnimSlotTreeRoot Root;
        public PlayerAnimSlot Slot;

        public PlayerAnimController(IAnimContext ctx, AnimGraphData defaultTree)
        {
            _ctx = ctx;
            Slot = new PlayerAnimSlot();
            _animBuild = new PlayerAnimTreeBuild(ctx, defaultTree, Slot);
            Root = _animBuild.RootNode;
        }

        /// <summary>
        /// 武器类型变化时，切换动画树。
        /// </summary>
        public void SwitchWeaponGraph(AnimGraphData weaponGraph, WeaponType weaponType)
        {
            if (_currentWeaponType == weaponType) return;
            _currentWeaponType = weaponType;
            _slotFactories.Clear();
            Slot.Clear();
            Root.InstallSubtree(AnimGraphRuntimeBuilder.Build(weaponGraph, _ctx, Slot));
        }

        /// <summary>
        /// 给指定槽安装特殊动作节点（换弹、拉栓等），并缓存 factory 以便后续重装
        /// </summary>
        public void InstallSlotAction(string slotName, Func<IAnimContext, AnimTreeNode<IAnimContext>> factory)
        {
            _slotFactories[slotName] = factory;
            var slot = Slot.Get(slotName);
            if (slot != null) slot.InstallSubtree(factory(_ctx));
        }

        /// <summary>
        /// 重新安装之前装过的槽位动作
        /// </summary>
        public void ReinstallSlotAction(string slotName)
        {
            if (_slotFactories.TryGetValue(slotName, out var factory))
            {
                var slot = Slot.Get(slotName);
                if (slot != null) slot.InstallSubtree(factory(_ctx));
            }
        }

        public PlayableGraph Graph => _animBuild.Graph;
        public void Update() => _animBuild.Update();
        public void Destroy() => _animBuild.DestroyGraph();
    }
}