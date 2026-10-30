# 用户输入 → 服务端模拟运动 全流程

---

## 概览

```
用户按键 → InputSystem → InputContext → 每 FixedUpdate 采集 InputSample
→ 累积到 List → 20Hz 打包为 InputPacket(FlatBuffer) → Bridge 发送
→ 服务端解包 → PlayerInputBuffer(排序+缓冲) → 每 FixedUpdate 消费 1 个 Sample
→ ServerMoveModule 更新移动状态 → CharacterController.Move()
```

---

## 一、客户端：输入采集

### 1.1 每帧处理输入 (`LocalPlayerLogic.OnUpdate`)

```csharp
// LocalPlayerLogic.cs
public void OnUpdate()
{
    if (_inputSystem.GamePlayEnable) _inputController.Process(_context);
    else _m.MoveModule.InputMove(Vector2.zero);

    _m.state.aimPosition = _m.cameraController.rayEnd;
    _m.AnimController.Update();
    _m.IkModule?.OnUpdate();
    _m.ViewModule?.OnUpdate();
}
```

`_inputController.Process(_context)` 将底层按键事件写入 `InputContext`，后续通过 `_context.Jump.IsActive()` 等方式读取。

### 1.2 采集 + 打包发送 (`LocalPlayerLogic.SendInput`)

```csharp
// LocalPlayerLogic.cs — 在 OnFixedUpdate 中调用
private void SendInput()
{
    _tick++;  // 每个 FixedUpdate 累加，作为采样序号

    // 采集当前帧输入
    ushort buttons = 0;
    if (_context.Jump.IsActive())   buttons |= PlayerInputStateBoolBit.BUTTON_JUMP;
    if (_context.Crouch.IsActive()) buttons |= PlayerInputStateBoolBit.BUTTON_CROUCH;
    if (_context.Sprint.IsActive()) buttons |= PlayerInputStateBoolBit.BUTTON_SPRINT;
    Vector2 move = _m.MoveModule.GetMoveInput;

    // 构造 InputSample（摇杆 [-1,1] → byte [0,254]，yaw/pitch ×10）
    _pendingSamples.Add(new InputDataConverter.InputSample
    {
        Tick   = _tick,
        InputX = (byte)Mathf.RoundToInt((move.x + 1f) * 127f),
        InputY = (byte)Mathf.RoundToInt((move.y + 1f) * 127f),
        Yaw    = (short)(_m.cameraController.Yaw * 10),
        Pitch  = (short)(_m.cameraController.Pitch * 10),
        Button = buttons
    });

    // 20Hz 发送
    _sendTimer += Time.fixedDeltaTime;
    if (_sendTimer < 0.05f) return;
    _sendTimer -= 0.05f;

    if (_pendingSamples.Count == 0) return;
    _sequence++;
    byte[] data = InputDataConverter.Pack(_sequence, _pendingSamples.ToArray());
    NetworkService.Bridge.SendToServer(new Message(MessageType.C2S_PlayerInput, data));
    _pendingSamples.Clear();
}
```

**关键参数：**
- 采集频率：50Hz（随 FixedUpdate）
- 发送频率：20Hz（每 50ms）
- 每包约含 2-3 个 InputSample

### 1.3 入口 (`LocalPlayerLogic.OnFixedUpdate`)

```csharp
public void OnFixedUpdate()
{
    int currentId = NetworkService.Bridge.GetMyId();
    if (!NetworkService.Instance.isSinglePlayer)
    {
        _m.state.playerId = currentId;
        _m.state.position = _m.transform.position;
        _m.state.rotation = _m.transform.rotation;
        _m.MoveModule.WriteToState(_m.state);
        _m.CombatModule.WriteToState(_m.state);
        SendInput();
    }
    Reload();
    _m.CombatModule?.OnFixedUpdate();
    _m.MoveModule?.OnFixedUpdate();
}
```

---

## 二、编码格式

### 2.1 FlatBuffer Schema (`InputData.fbs`)

```fbs
namespace Connect.InputData;

table InputDataFlat{
    tick:uint;        // 客户端 tick 序号
    input_x:ubyte;    // 摇杆 X [-1,1] → [0,254]，中心=127
    input_y:ubyte;    // 摇杆 Y
    yaw:short;        // 水平朝向角度 ×10
    pitch:short;      // 垂直朝向角度 ×10
    button:ushort;    // 按钮位标志
}

table InputPacket{
    sequence:uint;              // 包序号（用于排序和丢包检测）
    command:[InputDataFlat];    // 区间内所有采样
}
```

### 2.2 编解码 (`InputDataConverter`)

```csharp
// 客户端：打包
public static byte[] Pack(uint sequence, InputSample[] samples)
{
    _builder.Clear();
    var offsets = new Offset<InputDataFlat>[samples.Length];
    for (int i = 0; i < samples.Length; i++)
    {
        var s = samples[i];
        offsets[i] = InputDataFlat.CreateInputDataFlat(
            _builder, s.Tick, s.InputX, s.InputY, s.Yaw, s.Pitch, s.Button);
    }
    var cmdVector = InputPacket.CreateCommandVector(_builder, offsets);
    InputPacket.StartInputPacket(_builder);
    InputPacket.AddSequence(_builder, sequence);
    InputPacket.AddCommand(_builder, cmdVector);
    var root = InputPacket.EndInputPacket(_builder);
    _builder.Finish(root.Value);
    return _builder.SizedByteArray();
}

// 服务端：解包
public static void Unpack(byte[] data, out uint sequence, out InputSample[] samples)
{
    var bb = new ByteBuffer(data);
    var packet = InputPacket.GetRootAsInputPacket(bb);
    sequence = packet.Sequence;
    int count = packet.CommandLength;
    samples = new InputSample[count];
    for (int i = 0; i < count; i++)
    {
        var cmd = packet.Command(i).Value;
        samples[i] = new InputSample
        {
            Tick = cmd.Tick, InputX = cmd.InputX, InputY = cmd.InputY,
            Yaw = cmd.Yaw, Pitch = cmd.Pitch, Button = cmd.Button
        };
    }
}
```

---

## 三、服务端：输入缓冲与消费

### 3.1 接收 → 入缓冲 (`ServerPlayerObject.HandleBinaryMessage`)

```csharp
case MessageType.C2S_PlayerInput:
    InputDataConverter.Unpack(data, out uint sequence, out var samples);
    _inputBuffer.Enqueue(sequence, samples);
    break;
```

### 3.2 PlayerInputBuffer — 排序 + 缓冲

```csharp
public class PlayerInputBuffer
{
    private SortedDictionary<uint, InputDataConverter.InputSample[]> _pending = new();
    private Queue<InputDataConverter.InputSample> _ready = new();
    private uint _nextSequence = 1;
    private const int MinBuffer = 2;

    // 默认居中（摇杆无输入），避免缓冲空时自动移动
    private InputDataConverter.InputSample _lastSample = new InputDataConverter.InputSample
    {
        InputX = 127, InputY = 127,
    };

    public int ReadyCount => _ready.Count;

    public void Enqueue(uint sequence, InputDataConverter.InputSample[] samples)
    {
        if (sequence < _nextSequence) return; // 过期包丢弃
        _pending[sequence] = samples;

        // 将连续序号的包释放到 ready 队列
        while (_pending.TryGetValue(_nextSequence, out var next))
        {
            foreach (var s in next) _ready.Enqueue(s);
            _pending.Remove(_nextSequence);
            _nextSequence++;
        }
    }

    public bool TryDequeue(out InputDataConverter.InputSample sample)
    {
        // 保持至少 MinBuffer 个 sample 在队列中，避免断流导致卡顿
        if (_ready.Count >= MinBuffer)
        {
            sample = _ready.Dequeue();
            _lastSample = sample;
            return true;
        }
        sample = _lastSample; // 缓冲不足时复用上一次输入
        return false;
    }
}
```

**设计要点：**
- `SortedDictionary` 按 sequence 排序 → 乱序到达的包自动重排
- 连续 sequence 自动 flush 到 `_ready` 队列
- `MinBuffer = 2` → 始终保留缓冲垫，避免 burst-gap 导致的卡顿
- 缓冲空时返回 `_lastSample`→ 玩家保持上次方向移动，不会静止突变

### 3.3 每 tick 消费 (`ServerPlayerObject.FixedUpdate`)

```csharp
private void FixedUpdate()
{
    if (_inputBuffer.TryDequeue(out var sample))
    {
        // 解码移动输入（byte [0,254] → float [-1,1]）
        float yaw = sample.Yaw / 10f;
        Vector2 move = new Vector2(
            sample.InputX / 127f - 1f,
            sample.InputY / 127f - 1f);
        _moveModule.UpdateInfo(yaw, move);

        // Sprint — 电平触发（按住时有、松开时无）
        if ((sample.Button & PlayerInputStateBoolBit.BUTTON_SPRINT) != 0)
            _moveModule.RequestSprint(_animContext.MoveModule.GetQuery());

        // Jump — 电平触发（InputJump 内部有 isGround 守卫）
        if ((sample.Button & PlayerInputStateBoolBit.BUTTON_JUMP) != 0)
            _moveModule.InputJump();

        // Crouch — 电平触发 ⚠️ 已知问题：toggle 会反复翻转
        if ((sample.Button & PlayerInputStateBoolBit.BUTTON_CROUCH) != 0)
            _moveModule.Crouch();
    }

    _moveModule.Move();
    _moveModule.Sprint(_animContext.MoveModule.GetQuery());
}
```

> **⚠️ 已知问题：** Jump/Crouch 应从边沿触发改回 `↓ && !last`，防止 Crouch toggle 每 tick 反复翻转。Sprint 缺少 `ReleaseSprint()` 的 else 分支。

---

## 四、服务端：移动模拟

### 4.1 ServerMoveModule — 核心移动逻辑

```csharp
public class ServerMoveModule
{
    private PlayerStateData _state;
    private CharacterController _player;
    private float _verticalVelocity;
    private float _gravity = -8f;
    private float _terminalVelocity = -10f;
    private float _walkSpeed = 1.6f;
    private float _sprintSpeed = 4f;
    private float _crouchSprintSpeed = 3.5f;
    private Vector3 _airVelocity;
    private bool _sprintRequested;
    private float _yaw;

    public void UpdateInfo(float yaw, Vector2 move)
    {
        _yaw = yaw;
        _state.inputMove = move;
    }

    public void InputJump()
    {
        if (!_state.isGround) return;
        if (_state.isCrouch) { _state.isCrouch = false; return; }
        _state.isJump = true;
        _verticalVelocity = 4f;
    }

    public void Crouch()
    {
        if (!_state.isJump)
            _state.isCrouch = !_state.isCrouch;
    }

    public void RequestSprint(PlayerMoveQuery query)
    {
        if (!_state.isGround) return;
        _sprintRequested = true;
        Sprint(query);
    }

    public void Move()
    {
        if (!Enable) return;

        // 方向计算
        Vector3 forward = Quaternion.Euler(0, _yaw, 0) * Vector3.forward;
        Vector3 right   = Quaternion.Euler(0, _yaw, 0) * Vector3.right;
        Vector3 move = forward * _state.inputMove.y + right * _state.inputMove.x;

        // 速度选择
        float speed = _state.isSprint ? _sprintSpeed : _walkSpeed;
        bool wasGround = _state.isGround;
        CheckGrounded();

        // 离地瞬间：快照当前速度到 airVelocity
        ApplyGroundTransition(wasGround, move, speed);

        if (_state.isSprint && _state.isCrouch) speed = _crouchSprintSpeed;

        Vector3 horizontalMove;
        if (_state.isGround)
            horizontalMove = move.normalized * (speed * Time.deltaTime);
        else
            horizontalMove = _airVelocity * Time.deltaTime;

        // 旋转 + 位移
        _player.transform.rotation = Quaternion.Slerp(
            _player.transform.rotation,
            Quaternion.LookRotation(forward),
            Time.deltaTime * 10f);
        _player.Move(horizontalMove + Vector3.up * (_verticalVelocity * Time.deltaTime));

        ApplyGravity();
    }

    private void ApplyGroundTransition(bool wasGround, Vector3 moveDir, float speed)
    {
        // 离地：快照当前水平速度
        if (wasGround && !_state.isGround)
            _airVelocity = moveDir.normalized * speed;

        // 落地：重置跳跃和垂直速度
        if (!wasGround && _state.isGround)
        {
            _state.isJump = false;
            _verticalVelocity = 0f;
        }
    }

    private void ApplyGravity()
    {
        if (_state.isGround)
        {
            if (_verticalVelocity < 0) _verticalVelocity = -2f;
        }
        else
        {
            _verticalVelocity += _gravity * Time.deltaTime;
            if (_verticalVelocity < _terminalVelocity)
                _verticalVelocity = _terminalVelocity;
        }
    }
}
```

---

## 五、时序图

```
客户端 FixedUpdate (50Hz):
  tick=1 ──┐
  tick=2 ──┤ 累积到 _pendingSamples
  tick=3 ──┘ → 发送 InputPacket(seq=1, samples=[1,2,3])

服务端收到 → Unpack → Enqueue
  _ready: [s1, s2, s3]

服务端 FixedUpdate (50Hz):
  tick → TryDequeue → s1 → UpdateInfo → Move()
  tick → TryDequeue → s2 → UpdateInfo → Move()
  tick → TryDequeue → s3 → UpdateInfo → Move()
  tick → TryDequeue → (_ready 剩 0 < MinBuffer) → 复用 s3 → Move()
  ...等待下一批...

客户端 FixedUpdate:
  tick=4 ──┐
  tick=5 ──┘ → 发送 InputPacket(seq=2, samples=[4,5])

服务端收到 → Enqueue → _ready: [s4, s5] → 继续消费
```

---

## 六、已知待修复项

| 问题 | 原因 | 修复 |
|------|------|------|
| Crouch 反复翻转 | `Crouch()` 是 toggle，电平触发每 tick 调一次 | 恢复边沿检测 `crouchDown && !_lastCrouchBtn` |
| Jump 可能重复触发 | `InputJump()` 虽有 isGround 守卫，但同 tick 内可能多次调用 | 同上，恢复边沿检测 |
| Sprint 松开不生效 | 缺少 `ReleaseSprint()` 调用 | 加 else 分支：`else _moveModule.ReleaseSprint()` |
| 地面检测不准 | `Physics.OverlapSphere` 替代 `_player.isGrounded` | 改用 `_state.isGround = _player.isGrounded` |