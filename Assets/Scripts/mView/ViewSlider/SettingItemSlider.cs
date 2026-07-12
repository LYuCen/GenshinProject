using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace mView.ViewSlider
{
    public class SettingItemSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI val;

        private void Start()
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
            val.text = (slider.value * 100).ToString("F0");
        }

        private void OnSliderValueChanged(float value)
        {
            val.text = (100 * value).ToString("F0");
        }

        private void OnDestroy()
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }
}