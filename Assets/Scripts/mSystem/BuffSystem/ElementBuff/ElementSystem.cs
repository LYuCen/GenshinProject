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
    
    public class ElementSystem : AbstractSingleton<ElementSystem>
    {
        private ElementComponentSystem _elementCs = new();

        private void FixedUpdate()
        {
            _elementCs.FixedUpdate(); ;
        }
    }
}