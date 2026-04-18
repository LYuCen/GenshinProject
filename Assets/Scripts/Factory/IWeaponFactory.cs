using Firearms;
using GameFSM.FSM.PlayerFSM;
using mEnum;
using mModel.ScriptableConfig;
using Player.AnimPro;

namespace Factory
{
    public interface IWeaponFactory
    {
        IWeapon GetWeapon();
        WeaponType GetWeaponType();
        WeaponGunConfig GetConfig(); // 新增：方便获取动画路径
        AnimTreeNode<PlayerManager> CreateActionNode();
    }
}