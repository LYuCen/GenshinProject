using mEvent.FiringRange;
using QFramework;
using TMPro;
using UnityEngine;

namespace mView.SceneView
{
    public class PlayerTargetValueView : MonoBehaviour, IController
    {
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private int value;
        
        private void Start()
        {
            valueText.text = value.ToString();
            this.RegisterEvent<AddPlayerTargetValueEvent>(e =>
            {
                value += e.Value;
                value = Mathf.Clamp(value, 1, 10);
                valueText.text = value.ToString();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}