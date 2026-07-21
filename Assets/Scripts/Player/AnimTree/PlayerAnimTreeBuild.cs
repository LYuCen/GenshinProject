using mInterface;
using Player.AnimPro;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTree
{
    public class PlayerAnimTreeBuild
    {
        private PlayableGraph _graph;
        private PlayerAnimSlotTreeRoot _rootNode; // 使用新架构的根节点

        public PlayerAnimSlotTreeRoot RootNode => _rootNode;
        public PlayableGraph Graph => _graph;

        public PlayerAnimTreeBuild(IAnimContext ctx, AnimGraphData defaultTree, PlayerAnimSlot slot)
        {
            _graph = PlayableGraph.Create("PlayerAnimGraph");
            
            _rootNode = new PlayerAnimSlotTreeRoot(ctx);
            
            var rootPlayable = _rootNode.BuildNode(_graph);
            
            var output = AnimationPlayableOutput.Create(_graph, "Animation", ctx.Animator);
            output.SetSourcePlayable(rootPlayable);
            
            _rootNode.SetupMainTree(defaultTree, slot);

            _graph.Play();
        }
        
        
        /// <summary>
        /// 更新动画 IK 权重
        /// </summary>
        public void Update()
        {
            // 更新动画树逻辑
            _rootNode.Evaluate();
            _rootNode.UpdateNode();
        }

        public void DestroyGraph()
        {
            if (_graph.IsValid()) _graph.Destroy();
        }
    }
}