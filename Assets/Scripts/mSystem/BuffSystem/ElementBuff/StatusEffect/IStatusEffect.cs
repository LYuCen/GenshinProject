using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.StatusEffect
{
    /// <summary>
    /// 元素反应效果
    /// </summary>
    public interface IStatusEffect
    {
        bool IsFinished { get; }
        float Progress { get; } // 0~1, 进度条用
        void OnRemove();
        Sprite GetSprite();
    }
}