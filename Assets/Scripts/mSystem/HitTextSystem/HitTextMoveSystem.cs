using BaseClass;
using UnityEngine;

namespace mSystem.HitTextSystem
{
    public class HitTextMoveComponentSystem : ComponentSystem<HitText>
    {
        protected override void UpdateComponent()
        {
            int count = Queue.Count;
            for (int i = 0; i < count; i++)
            {
                var component = Queue.Dequeue();
                RectTransform rt = component.transform.GetComponent<RectTransform>();
                rt.anchoredPosition += Vector2.up * (500 * Time.fixedDeltaTime);

                if (component.endTime < Time.time) 
                    HitTextObjectPool.Instance.RemoveObj(component.transform.gameObject);
                else 
                    Queue.Enqueue(component);
            }
        }
    }
    
    public class HitTextMoveSystem : AbstractSingleton<HitTextMoveSystem>
    {
        private HitTextMoveComponentSystem _hitTextMoveCS = new HitTextMoveComponentSystem();

        public void EnQueue(HitText component)
        {
            _hitTextMoveCS.EnQueue(component);
        }

        private void FixedUpdate()
        {
            _hitTextMoveCS.FixedUpdate();
        }
    }
}