using Firearms;
using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;

namespace Player.AnimTree.UpperBodyNodes.GrenadeSubTreeNodes.UpperSubTreeNodes
{
    public class UpperSubTree : AnimTreeMixNode<PlayerManager>
    {
        private AbstractThrow _throw;
        
        public UpperSubTree(PlayerManager myManager, AbstractThrow s) : base(myManager)
        {
            _throw = s;
            AddNode(new UpperThrowStartTreeLeafNode(MyMessage, _throw));
            AddNode(new UpperThrowLoopTreeLeafNode(MyMessage, _throw));
            AddNode(new UpperThrowEndTreeLeafNode(MyMessage, _throw));
        }

        public override bool CheckState(bool parentActive)
        {
            return parentActive && !_throw.GetIsDownThrow();
        }
    }
}