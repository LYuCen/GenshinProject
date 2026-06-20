using mInterface;
using Player.AnimPro;
using UnityEngine;

namespace Player.AnimTree.UpperBodyNodes
{
    /// <summary>
    /// 上半身混合节点
    /// </summary>
    public class UpperBodySubTree : AnimTreeMixNode<IAnimContext>
    {
        public UpperBodySubTree(IAnimContext ctx) : base(ctx)
        {
        }

        /// <summary>
        /// 只有当父节点激活，且【至少有一个子节点】满足条件时，该 Mixer 节点才返回激活。
        /// </summary>
        public override bool CheckState(bool parentActive)
        {
            if (!parentActive) return false;

            // 遍历所有子节点，检查是否有任何一个子节点满足触发条件
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].CheckState(true))
                {
                    return true;
                }
            }

            // 如果所有子节点都不满足条件，我也不激活
            return false;
        }
    }
}
