using AssetBundleFramework.Core.Resource;
using mSystem.ServerTimeSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.StatusEffect
{
    /// <summary>
    /// 感电持续反应效果逻辑
    /// </summary>
    public class ElectrifyEffect : IStatusEffect, ITimeEvent
    {
        public bool IsFinished { get; private set; }
        public float Progress => 0f;
        public double TriggerTime { get; private set; }

        private double _interval = 0.5;
        private ElementBuffController _controller;
        private bool _cancelled;

        public ElectrifyEffect(double now, ElementBuffController controller)
        {
            TriggerTime = now;
            _controller = controller;
            TimeSystem.Instance.Add(this);
        }
        public void OnRemove() { _cancelled = true; }
        public Sprite GetSprite()
        {
            var ir = ResourceManager.instance.Load("Assets/AssetBundle/Icon/Buff/Electrify.png", false);
            return ir.GetAsset<Sprite>();
        }
        
        public void Execute()
        {
            if (_cancelled) return;

            if (!_controller.Elements[(int)ElementEnum.Water].Enable ||
                !_controller.Elements[(int)ElementEnum.Elect].Enable)
            {
                _controller.RemoveEffectByType(typeof(ElectrifyEffect));
                return;
            }
            
            // 消耗 0.4 单位水和雷
            _controller.ConsumeElement((int)ElementEnum.Water, 0.4f);
            _controller.ConsumeElement((int)ElementEnum.Elect, 0.4f);
            TriggerTime += _interval;
            TimeSystem.Instance.Add(this);
        }
    }
}