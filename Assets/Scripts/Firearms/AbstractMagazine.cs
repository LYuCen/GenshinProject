using mSystem.BuffSystem.ElementBuff;
using UnityEngine;

namespace Firearms
{
    public abstract class AbstractMagazine
    {
        public int BulletNumber; // 当前子弹数量
        public int BulletCapacity;  // 子弹容量
        public ElementEnum Element; // 元素
        public int BaseHurt; // 基础伤害
        
        public bool IsFull()
        {
            return BulletNumber == BulletCapacity;
        }

        public bool IsEmpty()
        {
            return BulletNumber == 0;
        }

        public int GetBulletNumber()
        {
            return BulletNumber;
        }

        /// <summary>
        /// 弹匣装弹
        /// </summary>
        /// <param name="val">子弹数</param>
        /// <param name="element">子弹元素</param>
        public void SetBulletNumber(int val, ElementEnum element)
        {
            BulletNumber = Mathf.Min(val, BulletCapacity);
            Element = element;
        }

        public int GetElementEnum()
        {
            return (int)Element;
        }
    }
}