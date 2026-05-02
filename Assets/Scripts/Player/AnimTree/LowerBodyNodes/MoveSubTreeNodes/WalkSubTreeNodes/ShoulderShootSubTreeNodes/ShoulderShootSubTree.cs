using GameFSM.LevelFSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.WalkSubTreeNodes.ShoulderShootSubTreeNodes
{
    /// <summary>
    /// 肩射瞄准子树
    /// </summary>
    public class ShoulderShootSubTree : AnimTreeMixNode<PlayerManager>
    {
        public ShoulderShootSubTree(PlayerManager myManager) : base(myManager) { }

        public override bool CheckState(bool parentActive)
        {
            // 处于移动状态并且不处与奔跑的前，前左，前右移动
            return parentActive && 
                   (!MyMessage.IsSprint || 
                    !(MyMessage.PlayerMove == Vector2.up || 
                      MyMessage.PlayerMove == new Vector2(1, 1) || 
                      MyMessage.PlayerMove == new Vector2(-1, 1)));
        }
    }
}