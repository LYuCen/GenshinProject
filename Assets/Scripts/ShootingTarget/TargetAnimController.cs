using Player.AnimPro;
using Player.AnimTree;
using Server;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace ShootingTarget
{
    public class TargetAnimController
    {
        private PlayerAnimSlotTreeRoot _rootNode;
        private PlayableGraph _graph;
        public PlayerAnimSlot Slot;
        
        public TargetAnimController(AnimGraphData data, ServerAnimContext context)
        {
            Slot = new PlayerAnimSlot();
            
            _graph = PlayableGraph.Create("target");
            _rootNode = new PlayerAnimSlotTreeRoot(context);

            var rootplayable = _rootNode.BuildNode(_graph);
            var output = AnimationPlayableOutput.Create(_graph, "Animator", context.Animator);
            output.SetSourcePlayable(rootplayable);
            
            _rootNode.SetupMainTree(data, Slot);

            _graph.Play();
        }
        
        public void Update()
        {
            // 更新动画树逻辑
            _rootNode.Evaluate();
            _rootNode.UpdateNode();
        }
    }
}