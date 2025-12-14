using BaseClass;
using mModel;
using UnityEngine;
using Object = UnityEngine.Object;

namespace mSystem.BuffSystem.ElementBuff
{
    public class ElementComponentSystem : ComponentSystem<ElementComponent>
    {
        protected override void UpdateComponent()
        {
            while (Queue.Count > 0)
            {
                var c = Queue.Dequeue();
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
                    GameObject obj = ElementUIObjectPool.Instance.GetObj();
                    obj.transform.SetParent(c.Transform.Find("Canvas/ElementBox"));
                    obj.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);
                    ElementUI ui = obj.GetComponent<ElementUI>();
                    ui.Init(ElementModel.Instance.elementUI[c.Element]);
                    ui.ShowOutUI();
                    c.Controller.elementUI[c.Element] = obj;
                }

            }
        }
    }

    public class ElementBuffControllerComponentSystem : ComponentSystem<ElementBuffController>
    {
        protected override void UpdateComponent()
        {
            for (int i = Queue.Count; i > 0; i--)
            {
                var component = Queue.Dequeue();
                if(component.electStart) component.ElectrificationReaction();
                component.electCd = Mathf.Max(0, component.electCd - Time.fixedDeltaTime);
                component.electScopeCd = Mathf.Max(0, component.electScopeCd - Time.fixedDeltaTime);
                if(!component.Decrease()) Object.Destroy(component);
                else Queue.Enqueue(component);
            }
        }
    }
    
    public class ElementSystem : AbstractSingleton<ElementSystem>
    {
        private ElementComponentSystem _elementCS = new();
        private ElementBuffControllerComponentSystem _controllerCS = new();
        private void Start()
        {
            GameObject prefab = Resources.Load<GameObject>("Prefab/ElementUI");
            ElementUIObjectPool.Instance.Init(prefab);
        }

        public void EnQueue(ElementComponent component)
        {
            _elementCS.EnQueue(component);
        }

        public void EnQueue(ElementBuffController component)
        {
            _controllerCS.EnQueue(component);
        }

        private void FixedUpdate()
        {
            _elementCS.FixedUpdate();
            _controllerCS.FixedUpdate();
        }
    }
}