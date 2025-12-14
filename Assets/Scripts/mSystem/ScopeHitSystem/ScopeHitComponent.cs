using UnityEngine;

namespace mSystem.ScopeHitSystem
{
    public class ScopeHitComponent
    {
        public Transform Transform; // 触发物体
        public float Radius;    // 半径
        public Vector3 Pos; // 位置
        public int Harm; // 伤害
        public int Element; // 元素
        public float ElementMeasure; // 元素量
        public bool IsHurtSelf; // 是否能伤害自己
        public int ScopeType; // 范围伤害种类
    }
}