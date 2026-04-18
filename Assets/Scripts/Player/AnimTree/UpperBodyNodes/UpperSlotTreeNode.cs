using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;

namespace Player.AnimTree.UpperBodyNodes
{
    public class UpperSlotTreeNode : AnimTreeSlotNode<PlayerManager>
    {
        public UpperSlotTreeNode(PlayerManager myMessage) : base(myMessage)
        {
            
        }
        
        public override bool CheckState(bool parentActive)
        {
            if (!parentActive) return false;

            // 1. 检查当前正在工作的武器子树 (比如 ReloadTreeLeafNode)
            if (_activeSubtree != null && _activeSubtree.CheckState(true)) 
            {
                // Debug.Log("插槽：检测到武器子树请求激活");
                return true;
            }

            // 2. 检查正在淡出的旧武器子树 (防止切换瞬间姿态消失)
            if (_fadingSubtree != null)
            {
                return true;
            }

            return false;
        }
    }
}