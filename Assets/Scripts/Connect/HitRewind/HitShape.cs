using mEnum;
using UnityEngine;

namespace Connect.HitRewind
{
    /// <summary>
    /// 判定框，字段含义按 Type 不同：
    ///   Capsule: Bottom = 底球心, Top = 顶球心,  Radius = 半径
    ///   Box:     Bottom = 中心,  Top = 半长宽高, Rotation = 朝向
    /// </summary>
    public struct HitShape
    {
        public HitShapeType Type; // 碰撞体种类
        public HitPart Part;    // 部位
        public Vector3 Bottom;  // 底部
        public Vector3 Top;     // 顶部
        public float Radius;    // 半径
        public Quaternion Rotation;
    }
}