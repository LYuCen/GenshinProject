# 武器系统技术文档

## 目录

- [1. 概述与设计理念](#1-概述与设计理念)
- [2. 架构分层](#2-架构分层)
- [3. 核心数据结构](#3-核心数据结构)
- [4. WeaponPro —— 武器运行时本体](#4-weaponpro--武器运行时本体)
- [5. Feature 系统 —— 武器行为](#5-feature-系统--武器行为)
- [6. Component 系统 —— 武器静态配置](#6-component-系统--武器静态配置)
- [7. 武器运行时状态](#7-武器运行时状态)
- [8. 任务系统](#8-任务系统)
- [9. 动画树集成](#9-动画树集成)
- [10. FSM 状态机集成](#10-fsm-状态机集成)
- [11. IK 系统集成](#11-ik-系统集成)
- [12. 网络同步](#12-网络同步)
- [13. WeaponPresenter 视图层](#13-weaponpresenter-视图层)
- [14. 完整生命周期](#14-完整生命周期)
- [15. 关键事件流转](#15-关键事件流转)

---

## 1. 概述与设计理念

武器系统采用**组合优于继承**的架构设计。每一把武器由一份 `WeaponData` ScriptableObject 资产定义，运行时通过工厂方法动态组装 Feature（行为）和 Component（静态数据），形成完整的武器运行时实例 `WeaponPro`。

**核心设计原则：**

- **数据驱动**：武器的所有属性（射速、伤害、弹匣容量、换弹时间、拉栓时长等）均由 ScriptableObject 配置，无需修改代码即可创建新武器。
- **组合式 Feature**：武器的每种行为（开火、换弹、拉栓、姿态切换）是独立的 Feature，通过组合实现差异化。例如狙击枪有 `BoltFeature`（拉栓），而手枪没有。
- **Component 只存数据**：Component 不包含逻辑方法，纯粹作为数据容器（弹匣容量、伤害倍率、音频片段等），由 Feature 和 Presenter 读取使用。
- **工厂模式创建**：`WeaponFeatureFactory` 和 `WeaponComponentFactory` 根据 ScriptableObject 的类型自动创建对应的 Feature/Component 实例。

**对比旧系统：**

| 方面 | 旧系统（IWeapon 继承） | 新系统（WeaponPro 组合） |
|---|---|---|
| 武器定义 | 每个武器类型一个类 | 一份 WeaponData 资产 |
| 行为扩展 | 继承 + 重写 | 添加 FeatureData + Feature |
| 数据配置 | 硬编码或分散的资产 | 统一的 WeaponData 资产 |
| 武器差异化 | 子类重写方法 | 组合不同的 Feature 集合 |

---

## 2. 架构分层

```
┌──────────────────────────────────────────────────────────┐
│                    输入层 (Input)                         │
│   InputSystem → InputContext → WeaponAction              │
├──────────────────────────────────────────────────────────┤
│                    逻辑层 (Logic)                         │
│   LocalPlayerLogic / RemotePlayerLogic                   │
│        ↓                                                 │
│   PlayerCombatModule.OnUpdate()                          │
│        ↓                                                 │
│   WeaponPro.HandleAction(action)  ← 分发给所有Feature     │
│   WeaponPro.Tick()                ← 驱动Task + Feature    │
├──────────────────────────────────────────────────────────┤
│                Feature 层 (行为)                          │
│   FireFeature │ BoltFeature │ ReloadFeature │ StanceFeature │
│        ↓            ↓              ↓              ↓       │
│   修改Runtime   发送Event     创建Task       修改Stance    │
├──────────────────────────────────────────────────────────┤
│              Component 层 (静态数据)                      │
│   MagazineComponent     TransformComponent               │
│   DamageProfileComponent   AudioComponent                │
│   AnimComponent                                           │
├──────────────────────────────────────────────────────────┤
│                    查询层 (Query)                         │
│   CombatModule.GetQuery() → WeaponQuery                  │
│        ↓                    ↓                             │
│   WeaponStateTags ────→ WeaponFlags (位掩码)             │
├──────────────────────────────────────────────────────────┤
│                    表现层 (View)                          │
│   ┌──────────────┬──────────────────┬─────────────────┐  │
│   │  动画树       │   FSM 状态机     │   IK 系统       │  │
│   │  (Playable)  │  (PlayerFsm)    │  (FinalIK)      │  │
│   │              │                 │                 │  │
│   │ BoltLeafNode │ PlayerBoltState │ PlayerIkModule  │  │
│   │ ReloadLeafNd │ PlayerReloadSt  │ AimIK + HandIK  │  │
│   └──────────────┴──────────────────┴─────────────────┘  │
│                                                           │
│   WeaponPresenter (音频/枪火/枪口特效)                     │
└──────────────────────────────────────────────────────────┘
```

---

## 3. 核心数据结构

### 3.1 WeaponData（ScriptableObject）

武器的根资产，在 Unity Editor 中创建和配置。

```csharp
[CreateAssetMenu(menuName = "Weapon/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;                    // 武器名称
    public WeaponType weaponType;                // 武器类型枚举 (Rifle/Sniper/...)
    public List<WeaponFeatureData> features;     // 行为数据列表
    public List<WeaponComponentData> components; // 组件数据列表
}
```

### 3.2 WeaponAction（输入动作）

```csharp
public class WeaponAction
{
    public ButtonState Primary;    // 主按键（开火）
    public ButtonState Secondary;  // 副按键（瞄准/肩射）
    public ButtonState Reload;     // 换弹键
}
```

每帧由 `PlayerCombatModule.OnUpdate()` 从 `InputContext` 构建，传递给 `WeaponPro.HandleAction()`。

### 3.3 WeaponFlags（位掩码枚举）

```csharp
public enum WeaponFlags
{
    None     = 0,
    Reload   = 1 << 0,  // 换弹中
    Bolt     = 1 << 1,  // 拉栓中
    Shoot    = 1 << 2,  // 瞄准/射击姿态
    Throw    = 1 << 3,  // 投掷中
    Shoulder = 1 << 4,  // 肩射姿态
}
```

动画树节点和 FSM 条件通过此枚举统一查询武器状态。

### 3.4 WeaponQuery（状态查询）

```csharp
public class WeaponQuery
{
    private int _state;

    public bool HasState(WeaponFlags flag) => (_state & (int)flag) != 0;
    public void AddFlags(WeaponFlags flags) => _state |= (int)flags;
    public void SetState(int state) => _state = state;
}
```

`PlayerCombatModule.GetQuery()` 每次调用时从 `WeaponRuntime` 重新构建，保证查询结果始终反映最新状态。

### 3.5 WeaponStateTags（武器状态标签）

```csharp
public enum WeaponStateTags
{
    Reloading,  // 正在换弹
    Bolting,    // 正在拉栓
}
```

由 Feature 通过 `WeaponRuntime.AddState()/RemoveState()` 管理，控制哪些操作当前可用。

### 3.6 WeaponStance（武器姿态）

```csharp
public enum WeaponStance
{
    Normal,    // 普通持枪
    Shoulder,  // 肩射
    Ads,       // 瞄准（开镜）
}
```

由 `StanceFeature` 管理，影响动画树行为和 IK 权重。

---

## 4. WeaponPro —— 武器运行时本体

`WeaponPro` 是武器的核心运行时类，**不是 MonoBehaviour**。它实现 QFramework 的 `IController` 接口，可以访问事件系统和 Model 层。

### 4.1 关键成员

```csharp
public class WeaponPro : IController
{
    // 属性
    public IReadOnlyWeaponRuntime Runtime { get; }     // 只读运行时状态
    public WeaponType WeaponType { get; }               // 武器类型
    public string WeaponName { get; }                   // 武器名称
    public WeaponPresenter Presenter { get; set; }      // MonoBehaviour 表现层
    public WeaponTaskRunner TaskRunner { get; }         // 任务执行器
    public readonly BagModel BagModel;                  // 背包数据（弹匣管理）

    // 内部状态
    private WeaponRuntime _runtime;                     // 可变运行时状态
    private readonly List<IWeaponFeature> _features;    // 行为列表
    private readonly Dictionary<Type, object> _components; // 组件字典
}
```

### 4.2 构造流程

```
new WeaponPro(WeaponData data)
    │
    ├─ new WeaponRuntime()       // 初始化状态容器
    ├─ new WeaponTaskRunner()    // 初始化任务执行器
    ├─ BuildComponents()         // 遍历 data.components，通过工厂创建
    │   └─ WeaponComponentFactory.Create(componentData)
    │       ├─ MagazineComponent
    │       ├─ DamageProfileComponent
    │       ├─ AudioComponent
    │       ├─ TransformComponent
    │       └─ AnimComponent
    │
    └─ BuildFeatures()          // 遍历 data.features，通过工厂创建
        └─ WeaponFeatureFactory.Create(featureData)
            ├─ FireFeature      ──→ Initialize(this)
            ├─ BoltFeature      ──→ Initialize(this) → 订阅 FireFeature.OnFire
            ├─ ReloadFeature    ──→ Initialize(this) → 订阅 FireFeature.OnEmptyMag
            └─ StanceFeature    ──→ Initialize(this) → 订阅 Reload/Bolt 事件
```

### 4.3 每帧驱动

```csharp
// 1. 处理输入
WeaponPro.HandleAction(action)
    └─ 遍历 _features → feature.HandleAction(action)

// 2. 更新逻辑
WeaponPro.Tick()
    ├─ TaskRunner.Tick()        // 驱动当前任务（换弹计时等）
    └─ 遍历 _features → feature.Tick()
```

---

## 5. Feature 系统 —— 武器行为

每个 Feature 实现 `IWeaponFeature` 接口，通过组合定义武器的全部行为。

### 5.1 接口定义

```csharp
public interface IWeaponFeature
{
    void Initialize(WeaponPro weapon);   // 初始化，可在此订阅其他 Feature 的事件
    void HandleAction(WeaponAction action); // 响应输入
    void Tick();                         // 每帧更新
    void OnEnter();                      // 武器激活时调用
    void OnExit();                       // 武器失效时调用
}
```

### 5.2 FireFeature（开火）

| 属性 | 说明 |
|---|---|
| `fireRate` | 射速（发/分钟），倒数为冷却时间 |
| `damage` | 基础伤害 |
| `muzzleVelocity` | 枪口初速 |
| `automatic` | 是否全自动（按住连发 vs 单发） |

**核心逻辑：**

```
TryFire()
    ├─ 冷却计时器检查
    ├─ Reloading/Bolting 状态检查 → 阻止开火
    ├─ 弹匣为空 → 触发 OnEmptyMag → ReloadFeature.TryReload()
    ├─ 消耗弹药: mag.BulletVal--
    ├─ 发送 WeaponDataUpdateEvent
    └─ 触发 OnFire → BoltFeature.StartBolt() / WeaponFirePresenter.OnFire()
```

`FireFeature` 是武器行为链的**起点**：开火 → 消耗弹药 → 触发 Bolt（拉栓）→ 弹空时触发 Reload（换弹）。

### 5.3 BoltFeature（拉栓）

| 属性 | 说明 |
|---|---|
| `boltDuration` | 拉栓动画时长（秒） |

**核心逻辑：**

```
StartBolt()
    ├─ Runtime.AddState(Bolting)   // 标记拉栓状态
    ├─ timer = boltDuration
    ├─ 发送 BoltEvent              // 通知 IK 系统禁用瞄准
    └─ 触发 OnBolt                 // 通知动画/音效

Tick()
    └─ 倒计时 → timer<=0 → Runtime.RemoveState(Bolting)
```

`BoltFeature` **不响应直接输入**，由 `FireFeature.OnFire` 触发。拉栓期间阻止开火。

### 5.4 ReloadFeature（换弹）

| 属性 | 说明 |
|---|---|
| `reloadDuration` | 换弹时长（秒） |

**核心逻辑：**

```
TryReload()
    ├─ Reloading 状态检查 → 已换弹中则跳过
    ├─ 弹匣已满检查 → 满则跳过
    ├─ 创建 ReloadTask(runtime, reloadDuration, weapon)
    ├─ TaskRunner.Run(task)        // 异步任务
    └─ 发送 OnReloadEvent          // 通知 IK 禁用

// 也可通过输入键手动触发
HandleAction(action)
    └─ action.Reload.Down → TryReload()
```

`ReloadFeature` 通过 `FireFeature.OnEmptyMag` 自动触发，也支持手动按键触发。

### 5.5 StanceFeature（姿态切换）

| 属性 | 说明 |
|---|---|
| `shoulderThreshold` | 短按/长按判定阈值（默认 0.2s） |

**核心逻辑：**

```
HandleSecondary()
    ├─ Reloading/Bolting 状态检查 → 阻止切换
    ├─ Down: 记录按下时间，进入 Shoulder
    ├─ Press: 累加按住时间
    └─ Up:
        ├─ 短按 (< threshold) → ToggleAds()   // 切换开镜
        └─ 长按 (>= threshold) → ExitShoulder() // 退出肩射
```

**状态转换图：**

```
         ┌──────────┐
         │  Normal  │
         └────┬─────┘
              │ 按下副键 (Down)
         ┌────▼─────┐
         │ Shoulder │
         └────┬─────┘
              │ 短按松开 → Ads
              │ 长按松开 → Normal
              │ 再次按下 → Normal
         ┌────▼─────┐
         │   Ads    │
         └────┬─────┘
              │ 按下或松开 → Normal
```

- `Reloading` 或 `Bolting` 时强制退出肩射/开镜

### 5.6 Feature 间协作关系

```
FireFeature.OnFire ──────────→ BoltFeature.StartBolt()
FireFeature.OnEmptyMag ──────→ ReloadFeature.TryReload()
ReloadFeature.OnReload ──────→ StanceFeature.ExitShoulder()
BoltFeature.OnBolt ──────────→ StanceFeature.ExitShoulder()
```

---

## 6. Component 系统 —— 武器静态配置

Component 是**纯数据容器**，不包含方法。通过 `Dictionary<Type, object>` 按类型索引。

### 6.1 MagazineComponent（弹匣）

```csharp
public class MagazineComponent
{
    public int BulletVal;         // 当前子弹数
    public readonly int MagSize;  // 弹匣容量
    public ElementEnum ElementEnum; // 元素类型（火/冰/雷/草/水）
    public bool IsEmpty => BulletVal <= 0;
    public bool IsFull => BulletVal >= MagSize;
}
```

### 6.2 DamageProfileComponent（伤害配置）

```csharp
public class DamageProfileComponent
{
    public float HeadMultiplier;  // 爆头倍率 (默认 1.9)
    public float BodyMultiplier;  // 身体倍率 (默认 1.0)
    public float LimbMultiplier;  // 四肢倍率 (默认 0.4)
}
```

### 6.3 TransformComponent（位置数据）

```csharp
public class TransformComponent
{
    public Vector3 Pos;  // 武器在 gunCube 下的本地位置
    public Vector3 Rot;  // 武器在 gunCube 下的本地旋转
}
```

用于 `ResetGun()` 时恢复武器到默认持枪位置。

### 6.4 AudioComponent（音频）

```csharp
public class AudioComponent
{
    public AudioClip GetAudioClip;    // 切出武器音效
    public AudioClip FireAudioClip;   // 开火音效
    public AudioClip ReloadAudioClip; // 换弹音效
    public AudioClip BoltAudioClip;   // 拉栓音效
}
```

### 6.5 AnimComponent（动画数据）

```csharp
public class AnimComponent
{
    public AnimGraphData AnimGraphData;       // 武器专属动画树资产
    public List<TagClipPair> AnimMappings;    // Tag → Clip 映射（用于换弹等）
}
```

`AnimGraphData` 在切换武器时传递给 `PlayerAnimController.SwitchWeaponGraph()` 重建 PlayableGraph。`AnimMappings` 提供武器特定的换弹动画片段。

### 6.6 对应的 ComponentData

| ComponentData 类 | 对应 Component | 字段 |
|---|---|---|
| `MagazineComponentData` | `MagazineComponent` | `magazineElement`, `magazineCapacity` |
| `DamageProfileComponentData` | `DamageProfileComponent` | `headMultiplier`, `bodyMultiplier`, `limbMultiplier` |
| `TransformComponentData` | `TransformComponent` | `pos`, `rot` |
| `AudioComponentData` | `AudioComponent` | `getAudioClip`, `fireAudioClip`, `reloadAudioClip`, `boltAudioClip` |
| `AnimComponentData` | `AnimComponent` | `animGraphData`, `animMappings` |

### 6.7 工厂方法

```csharp
public static class WeaponComponentFactory
{
    public static object Create(WeaponComponentData data)
    {
        switch (data)
        {
            case MagazineComponentData m:    return new MagazineComponent(m);
            case DamageProfileComponentData d: return new DamageProfileComponent(d);
            case AudioComponentData a:       return new AudioComponent(a);
            case TransformComponentData t:   return new TransformComponent(t);
            case AnimComponentData a:        return new AnimComponent(a);
            default: return null;
        }
    }
}
```

---

## 7. 武器运行时状态

### 7.1 WeaponRuntime

```csharp
public class WeaponRuntime : IReadOnlyWeaponRuntime
{
    private readonly HashSet<WeaponStateTags> _states;
    public MagazineComponent Component { get; set; }  // 快捷引用弹匣
    public WeaponStance Stance { get; private set; }

    public bool HasState(WeaponStateTags tag);   // 检查状态标签
    public void AddState(WeaponStateTags tag);   // 添加状态标签 + 发送事件
    public void RemoveState(WeaponStateTags tag); // 移除状态标签 + 发送事件
    public void SetStance(WeaponStance stance);   // 设置姿态

    public event Action OnAmmoChanged;   // 弹药变化回调
    public event Action OnStateChanged;  // 状态变化回调
}
```

### 7.2 状态标签

| 标签 | 设置者 | 影响 |
|---|---|---|
| `Reloading` | `ReloadTask.Start()` | 阻止开火、阻止姿态切换、阻止再次换弹 |
| `Bolting` | `BoltFeature.StartBolt()` | 阻止开火、阻止姿态切换 |

### 7.3 WeaponQuery 的生成

```csharp
// PlayerCombatModule.GetQuery() — 每帧由所有消费者调用
public WeaponQuery GetQuery()
{
    _query.SetState(0);

    if (_bagModel.Weapon.Runtime.HasState(WeaponStateTags.Reloading))
        _query.AddFlags(WeaponFlags.Reload);
    if (_bagModel.Weapon.Runtime.Stance == WeaponStance.Shoulder)
        _query.AddFlags(WeaponFlags.Shoulder);
    if (_bagModel.Weapon.Runtime.Stance == WeaponStance.Ads)
        _query.AddFlags(WeaponFlags.Shoot);
    if (_bagModel.Weapon.Runtime.HasState(WeaponStateTags.Bolting))
        _query.AddFlags(WeaponFlags.Bolt);
    // 远程玩家从 StateData 读取...
    return _query;
}
```

**`WeaponQuery` 不持久化**——每次调用 `GetQuery()` 都重新构建，确保动画树和 FSM 拿到的是当前帧的准确状态。

---

## 8. 任务系统

### 8.1 IWeaponTask 接口

```csharp
public interface IWeaponTask
{
    void Start();            // 任务开始
    void Tick();             // 每帧更新
    void Cancel();           // 取消任务
    bool Finished { get; }   // 是否完成
}
```

### 8.2 WeaponTaskRunner

```csharp
public class WeaponTaskRunner
{
    private IWeaponTask _currentTask;

    public void Run(IWeaponTask task)    // 取消当前任务并启动新任务
    {
        _currentTask?.Cancel();
        _currentTask = task;
        _currentTask.Start();
    }

    public void Tick()                   // 每帧更新当前任务
    {
        _currentTask?.Tick();
        if (_currentTask?.Finished == true)
            _currentTask = null;         // 完成后自动清理
    }
}
```

同一时间只有一个任务在运行，新任务会取消旧任务。

### 8.3 ReloadTask（唯一实现）

```csharp
public class ReloadTask : IWeaponTask
{
    private readonly WeaponRuntime _runtime;
    private float _timer;
    private readonly float _reloadTime;
    private WeaponPro _weapon;

    public void Start()
    {
        _runtime.AddState(WeaponStateTags.Reloading);  // 标记换弹中
        _timer = _reloadTime;
    }

    public void Tick()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0) Finish();
    }

    private void Finish()
    {
        _runtime.RemoveState(WeaponStateTags.Reloading); // 移除标记
        // 从 BagModel 获取对应元素的子弹，填充弹匣
        var info = new BulletInfo(mag.ElementEnum, mag.BulletVal);
        _weapon.BagModel.ReturnBullet(info);              // 返回旧子弹
        mag.BulletVal = Mathf.Min(bagBullets, mag.MagSize); // 填充
        _weapon.BagModel.DeductBullet(newBulletInfo);     // 扣除新子弹
        Finished = true;
    }
}
```

**流程：** `ReloadFeature.TryReload()` → `new ReloadTask(...)` → `TaskRunner.Run(task)` → 倒计时 → 完成弹药交换。

---

## 9. 动画树集成

武器系统通过 `PlayerAnimController` 和叶子节点（`ReloadTreeLeafNode`、`BoltTreeLeafNode`）与 Unity PlayableGraph 集成。

### 9.1 武器切换

```
CombatModule.ActivateWeapon()
    │
    ├─ AnimController.SwitchWeaponGraph(weaponGraph, weaponType)
    │   └─ Slot.Clear()
    │   └─ Root.InstallSubtree(AnimGraphRuntimeBuilder.Build(weaponGraph, ...))
    │       └─ 从 ScriptableObject 反射构建整个 PlayableGraph
    │       └─ 包含 BoltTreeLeafNode 等内嵌节点
    │
    └─ AnimController.InstallSlotAction("UpperSlot", factory)
        └─ new ReloadTreeLeafNode(...)  → Slot.InstallSubtree(node)
```

**两层构建：**
1. `SwitchWeaponGraph` 从 `AnimGraphData` 资产反射构建武器专属动画树（包含 Bolt、Idle 等内嵌节点）
2. `InstallSlotAction` 动态安装换弹节点（因为换弹动画片段来自武器 Component，非内嵌在资产中）

### 9.2 ReloadTreeLeafNode

| 阶段 | 逻辑 |
|---|---|
| **CheckState** | 检查 `WeaponFlags.Reload` |
| **OnEnterState** | 武器挂载到 `weaponPivot` + `_playable.SetTime(0)` + `LateUpdate()` 初始化第一帧姿态 |
| **UpdateNode（active）** | `LateUpdate()` 每帧采样烘焙曲线 → 驱动 `weaponPivot.localPosition/Rotation` + 调制 `SetHandIk(1-handBlend)` |
| **UpdateNode（fade-out）** | `Lerp(GetHandIkTarget(), 1-handBlend, currentWeight)` 平滑恢复手部 IK |
| **OnExitState** | `ResetGun()` + `SetAimIk(GetAimIkTarget())` —— 只归位武器和瞄准 IK，手部 IK 已在 crossfade 阶段平滑恢复 |

**武器位置驱动（LateUpdate）：**

```
烘焙曲线采样 (CompressionUtility.GetPos/GetRot)
    ↓
左手位置 + 偏移 = finalLP, finalLR
右手位置 + 偏移 = finalRP, finalRR
    ↓
_handBlend 曲线在左右手之间插值
    ↓
weaponPivot.localPosition = Lerp(finalLP, finalRP, _handBlend)
weaponPivot.localRotation = Slerp(finalLR, finalRR, _handBlend)
```

### 9.3 BoltTreeLeafNode

| 阶段 | 逻辑 |
|---|---|
| **CheckState** | 检查 `WeaponFlags.Bolt` |
| **OnEnterState** | `_playable.SetTime(0)` + `LateUpdate()`（武器挂载由 FSM 层处理） |
| **UpdateNode** | `_wasFullyActive` 检测 fade-out 期间重新拉栓 → 只 reset 时间不调 LateUpdate（避免武器抖动） |
| **OnExitState** | 仅日志（ResetGun 和 IK 恢复由 FSM `PlayerBoltState.OnExitState` 处理） |

**关键修复 —— fade-out 期间重新拉栓不抖动：**

```
fade-out 中 _wasFullyActive = false（weight < 0.9）
    ↓
重新触发 Bolt → weight 爬升 > 0.9
    ↓
isFullyActive && !_wasFullyActive && playable 在末尾
    ↓
仅 _playable.SetTime(0)   ← 不调 LateUpdate
    ↓
weaponPivot 原地不动（无抖动）
下一帧 LateUpdate 正常采样新曲线
```

### 9.4 PlayerAnimController

```csharp
public class PlayerAnimController
{
    private Dictionary<string, Func<...>> _slotFactories; // 缓存 factory

    public void SwitchWeaponGraph(AnimGraphData weaponGraph, WeaponType weaponType);
    public void InstallSlotAction(string slotName, Func<...> factory);  // 安装 + 缓存
    public void ReinstallSlotAction(string slotName);  // 重新安装（crossfade 避免硬跳）
    public void Update();
}
```

---

## 10. FSM 状态机集成

上半身 FSM（`UpperBodyLayer`）管理武器相关的角色状态。动画树节点处理动画播放，FSM 状态处理**武器挂载/卸载**和**IK 恢复**。

### 10.1 状态图

```
         ┌──────────────────────────────────────┐
         │              Empty（空状态）           │
         └──┬──────┬──────┬──────┬──────────────┘
            │      │      │      │
    Reload  │  Aim │Throw │ Bolt │
            ▼      ▼      ▼      ▼
    ┌─────────┐ ┌─────┐ ┌───────┐ ┌──────────┐
    │ Reload  │ │ Aim │ │ Throw │ │  Bolt    │
    └────┬────┘ └──┬──┘ └───┬───┘ └────┬─────┘
         │         │        │           │
         └─────────┴────────┴───────────┘
                       │
                       ▼
                    Empty
```

每个状态都有**条件对象**（`AbstractCondition`），每帧调用 `CombatModule.GetQuery()` 检查对应的 `WeaponFlags`：

| 条件 | 检查的标志 | 目标状态 |
|---|---|---|
| `PlayerReloadStateCondition` | `WeaponFlags.Reload` | Reload |
| `PlayerBoltStateCondition` | `WeaponFlags.Bolt` | Bolt |
| `PlayerAimStateCondition` | `WeaponFlags.Shoot` | Aim |
| `PlayerThrowStateCondition` | `WeaponFlags.Throw` | Throw |
| `PlayerEmptyStateCondition` | 以上全为 false | Empty |

### 10.2 各状态职责

#### PlayerBoltState（拉栓）

```csharp
OnEnterState()
    ├─ 获取武器 Transform
    ├─ gun.SetParent(weaponPivot)        // 从 gunCube 挂载到 weaponPivot
    └─ gun.localPosition/Rotation = 0    // 归零本地变换

OnExitState()
    ├─ CombatModule.ResetGun()           // 武器归位到 gunCube
    └─ IkModule.SetAimIk(GetAimIkTarget()) // 恢复瞄准 IK
```

**注意：** 武器挂载从 `BoltTreeLeafNode.OnEnterState` 迁移到了 FSM 层，解决了"动画慢半拍"的问题——FSM 状态切换早于动画树节点进入，武器先归位再播动画。

#### PlayerReloadState（换弹）

```csharp
OnEnterState()
    └─ gunCube.SetParent(hand)           // 换弹时枪挂在手上

OnExitState()   // 空 —— IK 恢复由 ReloadTreeLeafNode 的 crossfade 处理
```

**注意：** `OnExitState()` 清空后，IK 恢复不再由此处硬编码，改由动画树的 crossfade 阶段平滑处理。

#### PlayerAimState（瞄准）

```csharp
OnEnterState()
    └─ IkModule.SetAimIk(GetAimIkTarget())  // 恢复/设置瞄准 IK

OnUpdateState()
    └─ transform.rotation = Slerp(当前, 相机朝向, dt*20)  // 角色朝向跟随相机

OnExitState()
    └─ transform.rotation = 相机朝向  // 退出瞄准时瞬间对齐
```

### 10.3 PlayerManager.GetAimIkTarget() / GetHandIkTarget()

两个统一的 IK 目标判定方法，替代了散落在各状态中的硬编码逻辑：

```csharp
public float GetAimIkTarget()
{
    if (移动状态 == Sprint) return 0;
    if (武器状态 == Reload) return 0;
    if (武器状态 == Bolt)   return 0;
    if (武器状态 == Throw)  return 0;
    return 1;
}

public float GetHandIkTarget()  // 同上逻辑
```

---

## 11. IK 系统集成

`PlayerIkModule` 管理 FinalIK 的 `AimIK`（武器瞄准）和 `FullBodyBipedIK`（左手放置）两个求解器。

### 11.1 事件驱动的 IK 控制

```csharp
// 注册的事件监听
OnReloadEvent → AimIK = 0, HandIK = 0   // 换弹时完全禁用 IK
BoltEvent     → AimIK = 0               // 拉栓时禁用瞄准 IK
BoltEndEvent  → AimIK = 1               // 拉栓结束时恢复
```

### 11.2 平滑过渡

```csharp
public void OnUpdate()
{
    // 实际求解器权重以 7/秒 的速度平滑逼近目标值
    AimIK.solver.IKPositionWeight = MoveTowards(当前值, _aimIkWeight, 7*dt);
    BodyBipedIK.solver.IKPositionWeight = MoveTowards(当前值, _handIkWeight, 7*dt);
}
```

### 11.3 动画树对 IK 的直接控制

换弹期间，`ReloadTreeLeafNode.LateUpdate()` 通过曲线 `handWeight` 调制手部 IK：
```
SetHandIk(1 - _handBlend)
```
- `_handBlend = 0` → 手部完全由 IK 控制（左手持握武器）
- `_handBlend = 1` → 手部完全由动画控制（右手操作）

### 11.4 远程玩家 IK

远程玩家不通过 Camera 计算 IK 目标，而是由服务器同步 aim 位置：

```csharp
// 本地玩家
if (_context.IsLocal)
{
    Vector3 aimDir = (CameraController.rayEnd - camera.position).normalized;
    _target.position = camera.position + aimDir * 100f;
}

// 远程玩家
public void SetAimTargetPosition(Vector3 pos) => _target.position = pos;
```

`PlayerIkContext.IsLocal` 控制事件注册（远程玩家不注册本地事件）。

---

## 12. 网络同步

### 12.1 本地玩家到服务器

```
LocalPlayerLogic.SendPositionToServer()
    └─ 打包 PlayerStateData (position, rotation, aimPosition, isShoot, isReload, ...)
    └─ NetworkService.Bridge.SendToServer("UpdatePosition", data)
```

### 12.2 远程玩家同步

```
RemotePlayerLogic.SyncFromServer(dto)
    ├─ transform.position = dto.Position  // 插值预测
    ├─ IkModule.SetAimTargetPosition(aimPos)  // 同步瞄准点
    └─ 武器状态: StateData.isShoot/isReload/isThrow
```

### 12.3 回环服务器（本地测试）

```
LocalServerSimulator
    ├─ "Revive" → 重置玩家/bot HP
    ├─ "UpdatePosition" → 更新内部状态 → 广播 "AllPlayerInfo"
    └─ 管理 _fakeBots 列表模拟其他玩家
```

### 12.4 远程玩家的武器创建

```
RemotePlayerLogic.Init()
    ├─ 加载 WeaponData: ResourceManager.Load(".../FnIws.asset")
    ├─ 实例化武器预制体 → WeaponPresenter
    ├─ 读取 TransformComponent → 设置位置
    ├─ 读取 AnimComponent → SwitchWeaponGraph(...)
    └─ 设置 IK targets → SetMuzzle(muzzle), SetHandIk(hand)
```

---

## 13. WeaponPresenter 视图层

`WeaponPresenter` 是武器的 MonoBehaviour 表现层，负责音频、枪火特效、枪口闪光。

### 13.1 结构

```csharp
public class WeaponPresenter : MonoBehaviour
{
    // Transform 引用
    public Transform magazine, hand, muzzle, gunSight;

    // 子 Presenter
    private WeaponAudioPresenter _audioPresenter;    // 音频
    private WeaponFirePresenter _firePresenter;      // 弹道 + 子弹
    private WeaponMuzzlePresenter _muzzlePresenter;  // 枪口闪光
}
```

### 13.2 初始化与生命周期

```
Init(WeaponPro weapon, CameraController camera)
    └─ 创建三个子 Presenter

OnEnter()
    ├─ ResetTrans() → 读取 TransformComponent 设置本地位置/旋转
    ├─ 发送 WeaponDataUpdateEvent
    └─ 各子 Presenter.OnEnter()

OnExit()
    └─ gameObject.SetActive(false)
```

### 13.3 子 Presenter 事件订阅

| Presenter | 订阅事件 | 行为 |
|---|---|---|
| `WeaponAudioPresenter` | `FireFeature.OnFire` | 播放开火音效 |
| | `ReloadFeature.OnReload` | 播放换弹音效 |
| | `BoltFeature.OnBolt` | 播放拉栓音效 |
| `WeaponFirePresenter` | `FireFeature.OnFire` | 从对象池获取子弹 → 计算伤害 → 射向瞄准点 |
| `WeaponMuzzlePresenter` | `FireFeature.OnFire` | 从对象池获取枪口闪光 → 按元素类型着色 |

---

## 14. 完整生命周期

### 14.1 武器创建与激活

```
1. BagModel.OnInit()
   └─ 加载 WeaponData 资产 → new WeaponPro(data)
        ├─ BuildComponents() → MagazineComponent, TransformComponent, ...
        └─ BuildFeatures()   → FireFeature, BoltFeature, ReloadFeature, StanceFeature
             └─ Feature.Initialize() → 交叉订阅

2. PlayerCombatModule.GetPrimaryWeapon()
   ├─ bagModel.SetPrimaryWeapon()
   ├─ 加载武器预制体 → 实例化
   ├─ bagModel.Weapon.Presenter = prefab.GetComponent<WeaponPresenter>()
   └─ ActivateWeapon()
        ├─ Weapon.RequireComponent<AnimComponent>()
        ├─ AnimController.SwitchWeaponGraph(animGraphData, weaponType)
        │   └─ Slot.Clear() + Root.InstallSubtree(Build(weaponGraph, ...))
        ├─ InstallSlotAction("UpperSlot", factory) → 安装 ReloadTreeLeafNode
        ├─ Presenter.gameObject.SetActive(true)
        ├─ SendEvent(SetHandIkTargetEvent) → IK 目标设置
        └─ Presenter.Init(weapon, cameraController)
```

### 14.2 换弹完整流程

```
① 触发（自动或手动）
    ReloadFeature.TryReload()
    ├─ 创建 ReloadTask → TaskRunner.Run(task)
    ├─ Runtime.AddState(Reloading)
    └─ 发送 OnReloadEvent

② IK 响应
    PlayerIkModule 监听 OnReloadEvent
    └─ AimIK = 0, HandIK = 0   （完全禁用 IK）

③ FSM 响应
    PlayerReloadStateCondition → WeaponFlags.Reload = true
    └─ FSM 切换到 PlayerReloadState
        └─ gunCube.SetParent(hand)

④ 动画树响应
    ReloadTreeLeafNode.CheckState() → WeaponFlags.Reload = true
    └─ OnEnterState()
        └─ weapon.SetParent(weaponPivot)
        └─ _playable.SetTime(0) + LateUpdate()

⑤ 每帧更新
    UpdateNode() → LateUpdate()
    ├─ 采样 handWeight 曲线 → SetHandIk(1 - _handBlend)
    ├─ 采样烘焙曲线 → weaponPivot.localPosition/Rotation
    └─ ReloadTask.Tick() 倒计时

⑥ 换弹完成
    ReloadTask.Finish()
    └─ Runtime.RemoveState(Reloading)

⑦ 退出动画
    UpdateNode() → TargetWeight = 0 → 权重下降
    ├─ Crossfade 阶段 → Lerp(GetHandIkTarget(), 1-handBlend, currentWeight) 平滑恢复手部 IK
    └─ OnExitState() → ResetGun() + SetAimIk(GetAimIkTarget())

⑧ FSM 退出
    PlayerReloadStateCondition 不满足 → FSM 切换到 Empty
```

### 14.3 拉栓完整流程

```
① 触发（自动）
    FireFeature.OnFire → BoltFeature.StartBolt()
    ├─ Runtime.AddState(Bolting)
    ├─ timer = boltDuration
    └─ 发送 BoltEvent

② IK 响应
    PlayerIkModule 监听 BoltEvent
    └─ AimIK = 0

③ FSM 响应（先于动画树）
    PlayerBoltStateCondition → WeaponFlags.Bolt = true
    └─ FSM 切换到 PlayerBoltState
        └─ OnEnterState()
            └─ gun.SetParent(weaponPivot) + localPos/Rot = 0

④ 动画树响应（后于 FSM）
    BoltTreeLeafNode.CheckState() → WeaponFlags.Bolt = true
    └─ OnEnterState()
        └─ _playable.SetTime(0) + LateUpdate()

⑤ 每帧更新
    BoltFeature.Tick() 倒计时
    UpdateNode() → LateUpdate() → weaponPivot 位置更新

⑥ 拉栓完成
    BoltFeature → timer <= 0 → Runtime.RemoveState(Bolting)
    发送 BoltEndEvent → IkModule.AimIK = 1

⑦ 退出
    CheckState() → false → OnExitState()
    FSM PlayerBoltState.OnExitState()
        └─ ResetGun() + SetAimIk(GetAimIkTarget())
```

---

## 15. 关键事件流转

### 15.1 事件一览

| 事件 | 发送者 | 消费者 | 用途 |
|---|---|---|---|
| `BoltEvent` | `BoltFeature.StartBolt()` | `PlayerIkModule` | 拉栓时禁用瞄准 IK |
| `BoltEndEvent` | （由 Spr2BoltState 移除后不再发送） | `PlayerIkModule` | 拉栓后恢复瞄准 IK |
| `OnReloadEvent` | `ReloadFeature.TryReload()` | `PlayerIkModule` | 换弹时禁用 AimIK + HandIK |
| `SetHandIkTargetEvent` | `CombatModule.ActivateWeapon()` | `LocalPlayerLogic.InitHandIK()` | 设置左手 IK 目标 + 枪口 |
| `WeaponDataUpdateEvent` | `FireFeature.TryFire()` / `ReloadTask.Finish()` | UI 层 / Presenter | 弹药/状态变化通知 |
| `WeaponStateUpdateEvent` | `WeaponRuntime.AddState/RemoveState` | 外部监听者 | 武器状态标签变化 |
| `FrontSightEvent` | `StanceFeature.SetStance()` | UI 层 | 准星显示切换 |

### 15.2 核心数据流图

```
InputContext                     BagModel.WeaponPro
    │                                  │
    ▼                                  ▼
WeaponAction ────→ HandleAction ──→ Features (Fire/Bolt/Reload/Stance)
                                        │
                                        ▼
                                   WeaponRuntime
                                   (StateTags + Stance)
                                        │
                        ┌───────────────┼───────────────┐
                        ▼               ▼               ▼
                   CombatModule     Event System    WeaponPresenter
                   .GetQuery()      (OnReload/      (音效/特效)
                        │           BoltEvent)
                        ▼               │
                   WeaponQuery          ▼
                   (位掩码)        PlayerIkModule
                        │          (AimIK/HandIK)
          ┌─────────────┼─────────────┐
          ▼             ▼             ▼
    动画树节点        FSM条件       FSM状态
    (CheckState)   (CheckCondition) (Enter/Exit)
          │             │             │
          ▼             ▼             ▼
    PlayableGraph   状态转移      武器挂载/卸载
    (动画播放)     (Empty↔Bolt    IK恢复
    weaponPivot     ↔Reload         ResetGun
    位置驱动        ↔Aim↔Throw)
```

---

> 文档版本：v2.0
> 最后更新：2026-05-27
> 对应提交：重构武器系统 + 动画树 IK 修复 + Bolt FSM 状态新增