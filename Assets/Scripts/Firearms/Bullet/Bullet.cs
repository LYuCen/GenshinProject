using UnityEngine;

namespace Firearms.Bullet
{
    public class Bullet
    {
        public Transform Transform;
        public int shooterId;
        public Vector3 moveNormalize;   // 移动方向（单位向量）
        public float speed;             // 子弹速度
        public float baseHit;           // 基础伤害
        public float headHit;           // 爆头伤害
        public int element;             // 元素
        public float elementMeasure;    // 元素量
        public float MoveTime;  // 最长运动时间
    }
}