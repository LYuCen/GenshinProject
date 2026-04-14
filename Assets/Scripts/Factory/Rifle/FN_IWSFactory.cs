using Firearms;
using Firearms.Gun;
using GameFSM.FSM.PlayerFSM;
using mEnum;
using mModel.ScriptableConfig;
using Player.AnimPro;
using Player.AnimTree.UpperBodyNodes;

namespace Factory.Rifle
{
    public class FN_IWSFactory : IWeaponFactory
    {
        private PlayerManager _manager;
        private WeaponGunConfig _config;

        public FN_IWSFactory(PlayerManager manager)
        {
            _manager = manager;
        }

        public IWeapon GetWeapon()
        {
            FN_IWS fnIws = new FN_IWS();
            fnIws.Init();
            _config = fnIws.GetWeaponConfig(); // 假设 FN_IWS 内部持有了 Config

            // 无论什么枪，换弹动作几乎都是唯一的，所以每次 GetWeapon 都要更新 ReloadSlot
            var node = new ReloadTreeLeafNode(_manager, _config.reloadAnimConfig);
            _manager.AnimController.Slot.UpperSlot.InstallSubtree(node);

            return fnIws;
        }

        public WeaponGunConfig GetConfig() => _config;
        public WeaponType GetWeaponType() => WeaponType.Rifle;
        
        public AnimTreeNode<PlayerManager> CreateActionNode()
        {
            // 步枪只返回换弹节点
            return new ReloadTreeLeafNode(_manager, _config.reloadAnimConfig);
        }
    }
}