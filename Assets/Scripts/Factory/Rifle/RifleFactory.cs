using System.Collections.Generic;
using Firearms;
using GameFSM.FSM.PlayerFSM;
using mEnum;
using Player.AnimPro;
using Player.AnimTree.CrouchLowerBodyNode;
using Player.AnimTree.CrouchLowerBodyNode.MoveSubTreeNodes.SprintSubTreeNodes;
using Player.AnimTree.CrouchLowerBodyNode.MoveSubTreeNodes.WalkSubTreeNodes;
using Player.AnimTree.LowerBodyNodes;
using Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.SprintSubTreeNodes;
using Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.WalkSubTreeNodes;
using UnityEngine;

namespace Factory.Rifle
{
    public class RifleFactory : IWeaponTypeFactory
    {
        private PlayerManager _myManager;
        private Dictionary<string, IWeaponFactory> _gunFactories = new();

        public RifleFactory(PlayerManager manager)
        {
            _myManager = manager;
            // 注册具体步枪生产线
            _gunFactories.Add("FN_IWS", new FN_IWSFactory(manager));
            _gunFactories.Add("MK14", new MK14Factory(manager));
        }

        public IEnumerable<string> GetSupportedWeapons() => _gunFactories.Keys;

        public WeaponType GetWeaponType() => WeaponType.Rifle;

        public IWeapon GetWeapon(string name)
        {
            return _gunFactories[name].GetWeapon();
        }

        public void UpdateTypeAnimations()
        {
            // 随便找一个该类型的配置（因为同类型移动动画是一样的）
            // 或者你可以专门为 Rifle 做一个全局配置
            var anyGun = _gunFactories.Values.GetEnumerator();
            anyGun.MoveNext();
            var cfg = anyGun.Current.GetConfig();

            Debug.Log($"[RifleFactory] 检测到类型切换，更新移动动画集");
            
            var slots = _myManager.AnimController.Slot;
            slots.IdleSlot.InstallSubtree(new IdleTreeLeafNode(_myManager, cfg.idleAnimPath));
            slots.WalkSlot.InstallSubtree(new WalkTreeLeafNode(_myManager, cfg.walkAnimPath));
            slots.SprintSlot.InstallSubtree(new SprintTreeLeafNode(_myManager, cfg.sprintAnimPath));
            slots.CIdleSlot.InstallSubtree(new CrouchIdleTreeLeafNode(_myManager, cfg.cIdleAnimPath));
            slots.CWalkSlot.InstallSubtree(new CrouchWalkTreeLeafNode(_myManager, cfg.cWalkAnimPath));
            slots.CSprintSlot.InstallSubtree(new CrouchSprintTreeLeafNode(_myManager, cfg.cSprintAnimPath));
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