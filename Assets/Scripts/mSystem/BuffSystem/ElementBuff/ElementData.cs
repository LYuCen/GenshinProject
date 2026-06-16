using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff
{
    public struct ElementData
    {
        public int Level;        // 元素流逝档位
        public int Capacity;     // 容积
        public float AttachTime; // 附着/刷新时刻（ServerTime.Now）
        public bool Enable;

        /// <summary>
        /// 根据时间计算当前剩余元素量
        /// </summary>
        public float GetVal(float now)
        {
            float rate = GetDecayRate(Level);
            float val = Capacity - rate * (now - AttachTime);
            return Mathf.Max(0, val);
        }

        /// <summary>
        /// 获得每秒衰减速率
        /// </summary>
        public static float GetDecayRate(int level)
        {
            return level switch
            {
                1 => 1000f,
                2 => 1600f,
                4 => 2000f,
                _ => 0f
            };
        }
    }
}
