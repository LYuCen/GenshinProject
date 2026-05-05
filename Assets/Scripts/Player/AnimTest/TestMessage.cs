using Player.AnimPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Player.AnimTest
{
    // ==============================================
// 1. 管理器更新
// ==============================================
    public class TestMessage : MonoBehaviour
    {
        public Animator animator;
        public AvatarMask upperBodyMask; // 在 Inspector 中赋值
        private PlayableGraph _graph;
        private UpperBodyNode _rootNode;

        [Header("Conditions")]
        public bool moveKeyPressed;
        public bool isSprint;
        public bool isReload; // 换弹状态
        public bool endAll;

        void Start()
        {
            _graph = PlayableGraph.Create("AnimProGraph");
        
            // 构建树结构
            _rootNode = new UpperBodyNode(this, upperBodyMask);
        
            var rootPlayable = _rootNode.BuildNode(_graph);
            var output = AnimationPlayableOutput.Create(_graph, "Animation", animator);
            output.SetSourcePlayable(rootPlayable);
        
            _graph.Play();
        }

        void FixedUpdate()
        {
            // 逻辑输入：按下 R 键且不在换弹时触发
            if (Input.GetKeyDown(KeyCode.R) && !isReload) isReload = true;

            _rootNode.Evaluate();
            _rootNode.UpdateNode();
        }

        void OnDestroy() { if (_graph.IsValid()) _graph.Destroy(); }
    }

// ==============================================
// 2. UpperBodyNode (LayerMixer)
// ==============================================
    public class UpperBodyNode : AnimTreeLayerMixNode<TestMessage>
    {
        public UpperBodyNode(TestMessage message, AvatarMask mask) : base(message)
        {
            // Layer 0: 基础移动层 (不带Mask)
            AddLayer(new BaseMovementMixer(message), null);
        
            // Layer 1: 换弹层 (带上半身Mask)
            AddLayer(new AnimTreeReloadNode(message), mask);
        }

        // 只要管理器没结束，LayerMixer 自身总是激活的
        public override bool CheckState(bool parentActive) => parentActive && !MyMessage.endAll;
    }

// ==============================================
// 3. 基础移动层 (Selector Mixer)
// ==============================================
    public class BaseMovementMixer : AnimTreeMixNode<TestMessage>
    {
        public BaseMovementMixer(TestMessage message) : base(message)
        {
            AddNode(new IdleNode(message));
            AddNode(new MoveSubMixer(message));
        }

        public override bool CheckState(bool parentActive) => parentActive;
    }

// ==============================================
// 4. 移动子树 (Selector Mixer)
// ==============================================
    public class MoveSubMixer : AnimTreeMixNode<TestMessage>
    {
        public MoveSubMixer(TestMessage message) : base(message)
        {
            AddNode(new WalkNode(message));
            AddNode(new SprintNode(message));
        }

        public override bool CheckState(bool parentActive) => parentActive && MyMessage.moveKeyPressed;
    }

// ==============================================
// 5. 换弹节点 (Leaf)
// ==============================================
    public class AnimTreeReloadNode : AnimTreeLeafNode<TestMessage>
    {
        private AnimationClipPlayable _playable;
        private float _clipDuration;

        public AnimTreeReloadNode(TestMessage message) : base(message) { }

        public override bool CheckState(bool parentActive) => parentActive && MyMessage.isReload;

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            var cfg = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Weapon/Gun/FN_IWS/ReloadAnim.asset");
            _playable = AnimationClipPlayable.Create(graph, cfg.animClip);
            _playable.SetSpeed(cfg.animSpeed);
            _clipDuration = cfg.animClip.length;
            return _playable;
        }

        public override void OnEnterState()
        {
            Debug.Log("Start Reloading...");
            _playable.SetTime(0); // 每次进入时重置时间
        }

        public override float UpdateNode()
        {
            float currentWeight = base.UpdateNode();

            // 逻辑：如果当前权重较大（已切入），且动画播放完毕，则重置状态
            if (currentWeight > 0.5f && MyMessage.isReload)
            {
                if (_playable.GetTime() >= _clipDuration)
                {
                    Debug.Log("Reload Finished.");
                    MyMessage.isReload = false; // 自动重置 Manager 状态
                }
            }
            return currentWeight;
        }

        public override void OnExitState() => Debug.Log("Exit Reloading");
    }

// ==============================================
// 6. 其他叶子节点 (Idle, Walk, Sprint)
// ==============================================

    public class IdleNode : AnimTreeLeafNode<TestMessage>
    {
        public IdleNode(TestMessage message) : base(message) { }
        public override bool CheckState(bool parentActive) => parentActive && !MyMessage.moveKeyPressed;
        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            var cfg = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Idle/RifleAimingIdle.asset");
            return AnimationClipPlayable.Create(graph, cfg.animClip);
        }
    }

    public class WalkNode : AnimTreeLeafNode<TestMessage>
    {
        public WalkNode(TestMessage message) : base(message) { }
        public override bool CheckState(bool parentActive) => parentActive && !MyMessage.isSprint;
        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            var cfg = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Walk/RifleWalkForward.asset");
            return AnimationClipPlayable.Create(graph, cfg.animClip);
        }
    }

    public class SprintNode : AnimTreeLeafNode<TestMessage>
    {
        public SprintNode(TestMessage message) : base(message) { }
        public override bool CheckState(bool parentActive) => parentActive && MyMessage.isSprint;
        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            var cfg = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Run/RifleRun.asset");
            return AnimationClipPlayable.Create(graph, cfg.animClip);
        }
    }
}