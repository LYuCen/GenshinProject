using System.Collections.Generic;
using Firearms;
using GameFSM.FSM.PlayerFSM;
using mEnum;
using Player.AnimPro;

namespace Factory
{
    public interface IWeaponTypeFactory
    {
        IWeapon GetWeapon(string name);
        WeaponType GetWeaponType();
        IEnumerable<string> GetSupportedWeapons(); // 返回此工厂支持的所有武器名
        void UpdateTypeAnimations(); // 更新该类型的移动动画集
        
        AnimTreeNode<PlayerManager> CreateSpecialActionNode(string name);
    }
}