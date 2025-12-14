using UnityEngine;

namespace mSystem.HurtSystem
{
    public class HurtComponent
    {
        public Transform Transform;
        public int Harm; // 伤害
        public int Element; // 元素
        public float ElementMeasure; // 元素量
        public int HitType; // 击中位置 (身体 : 0; 头 : 1)
    }
}