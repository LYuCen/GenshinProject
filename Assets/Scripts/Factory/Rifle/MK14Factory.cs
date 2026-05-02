using AssetBundleFramework.Core.Resource;
using Firearms;
using Firearms.Gun;
using GameFSM.FSM.PlayerFSM;
using GameFSM.LevelFSM.PlayerFSM;
using mEnum;
using mModel.ScriptableConfig;
using Player.AnimPro;
using Player.AnimTree.UpperBodyNodes;

namespace Factory.Rifle
{
    public class MK14Factory : IWeaponFactory
    {
        private PlayerManager _manager;
        private WeaponGunConfig _config;

        public MK14Factory(PlayerManager manager)
        {
            _manager = manager;
        }

        public IWeapon GetWeapon()
        {
            MK14 mk14 = new MK14();
            mk14.Init();
            _config = mk14.GetWeaponConfig(); // 假设 FN_IWS 内部持有了 Config
            

            return mk14;
        }

        public WeaponGunConfig GetConfig() => _config;
        public WeaponType GetWeaponType() => WeaponType.Rifle;
        
        public AnimTreeNode<PlayerManager> CreateActionNode()
        {
            // 步枪只返回换弹节点
            ReloadTreeLeafNode node = new ReloadTreeLeafNode(_manager);
            node.AnimSpeed = 1.7f;
            node.Tag = "Reload";
            node.UseTagOnly = true;
            var ir = ResourceManager.instance.Load(
                "Assets/AssetBundle/ScriptableObject/Anim/Weapon/Gun/MK14/Weapon_Mk14_Reload_Charge_BaseAsset.asset",
                false);
            node.CurrentData = ir.GetAsset<WeaponClipAsset>();
            return node;
        }
    }
}