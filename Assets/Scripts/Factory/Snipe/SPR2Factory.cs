using Firearms;
using Firearms.Gun;
using GameFSM.LevelFSM.PlayerFSM;
using mEnum;
using mModel.ScriptableConfig;
using Player.AnimPro;
using Player.AnimTree.UpperBodyNodes;

namespace Factory.Snipe
{
    public class SPR2Factory : IWeaponFactory
    {
        private PlayerManager _manager;
        private WeaponGunConfig _config;

        public SPR2Factory(PlayerManager manager)
        {
            _manager = manager;
        }

        public IWeapon GetWeapon()
        {
            SPR2 spr2 = new SPR2();
            spr2.Init();
            _config = spr2.GetWeaponConfig();

            return spr2;
        }

        public WeaponGunConfig GetConfig() => _config;
        public WeaponType GetWeaponType() => WeaponType.Snipe;
        
        public AnimTreeNode<PlayerManager> CreateActionNode()
        {
            // 返回换弹节点
            ReloadTreeLeafNode node = new ReloadTreeLeafNode(_manager);
            node.AnimSpeed = 1.2f;
            node.Tag = "Reload";
            node.UseTagOnly = true;
            return node;
        }
    }
}