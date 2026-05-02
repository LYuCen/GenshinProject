using BaseClass;

namespace mSystem.BuffSystem.ElementBuff
{
    public class ElementComponentSystem : ComponentSystem<ElementComponent>
    {
        protected override void UpdateComponent()
        {
            
            while (Queue.Count > 0)
            {
                var c = Queue.Dequeue();
                // todo
                /* 
                if (c.Controller.elements[c.Element] <= 0)
                {
                    if (c.Controller.elementUI[c.Element])
                    {
                        ElementUIObjectPool.Instance.RemoveObj(c.Controller.elementUI[c.Element]);
                    }
                    
                    c.Controller.elementUI[c.Element] = null;
                    continue;
                }

                if (c.IsStart)
                {
                    c.IsStart = false;
                    var status = c.Transform.GetComponent<IEntityIdentity>();
                    if (status != null)
                    {
                        GameObject obj = ElementUIObjectPool.Instance.GetObj();
                        status.EntityStatusUI.AddElement(obj.transform);
                        ElementUI ui = obj.GetComponent<ElementUI>();
                        ui.Init(ElementModel.Instance.elementUI[c.Element]);
                        ui.ShowOutUI();
                        c.Controller.elementUI[c.Element] = obj;
                    }
                }
                */
            }
        }
    }

    public class ElementBuffControllerComponentSystem : ComponentSystem<ElementBuffController>
    {
        protected override void UpdateComponent()
        {
            for (int i = Queue.Count; i > 0; i--)
            {
                var controller = Queue.Dequeue();
                if (!controller) continue;

                // 驱动控制器逻辑
                controller.Tick();

                // 如果控制器还有附着元素或者还有运行中的 Effect，则继续保留在队列中
                if (controller.IsAlive())
                {
                    Queue.Enqueue(controller);
                }
                else
                {
                    controller.inQueue = false;
                }
            }
        }
    }
    
    public class ElementSystem : AbstractSingleton<ElementSystem>
    {
        private ElementComponentSystem _elementCs = new();
        private ElementBuffControllerComponentSystem _controllerCs = new();

        public void EnQueue(ElementComponent component)
        {
            _elementCs.EnQueue(component);
        }

        public void EnQueue(ElementBuffController component)
        {
            if (component.inQueue) return;
            component.inQueue = true;
            _controllerCs.EnQueue(component);
        }

        private void FixedUpdate()
        {
            _elementCs.FixedUpdate();
            _controllerCs.FixedUpdate();
        }
    }
}