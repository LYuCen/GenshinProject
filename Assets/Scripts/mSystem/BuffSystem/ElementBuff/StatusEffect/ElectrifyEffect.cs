using mSystem.HurtSystem;
using mSystem.ScopeHitSystem;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff.StatusEffect
{
    /// <summary>
    /// 感电持续反应效果逻辑
    /// </summary>
    public class ElectrifyEffect : IStatusEffect
    {
        private float _electCd;
        private int _electStep;
        public bool IsFinished { get; private set; }

        public void OnUpdate(float deltaTime, ElementBuffController owner)
        {
            // 如果雷(4)或水(2)元素枯竭，感电结束
            if (owner.elements[2] <= 0 || owner.elements[4] <= 0)
            {
                IsFinished = true;
                return;
            }

            _electCd -= deltaTime;
            if (_electCd <= 0)
            {
                _electCd = 0.5f; // 0.5秒触发一次
                _electStep++;

                // 消耗元素量
                owner.elements[2] = Mathf.Max(0, owner.elements[2] - 0.4f);
                owner.elements[4] = Mathf.Max(0, owner.elements[4] - 0.4f);

                // 发送单体伤害
                HurtSystem.HurtSystem.Instance.EnQueue(new HurtComponent()
                {
                    Element = 4,
                    Harm = 20,
                    Transform = owner.transform,
                    HitType = 0
                });

                // 如果是第二步，触发群体感电
                if (_electStep > 1)
                {
                    ScopeHitSystem.ScopeHitSystem.Instance.EnQueue(new ScopeHitComponent()
                    {
                        Element = 4,
                        Harm = 20,
                        IsHurtSelf = false,
                        Pos = owner.transform.position + Vector3.down * -0.5f,
                        Radius = 2,
                        Transform = owner.transform,
                        ScopeType = 1
                    });
                }
                
                // 假设跳 3 次后结束，或者等元素耗尽
                if (_electStep >= 3) IsFinished = true; 
            }
        }

        public void OnRemove(ElementBuffController owner) { /* 清理特效 */ }
    }
}