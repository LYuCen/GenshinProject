using mEvent;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace mController
{
    public class FrontSightController : MonoBehaviour, ICanRegisterEvent
    {
        [SerializeField] private GameObject[] hit;
        [SerializeField] private Image[] hitImages;
        [SerializeField] private float closeTime;
        [SerializeField] private int hitType;
        private void Start()
        {
            this.RegisterEvent<HitEvent>(e =>
            {
                hit[hitType].SetActive(false);
                hitType = e.HitType;
                closeTime = Time.time + 0.2f;
                foreach (var img in hitImages)
                {
                    img.color = e.IsKill ? Color.red : Color.white;
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void LateUpdate()
        {
            hit[hitType].SetActive(Time.time < closeTime);
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}