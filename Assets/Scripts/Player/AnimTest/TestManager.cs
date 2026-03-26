using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using Player.AnimPro;

// ==============================================
// 1. 管理器更新
// ==============================================
public class TestManager : MonoBehaviour
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
public class UpperBodyNode : AnimTreeLayerMixNode<TestManager>
{
    public UpperBodyNode(TestManager manager, AvatarMask mask) : base(manager)
    {
        // Layer 0: 基础移动层 (不带Mask)
        AddLayer(new BaseMovementMixer(manager), null);
        
        // Layer 1: 换弹层 (带上半身Mask)
        AddLayer(new AnimTreeReloadNode(manager), mask);
    }

    // 只要管理器没结束，LayerMixer 自身总是激活的
    public override bool CheckState(bool parentActive) => parentActive && !MyManager.endAll;
}

// ==============================================
// 3. 基础移动层 (Selector Mixer)
// ==============================================
public class BaseMovementMixer : AnimTreeMixNode<TestManager>
{
    public BaseMovementMixer(TestManager manager) : base(manager)
    {
        AddNode(new IdleNode(manager));
        AddNode(new MoveSubMixer(manager));
    }

    public override bool CheckState(bool parentActive) => parentActive;
}

// ==============================================
// 4. 移动子树 (Selector Mixer)
// ==============================================
public class MoveSubMixer : AnimTreeMixNode<TestManager>
{
    public MoveSubMixer(TestManager manager) : base(manager)
    {
        AddNode(new WalkNode(manager));
        AddNode(new SprintNode(manager));
    }

    public override bool CheckState(bool parentActive) => parentActive && MyManager.moveKeyPressed;
}

// ==============================================
// 5. 换弹节点 (Leaf)
// ==============================================
public class AnimTreeReloadNode : AnimTreeLeafNode<TestManager>
{
    private AnimationClipPlayable _playable;
    private float _clipDuration;

    public AnimTreeReloadNode(TestManager manager) : base(manager) { }

    public override bool CheckState(bool parentActive) => parentActive && MyManager.isReload;

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
        if (currentWeight > 0.5f && MyManager.isReload)
        {
            if (_playable.GetTime() >= _clipDuration)
            {
                Debug.Log("Reload Finished.");
                MyManager.isReload = false; // 自动重置 Manager 状态
            }
        }
        return currentWeight;
    }

    public override void OnExitState() => Debug.Log("Exit Reloading");
}

// ==============================================
// 6. 其他叶子节点 (Idle, Walk, Sprint)
// ==============================================

public class IdleNode : AnimTreeLeafNode<TestManager>
{
    public IdleNode(TestManager manager) : base(manager) { }
    public override bool CheckState(bool parentActive) => parentActive && !MyManager.moveKeyPressed;
    public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
    {
        var cfg = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Idle/RifleAimingIdle.asset");
        return AnimationClipPlayable.Create(graph, cfg.animClip);
    }
}

public class WalkNode : AnimTreeLeafNode<TestManager>
{
    public WalkNode(TestManager manager) : base(manager) { }
    public override bool CheckState(bool parentActive) => parentActive && !MyManager.isSprint;
    public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
    {
        var cfg = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Walk/RifleWalkForward.asset");
        return AnimationClipPlayable.Create(graph, cfg.animClip);
    }
}

public class SprintNode : AnimTreeLeafNode<TestManager>
{
    public SprintNode(TestManager manager) : base(manager) { }
    public override bool CheckState(bool parentActive) => parentActive && MyManager.isSprint;
    public override Playable BuildNode(PlayableGraph graph, int mixIndex = -1)
    {
        var cfg = GetAnimConfig("Assets/AssetBundle/ScriptableObject/Anim/Rifle/Run/RifleRun.asset");
        return AnimationClipPlayable.Create(graph, cfg.animClip);
    }
}