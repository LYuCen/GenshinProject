using Firearms;
using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using Player.AnimTree.UpperBodyNodes.GrenadeSubTreeNodes.UpperSubTreeNodes;
using UnityEngine;

namespace Player.AnimTree.UpperBodyNodes.GrenadeSubTreeNodes
{
    public class GrenadeThrowSubTree : AnimTreeMixNode<PlayerManager>
    {
        private AbstractThrow _throw;
        
        public GrenadeThrowSubTree(PlayerManager myManager) : base(myManager)
        {
            if (MyMessage.firearmController.Weapon is AbstractThrow res)
            {
                _throw = res;
                AddNode(new UpperSubTree(MyMessage, _throw));
            }
            else Debug.LogWarning("[GrenadeThrowAnim] 未获取到投掷物");
        }

        public override bool CheckState(bool parentActive)
        {
            return parentActive && _throw != null && MyMessage.IsThrow;
        }
    }
}