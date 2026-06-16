using AssetBundleFramework.Core.Resource;
using mSystem.HurtSystem;
using mSystem.ServerTimeSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.StatusEffect
{
    /// <summary>
    /// 燃烧反应效果逻辑
    /// </summary>
    public class BurnEffect : IStatusEffect, ITimeEvent
    {
        public bool IsFinished { get; private set; }
        public float Progress => (float)((_endTime - _controller.TimeProvider()) / _duration);
        public double TriggerTime { get; private set; }
        private double _endTime;
        private double _duration = 2.0;
        private double _interval = 0.25;
        private ElementBuffController _controller;
        private bool _cancelled;

        public BurnEffect(double now, ElementBuffController controller)
        {
            _endTime = now + _duration;
            TriggerTime = now;
            _controller = controller;
            TimeSystem.Instance.Add(this);
        }

        public void OnRemove()
        {
            _cancelled = true;
        }

        public Sprite GetSprite()
        {
            var ir = ResourceManager.instance.Load("Assets/AssetBundle/Icon/Buff/Burn.png", false);
            return ir.GetAsset<Sprite>();
        }
        
        public void Execute()
        {
            if (_cancelled) return;

            HurtSystem.HurtSystem.Instance.EnQueue(new HurtComponent()
            {
                victimId = _controller.id,
                Element = 1,
                ElementMeasure = 0,
                Harm = 5,
                HitType = 0,
                Transform = _controller.transform
            });
            double next = TriggerTime + _interval;
            if (next < _endTime)
            {
                TriggerTime = next;
                TimeSystem.Instance.Add(this);
            }
            else
            {
                _controller.RemoveEffectByType(typeof(BurnEffect));
            }
        }
    }
}