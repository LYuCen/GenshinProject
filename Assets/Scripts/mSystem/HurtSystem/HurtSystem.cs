using BaseClass;
using Connect;
using mController;
using mSystem.BuffSystem.ElementBuff;
using mSystem.BuffSystem.ElementBuff.ElementBuffSystem;
using mSystem.HitTextSystem;
using QFramework;
using Unity.VisualScripting;
using UnityEngine;
using IManager = BaseClass.IManager;

namespace mSystem.HurtSystem
{
    public class HurtComponentSystem : ComponentSystem<HurtComponent>, ICanSendEvent
    {
        // 使用重构后的元素反应系统
        private static IElementBuffSystem _buffSystem = new ElementBuffSystem();

        protected override void UpdateComponent()
        {
            while (Queue.Count > 0)
            {
                var component = Queue.Dequeue();
                
                // 受击对象为空或者对象正在被销毁
                if (!component.Transform || component.Transform.IsDestroyed()) continue;

                // 获取或添加元素控制器
                var controller = component.Transform.GetComponent<ElementBuffController>();
                if (!controller) continue;

                if (!component.SkipReaction)
                {
                    ProcessElementReaction(component, controller);
                }
                
                LoseHp(component);
            }
        }

        private void LoseHp(HurtComponent component)
        {
            // 优先处理模型层面的伤害逻辑
            if (!component.SkipHp && PlayerPool.Instance && PlayerPool.Instance.Models.TryGetValue(component.victimId, out var model))
            {
                Debug.Log($"{component.victimId} 受到：{component.Harm} 伤害");
                model.HpModule.HurtHp(component);
            }

            // 飘字系统：显示最终计算后的伤害
            var textComponent = new HitTextComponent()
            {
                transform = component.Transform,
                element = component.Element,
                Text = component.Harm.ToString(),
                textType = component.HitType // 可以根据 result.IsReaction 修改为不同的飘字类型
            };

            // 判断是否有 IManager 或 BodyController
            IManager im = component.Transform.GetComponent<IManager>();
            if (im != null)
            {
                im.UpdateHp(component.Harm);
            }
            else
            {
                BodyController body = component.Transform.GetComponent<BodyController>();
                if (body) body.LoseHp(component.Harm);
            }
        }

        public static void ProcessElementReaction(HurtComponent component, ElementBuffController controller)
        {
            var result = _buffSystem.Reaction(component.Transform, component.Element, ref component.ElementMeasure,
                controller);

            if (result.IsReaction)
            {
                component.Harm = (int)(component.Harm * result.DamageMultiplier + result.BonusDamage);
                // 如果反应携带 Buff
                if (result.TriggeredEffect != null)
                {
                    Debug.Log($"新增 {result.TriggeredEffect} Buff");
                    controller.AddEffect(result.TriggeredEffect);
                }
            }
            else
            {
                // 如果没有发生反应，则尝试进行元素附着
                // 只有攻击元素量大于0时才附着（某些反应可能会在无反应的情况下消耗掉元素量）
                if (component.ElementMeasure > 0)
                {
                    controller.AddElement(component);
                }
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
    
    public class HurtSystem : AbstractSingleton<HurtSystem>
    {
        private HurtComponentSystem _hurtCs = new HurtComponentSystem();

        // 伤害广播 Action
        public static System.Action<HurtComponent> OnHurtEnqueued;

        public void EnQueue(HurtComponent component)
        {
            OnHurtEnqueued?.Invoke(component);
        
            _hurtCs.EnQueue(component);
        }

        private void FixedUpdate()
        {
            _hurtCs.FixedUpdate();
        }
    }
    
}