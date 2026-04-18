using Firearms;
using Firearms.Gun;
using GameFSM.FSM.PlayerFSM;
using mEnum;
using mModel.ScriptableConfig;
using Player.AnimPro;
using Player.AnimTree.UpperBodyNodes;

namespace Factory.Rifle
{
    public class HK416Factory : IWeaponFactory
    {
        private PlayerManager _manager;
        private WeaponGunConfig _config;

        public HK416Factory(PlayerManager manager)
        {
            _manager = manager;
        }

        public IWeapon GetWeapon()
        {
            HK416 hk416 = new HK416();
            hk416.Init();
            _config = hk416.GetWeaponConfig(); // 假设 FN_IWS 内部持有了 Config

            

            return hk416;
        }

        public WeaponGunConfig GetConfig() => _config;
        public WeaponType GetWeaponType() => WeaponType.Rifle;
        
        public AnimTreeNode<PlayerManager> CreateActionNode()
        {
            // 步枪只返回换弹节点
            ReloadTreeLeafNode node = new ReloadTreeLeafNode(_manager);
            node.AnimSpeed = 1.2f;
            node.Tag = "Reload";
            node.UseTagOnly = true;
            return node;
        }
    }
}