using GameFSM.FSM.PlayerFSM;
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

        public PlayerAnimTreeBuild(PlayerManager manager, AnimGraphData defaultTree, PlayerAnimSlot slot)
        {
            _graph = PlayableGraph.Create("PlayerAnimGraph");
        
            // 1. 创建根插槽
            _rootNode = new PlayerAnimSlotTreeRoot(manager);
        
            // 2. 构建 Playable 节点
            var rootPlayable = _rootNode.BuildNode(_graph);
        
            // 3. 将根节点连接到 Graph 输出
            var output = AnimationPlayableOutput.Create(_graph, "Animation", manager.animator);
            output.SetSourcePlayable(rootPlayable);

            // 4. 安装你从编辑器里做好的那棵“主树”
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