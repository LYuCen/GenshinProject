using mSystem.BuffSystem.ElementBuff;

namespace Firearms.WeaponInterface
{
    /// <summary>
    /// 是否能进行换弹
    /// </summary>
    public interface IReloadable
    {
        /// <summary>
        /// 换弹开始，给弹药下标和子弹数判断是否能进行换弹
        /// </summary>
        /// <param name="elementEnum">换弹元素</param>
        /// <param name="bullet">子弹</param>
        public bool TryReload(ElementEnum elementEnum, int bullet);
        
        /// <summary>
        /// 换弹结束，实际换弹调用
        /// </summary>
        /// <param name="elementEnum">换弹元素</param>
        public void ReloadEnd(ElementEnum elementEnum);

        /// <summary>
        /// 换弹开始，给子弹弹匣
        /// </summary>
        /// <param name="magazine"></param>
        /// <returns></returns>
        public bool TryReload(AbstractMagazine magazine);
        
        /// <summary>
        /// 换弹结束，实际换弹调用
        /// </summary>
        /// <param name="magazine">弹匣</param>
        public void ReloadEnd(AbstractMagazine magazine);
    }
}