using mEnum;
using UnityEngine;

namespace mInterface
{
    public struct HitInfo
    {
        public Vector3 Point;   // 命中位置
        public Vector3 Normal;  // 表面法向向量
        public HitPart Part;    // 命中部位
        public int Damage;
        public int Element;
        public float ElementMeasure;
    }

    public interface IHittable
    {
        void OnHit(HitInfo info);
    }
}
