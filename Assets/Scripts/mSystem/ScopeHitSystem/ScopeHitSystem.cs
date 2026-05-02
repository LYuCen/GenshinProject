using System.Collections.Generic;
using BaseClass;
using mSystem.BuffSystem.ElementBuff;
using mSystem.HurtSystem;
using UnityEngine;

namespace mSystem.ScopeHitSystem
{
    public class ScopeHitComponentSystem : ComponentSystem<ScopeHitComponent>
    {
        private Collider[] _hits = new Collider[50]; 

        protected override void UpdateComponent()
        {
            while (Queue.Count > 0)
            {
                var component = Queue.Dequeue();
                // 物理检测
                int hitCount = Physics.OverlapSphereNonAlloc(component.Pos, component.Radius, _hits);

                HashSet<Transform> uniqueTargets = new HashSet<Transform>();

                for (int i = 0; i < hitCount; i++)
                {
                    Transform target = _hits[i].transform.root;
                    
                    // 1. 基础过滤
                    if (target == component.Transform && !component.IsHurtSelf) continue; 
                    
                }

                // 3. 发送伤害组件
                foreach (var hit in uniqueTargets)
                {
                    HurtSystem.HurtSystem.Instance.EnQueue(new HurtComponent()
                    {
                        victimId = hit.gameObject.GetInstanceID(), // 假设你用 InstanceID 作为 victimId
                        Transform = hit,
                        Element = component.Element,
                        ElementMeasure = component.ElementMeasure,
                        Harm = component.Harm,
                        HitType = 0
                    });
                }
            }
        }
    }

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