using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using Player.AnimTree.CrouchLowerBodyNode;
using Player.AnimTree.LowerBodyNodes;
using Player.AnimTree.UpperBodyNodes;
using UnityEngine;

namespace Player.AnimTree
{
    public class PlayerAnimTreeRoot : AnimTreeLayerMixNode<PlayerManager>
    {
        public PlayerAnimTreeRoot(PlayerManager myManager, AvatarMask upperBodyMask) : base(myManager)
        {
            // 第 0 层：全身层（通常放 Idle, Walk, Run）
            AddLayer(new LowerBodySubTree(MyMessage));
            AddLayer(new CrouchLowerBodySubTree(MyMessage));
            
            // 第 1 层：上半身叠加层（通常放 Reload, Attack, Aim）
            // 使用传入的 AvatarMask
            AddLayer(new UpperBodySubTree(MyMessage), upperBodyMask);
        }

        // 根节点始终保持激活
        public override bool CheckState(bool parentActive) => true;
    }
}