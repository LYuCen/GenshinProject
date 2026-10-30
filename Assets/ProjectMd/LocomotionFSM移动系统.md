# LocomotionFSM 移动系统

## 架构概览

```
输入层 (ActionHandler)     FSM 状态机层 (LocomotionFsm)      物理执行层 (PlayerMoveModule)
─────────────────────     ─────────────────────────────     ────────────────────────────
                          ┌─────────────────────┐
MoveActionHandler ──────▶ │    Locomotion       │ ──────▶ PlayerMoveModule.OnFixedUpdate()
SprintActionHandler ────▶ │  (共享数据对象)      │         读 LevelSpeed/VerticalSpeed
CrouchActionHandler ────▶ │                     │         读 IsSprint/IsCrouch/IsJump
JumpActionHandler ──────▶ │                     │         写 IsGround (物理检测)
                          └─────────────────────┘
                                    ▲
                          LocomotionFsm (读写)
```

三层职责分离：
- **输入层**：ActionHandler 把按键事件写入 `Locomotion` 共享对象（不直接操作 FSM）
- **FSM 层**：`LocomotionFsm` 根据 `Locomotion` 中的输入+物理状态，判定当前状态，在 `OnEnterState` 中设置速度参数
- **物理层**：`PlayerMoveModule` 每 `FixedUpdate` 从 `Locomotion` 读取速度/状态，执行 CharacterController 移动，写回 `IsGround`

---

## 共享数据对象 — Locomotion

```csharp
// Locomotion.cs
public class Locomotion
{
    // === 按键输入 (由 ActionHandler 写入) ===
    public bool IsInputMove;      // 是否有移动输入
    public bool IsMoveForward;    // 是否向前移动
    public bool JumpDown;         // 跳跃按下(单帧)
    public bool SprintDown;       // 冲刺按下
    public bool CrouchDown;       // 蹲姿切换按下
    public Vector2 InputMoveVec;  // 移动输入向量

    // === 运行状态 (由 FSM 写入) ===
    public float LevelSpeed;      // 水平移动速度
    public float VerticalSpeed;   // 垂直速度
    public bool IsSprint;         // 是否冲刺中
    public bool IsCrouch;         // 是否蹲姿
    public bool IsJump;           // 是否跳跃中
    public Vector3 MoveVec;       // 世界空间移动方向

    // === 物理模拟 (由 MoveModule 写入) ===
    public bool IsGround;         // 是否在地面
    public float Yaw;             // 相机朝向
}
```

---

## FSM 状态树

```
LocomotionFsm (根)
├── Ground (MoveGroundLayer)
│   ├── Idle (MoveIdleState)
│   ├── Stand (MoveStandLayer)
│   │   ├── Walk (MoveStandWalkState)
│   │   └── Sprint (MoveStandSprintState)
│   └── Crouch (MoveCrouchLayer)
│       ├── Walk (MoveCrouchWalkState)
│       └── Sprint (MoveCrouchSprintState)
└── Air (MoveAirLayer)
    ├── Jump (MoveJumpState)
    └── Fall (MoveFallState)
```

### 根层 — LocomotionFsm

| 子状态 | 进入条件 |
|--------|---------|
| Ground | `IsGround == true` |
| Air | `!IsGround \|\| IsJump` |

### Ground 层 — MoveGroundLayer

进入 Ground 层后按优先级判定：Idle > Crouch > Stand。

| 子状态 | 进入条件 |
|--------|---------|
| Idle | `!IsInputMove` (无移动输入) |
| Crouch | `(IsCrouch \|\| CrouchDown) && IsInputMove` |
| Stand | `!(IsCrouch \|\| CrouchDown) && IsInputMove` |

Ground 层 `OnUpdateState` 额外处理：
- `CrouchDown` → 切换 `IsCrouch`
- `SprintDown && IsMoveForward` → 设 `IsSprint = true`
- `JumpDown` → 设 `IsJump = true`, `VerticalSpeed = 4f`

### Stand 层 — MoveStandLayer

| 子状态 | 速度 | 进入条件 |
|--------|------|---------|
| Walk | 1.6 | `!(IsSprint \|\| SprintDown)` |
| Sprint | 4.0 | `IsMoveForward && (SprintDown \|\| IsSprint)` |

### Crouch 层 — MoveCrouchLayer

| 子状态 | 速度 | 进入条件 |
|--------|------|---------|
| Walk | 1.6 | `!(IsSprint \|\| SprintDown)` |
| Sprint | 3.5 | `IsMoveForward && (SprintDown \|\| IsSprint)` |

### Air 层 — MoveAirLayer

| 子状态 | 进入条件 | OnEnterState |
|--------|---------|-------------|
| Jump | `IsJump == true` | 空（速度由 Ground 层 `OnUpdateState` 设） |
| Fall | `!IsJump` | 无 |

Air 层切回 Ground 的条件：`IsGround == true`。

---

## 跳跃流程

```
1. JumpActionHandler 检测按下 → Locomotion.JumpDown = true
2. MoveGroundLayer.OnUpdateState() 或 MoveIdleState.OnUpdateState()
   → IsJump = true, VerticalSpeed = 4f
3. MoveAirLayerCondition 检测到 IsJump → 切换 Air 层
4. MoveJumpStateCondition 检测到 IsJump → 进入 Jump 状态
5. PlayerMoveModule.OnFixedUpdate() 读取 VerticalSpeed = 4
   → CharacterController.Move(Vector3.up * 4 * 0.02f)
   → 玩家离开地面
6. CheckGrounded() → IsGround = false
7. 垂直速度受重力影响逐渐减小 → 到 0 后变负
8. MoveFallStateCondition.IsJump 为 false → Jump → Fall
9. Fall 状态持续下落，直到落地
10. MoveAirLayerCondition 返回 false (IsGround = true 且 IsJump = false)
    → 切回 Ground 层
```

关键设计点：
- `IsJump` 在 Ground 层设为 true，在 Jump 状态退出时（`OnExitState`）设为 false
- 跳跃初速度在 Ground 层或 MoveIdleState 中设置，而非在 MoveJumpState.OnEnterState 中，因为进入 Air 层时 Ground 层还没退出
- `MoveAirLayerCondition` 同时检查 `!IsGround || IsJump`，确保起跳瞬间（仍在地面）就能切到 Air 层

---

## 输入流向

### 移动输入
```
InputSystem (KeyState)
  → MoveActionHandler.Handle()
    → Locomotion.IsInputMove / IsMoveForward / InputMoveVec
      → FSM Condition 判定 (Idle/Walk/Sprint)
      → PlayerMoveModule.OnFixedUpdate() 读取 InputMoveVec 计算位移
```

### 冲刺输入
```
InputSystem (KeyState)
  → SprintActionHandler.Handle()
    → Locomotion.SprintDown = context.Sprint.Down
      → MoveGroundLayer.OnUpdateState() 设 IsSprint = true
      → MoveStandSprintStateCondition / MoveCrouchSprintStateCondition 判定
      → FSM 切换到 Sprint 状态 → OnEnterState 设 LevelSpeed = 4.0 / 3.5
      → PlayerMoveModule.OnFixedUpdate() 读取 LevelSpeed 执行移动
```

### 蹲姿输入
```
InputSystem (KeyState)
  → CrouchActionHandler.Handle()
    → Locomotion.CrouchDown = context.Crouch.Down
      → MoveGroundLayer.OnUpdateState() 切换 IsCrouch
      → MoveStandLayerCondition / MoveCrouchLayerCondition 判定
      → FSM 切换到对应层
```

### 跳跃输入
```
InputSystem (KeyState)
  → JumpActionHandler.Handle()
    → Locomotion.JumpDown = context.Jump.Down
      → MoveGroundLayer / MoveIdleState / MoveStandLayer 等处理跳跃
      → IsJump = true, VerticalSpeed = 4f
      → 切到 Air → Jump 状态
```

---

## PlayerMoveModule — 物理执行

```csharp
void OnFixedUpdate()
{
    // 1. 从 Locomotion 同步状态
    _moveSpeed = _fsmContext.Locomotion.LevelSpeed;
    _isSprint = _fsmContext.Locomotion.IsSprint;
    _isCrouch = _fsmContext.Locomotion.IsCrouch;
    _isJump   = _fsmContext.Locomotion.IsJump;
    _move     = _fsmContext.Locomotion.InputMoveVec;
    _fsmContext.Locomotion.Yaw = cameraYaw;

    // 2. 计算水平移动
    if (IsGround)
        horizontalMove = direction * (_moveSpeed * 0.02f);
    else
        horizontalMove = airVelocity * 0.02f;  // 空中锁定水平速度

    // 3. 应用位移
    CharacterController.Move(horizontalMove + up * (VerticalSpeed * 0.02f));

    // 4. 物理检测 → 写回 Locomotion.IsGround
    CheckGrounded();

    // 5. 重力更新 VerticalSpeed
    ApplyGravity();
}
```

**重力逻辑**：
- 地面时：`VerticalSpeed < 0` 则固定为 `-2.0f`（保持贴地）
- 空中时：每帧 `VerticalSpeed += gravity * 0.02f`，最大下落速度 `-10f`

**地面检测**：`Physics.SphereCast` 从玩家脚下 `0.2f` 处向下检测 `0.2f`，斜面角度 ≤ 45° 算作地面。

---

## 状态转移速查

### 站姿 ↔ 蹲姿
```
Stand ──(CrouchDown)──▶ Crouch
Crouch ──(CrouchDown)──▶ Stand
Stand/Crouch ──(!InputMove)──▶ Idle
```

### 行走 ↔ 冲刺
```
Walk ──(MoveForward && SprintDown)──▶ Sprint
Sprint ──(!(SprintDown||IsSprint))──▶ Walk
Sprint ──(!MoveForward)──▶ 设 IsSprint=false → 下一帧切 Walk
```

### 地面 ↔ 空中
```
Ground ──(!IsGround || IsJump)──▶ Air
Air ──(IsGround)──▶ Ground
```

### 跳跃 ↔ 坠落
```
Jump ──(!IsJump)──▶ Fall
Fall ──(IsJump)──▶ Jump (二段跳等)
```
