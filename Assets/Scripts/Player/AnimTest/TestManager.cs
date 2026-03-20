using AssetBundleFramework.Core.Resource;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using Player.AnimTest;

// ======================================
// 管理器（示例）
// ==============================================
    // 管理器示例
    // ==============================================
    // ====================
    // 测试管理器
    // ====================
    public class TestManager : MonoBehaviour
    {
        public Animator animator;
        private PlayableGraph _graph;
        private RootTestState _rootState;

        public bool moveKeyPressed;
        public bool isSprint;
        public bool endAll;

        void Start()
        {
            _graph = PlayableGraph.Create("TestAnimGraph");
            _rootState = new RootTestState(this);
            var rootPlayable = _rootState.BuildNode(_graph);
            var output = AnimationPlayableOutput.Create(_graph, "Animation", animator);
            output.SetSourcePlayable(rootPlayable);
            _graph.Play();
        }

        void FixedUpdate()
        {
            _rootState.Evaluate();
            _rootState.UpdateNode();
        }

        void OnDestroy() { _graph.Destroy(); }
    }

    // ====================
    // 根节点
    // ====================
    public class RootTestState : AnimMixTestState<TestManager>
    {
        public RootTestState(TestManager manager) : base(manager)
        {
            AddState(new IdleState(manager));
            AddState(new MoveTestState(manager));
        }
        protected override bool CheckState(bool parentActive) => parentActive && !MyManager.endAll;
    }

    // ====================
    // Idle
    // ====================
    public class IdleState : AnimTestState<TestManager>
    {
        public IdleState(TestManager manager) : base(manager) => IsLeaf = true;

        protected override bool CheckState(bool parentActive)
        {
            if (!parentActive) return false;
            return !MyManager.moveKeyPressed && !MyManager.endAll;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            var cfg = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Idle/RifleAimingIdle.asset");
            var playable = AnimationClipPlayable.Create(graph, cfg.animClip);
            playable.SetSpeed(cfg.animSpeed);
            return playable;
        }

        public override void OnEnterState() => Debug.Log("Enter Idle");
        public override void OnExitState() => Debug.Log("Exit Idle");
    }

    // ====================
    // Walk
    // ====================
    public class WalkState : AnimTestState<TestManager>
    {
        public WalkState(TestManager manager) : base(manager) => IsLeaf = true;

        protected override bool CheckState(bool parentActive)
        {
            if (!parentActive) return false;
            return MyManager.moveKeyPressed && !MyManager.isSprint;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            var cfg = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Walk/RifleWalkForward.asset");
            var playable = AnimationClipPlayable.Create(graph, cfg.animClip);
            playable.SetSpeed(cfg.animSpeed);
            return playable;
        }

        public override void OnEnterState() => Debug.Log("Enter Walk");
        public override void OnExitState() => Debug.Log("Exit Walk");
    }

    // ====================
    // Sprint
    // ====================
    public class SprintState : AnimTestState<TestManager>
    {
        public SprintState(TestManager manager) : base(manager) => IsLeaf = true;

        protected override bool CheckState(bool parentActive)
        {
            if (!parentActive) return false;
            return MyManager.moveKeyPressed && MyManager.isSprint;
        }

        public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
        {
            var cfg = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Weapon/Grenade/GrenadeSprintF.asset");
            var playable = AnimationClipPlayable.Create(graph, cfg.animClip);
            playable.SetSpeed(cfg.animSpeed);
            return playable;
        }

        public override void OnEnterState() => Debug.Log("Enter Sprint");
        public override void OnExitState() => Debug.Log("Exit Sprint");
    }

    // ====================
    // MoveTestState
    // ====================
    public class MoveTestState : AnimMixTestState<TestManager>
    {
        public MoveTestState(TestManager manager) : base(manager)
        {
            AddState(new WalkState(manager));
            AddState(new SprintState(manager));
        }

        protected override bool CheckState(bool parentActive)
        {
            if (!parentActive) return false;
            return MyManager.moveKeyPressed && !MyManager.endAll;
        }
    }