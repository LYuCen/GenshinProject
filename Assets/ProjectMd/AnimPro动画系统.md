# AnimPro 动画系统

## 概述

AnimPro 是一套基于 Unity Playable API 的可视化动画树系统。核心设计理念是将动画状态机的**逻辑决策**与**混合播放**统一为树形结构，通过数据驱动（ScriptableObject）构建运行时的 PlayableGraph，避免传统 Animator Controller 的状态爆炸和维护困难。

---

## 架构总览

```
┌─────────────────────────────────────────────────────────┐
│ AnimGraphData (ScriptableObject)                        │
│   Nodes: [{ GUID, NodeType, Tag, DefaultClip, Mask }]   │
│   Edges: [{ ParentGUID → ChildGUID }]                   │
└──────────────────┬──────────────────────────────────────┘
                   │ AnimGraphRuntimeBuilder.Build()
                   ▼
┌─────────────────────────────────────────────────────────┐
│ AnimTreeNode<IAnimContext> (运行时动画树)                │
│                                                         │
│  PlayerAnimSlotTreeRoot (AnimTreeSlotNode)              │
│  ├── LowerBodySubTree (AnimTreeMixNode)                 │
│  │   ├── IdleTreeLeafNode                              │
│  │   ├── MoveSubTree (AnimTreeMixNode)                 │
│  │   │   ├── SprintSubTree ← SprintForward/FL/FR       │
│  │   │   └── WalkSubTree   ← WalkF/L/R/FL/FR/BL/BR    │
│  │   ├── JumpTreeLeafNode                              │
│  │   └── CrouchLowerBodySubTree                        │
│  ├── UpperBodySubTree (AnimTreeMixNode)                │
│  │   ├── UpperSlotTreeNode (AnimTreeSlotNode)          │
│  │   │   └── ReloadTreeLeafNode (动态安装)              │
│  │   ├── BoltTreeLeafNode                              │
│  │   └── GrenadeThrowSubTree                           │
│  └── SlidingAimTreeNode                                │
└──────────────────┬──────────────────────────────────────┘
                   │ PlayableGraph
                   ▼
              Animator (Unity)
```

---

## 节点类型体系

所有节点继承自 `AnimTreeNode<T>`（泛型参数 `T` 即上下文类型，当前统一为 `IAnimContext`）。

### 四种节点

| 节点类型 | 基类 | 对应 Playable | 职责 |
|---------|------|--------------|------|
| **LeafNode** | `AnimTreeLeafNode<T>` | `AnimationClipPlayable` | 播放单个动画片段 |
| **MixNode** | `AnimTreeMixNode<T>` | `AnimationMixerPlayable` | 在多个子节点间**互斥**混合（归一化到 1.0） |
| **LayerMixNode** | `AnimTreeLayerMixNode<T>` | `AnimationLayerMixerPlayable` | 多层**叠加**混合（支持 AvatarMask） |
| **SlotNode** | `AnimTreeSlotNode<T>` | `AnimationMixerPlayable` (2 路) | 动态替换子树，支持 Crossfade 过渡 |

### 节点生命周期

```
BuildNode(graph)  →  OnEnterState()  →  [ Evaluate() → UpdateNode() 循环 ]
                                   →  OnExitState()
```

- `BuildNode`：创建 Playable 节点并连接到图中（仅调用一次）
- `CheckState(state)`：纯函数，查询 `IAnimContext` 判断条件是否满足，**无副作用**
- `Evaluate(keepPoseAlive)`：从上到下传播激活指令，设置每个节点的 `TargetWeight`
- `UpdateNode()`：权重平滑过渡（`Mathf.MoveTowards`），触发 Enter/Exit，设置 Mixer 端口权重
- `OnEnterState / OnExitState`：进入/退出时的副作用（如重置播放时间、恢复 IK 权重）

### 关键设计：keepPoseAlive 机制

当父节点即将淡出但 `Weight > 0.001f` 时，会继续调用子节点的 `Evaluate(true)` 但设置 `TargetWeight = 0`。这确保：
- 子节点的 Playable 继续更新（姿态不会坍缩为 T-Pose）
- 子节点的权重逐渐衰减到 0（平滑过渡）

---

## 节点详解

### 1. AnimTreeLeafNode — 叶子节点

最底层的动画播放节点。一个 LeafNode 对应一个 `AnimationClip`。

```csharp
public abstract class AnimTreeLeafNode<T> : AnimTreeNode<T>
{
    public WeaponClipAsset CurrentData; // 武器动画曲线数据（换弹/拉栓用）

    public override void Evaluate(bool parentActive)
    {
        bool conditionMet = CheckState(true);
        bool selfActive = parentActive && conditionMet;
        TargetWeight = selfActive ? 1f : 0f;
    }

    public override float UpdateNode()
    {
        // 权重平滑逼近
        Weight = Mathf.MoveTowards(Weight, TargetWeight, Time.fixedDeltaTime * WeightSpeed);
        return Weight;
    }
}
```

**具体实现示例：**

| 节点 | CheckState 条件 |
|------|----------------|
| `IdleTreeLeafNode` | `Ground && !Move` |
| `WalkF` | `Ground && Forward && !Sprint` |
| `SprintForwardTreeNode` | `Sprint && Forward` |
| `JumpTreeLeafNode` | `Jump && !Ground` |
| `RifleSlidingTreeLeafNode` | `Sliding` |
| `ReloadTreeLeafNode` | `WeaponFlags.Reload` |
| `BoltTreeLeafNode` | `WeaponFlags.Bolt` |
| `SlidingAimTreeNode` | `Sliding`（上半身滑铲姿态） |

### 2. AnimTreeMixNode — 互斥混合节点

用于**互斥状态**之间的切换（如 Idle ↔ Walk ↔ Sprint）。内部使用 `AnimationMixerPlayable`，所有子节点权重强制归一化到 1.0。

关键行为：
- 选中**第一个**满足 `CheckState` 的子节点
- 混合器端口权重归一化（最大权重端口补齐到 1.0，防止向 BindPose 坍缩）
- 无子节点激活时，保持 `LastActiveChild` 防止姿态丢失

```csharp
// 权重归一化核心逻辑
float childWeightSum = 0f;
int maxWeightIndex = 0;
for (int i = 0; i < Children.Count; i++)
{
    childWeightSum += Children[i].Weight;
    if (Children[i].Weight > maxWeightVal)
    {
        maxWeightVal = Children[i].Weight;
        maxWeightIndex = i;
    }
}
// 低权重子节点先分配，最大权重子节点补齐到 1.0
SetInputWeight(maxWeightIndex, 1.0f - assignedSum);
```

**具体实现示例：**

| 节点 | 子节点 |
|------|--------|
| `MoveSubTree` | SprintSubTree → WalkSubTree |
| `SprintSubTree` | SprintForwardTreeNode / SprintForwardLeftTreeNode / SprintForwardRightTreeNode |
| `WalkSubTree` | WalkF / WalkL / WalkR / WalkFL / WalkFR / WalkBL / WalkBR |
| `LowerBodySubTree` | Idle → MoveSubTree → Jump → Crouch |
| `UpperBodySubTree` | 各上半身动作节点（换弹/拉栓/投掷等） |

### 3. AnimTreeLayerMixNode — 分层叠加节点

使用 Unity 的 `AnimationLayerMixerPlayable` 实现**多层动画叠加**。典型用途：
- Layer 0（Base）：下半身移动动画（权重始终 1.0）
- Layer 1+：上半身动作叠加层（权重 = 子节点逻辑权重）

每层可选配 `AvatarMask`，实现上半身/下半身的分离混合。

```csharp
// 第 0 层始终为 1.0（Base 层）
SetInputWeight(0, 1.0f);
// 其他层使用子节点逻辑权重
SetInputWeight(i, newW);
```

当前实现中 `PlayerAnimTreeRoot` 继承此类，但主体动画逻辑在 `AnimTreeSlotNode` 的子树上运行。

### 4. AnimTreeSlotNode — 动态插槽节点

最核心的创新设计。用于需要**运行时动态替换子树**的场景（如切换武器 → 换弹动画树替换为拉栓动画树）。

内部使用 2 路 `AnimationMixerPlayable`：
- Port 0：当前激活的子树 (activeSubtree)
- Port 1：正在淡出的旧子树 (fadingSubtree)

**Crossfade 过渡机制：**

```
切换流程：
1. 旧 activeSubtree → 移到 Port 1 (fadingSubtree)
2. 新 subtree → 装到 Port 0 (activeSubtree, weight=0)
3. crossfadeWeight 从 0 平滑过渡到 1
4. 旧子树权重降为 0 后销毁
```

```csharp
public void InstallSubtree(AnimTreeNode<T> newSubtree)
{
    // 当前不活跃 → 直接替换
    if (!isCurrentActive) { ClearAllSubtrees(); _activeSubtree = newSubtree; return; }

    // 正在活跃 → 启动 Crossfade
    _fadingSubtree = _activeSubtree;          // 转移旧树
    _activeSubtree = newSubtree;              // 装入新树
    _crossfadeWeight = 0f;                    // 开始过渡
}

// UpdateNode 中驱动过渡
if (_crossfadeWeight < 1f)
    _crossfadeWeight = Mathf.MoveTowards(_crossfadeWeight, 1f, Time.fixedDeltaTime * CrossfadeSpeed);

_slotMixer.SetInputWeight(0, _crossfadeWeight);      // 新树比重递增
_slotMixer.SetInputWeight(1, 1f - _crossfadeWeight); // 旧树比重递减
```

**使用场景：**
- `UpperSlotTreeNode`：武器切换时，换弹动画被拉栓动画替换
- `PlayerAnimSlotTreeRoot`：整个武器动画树的挂载点，切换武器类型时替换整棵子树

### PlayerAnimSlot — 插槽注册表

以 Tag 为键的全局注册表，存储树中所有的 `AnimTreeSlotNode`：

```csharp
public class PlayerAnimSlot
{
    private Dictionary<string, AnimTreeSlotNode<IAnimContext>> _slots = new();

    public void Register(string tag, AnimTreeSlotNode<IAnimContext> node);
    public AnimTreeSlotNode<IAnimContext> Get(string tag);
}
```

外部可通过 Tag 获取插槽并安装自定义子树：
```csharp
// 例如：在切换武器时，给 UpperSlot 安装换弹动画
controller.InstallSlotAction("UpperSlot", ctx => new ReloadTreeLeafNode(ctx));
```

---

## 数据驱动流程

### AnimGraphData (ScriptableObject)

```
[CreateAssetMenu(menuName = "AnimPro/AnimGraphData")]
AnimGraphData
├── Nodes: List<NodeData>
│   ├── GUID (唯一标识)
│   ├── NodeType (完全限定类名，如 "Player.AnimTree.LowerBodyNodes.IdleTreeLeafNode")
│   ├── Tag / DefaultClip / Mask
│   ├── AnimSpeed / WeightSpeed
│   └── weaponAsset (武器动画曲线)
└── Edges: List<EdgeData>
    ├── ParentGUID
    └── ChildGUID
```

### AnimGraphRuntimeBuilder

运行时工厂，将 ScriptableObject 数据反序列化为 C# 对象树：

```
Build(AnimGraphData, IAnimContext, PlayerAnimSlot):
  1. 反射创建每个 NodeData 对应的 C# 类实例
  2. 注入编辑器参数 (Tag, DefaultClip, Mask, AnimSpeed ...)
  3. 识别 SlotNode → 注册到 PlayerAnimSlot
  4. 根据 Edges 建立父子关系
  5. 找到根节点（入度为 0 的节点）并返回
```

---

## IAnimContext — 上下文抽象

动画树通过 `IAnimContext` 接口读取游戏状态，**不直接依赖 PlayerManager**：

```csharp
public interface IAnimContext
{
    IMoveQueryProvider MoveModule { get; }      // 移动状态查询
    ICombatQueryProvider CombatModule { get; }  // 战斗/武器状态查询
    IIkProvider IkModule { get; }               // IK 权重控制
    Transform WeaponPivot { get; }              // 武器挂点
    Animator Animator { get; }                  // Unity Animator
    float GetAimIkTarget();                     // 瞄准 IK 权重
    float GetHandIkTarget();                    // 手部 IK 权重
}
```

PlayerManager 通过显式接口实现将自身暴露为 `IAnimContext`：

```csharp
// PlayerManager.cs
IMoveQueryProvider IAnimContext.MoveModule => MoveModule;
ICombatQueryProvider IAnimContext.CombatModule => CombatModule;
IIkProvider IAnimContext.IkModule => IkModule;
```

这使得动画树与具体类解耦，方便测试和扩展。

---

## PlayerAnimController — 控制器

`PlayerAnimController` 是动画系统的外部入口，由 `PlayerManager` 持有：

```csharp
public class PlayerAnimController
{
    private IAnimContext _ctx;
    private PlayerAnimTreeBuild _animBuild;
    private PlayerAnimSlotTreeRoot Root;
    private PlayerAnimSlot Slot;

    // 切换武器时重建整棵动画树
    public void SwitchWeaponGraph(AnimGraphData weaponGraph, WeaponType weaponType);

    // 动态安装槽位动作（换弹、拉栓等）
    public void InstallSlotAction(string slotName, Func<IAnimContext, AnimTreeNode<IAnimContext>> factory);

    // 每帧驱动
    public void Update()
    {
        _rootNode.Evaluate();     // ← 决策层：检查条件，设置 TargetWeight
        _rootNode.UpdateNode();   // ← 数值层：平滑权重，触发 Enter/Exit，设置 Mixer 权重
    }
}
```

---

## 每帧更新顺序

```
PlayerManager.Update()
    │
    ├── _logic.OnUpdate()          ← 输入采集，模块更新
    │
    ├── ClientTick (50Hz)          ← 模块 FixedUpdate，状态回写
    │
    └── AnimController.Update()    ← 动画树 Evaluate + UpdateNode
        │
        ├── Evaluate() [递归]
        │   ├── 每种节点实现自己的决策逻辑
        │   ├── LeafNode: CheckState → TargetWeight = 0 or 1
        │   ├── MixNode: 选第一个满足条件的子节点
        │   ├── LayerMixNode: 允许权重 > 0 时继续传播
        │   └── SlotNode: 传播到 activeSubtree + fadingSubtree
        │
        └── UpdateNode() [递归]
            ├── Weight = MoveTowards(Weight, TargetWeight)
            ├── 检测 Enter/Exit 边界触发回调
            ├── MixNode: 归一化子节点权重到 Mixer 端口
            ├── LayerMixNode: Base 层 1.0，叠加层用子节点权重
            └── SlotNode: Crossfade 权重分配
```

---

## 关键设计决策

### 1. 决策-执行分离

`Evaluate()` 只设置 `TargetWeight`（纯决策），`UpdateNode()` 才做权重平滑和副作用触发（执行）。两层分开确保第一帧不会出现姿态跳变。

### 2. LastActiveChild 保持

MixNode 当自身条件为 false 但 `Weight > 0.001f` 时，保持最后一个活跃子节点的引用，防止淡出期间 Mixer 输入坍缩为全零（会导致 BindPose）。

### 3. Slot 的动态替换 vs 状态机切换

传统 Animator Controller 通过状态机参数切换动画，换弹/拉栓/投掷等动作需要大量 Transition 连线。AnimPro 的 Slot 机制让这类"临时动作"可以动态安装/卸载，不用在编辑器中维护复杂的 Transition 图。

### 4. AvatarMask 在 LayerMixNode 层设置

上半身/下半身的分离通过 `AnimationLayerMixerPlayable` 的 AvatarMask 实现：
- Layer 0：无 Mask（全身），播放下半身动画
- Layer 1：上半身 Mask，叠加武器操作动画

### 5. 武器动画曲线驱动

`WeaponClipAsset` 存储动画曲线数据（位置/旋转），LeafNode 如 `ReloadTreeLeafNode` 在 `UpdateNode()` 中根据当前播放时间采样曲线，驱动 `WeaponPivot` 的 Transform 和 IK 权重。这使得换弹动画不仅播放 Clip，还能同步驱动武器的位移和手部 IK 变化。

---

## 文件索引

### 核心框架 (`Player/AnimPro/`)

| 文件 | 说明 |
|------|------|
| `AnimTreeNode.cs` | 抽象基类，定义 Evaluate/UpdateNode/BuildNode/GetInputWeight/SetInputWeight |
| `AnimTreeLeafNode.cs` | 叶子节点基类，播放单个 Clip |
| `AnimTreeMixNode.cs` | 互斥混合节点，归一化权重分配 |
| `AnimTreeLayerMixNode.cs` | 分层叠加节点，支持 AvatarMask |
| `AnimTreeSlotNode.cs` | 动态插槽节点，Crossfade 子树替换 |
| `AnimGraphData.cs` | ScriptableObject，编辑器配置的节点和连线数据 |

### 运行时构建 (`Player/AnimTree/`)

| 文件 | 说明 |
|------|------|
| `AnimGraphRuntimeBuilder.cs` | 反射创建 + 注入参数 + 建立父子关系 → 返回根节点 |
| `PlayerAnimTreeBuild.cs` | 创建 PlayableGraph，挂载根节点，绑定 Animator 输出 |
| `PlayerAnimTreeRoot.cs` | 根节点（LayerMixNode），Always CheckState=true |
| `PlayerAnimSlotTreeRoot.cs` | 实际使用的根节点（SlotNode），支持子树切换 |
| `PlayerAnimSlot.cs` | 全局插槽注册表 |
| `PlayerAnimController.cs` | 外部入口，管理动画树切换和 Slot 动作安装 |

### 下半身节点 (`Player/AnimTree/LowerBodyNodes/`)

| 文件 | 说明 |
|------|------|
| `LowerBodySubTree.cs` | 下半身根 Mixer（非蹲伏状态） |
| `NotThrowLowerBodySubTree.cs` | 非投掷时的下半身分支（目前未实现） |
| `InThrowLowerBodySubTree.cs` | 投掷时的下半身分支（目前未实现） |
| `IdleTreeLeafNode.cs` | 待机动画 |
| `RifleSlidingTreeLeafNode.cs` | 步枪滑铲动画 |
| `JumpTreeLeafNode.cs` | 跳跃动画 |
| `MoveSubTreeNodes/MoveSubTree.cs` | 移动子树（Sprint / Walk 分支） |
| `MoveSubTreeNodes/SprintSubTreeNodes/` | 冲刺方向节点（Forward / ForwardLeft / ForwardRight） |
| `MoveSubTreeNodes/WalkSubTreeNodes/` | 行走方向节点（8 方向） |

### 上半身节点 (`Player/AnimTree/UpperBodyNodes/`)

| 文件 | 说明 |
|------|------|
| `UpperBodySubTree.cs` | 上半身根 Mixer |
| `UpperSlotTreeNode.cs` | 上半身动态插槽（换弹/拉栓的挂载点） |
| `ReloadTreeLeafNode.cs` | 换弹动画 + 武器曲线驱动 + IK 权重 |
| `BoltTreeLeafNode.cs` | 拉栓动画 + 武器曲线驱动 |
| `SlidingAimTreeNode.cs` | 滑铲时上半身姿态 |
| `GrenadeSubTreeNodes/` | 投掷手雷动画（High/Low，Start/Loop/End） |

### 蹲伏节点 (`Player/AnimTree/CrouchLowerBodyNode/`)

| 文件 | 说明 |
|------|------|
| `CrouchLowerBodySubTree.cs` | 蹲伏状态的下半身 Mixer |

### 接口层

| 文件 | 说明 |
|------|------|
| `mInterface/IAnimContext.cs` | 动画上下文接口 |
| `mInterface/IMoveQueryProvider.cs` | 移动状态查询 |
| `mInterface/ICombatQueryProvider.cs` | 战斗状态查询 |
| `mInterface/IIkProvider.cs` | IK 接口 |
