using mEvent.FiringRange;
using mInterface;
using QFramework;
using UnityEngine;

namespace mView.SceneView
{
    public class TogglePlayerTargetCrouchView : MonoBehaviour, IHittable, ICanSendEvent
    {
        [SerializeField] private GameObject checkUI;

        private bool _isOpen;

        public void OnHit(HitInfo info)
        {
            _isOpen = !_isOpen;
            checkUI.SetActive(_isOpen);
            this.SendEvent<TogglePlayerTargetCrouchEvent>();
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}