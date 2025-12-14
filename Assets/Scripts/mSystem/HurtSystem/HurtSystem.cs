using BaseClass;
using mController;
using mSystem.BuffSystem.ElementBuff;
using mSystem.BuffSystem.ElementBuff.ElementBuffSystem;
using mSystem.HitTextSystem;
using QFramework;
using Unity.VisualScripting;

namespace mSystem.HurtSystem
{
    public class HurtComponentSystem : ComponentSystem<HurtComponent>, ICanSendEvent
    {
        private IElementBuffSystem _buffSystem = new ElementBuffSystem();

        protected override void UpdateComponent()
        {
            while (Queue.Count > 0)
            {
                var component = Queue.Dequeue();
                
                var controller = component.Transform.GetComponent<ElementBuffController>();
                int harm = component.Harm;
                if (!controller)
                {
                    controller = component.Transform.AddComponent<ElementBuffController>();
                    ElementSystem.Instance.EnQueue(controller);
                    controller.AddElement(component.Element, component.ElementMeasure * 0.8f, component);
                    LoseHp(component, harm);
                }
                else
                {
                    bool res;
                    harm = (int)_buffSystem.Reaction(component.Transform, harm, component.Element,
                        ref component.ElementMeasure, controller.elements,
                        out res);

                    if(!res)
                    {
                        controller.AddElement(component.Element, component.ElementMeasure * 0.8f, component);
                    }
                    LoseHp(component, harm);
                }
            }
        }

        private void LoseHp(HurtComponent component,int harm)
        {
            var textComponent = new HitTextComponent()
            {
                transform = component.Transform,
                element = component.Element,
                Text = harm.ToString(),
                textType = component.HitType
            };
            HitTextSystem.HitTextSystem.Instance.EnQueue(textComponent);

            BodyController body = component.Transform.GetComponent<BodyController>();
            if (body) body.LoseHp(harm);
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
    
    public class HurtSystem : AbstractSingleton<HurtSystem>
    {
        private HurtComponentSystem _hurtCS = new HurtComponentSystem();

        public void EnQueue(HurtComponent component)
        {
            _hurtCS.EnQueue(component);
        }

        private void FixedUpdate()
        {
            _hurtCS.FixedUpdate();
        }
    }
}