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
                    if (!target.CompareTag("Enemy")) continue; 

                    // 2. 反应过滤逻辑 (例如：感电只会弹射到有水元素的敌人身上)
                    var controller = target.GetComponent<ElementBuffController>();
                    
                    if (component.ScopeType == 1) // 假设 ScopeType 1 代表感电弹射
                    {
                        // 逻辑：必须有水元素(2) 且 该目标的雷属性反应(4) 冷却已转好
                        if (controller != null)
                        {
                            bool hasWater = controller.elements[2] > 0;
                            bool isCdReady = controller.reactionCDs[4] <= 0;

                            if (!hasWater || !isCdReady) continue;

                            // 触发成功，给该目标设置雷元素反应冷却，防止瞬间被多次弹射
                            controller.reactionCDs[4] = 1.0f; 
                        }
                        else continue; // 没有控制器的敌人不参与元素弹射
                    }

                    uniqueTargets.Add(target);
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