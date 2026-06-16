using AssetBundleFramework.Core.Resource;
using mSystem.ServerTimeSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.StatusEffect
{
    /// <summary>
    /// 冻结反应逻辑
    /// </summary>
    public class FreezeEffect : IStatusEffect, ITimeEvent
    {
        public bool IsFinished { get; }
        public float Progress => (float)(_endTime - _controller.TimeProvider()) / 3f;
        public double TriggerTime { get; }

        private ElementBuffController _controller;
        private double _endTime;
        private bool _cancelled;

        public FreezeEffect(double now, ElementBuffController controller)
        {
            _endTime = now + 3f;
            TriggerTime = _endTime;
            _controller = controller;
            TimeSystem.Instance.Add(this);
        }

        public void OnRemove()
        {
            _cancelled = true;
        }

        public Sprite GetSprite()
        {
            var ir = ResourceManager.instance.Load("Assets/AssetBundle/Icon/Buff/Freeze.png", false);
            return ir.GetAsset<Sprite>();
        }

        
        public void Execute()
        {
            if (_cancelled) return;
            _controller.RemoveEffectByType(typeof(FreezeEffect));
        }
    }
}