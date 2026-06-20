namespace Connect.Protocol
{
    public enum MessageType : ushort
    {
        // C2S: 客户端上报服务器
        C2S_UpdatePosition      = 1,    // 位置同步
        C2S_PlayerShoot         = 2,    // 开火
        C2S_PlayerHit           = 3,    // 命中请求
        C2S_Revive              = 4,    // 复活
        C2S_Move                = 5,    // 位置旋转同步
        C2S_Bones               = 6,    // 骨骼同步
        C2S_StateChange         = 7,    // 状态
        C2S_AnimationState      = 8,    // 动画状态同步
        
        
        // S2C: 服务器通知客户端
        S2C_AllocId             = 100,  // 分配 ID
        S2C_AllPlayerInfo       = 101,  // 全量玩家状态
        S2C_DamageConfirm       = 102,  // 伤害确认
        S2C_PlayerShoot         = 103,  // 其他玩家开火
        S2C_PlayerExit          = 104,  // 玩家离开
        S2C_PlayerSpawn         = 105,  // 新玩家加入
        S2C_PlayerDeath         = 106,  // 死亡
        S2C_Position            = 107,  // 位置旋转
        S2C_Bone                = 108,  // 骨骼
        S2C_Hp                  = 109,  // 血量
        S2C_StateChange         = 110,  // 状态同步
        S2C_ElementAttach       = 111,  // 某实体被附着元素
        S2C_ElementDetach       = 112,  // 某实体元素消失
        S2C_StatusEffect        = 113,  // 状态效果变化
        S2C_AnimationState      = 114,  // 动画信息广播



        // 双工
        Both_Heartbeat          = 200, // 心跳
        
        // 单机
        Local_AddBot            = 300, // 添加人机
    }
}