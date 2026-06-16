using AssetBundleFramework.Core.Resource;
using mSystem.ServerTimeSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.StatusEffect
{
    public class SharpenEffect : IStatusEffect, ITimeEvent
    {
        public bool IsFinished { get; private set; }
        public float Progress => (float)(_endTime - _controller.TimeProvider()) / 10f;
        public double TriggerTime { get; }
        private ElementBuffController _controller;
        private double _endTime;
        private bool _cancelled;

        public SharpenEffect(double now, ElementBuffController controller)
        {
            _endTime = now + 10.0;
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
            var ir = ResourceManager.instance.Load("Assets/AssetBundle/Icon/Buff/Sharpen.png", false);
            return ir.GetAsset<Sprite>();
        }

        
        public void Execute()
        {
            if (_cancelled) return;
            _controller.RemoveEffectByType(typeof(SharpenEffect));
        }
    }
}