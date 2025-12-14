using mSystem.BuffSystem.ElementBuff;

namespace Firearms
{
    public abstract class AbstractMagazine
    {
        public int BulletNumber; // 当前子弹数量
        public int BulletCapacity;  // 子弹容量
        public ElementEnum Element; // 元素
        public int BaseHurt; // 基础伤害
    }
}