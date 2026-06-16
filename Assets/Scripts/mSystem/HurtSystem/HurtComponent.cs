using UnityEngine;

namespace mSystem.HurtSystem
{
    public class HurtComponent
    {
        public int attackerId;      // 谁打的
        public int victimId;        // 打中了谁
        public int Element;         // 元素
        public float ElementMeasure; // 元素量
        public int Harm;            // 伤害
        public int HitType;         // 0:身体, 1:头部
        public Transform Transform; // 目标的 Transform（由 ID 转换而来）
        public bool SkipReaction;   // 跳过元素反应（服务端已确认伤害，客户端只扣血+入队）
    }
}