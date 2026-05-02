using GameFSM.LevelFSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine;

namespace Player.AnimTree.LowerBodyNodes.MoveSubTreeNodes.SprintSubTreeNodes
{
    public class SprintSubTree : AnimTreeMixNode<PlayerManager>
    {
        public SprintSubTree(PlayerManager myMessage) : base(myMessage)
        {
        }

        public override bool CheckState(bool parentActive)
        {
            // 处于奔跑状态并且方向为 前，前左，前右
            return parentActive && MyMessage.IsSprint && 
                   (MyMessage.PlayerMove == Vector2.up ||
                    MyMessage.PlayerMove == new Vector2(-1, 1) ||
                    MyMessage.PlayerMove == new Vector2(1, 1));
        }
    }
}