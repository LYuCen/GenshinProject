using System;
using mSystem.mSettingSystem.ItemData;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace mView.Setting.SettingItemView
{
    public class SettingSliderView : MonoBehaviour, ISettingItemView, IPointerEnterHandler, IPointerExitHandler 
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private TextMeshProUGUI value;
        [SerializeField] private Slider slider;
        [SerializeField] private GameObject enterImg;
        
        public Action OnUpdate;
        public Action<string> OnHoverIntroduce;

        private string _introduce;
        
        public void Build(SettingItemData data)
        {
            label.text = data.name;
            _introduce = data.introduce;

            slider.minValue = data.min;
            slider.maxValue = data.max;
            slider.value = data.value;
            value.text = data.value.ToString("F0");
            
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            enterImg.SetActive(true);
            OnHoverIntroduce?.Invoke(_introduce); 
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            enterImg.SetActive(false);
        }
        
        private void OnSliderValueChanged(float newValue)
        {
            int val = (int)newValue;
            value.text = val.ToString("F0");
            OnUpdate?.Invoke();
        }
        
        public void SetValue(float val)
        {
            slider.value = val;
            value.text = val.ToString("F0");
        }

        public float GetCurrentValue()
        {
            return slider.value;
        }
        
        private void OnDisable()
        {
            OnHoverIntroduce?.Invoke(""); 
        }
        
        private void OnDestroy()
        {
            if (slider != null)
            {
                slider.onValueChanged.RemoveListener(OnSliderValueChanged);
            }
        }
    }
}