using mEvent.FiringRange;
using mInterface;
using QFramework;
using UnityEngine;

namespace mView.SceneView
{
    public class AddPlayerTargetValueView : MonoBehaviour, IHittable, ICanSendEvent
    {
        [SerializeField] private int addValue;
        public void OnHit(HitInfo info)
        {
            this.SendEvent(new AddPlayerTargetValueEvent
            {
                Value = addValue
            });
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}