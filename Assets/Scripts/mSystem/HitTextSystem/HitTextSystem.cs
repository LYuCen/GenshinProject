using BaseClass;
using mModel;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace mSystem.HitTextSystem
{
    public class HitTextComponentSystem : ComponentSystem<HitTextComponent>
    {
        protected override void UpdateComponent()
        {
            while (Queue.Count > 0)
            {
                var component = Queue.Dequeue();
                Transform t = component.transform.Find("Canvas");
                GameObject obj = HitTextObjectPool.Instance.GetObj();
                obj.transform.SetParent(t);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = new Vector3(Random.Range(-700, -600), 700, 0);
                obj.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 180, 0);
                TextMeshProUGUI proUGUI = obj.GetComponent<TextMeshProUGUI>();
                proUGUI.text = component.Text;
                if (component.textType == 1) proUGUI.fontSize = 150;
                else if(component.textType == 2) obj.transform.localPosition = new Vector3(Random.Range(200, -200), -100, 400);
                proUGUI.color = ElementModel.Instance.colors[component.element];
                HitText hitText = new HitText()
                {
                    transform = obj.transform,
                    endTime = Time.time + 1f
                };
                HitTextMoveSystem.Instance.EnQueue(hitText);
            }
        }
    }
    
    public class HitTextSystem : AbstractSingleton<HitTextSystem>
    {
        private HitTextComponentSystem _hitTextCS = new HitTextComponentSystem();

        public void EnQueue(HitTextComponent component)
        {
            _hitTextCS.EnQueue(component);
        }

        private void FixedUpdate()
        {
            _hitTextCS.FixedUpdate();
        }
    }
}