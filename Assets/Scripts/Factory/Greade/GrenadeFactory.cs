using Firearms;
using Firearms.ThrowWeapon;
using GameFSM.FSM.PlayerFSM;
using mEnum;
using mModel.ScriptableConfig;
using Player.AnimPro;
using Player.AnimTree.UpperBodyNodes.GrenadeSubTreeNodes;

namespace Factory.Greade
{
    public class GrenadeFactory : IWeaponFactory
    {
        private PlayerManager _manager;
        private WeaponGunConfig _config;

        public GrenadeFactory(PlayerManager manager)
        {
            _manager = manager;
        }

        public IWeapon GetWeapon()
        {
            Grenade g = new Grenade();
            g.Init();
            _config = g.GetWeaponConfig();
            return g;
        }

        public WeaponGunConfig GetConfig() => _config;
        public AnimTreeNode<PlayerManager> CreateActionNode()
        {
            return new GrenadeThrowSubTree(_manager);
        }

        public WeaponType GetWeaponType() => WeaponType.Grenade;
    }
}