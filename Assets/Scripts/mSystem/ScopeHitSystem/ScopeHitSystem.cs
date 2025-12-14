using System.Collections.Generic;
using BaseClass;
using mSystem.BuffSystem.ElementBuff;
using mSystem.HurtSystem;
using UnityEngine;

namespace mSystem.ScopeHitSystem
{
    public class ScopeHitComponentSystem : ComponentSystem<ScopeHitComponent>
    {
        private Collider[] _hits = new Collider[50]; // 缓存池最多允许判定50个物体
        protected override void UpdateComponent()
        {
            while (Queue.Count > 0)
            {
                var component = Queue.Dequeue();
                int hitCount = Physics.OverlapSphereNonAlloc(component.Pos, component.Radius, _hits);

                HashSet<Transform> uniqueTargets = new HashSet<Transform>();

                for (int i = 0; i < hitCount; i++)
                {
                    // 找到目标的根对象
                    Transform target = _hits[i].transform.root;
                    if (target == component.Transform && !component.IsHurtSelf) continue; // 排除自己
                    if (!target.CompareTag("Enemy")) continue; // 不是敌人也排除
                    if (component.ScopeType == 1) // 是感电反应的范围伤害
                    {
                        var controller = target.GetComponent<ElementBuffController>();
                        // !(获取到元素控制器 && 感电Cd转好 && 有水元素附着)
                        if (!(controller && controller.electScopeCd == 0 && controller.elements[2] != 0))
                        {
                            continue;
                        }
                        
                        controller.electScopeCd = 1; // 触发感电反应，进入 1s 冷却期
                    }
                    uniqueTargets.Add(target);
                }

                foreach (var hit in uniqueTargets)
                {
                    var hurtComponent = new HurtComponent()
                    {
                        Transform = hit,
                        Element = component.Element,
                        ElementMeasure = component.ElementMeasure,
                        Harm = component.Harm,
                        HitType = 0
                    };
                    HurtSystem.HurtSystem.Instance.EnQueue(hurtComponent);
                }
            }
        }
    }
    
    /// <summary>
    /// 范围伤害系统
    /// </summary>
    public class ScopeHitSystem : AbstractSingleton<ScopeHitSystem>
    {
        private ScopeHitComponentSystem _scopeHitCS = new ScopeHitComponentSystem();

        public void EnQueue(ScopeHitComponent component)
        {
            _scopeHitCS.EnQueue(component);
        }

        private void FixedUpdate()
        {
            _scopeHitCS.FixedUpdate();
        }
    }
}